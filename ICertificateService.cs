using Azure.Security.KeyVault.Certificates;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FunctionAppLoggerTest
{
    public interface ICertificateService
    {
        Task<KeyVaultCertificateWithPolicy> CreateKvCertificate(string kvcertificateName);
        Task<X509Certificate2> AddKvCertificateToLocal(string kvcertificateName);

    }
}
