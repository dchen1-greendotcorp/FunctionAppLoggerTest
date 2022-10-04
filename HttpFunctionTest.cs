using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GreenDotShares;
using Microsoft.ApplicationInsights;

namespace FunctionAppLoggerTest
{
    public  class HttpFunctionTest
    {
        private readonly ILogger _gdlog;
        private readonly ILogger<HttpFunctionTest> _logger;

        public HttpFunctionTest( GDApplicationInsightsLoggerProvider provider, ILogger<HttpFunctionTest> logger)
        {
            this._gdlog=provider.CreateLogger("HttpFunctionAppTest.HttpFunctionTest");
            _logger=logger;
        }

        [FunctionName("HttpFunctionTest")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            log.LogInformation($"using function log, msg={responseMessage}");

            _gdlog.LogInformation($"using GDApplicationInsightsLogger _gdlog, _gdlog={responseMessage}");
            
            _logger.LogInformation($"using ILogger<HttpFunctionTest> _logger, msg={responseMessage}");

            log.LogTrace(responseMessage);

            log.LogError("No ERROR!");
            log.LogWarning("No WARN!");

            var fakeException = new Exception("fake exception");

            log.LogError(new EventId(1, "DavidTest"),fakeException, "fake exception message", null);

            await Task.Delay(1000);

            return new OkObjectResult(responseMessage);
        }
    }
}
