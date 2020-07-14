using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TDMarketData.Service.DataStorage
{
    public class AccessTokenFileStorageService
    {
        private StorageApiSettings _storageApiSettings;

        public AccessTokenFileStorageService(StorageApiSettings storageApiSettings)
        {
            _storageApiSettings = storageApiSettings;

        }
    }
}
