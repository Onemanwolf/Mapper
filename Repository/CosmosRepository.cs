using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Azure.Identity;
using Mapper.Models;
using Mapper.Models.DTOs;

namespace Mapper.Repository
{

    public class CosmosRepository : ICosmosRepository
    {



        public string endpoint = "https://cosmosdb00.documents.azure.com:443/";
        public string connectionString;
        public string databaseName = "customformdb";
        public string containerName = "customform";
        string partitionKeyPath = "/formId";
        private CosmosClient cosmosClient;
        private ContainerResponse _containerResponse;
        private Container _container;
        private IConfiguration configuration;
        public CosmosRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration["CONNECTION_STRING_COSMOS"];
            //Run this command to set the environment variable
            //setx CONNECTION_STRING_COSMOS "" /M
            InitializeCosmosClientAsync(connectionString).Wait();

        }


        public async Task InitializeCosmosClientAsync(string connectionString)
        {
            var _connectionString = connectionString;

            cosmosClient = new CosmosClient(_connectionString);
            DatabaseResponse database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);

            _containerResponse = await database.Database.CreateContainerIfNotExistsAsync(containerName, partitionKeyPath, 1000);
            //ContainerResponse container = await database.Database.CreateContainerIfNotExistsAsync(containerName, "/formId", 1000);
            _container = _containerResponse.Container;


}

            public async Task<UserDTO> InsertItemAsync(UserDTO user)
            {

                // Insert the item into the container
                var response = await _container.CreateItemAsync<UserDTO>(user, new PartitionKey(user.Id));

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return user;
                }
                return null;


            }

            public async Task<UserDTO> GetUserAsync(string id)
            {
                try
                {
                    ItemResponse<UserDTO> response = await _container.ReadItemAsync<UserDTO>(id, new PartitionKey(id));
                    return response.Resource;
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
            }


        }

        public interface ICosmosRepository
        {

            Task<UserDTO> InsertItemAsync(UserDTO user);
            Task<UserDTO> GetUserAsync(string id);
        }
    }