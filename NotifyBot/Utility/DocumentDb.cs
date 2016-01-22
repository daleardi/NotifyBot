using System;
using System.Linq;
using System.Threading.Tasks;

// Add DocumentDB references
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace NotifyBot.Utility
{
    public class DocumentDbRepository
    {
        private DocumentClient client { get; set; }
        private Database database { get; set; }
        private DocumentCollection collection { get; set; }
        public void Setup()
        {
            try
            {
                const string EndpointUrl = "https://daleardi.documents.azure.com:443/";
                const string AuthorizationKey = "kH6RAnNRSsUrGSvgn5dRQM6fJnfgi9O37uSLlnx/5M1zLNkIlLjGRtUk0gi0PrR++ptERRaKsn4iTpXP2kevnA==";

                // Create a new instance of the DocumentClient
                this.client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

                var databaseTask = this.ConnectToDatabase();
                databaseTask.Wait();
                this.database = databaseTask.Result;

                var collectionTask = this.GetCollection();
                collectionTask.Wait();
                this.collection = collectionTask.Result;
            }
            catch (Exception)
            {
                this.client.Dispose();
                throw new Exception("Error setting up database");

            }
        }

        private async Task<Database> ConnectToDatabase()
        {
            // Check to verify a database with the id=FamilyRegistry does not exist
            this.database = this.client.CreateDatabaseQuery().Where(db => db.Id == "Notify").AsEnumerable().FirstOrDefault();

            // If the database does not exist, create a new database
            if (this.database == null)
            {
                this.database = await this.client.CreateDatabaseAsync(
                    new Database
                    {
                        Id = "Notify"
                    });
            }

            return this.database;
        }

        private async Task<DocumentCollection> GetCollection()
        {
            // Check to verify a document collection with the id=FamilyCollection does not exist
            this.collection = this.client.CreateDocumentCollectionQuery("dbs/" + this.database.Id).Where(c => c.Id == "Notifications").AsEnumerable().FirstOrDefault();

            // If the document collection does not exist, create a new collection
            if (this.collection == null)
            {
                this.collection = await this.client.CreateDocumentCollectionAsync("dbs/" + this.database.Id,
                    new DocumentCollection
                    {
                        Id = "Notifications"
                    });
            }

            return this.collection;
        }

        public async Task<Document> CreateDocumentAsync(string documentId, object documentObject)
        {
            // Check to verify a document with the id=AndersenFamily does not exist
            var document = this.client.CreateDocumentQuery("dbs/" + this.database.Id + "/colls/" + this.collection.Id).Where(d => d.Id == documentId).AsEnumerable().FirstOrDefault();

            // If the document does not exist, create a new document
            if (document == null)
            {
                // id based routing for the first argument, "dbs/FamilyRegistry/colls/FamilyCollection"
                document = await this.client.CreateDocumentAsync("dbs/" + this.database.Id + "/colls/" + this.collection.Id, documentObject);
                return document;
            }

            return null;
        }

        public Document GetDocument(string documentId)
        {
            // Check to verify a document with the id=AndersenFamily does not exist
            var document =
                this.client.CreateDocumentQuery("dbs/" + this.database.Id + "/colls/" + this.collection.Id)
                    .Where(d => d.Id == documentId)
                    .AsEnumerable()
                    .FirstOrDefault();
            if (document == null)
            {
                throw new Exception("Document Name: " + documentId + " not found");
            }
            return document;
        }

        public void Dispose()
        {
            this.client.Dispose();
        }
    }
}
