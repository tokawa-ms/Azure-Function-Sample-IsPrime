using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace IsPrime
{
    public static class IsPrime
    {
        [FunctionName("IsPrime")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string number = req.Query["number"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            number = number ?? data?.number;

            if (number == null)
            {
                return new BadRequestObjectResult("Please set number param");
            }

            long num;
            try
            {
                if (!long.TryParse(number, out num)){
                    return new BadRequestObjectResult("Request Param Must Be in LongMax > number > 0");
                }

                if (num < 0)
                {
                    return new BadRequestObjectResult("Request Param Must Be in LongMax > number > 0");
                }
            }
            catch (Exception ex)
            {
                return new InternalServerErrorResult();
            }

            string responseMessage = isPrime(num)
                ? $"{{\"number\" : \"{number}\",\"isPrime\" : \"true\"}}"
                : $"{{\"number\" : \"{number}\",\"isPrime\" : \"false\"}}";

            return new OkObjectResult(responseMessage);
        }

        static bool isPrime(long num)
        {
            long i;

            if (num <= 1)
            {
                return false;
            }

            for (i = 2; i < Math.Sqrt(num); i++)
            {
                if (num % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
