using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Wampoon.Installer.Events;
using Wampoon.Installer.Models;
using Wampoon.Installer.Helpers;

namespace Wampoon.Installer.Core
{
    public class PackageDownloader : IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool _disposed = false;

        public event EventHandler<DownloadProgressEventArgs> ProgressReported;

        public PackageDownloader()
        {
            // Configure global TLS/SSL settings for .NET 4.8 compatibility
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | (SecurityProtocolType)192;
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            
            // Create HttpClientHandler with TLS configuration
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            };
            
            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = InstallerConstants.HttpClientTimeout;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", AppConstants.USER_AGENT);
        }

        public async Task<string> DownloadPackageAsync(InstallablePackage package, string downloadDirectory, CancellationToken cancellationToken = default)
        {
            if (package?.DownloadUrl == null)
                throw new ArgumentException("Package or download URL cannot be null", nameof(package));

            var fileName = Path.GetFileName(package.DownloadUrl.AbsolutePath);
            if (string.IsNullOrEmpty(fileName) || !fileName.Contains("."))
                fileName = $"{package.Name.Replace(" ", "_")}_{package.Version}.{package.ArchiveFormat}";

            var filePath = Path.Combine(downloadDirectory, fileName);
            
            if (!Directory.Exists(downloadDirectory))
                Directory.CreateDirectory(downloadDirectory);

            if (File.Exists(filePath))
            {
                OnProgressReported(new DownloadProgressEventArgs
                {
                    PackageName = package.Name,
                    Status = "File already exists, validating..."
                });

                if (await ValidateExistingFile(filePath, package))
                {
                    OnProgressReported(new DownloadProgressEventArgs
                    {
                        PackageName = package.Name,
                        PercentComplete = 100,
                        Status = "Using existing valid file"
                    });
                    return filePath;
                }
                
                File.Delete(filePath);
            }

            var startTime = DateTime.UtcNow;
            long totalBytesRead = 0;
            var lastReportTime = DateTime.UtcNow;
            double lastReportedPercentage = 0;

            const int maxRetries = 3;
            Exception lastException = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    OnProgressReported(new DownloadProgressEventArgs
                    {
                        PackageName = package.Name,
                        Status = attempt == 1 ? "Starting download..." : $"Retrying download (attempt {attempt}/{maxRetries})..."
                    });

                    using (var response = await _httpClient.GetAsync(package.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                    {
                        response.EnsureSuccessStatusCode();

                        var totalBytes = response.Content.Headers.ContentLength ?? package.EstimatedSize;
                        
                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, InstallerConstants.FileStreamBufferSize, true))
                        {
                            var bufferSize = GetOptimalBufferSize(totalBytes);
                            var buffer = new byte[bufferSize];
                            int bytesRead;

                            // Reset tracking variables for retry attempts
                            totalBytesRead = 0;
                            startTime = DateTime.UtcNow;
                            lastReportTime = DateTime.UtcNow;
                            lastReportedPercentage = 0;

                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                                totalBytesRead += bytesRead;

                                var now = DateTime.UtcNow;
                                var currentPercentage = totalBytes > 0 ? Math.Min(100, (double)totalBytesRead / totalBytes * 100) : 0;
                                
                                // Report progress only at exact 10% intervals (10%, 20%, 30%, etc.)
                                var currentTenPercent = Math.Floor(currentPercentage / 10) * 10;
                                var lastTenPercent = Math.Floor(lastReportedPercentage / 10) * 10;
                                
                                if (currentTenPercent > lastTenPercent && currentTenPercent > 0)
                                {
                                    var elapsed = now - startTime;
                                    var speed = totalBytesRead / elapsed.TotalSeconds;
                                    var remainingBytes = Math.Max(0, totalBytes - totalBytesRead);
                                    var timeRemaining = speed > 0 ? TimeSpan.FromSeconds(remainingBytes / speed) : TimeSpan.Zero;

                                    OnProgressReported(new DownloadProgressEventArgs
                                    {
                                        PackageName = package.Name,
                                        BytesReceived = totalBytesRead,
                                        TotalBytes = totalBytes,
                                        PercentComplete = currentPercentage,
                                        TimeRemaining = timeRemaining,
                                        DownloadSpeed = speed,
                                        Status = $"Downloading... {FormatBytes(speed)}/s"
                                    });

                                    lastReportTime = now;
                                    lastReportedPercentage = currentPercentage;
                                }
                            }
                        }
                    }

                    // If we reach here, download was successful
                    break;
                }
                catch (Exception ex) when (attempt < maxRetries && !cancellationToken.IsCancellationRequested)
                {
                    lastException = ex;
                    
                    // Get more detailed error information
                    var errorDetails = GetDetailedErrorMessage(ex);
                    
                    OnProgressReported(new DownloadProgressEventArgs
                    {
                        PackageName = package.Name,
                        Status = $"Download failed (attempt {attempt}/{maxRetries}): {errorDetails}"
                    });

                    // Clean up partial file before retry
                    if (File.Exists(filePath))
                    {
                        try { File.Delete(filePath); } catch { }
                    }

                    // Wait before retry (exponential backoff)
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                    OnProgressReported(new DownloadProgressEventArgs
                    {
                        PackageName = package.Name,
                        Status = $"Waiting {delay.TotalSeconds:F0} seconds before retry..."
                    });
                    
                    await Task.Delay(delay, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Final attempt failed or cancellation was requested
                    lastException = ex;
                    throw;
                }
            }

            // If all retries failed, try using WebClient as a fallback
            if (lastException != null && !File.Exists(filePath))
            {
                OnProgressReported(new DownloadProgressEventArgs
                {
                    PackageName = package.Name,
                    Status = "Trying alternative download method..."
                });

                try
                {
                    await DownloadWithWebClientFallback(package, filePath, cancellationToken);
                }
                catch (Exception fallbackEx)
                {
                    ErrorLogHelper.LogExceptionInfo(fallbackEx);
                    throw new InvalidOperationException($"Failed to download {package.Name} using all available methods. Last error: {GetDetailedErrorMessage(lastException)}", lastException);
                }
            }

            OnProgressReported(new DownloadProgressEventArgs
            {
                PackageName = package.Name,
                BytesReceived = totalBytesRead,
                TotalBytes = totalBytesRead,
                PercentComplete = 100,
                Status = "Download completed"
            });

            // Validate ZIP file integrity immediately after download.
            if (package.ArchiveFormat?.ToLower() == "zip")
            {
                OnProgressReported(new DownloadProgressEventArgs
                {
                    PackageName = package.Name,
                    PercentComplete = 100,
                    Status = "Validating archive integrity..."
                });

                try
                {
                    using (var archive = System.IO.Compression.ZipFile.OpenRead(filePath))
                    {
                        // Try to read the central directory - this will fail if ZIP is corrupted.
                        var entryCount = archive.Entries.Count;
                        OnProgressReported(new DownloadProgressEventArgs
                        {
                            PackageName = package.Name,
                            PercentComplete = 100,
                            Status = $"Archive validated ({entryCount} entries)"
                        });
                    }
                }
                catch (System.IO.InvalidDataException ex)
                {
                    if (File.Exists(filePath))
                    {
                        try { File.Delete(filePath); } catch { }
                    }
                    throw new InvalidOperationException($"Downloaded archive appears to be corrupted or incomplete: {ex.Message}. This could be due to a network issue or the file being unavailable. Please try again.", ex);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogExceptionInfo(ex);
                    if (File.Exists(filePath))
                    {
                        try { File.Delete(filePath); } catch { }
                    }
                    throw new InvalidOperationException($"Failed to validate downloaded archive: {ex.Message}", ex);
                }
            }

            return filePath;
        }

        private async Task<bool> ValidateExistingFile(string filePath, InstallablePackage package)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists) return false;

                if (package.EstimatedSize > 0 && Math.Abs(fileInfo.Length - package.EstimatedSize) > (long)(package.EstimatedSize * InstallerConstants.FileSizeTolerancePercent))
                    return false;

                // Validate ZIP file integrity for ZIP archives.
                if (package.ArchiveFormat?.ToLower() == "zip")
                {
                    try
                    {
                        using (var archive = System.IO.Compression.ZipFile.OpenRead(filePath))
                        {
                            // Try to read the central directory - this will fail if ZIP is corrupted.
                            var entryCount = archive.Entries.Count;
                        }
                    }
                    catch (System.IO.InvalidDataException)
                    {
                        // ZIP file is corrupted.
                        return false;
                    }
                    catch
                    {
                        // Other ZIP-related errors.
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(package.ChecksumUrl))
                    return await ValidateChecksumAsync(filePath, package.ChecksumUrl);
                else if (!string.IsNullOrEmpty(package.Checksum))
                    return ValidateDirectChecksum(filePath, package.Checksum);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateChecksumAsync(string filePath, string checksumUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(checksumUrl))
                return true;

            try
            {
                var expectedChecksum = await _httpClient.GetStringAsync(checksumUrl);
                expectedChecksum = expectedChecksum.Trim().Split(' ')[0].ToLowerInvariant();

                using (var md5 = System.Security.Cryptography.MD5.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    var hashBytes = await ComputeHashAsync(md5, stream, cancellationToken);
                    var actualChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                    return string.Equals(expectedChecksum, actualChecksum, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateDirectChecksum(string filePath, string expectedChecksum)
        {
            if (string.IsNullOrEmpty(expectedChecksum))
                return true;

            try
            {
                expectedChecksum = expectedChecksum.Trim().ToLowerInvariant();

                using (var md5 = System.Security.Cryptography.MD5.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    var hashBytes = md5.ComputeHash(stream);
                    var actualChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                    return string.Equals(expectedChecksum, actualChecksum, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }

        private int GetOptimalBufferSize(long totalBytes)
        {
            // Adaptive buffer sizing based on file size.
            const long TenMB = 10 * 1024 * 1024;
            const long HundredMB = 100 * 1024 * 1024;

            if (totalBytes >= HundredMB)
            {
                // 1MB buffer for files >= 100MB.
                return InstallerConstants.HugeDownloadBufferSize;
            }
            else if (totalBytes >= TenMB)
            {
                // 256KB buffer for files >= 10MB.
                return InstallerConstants.LargeDownloadBufferSize;
            }
            else
            {
                // 64KB buffer for smaller files.
                return InstallerConstants.DownloadBufferSize;
            }
        }

        private async Task<byte[]> ComputeHashAsync(System.Security.Cryptography.HashAlgorithm hashAlgorithm, Stream stream, CancellationToken cancellationToken)
        {
            var buffer = new byte[InstallerConstants.DownloadBufferSize];
            int bytesRead;
            
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                hashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);
            }
            
            hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
            return hashAlgorithm.Hash;
        }

        private string FormatBytes(double bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int counter = 0;
            double number = bytes;
            
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            
            return $"{number:n1} {suffixes[counter]}";
        }

        protected virtual void OnProgressReported(DownloadProgressEventArgs e)
        {
            ProgressReported?.Invoke(this, e);
        }

        private async Task DownloadWithWebClientFallback(InstallablePackage package, string filePath, CancellationToken cancellationToken)
        {
            using (var webClient = new System.Net.WebClient())
            {
                // Configure WebClient for better compatibility
                webClient.Headers.Add("User-Agent", AppConstants.USER_AGENT);
                
                var progress = new Progress<int>(percent =>
                {
                    OnProgressReported(new DownloadProgressEventArgs
                    {
                        PackageName = package.Name,
                        PercentComplete = percent,
                        Status = $"Downloading (fallback method)... {percent}%"
                    });
                });

                // Download with progress tracking
                await Task.Run(() =>
                {
                    webClient.DownloadProgressChanged += (sender, e) =>
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            ((IProgress<int>)progress).Report(e.ProgressPercentage);
                        }
                    };

                    webClient.DownloadFileCompleted += (sender, e) =>
                    {
                        if (e.Error != null)
                            throw e.Error;
                    };

                    var downloadTask = webClient.DownloadFileTaskAsync(package.DownloadUrl, filePath);
                    
                    // Wait for download or cancellation
                    while (!downloadTask.IsCompleted && !cancellationToken.IsCancellationRequested)
                    {
                        Thread.Sleep(100);
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        webClient.CancelAsync();
                        throw new OperationCanceledException();
                    }

                    downloadTask.Wait(); // This will throw if download failed
                }, cancellationToken);
            }
        }

        private string GetDetailedErrorMessage(Exception ex)
        {
            switch (ex)
            {
                case HttpRequestException httpEx:
                    if (httpEx.Message.Contains("An error occurred while sending the request"))
                    {
                        return "Network connection failed - this may be due to TLS/SSL configuration, proxy settings, or network connectivity issues";
                    }
                    return $"HTTP error: {httpEx.Message}";
                
                case TaskCanceledException timeoutEx when timeoutEx.InnerException is TimeoutException:
                    return "Request timed out - the server is taking too long to respond";
                
                case TaskCanceledException cancelEx:
                    return "Request was cancelled";
                
                case System.Net.WebException webEx:
                    return $"Web exception ({webEx.Status}): {webEx.Message}";
                
                default:
                    return $"{ex.GetType().Name}: {ex.Message}";
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_httpClient != null)
                    _httpClient.Dispose();
                _disposed = true;
            }
        }
    }
}