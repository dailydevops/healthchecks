namespace NetEvolve.HealthChecks.Tests.Integration.Azure.KeyVault;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using global::Azure.Core;
using global::Azure.Core.Pipeline;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.LowkeyVault;
using TUnit.Core.Exceptions;

public sealed class LowkeyVaultAccess : IAsyncInitializer, IAsyncDisposable
{
    private readonly LowkeyVaultContainer _container = new LowkeyVaultBuilder(
        /*dockerimage*/"nagyesta/lowkey-vault:2.7.1-ubi9-minimal"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public Uri VaultUri { get; private set; } = default!;

    public TokenCredential Credential { get; private set; } = default!;

#pragma warning disable CA2000 // Dispose objects before losing scope
#pragma warning disable S2325 // Methods and properties that don't access instance data can be static
    public Action<SecretClientOptions> ConfigureSecretClientOptions =>
        options =>
        {
            options.DisableChallengeResourceVerification = true;
            options.Transport = new HttpClientTransport(CreateHandler());
        };
#pragma warning restore S2325 // Methods and properties that don't access instance data can be static
#pragma warning restore CA2000 // Dispose objects before losing scope

#pragma warning disable CA1024 // Use properties where appropriate
    public SecretClient GetSecretClient()
    {
        var clientOptions = new SecretClientOptions();
        ConfigureSecretClientOptions(clientOptions);
        return new SecretClient(VaultUri, Credential, clientOptions);
    }
#pragma warning restore CA1024 // Use properties where appropriate

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync().ConfigureAwait(false);
        if (Credential is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "Ownership transferred to LowkeyVaultTokenCredential"
    )]
    public async Task InitializeAsync()
    {
        try
        {
            await _container.StartAsync().ConfigureAwait(false);

            VaultUri = new Uri(_container.GetBaseAddress());
            Credential = new LowkeyVaultTokenCredential(_container.GetAuthTokenUrl(), CreateHandler());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new SkipTestException($"Failed to start LowkeyVault container. {ex.Message}");
        }
    }

#pragma warning disable S4830 // Enable server certificate validation on this SSL/TLS connection
    private static HttpClientHandler CreateHandler() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        };
#pragma warning restore S4830 // Enable server certificate validation on this SSL/TLS connection

    private sealed class LowkeyVaultTokenCredential : TokenCredential, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _tokenUri;

        public LowkeyVaultTokenCredential(string tokenUrl, HttpClientHandler handler)
        {
            _httpClient = new HttpClient(handler);
            _tokenUri = new Uri(
                $"{tokenUrl}?api-version=2019-08-01&resource={Uri.EscapeDataString("https://vault.azure.net")}"
            );
        }

        public void Dispose() => _httpClient.Dispose();

#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            using var response = _httpClient.GetAsync(_tokenUri, cancellationToken).GetAwaiter().GetResult();
            _ = response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            using var doc = JsonDocument.Parse(content);
            return new AccessToken(
                doc.RootElement.GetProperty("access_token").GetString()!,
                DateTimeOffset.UtcNow.AddHours(1)
            );
        }
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits

        public override async ValueTask<AccessToken> GetTokenAsync(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken
        )
        {
            using var response = await _httpClient.GetAsync(_tokenUri, cancellationToken).ConfigureAwait(false);
            _ = response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            using var doc = JsonDocument.Parse(content);
            return new AccessToken(
                doc.RootElement.GetProperty("access_token").GetString()!,
                DateTimeOffset.UtcNow.AddHours(1)
            );
        }
    }
}
