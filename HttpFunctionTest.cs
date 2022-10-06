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
using FunctionAppLoggerTest.Models;
using System.Collections.Generic;

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

            //log.LogInformation($"using function log, msg={responseMessage}");

            //_gdlog.LogInformation($"using GDApplicationInsightsLogger _gdlog, _gdlog={responseMessage} at {DateTime.Now}");

            //_logger.LogInformation($"using ILogger<HttpFunctionTest> _logger, msg={responseMessage} at {DateTime.Now}");

            Account account1 = new Account();
            account1.Id = "0001";
            account1.FirstName = "David1";
            account1.LastName = "Chen1";
            account1.SSN = "678184379";

            Account account2 = new Account();
            account2.Id = "0002";
            account2.FirstName = "David2";
            account2.LastName = "Chen2";
            account2.SSN = "678194376";

            Dictionary<string, Account> dict= new Dictionary<string, Account>();
            dict[account1.Id] = account1;
            dict[account2.Id] = account2;

            _logger.LogInformation("Account is {Account}", account1);
            _logger.LogInformation("AccountDict is {AccountDict}", dict);

            //_logger.LogInformation("Name is {Name}", name);

            //log.LogTrace(responseMessage);

            //log.LogError("No ERROR!");
            //log.LogWarning("No WARN!");

            //var fakeException = new Exception("fake exception");

            //log.LogError(new EventId(1, "DavidTest"),fakeException, "fake exception message", null);

            //await Task.Delay(1000);

            return new OkObjectResult($"{responseMessage} at {DateTime.Now}");
        }
    }
}
