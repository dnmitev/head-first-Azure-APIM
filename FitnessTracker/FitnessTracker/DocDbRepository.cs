using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FitnessTracker
{
    public class DocDbRepository<T> : IDocDbRepository<T> where T : class
    {
        private readonly CosmosDB config;
        private readonly DocumentClient client;

        public DocDbRepository(IOptions<CosmosDB> config)
        {
            this.config = config.Value;

            this.client = new DocumentClient(new Uri(this.config.Endpoint), this.config.Key);
            this.CreateDatabaseIfNotExistsAsync().Wait();
            this.CreateCollectionIfNotExistsAsync().Wait();
        }
        
        public async Task<Document> CreateItemAsync(T item)
        {
            return await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(
                    this.config.DatabaseId, this.config.CollectionId),
                item);
        }

        public async Task DeleteItemAsync(string id)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(
                this.config.DatabaseId, 
                this.config.CollectionId, 
                id));
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                var doc = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(
                    this.config.DatabaseId, 
                    this.config.CollectionId,
                    id));

                return (T)(dynamic)doc;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            var query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(
                    this.config.DatabaseId, 
                    this.config.CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(
                    this.config.DatabaseId, 
                    this.config.CollectionId, 
                    id), 
                item);
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(this.config.DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = this.config.DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(
                    this.config.DatabaseId, 
                    this.config.CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(this.config.DatabaseId),
                        new DocumentCollection { Id = this.config.CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
