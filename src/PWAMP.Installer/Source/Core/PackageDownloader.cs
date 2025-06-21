using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Events;
using PWAMP.Installer.Models;

namespace PWAMP.Installer.Core
{
    public class PackageDownloader : IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool _disposed = false;

        public event EventHandler<DownloadProgressEventArgs> ProgressReported;

        public PackageDownloader()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(30);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PWAMP-Installer/1.0");
        }

        public async Task<string> DownloadPackageAsync(InstallablePackage package, string downloadDirectory, CancellationToken cancellationToken = default)
        {
            if (package?.DownloadUrl == null)
                throw new ArgumentException("Package or download URL cannot be null", nameof(package));

            var fileName = Path.GetFileName(package.DownloadUrl.AbsolutePath);
            if (string.IsNullOrEmpty(fileName) || !fileName.Contains("."))
                fileName = string.Format("{0}_{1}.{2}", 
                    package.Name.Replace(" ", "_"), 
                    package.Version, 
                    package.ArchiveFormat);

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

            try
            {
                OnProgressReported(new DownloadProgressEventArgs
                {
                    PackageName = package.Name,
                    Status = "Starting download..."
                });

                using (var response = await _httpClient.GetAsync(package.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? package.EstimatedSize;
                    
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        int bytesRead;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                            totalBytesRead += bytesRead;

                            var now = DateTime.UtcNow;
                            if (now - lastReportTime >= TimeSpan.FromMilliseconds(250))
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
                                    PercentComplete = totalBytes > 0 ? Math.Min(100, (double)totalBytesRead / totalBytes * 100) : 0,
                                    TimeRemaining = timeRemaining,
                                    DownloadSpeed = speed,
                                    Status = string.Format("Downloading... {0}/s", FormatBytes(speed))
                                });

                                lastReportTime = now;
                            }
                        }
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

                return filePath;
            }
            catch (Exception ex)
            {
                if (File.Exists(filePath))
                {
                    try { File.Delete(filePath); } catch { }
                }
                throw new InvalidOperationException(string.Format("Failed to download {0}: {1}", package.Name, ex.Message), ex);
            }
        }

        private async Task<bool> ValidateExistingFile(string filePath, InstallablePackage package)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists) return false;

                if (package.EstimatedSize > 0 && Math.Abs(fileInfo.Length - package.EstimatedSize) > (long)(package.EstimatedSize * 0.1))
                    return false;

                if (!string.IsNullOrEmpty(package.ChecksumUrl))
                    return await ValidateChecksumAsync(filePath, package.ChecksumUrl);

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
                    var hashBytes = await Task.Run(() => md5.ComputeHash(stream), cancellationToken);
                    var actualChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                    return string.Equals(expectedChecksum, actualChecksum, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
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
            
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }

        protected virtual void OnProgressReported(DownloadProgressEventArgs e)
        {
            if (ProgressReported != null)
                ProgressReported.Invoke(this, e);
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