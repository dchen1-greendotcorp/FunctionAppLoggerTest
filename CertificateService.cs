using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionAppLoggerTest
{
    public class CertificateService : ICertificateService
    {
        CertificateClient _certificateClient;
        public CertificateService(IConfiguration configuration)
        {
            // Create a new certificate client using the default credential from Azure.Identity using environment variables previously set,
            // including AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, and AZURE_TENANT_ID.

            ClientSecretCredential clientCredential = new ClientSecretCredential(configuration["tenant"], configuration["appId"], configuration["password"]);
            _certificateClient = new CertificateClient(vaultUri: new Uri(configuration["vaultUrl"]), credential: clientCredential);

            //_certificateClient = new CertificateClient(vaultUri: new Uri(configuration["vaultUrl"]), credential: new DefaultAzureCredential());
        }

        public async Task<KeyVaultCertificateWithPolicy> CreateKvCertificate(string kvcertificateName)
        {
            // Create a certificate. This starts a long running operation to create and sign the certificate.
            //CertificateOperation operation = _certificateClient.StartCreateCertificate(certificateName, CertificatePolicy.Default);

            //// You can await the completion of the create certificate operation.
            //// You should run UpdateStatus in another thread or do other work like pumping messages between calls.
            //while (!operation.HasCompleted)
            //{
            //    Thread.Sleep(2000);

            //    operation.UpdateStatus();
            //}

            //KeyVaultCertificateWithPolicy certificate = operation.Value;
            //return certificate;

            var cert = await _certificateClient.GetCertificateAsync(kvcertificateName);
            if (cert == null)
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

            return cert;
        }

        public async Task<X509Certificate2> AddKvCertificateToLocal(string kvcertificateName)
        {
            X509Certificate2 x509 = await GetX590CertificateFromKV(kvcertificateName);
            var thum = x509.Thumbprint;

            X509Certificate2 existCert = null;
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
            {
                existCert = store.Certificates.FirstOrDefault(c => c.Thumbprint.Equals(thum));
            }

            if (existCert != null)
            {
                return existCert;
            }
            
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
            {
                store.Add(x509);
            }

            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
            {
                var cert = store.Certificates.FirstOrDefault(c => c.Thumbprint.Equals(thum));
                return cert;
            }
        }

        private async Task<X509Certificate2> GetX590CertificateFromKV(string kvcertificateName)
        {
            KeyVaultCertificateWithPolicy certificateWithPolicy = await _certificateClient.GetCertificateAsync(kvcertificateName);
            var cert_content = certificateWithPolicy.Cer;
            X509Certificate2 x509 = new X509Certificate2(cert_content);
            return x509;
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
