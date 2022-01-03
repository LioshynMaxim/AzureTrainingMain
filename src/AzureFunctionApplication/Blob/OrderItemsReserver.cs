using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AzureFunctionApplication.Blob
{
    public class OrderItemsReserver
    {
        private static IConfiguration _configuration { get; set; }

        public OrderItemsReserver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private const string containerName = "lioshyncontainer";
        private const string connectionStringServiceBus = "Endpoint=sb://servicebusslioshyn.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=iSoip23cBSXcEEdViM2wAeRvWazaAME/x2hOFxoMws8=";
        private const string connectionStringBlob = "DefaultEndpointsProtocol=https;AccountName=lioshynstorageaccount;AccountKey=6WFGWqX6f5q7TUf0EgOUsbb6SkoTWc6c8Iv9n5LLe5b9sLWJqGxt7oigbgqu2lIewpEhbGtv7qPhCXuDxXRSbA==;BlobEndpoint=https://lioshynstorageaccount.blob.core.windows.net/;TableEndpoint=https://lioshynstorageaccount.table.core.windows.net/;QueueEndpoint=https://lioshynstorageaccount.queue.core.windows.net/;FileEndpoint=https://lioshynstorageaccount.file.core.windows.net/";
        [FunctionName("OrderItemsReserver")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            OrderData orderData = new OrderData()
            {
                OrderID = req.Query["id"],
                Quantity = int.Parse(req.Query["quantity"])
            };

            string namefile = Guid.NewGuid().ToString("n");
            await WriteInServiceBus(orderData);
            await CreateBlob(namefile + ".json", orderData, log);
            return new OkObjectResult($"Hello, { orderData.OrderID} { orderData.Quantity} the time now is :" + DateTime.Now.Date
                                                   + Environment.NewLine + JsonConvert.SerializeObject(orderData));
        }

        private async static Task CreateBlob(string name, OrderData data, ILogger log)
        {

            var connectionString = _configuration["AzureBlob"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);
            CloudBlockBlob blob;

            await container.CreateIfNotExistsAsync();
            blob = container.GetBlockBlobReference(name);
            blob.Properties.ContentType = "application/json";
            _ = blob.UploadFromStreamAsync(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))));
        }

        private async Task WriteInServiceBus(OrderData data) 
        {
            await using var client = new ServiceBusClient(connectionStringServiceBus);
            ServiceBusSender sender = client.CreateSender("servicebus");
            var message = new ServiceBusMessage(JsonConvert.SerializeObject(data));
            await sender.SendMessageAsync(message);
        }
    }
}
