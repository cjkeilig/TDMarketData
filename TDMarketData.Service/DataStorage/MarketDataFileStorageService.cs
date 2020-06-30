using CsvHelper;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TDMarketData.Domain.TableStorageDto;

namespace TDMarketData.Service.DataStorage
{
    public class MarketDataFileStorageService : IMarketDataFileStorageService
    {
        private StorageApiSettings _storageApiSettings;
        private string symbolVolumeFileName = "symbol_volume_snapshot.json";

        public MarketDataFileStorageService(StorageApiSettings storageApiSettings)
        {
            _storageApiSettings = storageApiSettings;
        }
        public async Task<JObject> GetSymbolVolumeSnapshot()
        {
            var cloudFile = await GetCloudFile(symbolVolumeFileName);

            var content = await cloudFile.DownloadTextAsync();

            if (!string.IsNullOrEmpty(content))
            {
                var jObject = JObject.Parse(content);

                return jObject;
            }

            return new JObject();
        }

        public async Task SaveSymbolVolumeSnapshot(JObject symbolVolumeSnapshot)
        {
            var content = symbolVolumeSnapshot.ToString();
            var bytes = Encoding.UTF8.GetBytes(content);
            var cloudFile = await GetCloudFile(symbolVolumeFileName);

            var ranges = await cloudFile.ListRangesAsync();
            ranges.ToList().ForEach(r => cloudFile.ClearRange(r.StartOffset, r.EndOffset));

            var cloudFileStream = await cloudFile.OpenWriteAsync(bytes.Length);
            await cloudFileStream.WriteAsync(bytes, 0, bytes.Length);
            cloudFileStream.Close();

        }
        public async Task SaveCandles(IEnumerable<Candle> candles)
        {
            var cloudFile = await GetLastCloudFileByType("underlying_candle");

            await SaveToEntitiesToCloudFile(candles, cloudFile);
        }

        public async Task SaveOptions(IEnumerable<OptionCandle> options)
        {
            var cloudFile = await GetLastCloudFileByType("option_candle");

            await SaveToEntitiesToCloudFile(options, cloudFile);
        }

        private async Task SaveToEntitiesToCloudFile<T>(IEnumerable<T> records, CloudFile cloudFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        if (cloudFile.Properties.Length > 0)
                            csv.Configuration.HasHeaderRecord = false;

                        csv.WriteRecords(records);
                        writer.Flush();

                        var bytes = memoryStream.Position;
                        memoryStream.Position = 0;

                        cloudFile.Resize(cloudFile.Properties.Length + bytes);

                        using (var fileStream = cloudFile.OpenWrite(null))
                        {
                            fileStream.Seek(bytes * -1, SeekOrigin.End);
                            await fileStream.WriteAsync(memoryStream.ToArray(), 0, (int)bytes);

                        }
                    }
                }


            }

            cloudFile.CloseAllHandlesSegmented();
        }

        private async Task<CloudFile> GetLastCloudFileByType(string fileType)
        {
            var maxFileLength = 10E9; // 10 gig max file

            var cloudDataDir = GetCloudDataDir();

            var folderListing = cloudDataDir.ListFilesAndDirectories();
            var filesWithType = folderListing.ToList().OfType<CloudFile>().Where(c => c.Name.StartsWith(fileType, StringComparison.OrdinalIgnoreCase)).ToList();

            if (filesWithType.Count > 0)
            {
                var lastFile = filesWithType.OrderBy(f => f.Properties.CreationTime).Last();

                if (lastFile.Properties.Length < maxFileLength)
                    return lastFile;

            }

            var fileName = fileType + "_" + string.Format("{0:yyyyMMddHHmm}", DateTime.Now) + ".txt";
            var cloudFile = cloudDataDir.GetFileReference(fileName);

            await cloudFile.CreateAsync(0);
            return cloudFile;
        }

        private async Task<CloudFile> GetCloudFile(string fileName)
        {
            var cloudDataDir = GetCloudDataDir();

            // Ensure that the directory exists.
            if (cloudDataDir.Exists())
            {
                // Get a reference to the file we created previously.
                CloudFile file = cloudDataDir.GetFileReference(fileName);

                // Ensure that the file exists.
                if (!file.Exists())
                {
                    await file.CreateAsync(0);
                }

                return file;

            }

            return null;
        }

        private CloudFileDirectory GetCloudDataDir()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageApiSettings.StorageConnectionString);
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference(Environment.GetEnvironmentVariable("WEBSITE_CONTENTSHARE"));

            // Ensure that the share exists.
            if (share.Exists())
            {
                // Get a reference to the root directory for the share.
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                // Get a reference to the directory we created previously.
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("data");

                return sampleDir;
            }
            else
            {
                throw new Exception("Cloud Share doesn't exist");
            }
        }
    }

}