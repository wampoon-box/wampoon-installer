using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Wampoon.Installer.Events;
using Wampoon.Installer.Helpers;
using Wampoon.Installer.Models;

namespace Wampoon.Installer.Core.PackageOperations
{
    /// <summary>
    /// Validates package download URLs before installation to detect dead links early.
    /// </summary>
    public class UrlValidationService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool _disposed = false;

        private static readonly TimeSpan ValidationTimeout = TimeSpan.FromSeconds(10);
        private static readonly int[] AcceptableStatusCodes = { 200, 301, 302, 307, 308 };

        public event EventHandler<UrlValidationProgressEventArgs> ValidationProgressReported;

        public UrlValidationService()
        {
            // Configure TLS/SSL settings for .NET 4.8 compatibility
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | (SecurityProtocolType)192;

            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false, // Don't follow redirects, just verify they exist
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            };

            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = ValidationTimeout;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", AppConstants.USER_AGENT);
        }

        /// <summary>
        /// Validates all package download URLs in parallel.
        /// </summary>
        /// <param name="packages">Packages to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Validation result with any failures</returns>
        public async Task<UrlValidationResult> ValidatePackageUrlsAsync(
            IEnumerable<InstallablePackage> packages,
            CancellationToken cancellationToken = default)
        {
            var packageList = packages.ToList();
            var failures = new ConcurrentBag<UrlValidationFailure>();
            var totalCount = packageList.Count;
            var currentIndex = 0;

            // Validate all URLs in parallel for speed
            var validationTasks = packageList.Select(async package =>
            {
                var index = Interlocked.Increment(ref currentIndex);

                OnValidationProgress(new UrlValidationProgressEventArgs
                {
                    PackageName = package.Name,
                    Url = package.DownloadUrl?.ToString(),
                    CurrentIndex = index,
                    TotalCount = totalCount,
                    Status = "Validating...",
                    IsValid = false
                });

                var failure = await ValidateSingleUrlAsync(package, cancellationToken);

                if (failure != null)
                {
                    failures.Add(failure);
                    OnValidationProgress(new UrlValidationProgressEventArgs
                    {
                        PackageName = package.Name,
                        Url = package.DownloadUrl?.ToString(),
                        CurrentIndex = index,
                        TotalCount = totalCount,
                        Status = $"Failed: {failure.Reason}",
                        IsValid = false
                    });
                }
                else
                {
                    OnValidationProgress(new UrlValidationProgressEventArgs
                    {
                        PackageName = package.Name,
                        Url = package.DownloadUrl?.ToString(),
                        CurrentIndex = index,
                        TotalCount = totalCount,
                        Status = "Valid",
                        IsValid = true
                    });
                }
            });

            await Task.WhenAll(validationTasks);

            return new UrlValidationResult
            {
                AllUrlsValid = failures.IsEmpty,
                Failures = failures.ToList()
            };
        }

        private async Task<UrlValidationFailure> ValidateSingleUrlAsync(
            InstallablePackage package,
            CancellationToken cancellationToken)
        {
            if (package?.DownloadUrl == null)
            {
                return new UrlValidationFailure
                {
                    PackageName = package?.Name ?? "Unknown",
                    Url = null,
                    Reason = "Missing download URL",
                    PackageWebsite = package?.PackageWebsite
                };
            }

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Head, package.DownloadUrl))
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    cts.CancelAfter(ValidationTimeout);

                    var response = await _httpClient.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cts.Token);

                    var statusCode = (int)response.StatusCode;

                    if (!AcceptableStatusCodes.Contains(statusCode))
                    {
                        return new UrlValidationFailure
                        {
                            PackageName = package.Name,
                            Url = package.DownloadUrl.ToString(),
                            Reason = $"HTTP {statusCode} {response.ReasonPhrase}",
                            HttpStatusCode = statusCode,
                            PackageWebsite = package.PackageWebsite
                        };
                    }

                    // Check Content-Type to detect servers returning HTML error pages with 200 OK
                    var contentType = response.Content?.Headers?.ContentType?.MediaType?.ToLowerInvariant() ?? "";
                    if (contentType.Contains("text/html"))
                    {
                        return new UrlValidationFailure
                        {
                            PackageName = package.Name,
                            Url = package.DownloadUrl.ToString(),
                            Reason = "Server returned an HTML page instead of a downloadable file (URL may be invalid)",
                            PackageWebsite = package.PackageWebsite
                        };
                    }

                    return null; // Success
                }
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return new UrlValidationFailure
                {
                    PackageName = package.Name,
                    Url = package.DownloadUrl.ToString(),
                    Reason = "Request timed out (server unresponsive)",
                    PackageWebsite = package.PackageWebsite
                };
            }
            catch (HttpRequestException ex)
            {
                return new UrlValidationFailure
                {
                    PackageName = package.Name,
                    Url = package.DownloadUrl.ToString(),
                    Reason = $"Network error: {GetSimplifiedErrorMessage(ex)}",
                    PackageWebsite = package.PackageWebsite
                };
            }
            catch (Exception ex)
            {
                return new UrlValidationFailure
                {
                    PackageName = package.Name,
                    Url = package.DownloadUrl.ToString(),
                    Reason = $"Validation error: {ex.Message}",
                    PackageWebsite = package.PackageWebsite
                };
            }
        }

        private string GetSimplifiedErrorMessage(Exception ex)
        {
            // Simplify common error messages for better user experience
            var message = ex.Message;

            if (message.Contains("The remote name could not be resolved"))
                return "Could not reach server (DNS resolution failed)";

            if (message.Contains("Unable to connect to the remote server"))
                return "Could not connect to server";

            if (message.Contains("The underlying connection was closed"))
                return "Connection was closed unexpectedly";

            return message;
        }

        protected virtual void OnValidationProgress(UrlValidationProgressEventArgs e)
        {
            ValidationProgressReported?.Invoke(this, e);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
