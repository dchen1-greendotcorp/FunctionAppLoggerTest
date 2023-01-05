using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GreenDotLogger;
using Microsoft.ApplicationInsights;

namespace FunctionAppLoggerTest
{
    public class CertificateTest
    {
        private readonly ICertificateService _certificateService;

        public CertificateTest(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        //[FunctionName("CreateCertificate")]
        //public async Task<IActionResult> Run(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        //    ILogger log)
        //{
        //    string name = req.Query["name"];

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    name = name ?? data?.name;

        //    if (string.IsNullOrEmpty(name))
        //    {
        //        name = "mycertificatetest";
        //    }

        //    await _certificateService.CreateKvCertificate(name);

        //    string responseMessage = $"Certificate {name} created successfully";

        //    log.LogInformation($"using function log, msg={responseMessage}");
        //    return new OkObjectResult(responseMessage);
        //}

        [FunctionName("AddKvCertificateTolocal")]
        public async Task<IActionResult> AddKvCertificateToLocal(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            if (string.IsNullOrEmpty(name))
            {
                name = "davidcerttest";
            }

            try
            {
                var cert = await _certificateService.AddKvCertificateToLocal(name);
                if (cert != null)
                {
                    var responseMessage = $"Certificate {name} with Thumbprint '{cert.Thumbprint}' add to local successfully at {DateTime.Now}";
                    log.LogInformation($"{responseMessage}");
                    return new OkObjectResult(responseMessage);
                }
                log.LogError($"Create certificate {name} failed at {DateTime.Now}.");
                return new OkObjectResult($"Create certificate {name} failed at {DateTime.Now}.");
            }
            catch (Exception e)
            {
                log.LogError(new EventId(1, "DavidTest"), e, e.Message, null);
                throw;
            }
        }
    }
}
