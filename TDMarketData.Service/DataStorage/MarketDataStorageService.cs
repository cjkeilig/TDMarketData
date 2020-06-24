using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDMarketData.Domain.TableStorageDto;
using TDMarketData.Service.DataStorage;

namespace TDMarketData.Service
{
    public class MarketDataStorageService
    {
        private readonly TableStorageApiSettings _tableStorageApiSettings;
        public MarketDataStorageService(TableStorageApiSettings tableStorageApiSettings)
        {
            _tableStorageApiSettings = tableStorageApiSettings;
        }

        public async Task SaveCandles(IEnumerable<Candle> candles)
        {
            var candleEntities = candles.Select(c => new TableEntityAdapter<Candle>(c)).ToList();

            candleEntities.ForEach(c => { c.PartitionKey = c.OriginalEntity.Symbol; c.RowKey = c.OriginalEntity.Datetime; });

            var cloudTable = await CreateTableAsync("candledata1");

            await BatchInsertCandleRecords(cloudTable, candleEntities);
        }

        private async Task<CloudTable> CreateTableAsync(string tableName)
        {
            string storageConnectionString = _tableStorageApiSettings.StorageConnectionString;

            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            Console.WriteLine("Create a Table for the demo");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table named: {0}", tableName);
            }
            else
            {
                Console.WriteLine("Table {0} already exists", tableName);
            }

            Console.WriteLine();
            return table;
        }

        public CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        private async Task BatchInsertCandleRecords(CloudTable table,IEnumerable<TableEntity> entities)
        {
            try
            {
                // The following code  generates test data for use during the query samples.  
                var skip = 0;
                var operationsPerBatch = 100;

                var entitiesToSave = entities.Skip(skip).Take(operationsPerBatch);
                while (entitiesToSave.Count() > 0)
                {

                    TableBatchOperation batchOperation = new TableBatchOperation();

                    foreach (var entity in entitiesToSave)
                    {
                        batchOperation.InsertOrMerge(entity);
                    }

                    // Execute the batch operation.
                    IList<TableResult> results = await table.ExecuteBatchAsync(batchOperation);
                    foreach (var res in results)
                    {
                        var customerInserted = res.Result as TableEntity;
                        Console.WriteLine("Inserted entity with\t Etag = {0} and PartitionKey = {1}, RowKey = {2}", customerInserted.ETag, customerInserted.PartitionKey, customerInserted.RowKey);
                    }

                    skip += operationsPerBatch;
                    entitiesToSave = entities.Skip(skip).Take(operationsPerBatch);
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

    }
}
