using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example
{
    public abstract class MongoDB
    {
        const string connectionURI = "mongodb+srv://myAtlasDBUser:myatlasdbuser-01@myatlasclusteredu.6un7q.mongodb.net/?retryWrites=true&w=majority&appName=myAtlasClusterEDU";
        protected readonly MongoClient _client;
        protected readonly IMongoDatabase _database;

        public MongoDB()
        {
            MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionURI);
            // Set the ServerApi field of the settings object to set the version of the Stable API on the client
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            // Create a new client and connect to the server
            _client = new MongoClient(settings);

            _database = _client.GetDatabase("myAtlasClusterEDU");
        }

        public void Example()
        {
            // Send a ping to confirm a successful connection
            try
            {
                var result = _client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));

                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void ListDatabases()
        {
            try
            {
                // List all the databases in the cluster
                var databases = _client.ListDatabases().ToList();

                foreach (var database in databases) Console.WriteLine(database);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #region CRUD Functions

        public abstract void InsertOne();
        public abstract void InsertMany();
        public abstract void Find();
        public abstract void UpdateOne();
        public abstract void UpdateMany();
        public abstract void DeleteOne();
        public abstract void DeleteMany();

        #endregion

        public void Transactions()
        {
            using (var session = _client.StartSession())
            {
                session.WithTransaction((s, ct) =>
                {
                    //Define the sequence of operations to perform inside the transactions
                    return "Message";
                });
            }
        }

        #region Aggregation Functions

        public abstract void Match();
        public abstract void Group();
        public abstract void Sort();
        public abstract void Project();

        #endregion
    }

    public class BsonDocumentExample : MongoDB
    {
        private IMongoCollection<BsonDocument> AccountsCollection { get; set; }

        public BsonDocumentExample() : base()
        {
            AccountsCollection = _database.GetCollection<BsonDocument>("account");
        }

        #region CRUD Functions

        public override void InsertOne()
        {
            AccountsCollection.InsertOne(new BsonDocument
            {
                { "account_id", "MDB829001337" },
                { "account_holder", "Linus Torvalds" },
                { "account_type", "checking" },
                { "balance", 50352434m }
            });
        }

        public override void InsertMany()
        {
            AccountsCollection.InsertMany([
                new BsonDocument
                {
                    { "account_id", "MDB011235813" },
                    { "account_holder", "Ada Lovelace" },
                    { "account_type", "checking" },
                    { "balance", 60218m }
                },
                new BsonDocument
                {
                    { "account_id", "MDB829000001" },
                    { "account_holder", "Muhammad ibn Musa al-Khwarizmi" },
                    { "account_type", "savings" },
                    { "balance", 267914296m }
                }
            ]);
        }

        public override void Find()
        {
            var accounts = AccountsCollection.Find(Builders<BsonDocument>.Filter.Gt("balance", 1000m)).ToList();

            foreach (var account in accounts) Console.WriteLine(account);
        }

        public override void UpdateOne()
        {
            var result = AccountsCollection.UpdateOne(
                Builders<BsonDocument>.Filter.Eq("account_id", "MDB829001337"),
                Builders<BsonDocument>.Update.Set("balance", 5000m));

            if (result.IsAcknowledged)
                Console.WriteLine($"MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}");
        }

        public override void UpdateMany()
        {
            var result = AccountsCollection.UpdateMany(
                Builders<BsonDocument>.Filter.Eq("account_type", "checking"),
                Builders<BsonDocument>.Update.Inc("balance", 5m));

            if (result.IsAcknowledged)
                Console.WriteLine($"MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}");
        }

        public override void DeleteOne()
        {
            var result = AccountsCollection.DeleteOne(Builders<BsonDocument>.Filter.Eq("account_id", "MDB829001337"));

            if (result.IsAcknowledged)
                Console.WriteLine($"DeletedCount: {result.DeletedCount}");
        }

        public override void DeleteMany()
        {
            var result = AccountsCollection.DeleteMany(Builders<BsonDocument>.Filter.Lt("balance", 500m));

            if (result.IsAcknowledged)
                Console.WriteLine($"DeletedCount: {result.DeletedCount}");
        }

        #endregion

        #region Aggregation Functions

        public override void Match()
        {
            var accounts = AccountsCollection.Aggregate().Match(Builders<BsonDocument>.Filter.Lte("balance", 1000m)).ToList();

            foreach (var account in accounts) Console.WriteLine(account);
        }

        public override void Group()
        {
            var accounts = AccountsCollection
                .Aggregate()
                .Group(new BsonDocument
                {
                    { "_id", "$account_type" },
                    { "total", new BsonDocument("$sum", 1) }
                })
                .ToList();

            foreach (var account in accounts) Console.WriteLine(account);
        }

        public override void Sort()
        {
            var accounts = AccountsCollection.Aggregate().Sort(Builders<BsonDocument>.Sort.Descending("balance")).ToList();

            foreach (var account in accounts) Console.WriteLine(account);
        }

        public override void Project()
        {
            var accounts = AccountsCollection
                .Aggregate()
                .Project(new BsonDocument
                {
                    { "_id", 0 },
                    { "account_id", 1 },
                    { "account_type", 1 },
                    { "balance", 1 },
                    { "GBP", new BsonDocument("$divide", new BsonArray { "$balance", 1.3m }) }
                })
                .ToList();

            foreach (var account in accounts) Console.WriteLine(account);
        }

        #endregion
    }

    public class ModelExample : MongoDB
    {
        private IMongoCollection<Account> AccountsCollection { get; set; }

        public ModelExample() : base()
        {
            AccountsCollection = _database.GetCollection<Account>("account");
        }

        #region CRUD Functions

        public override void InsertOne()
        {
            AccountsCollection.InsertOne(new Account
            {
                AccountId = "MDB829001337",
                AccountHolder = "Linus Torvalds",
                AccountType = "checking",
                Balance = 50352434
            });
        }

        public async Task InsertOneAsync()
        {
            await AccountsCollection.InsertOneAsync(new Account
            {
                AccountId = "MDB829001337",
                AccountHolder = "Linus Torvalds",
                AccountType = "checking",
                Balance = 50352434
            });
        }

        public override void InsertMany()
        {
            AccountsCollection.InsertMany([
                new Account
                {
                    AccountId = "MDB829001337",
                    AccountHolder = "Linus Torvalds",
                    AccountType = "checking",
                    Balance = 50352434
                },
                new Account
                {
                    AccountId = "MDB011235813",
                    AccountHolder = "Ada Lovelace",
                    AccountType = "checking",
                    Balance = 60218
                }
            ]);
        }

        public async Task InsertManyAsync()
        {
            await AccountsCollection.InsertManyAsync([
                new Account
                {
                    AccountId = "MDB829001337",
                    AccountHolder = "Linus Torvalds",
                    AccountType = "checking",
                    Balance = 50352434
                },
                new Account
                {
                    AccountId = "MDB011235813",
                    AccountHolder = "Ada Lovelace",
                    AccountType = "checking",
                    Balance = 60218
                }
            ]);
        }

        public override void Find()
        {
            List<Account> accounts;
            accounts = AccountsCollection.Find(_ => true).ToList();
            accounts = AccountsCollection.Find(a => a.Balance > 1000).ToList();
            accounts = AccountsCollection.Find(Builders<Account>.Filter.Gt(a => a.Balance, 1000)).ToList();
        }

        public async Task FindAsync()
        {
            IAsyncCursor<Account> cursor;
            cursor = await AccountsCollection.FindAsync(a => a.Balance > 1000);
            var accounts = cursor.ToList();
        }

        public override void UpdateOne()
        {
            var result = AccountsCollection.UpdateOne(
                Builders<Account>.Filter.Eq(a => a.AccountId, "MDB829001337"),
                Builders<Account>.Update.Set(a => a.Balance, 5000));

            if (result.IsAcknowledged)
                Console.WriteLine($"MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}");
        }

        public async Task UpdateOneAsync()
        {
            var result = await AccountsCollection.UpdateOneAsync(
                Builders<Account>.Filter.Eq(a => a.AccountId, "MDB829001337"),
                Builders<Account>.Update.Set(a => a.Balance, 5000));

            if (result.IsAcknowledged)
                Console.WriteLine($"MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}");
        }

        public override void UpdateMany()
        {
            var result = AccountsCollection.UpdateMany(
                Builders<Account>.Filter.Eq("account_type", "checking"),
                Builders<Account>.Update.Inc("balance", 5m));

            if (result.IsAcknowledged)
                Console.WriteLine($"MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}");
        }

        public async Task UpdateManyAsync()
        {
            var result = await AccountsCollection.UpdateManyAsync(
                Builders<Account>.Filter.Eq("account_type", "checking"),
                Builders<Account>.Update.Inc("balance", 5m));

            if (result.IsAcknowledged)
                Console.WriteLine($"MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}");
        }

        public override void DeleteOne()
        {
            var result = AccountsCollection.DeleteOne(a => a.AccountId == "MDB829001337");

            if (result.IsAcknowledged)
                Console.WriteLine($"DeletedCount: {result.DeletedCount}");
        }

        public async Task DeleteOneAsync()
        {
            var result = await AccountsCollection.DeleteOneAsync(a => a.AccountId == "MDB829001337");

            if (result.IsAcknowledged)
                Console.WriteLine($"DeletedCount: {result.DeletedCount}");
        }

        public override void DeleteMany()
        {
            var result = AccountsCollection.DeleteMany(a => a.Balance < 500);

            if (result.IsAcknowledged)
                Console.WriteLine($"DeletedCount: {result.DeletedCount}");
        }

        public async Task DeleteManyAsync()
        {
            var result = await AccountsCollection.DeleteManyAsync(a => a.Balance < 500);

            if (result.IsAcknowledged)
                Console.WriteLine($"DeletedCount: {result.DeletedCount}");
        }

        #endregion

        #region Aggregation Functions

        public override void Match()
        {
            var accounts = AccountsCollection.Aggregate().Match(a => a.Balance <= 1000).ToList();
        }

        public override void Group()
        {
            var accounts = AccountsCollection
                .Aggregate()
                .Group(
                    a => a.AccountType,
                    r => new
                    {
                        AccountType = r.Key,
                        Total = r.Sum(_ => 1)
                    }
                )
                .ToList();
        }

        public override void Sort()
        {
            var accounts = AccountsCollection.Aggregate().SortByDescending(a => a.Balance).ToList();
        }

        public override void Project()
        {
            var accounts = AccountsCollection
                .Aggregate()
                .Project(Builders<Account>.Projection.Expression(a => new
                {
                    a.AccountId,
                    a.AccountType,
                    a.Balance,
                    GBP = a.Balance / 1.3m
                }))
                .ToList();
        }

        #endregion
    }

    //Plain Old C# Object (POCO)
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("account_id")]
        public string AccountId { get; set; }
        [BsonElement("account_holder")]
        public string AccountHolder { get; set; }
        [BsonElement("account_type")]
        public string AccountType { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        [BsonElement("balance")]
        public decimal Balance { get; set; }
        [BsonElement("transfers_complete")]
        public string[] TransfersComplete { get; set; }
    }
}
