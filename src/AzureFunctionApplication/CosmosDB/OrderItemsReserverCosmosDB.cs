using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionApplication.CosmosDB
{
    public class OrderItemsReserverCosmosDB
    {
        private CosmosClient _cosmosClient;
        private Database _database;
        private Container _container;
        private const string _primaryKey = "Jbkx5Z3xb86zZzTXIC2ABMdJ5eEe9JKl36Pww8coQihTlH3PZKueEmNlej5yayKKzkARNl86jHc9PBVjnTx2Yw==";
        private const string _endpointUrl = "https://tetetelioshyn.documents.azure.com:443/";

        [FunctionName("OrderItemsReserverCosmosDB")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            ILogger log)
        {
            await CreateDatabase();

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var orderData = JsonConvert.DeserializeObject<OrderDataCosmosDB>(requestBody);
            var id = Guid.NewGuid();
            orderData.Id = id;

            try
            {
                await _container.CreateItemAsync(orderData, new PartitionKey(orderData.BuyerId));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return new BadRequestObjectResult($"Item in database with id: {orderData.BuyerId} already exists\n");
            }

            return new OkObjectResult($"Hello, {orderData.BuyerId} was added.");
        }

        private async Task CreateDatabase()
        {
            //var endpointUrl = Environment.GetEnvironmentVariable("AzureCosmosDBEndpointUrl");
            //var primaryKey = Environment.GetEnvironmentVariable("AzureCosmosDBPrimaryKey");
            var databaseId = "OrderDatabase";
            var containerId = "OrderContainer";
            _cosmosClient = new CosmosClient(_endpointUrl, _primaryKey);

            _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            _container = await _database.CreateContainerIfNotExistsAsync(containerId, "/BuyerId");
        }
    }
}
