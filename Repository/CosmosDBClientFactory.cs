using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Mapper.Repository
{
    public class CosmosDBClientFactory : ICosmosDBClientFactory
    {

        private CosmosClient _cosmosClient;





        public async Task<CosmosClient> CreateCosmosClient(string connectionString)
        {
            if (_cosmosClient == null){
               await Task.Run(() => { _cosmosClient = new CosmosClient(connectionString);} );
            }
            if (_cosmosClient != null){

                 return _cosmosClient;
            }
            else{

                throw new Exception("CosmosClient not initialized");
            }
        }
    }

    public interface ICosmosDBClientFactory
    {
        Task<CosmosClient> CreateCosmosClient(string connectionString);
    }
}