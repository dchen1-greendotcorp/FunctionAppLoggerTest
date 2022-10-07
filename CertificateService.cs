using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionAppLoggerTest
{
    public class CertificateService : ICertificateService
    {
        private CertificateClient _certificateClient;
        private readonly ILogger<CertificateService> _logger;

        public CertificateService(IConfiguration configuration, ILogger<CertificateService> logger)
        {
            // Create a new certificate client using the default credential from Azure.Identity using environment variables previously set,
            // including AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, and AZURE_TENANT_ID.

            ClientSecretCredential clientCredential = new ClientSecretCredential(configuration["tenant"], configuration["appId"],
                configuration["aad:clientSecret"]);// configuration["password"]);
            _certificateClient = new CertificateClient(vaultUri: new Uri(configuration["vaultUrl"]), credential: clientCredential);
            this._logger = logger;

            //_certificateClient = new CertificateClient(vaultUri: new Uri(configuration["vaultUrl"]), credential: new DefaultAzureCredential());
        }

        public async Task<KeyVaultCertificateWithPolicy> CreateKvCertificate(string kvcertificateName)
        {
            var operation = await _certificateClient.StartCreateCertificateAsync(kvcertificateName, CertificatePolicy.Default);
            while (!operation.HasCompleted)
            {
                Thread.Sleep(2000);

                operation.UpdateStatus();
            }

            KeyVaultCertificateWithPolicy certificate = operation.Value;
            return certificate;
        }

        public async Task<X509Certificate2> AddKvCertificateToLocal(string kvcertificateName)
        {
            X509Certificate2 x509 = await GetX590CertificateFromKV(kvcertificateName).ConfigureAwait(false);
            if (x509 == null)
            {
                throw new ArgumentNullException($"{kvcertificateName} does not exist.");
            }

            var thum = x509.Thumbprint;

            X509Certificate2 existCert = null;
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
            {
                existCert = store.Certificates.FirstOrDefault(c => c.Thumbprint.Equals(thum));
            }

            if (existCert != null)
            {
                _logger.LogInformation($"System find the certificate with Thumbprint {existCert.Thumbprint} at X509Store");
                return existCert;
            }

            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
            {
                store.Add(x509);

                _logger.LogInformation($"System create the certificate with Thumbprint {x509.Thumbprint} at X509Store");
            }

            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
            {
                var cert = store.Certificates.FirstOrDefault(c => c.Thumbprint.Equals(thum));
                _logger.LogInformation($"System find the certificate with Thumbprint {cert.Thumbprint} at X509Store");

                return cert;
            }
        }

        private async Task<X509Certificate2> GetX590CertificateFromKV(string kvcertificateName)
        {
            var result = await _certificateClient.DownloadCertificateAsync(kvcertificateName).ConfigureAwait(false);

            return result.Value;

            //KeyVaultCertificateWithPolicy certificateWithPolicy = await _certificateClient.GetCertificateAsync(kvcertificateName).ConfigureAwait(false);
            //var cert_content = certificateWithPolicy.Cer;
            //X509Certificate2 x509 = new X509Certificate2(cert_content);
            //return x509;
        }

        //public async Task AddCertificateToLocal(string certificateName, string clientId, string clientSecret, string keyVaultAddress, string tenantId)
        //{
        //    var x509cert = await GetCertificate(certificateName, clientId, clientSecret, keyVaultAddress, tenantId);


        //    using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
        //    {
        //        store.Add(x509cert);
        //    }

        //}


        //public async Task<X509Certificate2> GetCertificate(string certificateName, string clientId, string clientSecret, string keyVaultAddress, string tenantId)
        //{
        //    ClientSecretCredential clientCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        //    var secretClient = new SecretClient(new Uri(keyVaultAddress), clientCredential);
        //    var response = await secretClient.GetSecretAsync(certificateName);
        //    var keyVaultSecret = response?.Value;
        //    if (keyVaultSecret != null)
        //    {
        //        var privateKeyBytes = Convert.FromBase64String(keyVaultSecret.Value);
        //        return new X509Certificate2(privateKeyBytes);
        //    }
        //    return null;
        //}
    }
}
