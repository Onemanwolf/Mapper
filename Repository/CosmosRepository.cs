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

    public class CosmosRepository : IRepository
    {
        public string endpoint = "https://cosmosdb00.documents.azure.com:443/";
        public string _connectionString;
        public string databaseName = "customformdb";
        public string containerName = "customform";
        string partitionKeyPath = "/formId";
        private CosmosClient _cosmosClient;
        private ICosmosDBClientFactory _cosmosDBClientFactory;
        private ContainerResponse _containerResponse;
        private Container _container;
        private IConfiguration configuration;
        private readonly ILogger<CosmosRepository> _logger;
        public CosmosRepository(IConfiguration configuration, ICosmosDBClientFactory cosmosDBClientFactory, ILogger<CosmosRepository> logger)
        {
            this.configuration = configuration;
            _connectionString = configuration["CONNECTION_STRING_COSMOS"];
            _cosmosDBClientFactory = cosmosDBClientFactory;
            _logger = logger;

            //Run this command to set the environment variable
            //setx CONNECTION_STRING_COSMOS "" /M
            InitializeCosmosClientAsync().Wait();

        }


        public async Task InitializeCosmosClientAsync()
        {


            _cosmosClient = await _cosmosDBClientFactory.CreateCosmosClient(_connectionString);

            DatabaseResponse database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);

            _containerResponse = await database.Database.CreateContainerIfNotExistsAsync(containerName, partitionKeyPath, 1000);
            //ContainerResponse container = await database.Database.CreateContainerIfNotExistsAsync(containerName, "/formId", 1000);
            _container = _containerResponse.Container;


        }

        public async Task<UserDTO> InsertItemAsync(UserDTO user)
        {

            //Try Catch block when leaving or entering the boundary of the application
            try
            {
                // Insert the item into the container
                var response = await _container.CreateItemAsync<UserDTO>(user, new PartitionKey(user.Id));

                _logger.LogInformation($"Inserted item in database with id: {user.Id}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return user;
                }
                return null;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return null;
            }


        }

        public async Task<UserDTO> GetUserAsync(string id)
        {
            try
            {
                ItemResponse<UserDTO> response = await _container.ReadItemAsync<UserDTO>(id, new PartitionKey(id));
                _logger.LogInformation($"Get item in database with id: {id}");
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<UserDTO> UpdateUserAsync(UserDTO user)
        {
            try
            {
                ItemResponse<UserDTO> response = await _container.UpsertItemAsync<UserDTO>(user, new PartitionKey(user.Id));
                _logger.LogInformation($"Updated item in database with id: {user.Id}");
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var sqlQueryText = "SELECT * FROM c";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                FeedIterator<UserDTO> queryResultSetIterator = _container.GetItemQueryIterator<UserDTO>(queryDefinition);

                List<UserDTO> users = new List<UserDTO>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<UserDTO> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (UserDTO user in currentResultSet)
                    {
                        users.Add(user);
                    }
                }
                _logger.LogInformation($"Get all users in database");
                return users;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
        // generate a method to get all users and order by name
        public async Task<List<UserDTO>> GetAllUsersOrderByNameAsync()
        {
            try
            {
                var sqlQueryText = "SELECT * FROM c ORDER BY c.Name";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                FeedIterator<UserDTO> queryResultSetIterator = _container.GetItemQueryIterator<UserDTO>(queryDefinition);

                List<UserDTO> users = new List<UserDTO>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<UserDTO> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (UserDTO user in currentResultSet)
                    {
                        users.Add(user);
                    }
                }

                return users;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }

    public interface IRepository
    {

        Task<UserDTO> InsertItemAsync(UserDTO user);
        Task<UserDTO> GetUserAsync(string id);
        Task<UserDTO> UpdateUserAsync(UserDTO user);
        Task<List<UserDTO>> GetAllUsersAsync();
    }
}
