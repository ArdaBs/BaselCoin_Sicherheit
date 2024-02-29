using MongoDB.Bson;
using MongoDB.Driver;
using NoSQLSkiServiceManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Service for managing MongoDB operations related to the initial database setup and indexing.
/// </summary>
public class MongoDBService
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;

    /// <summary>
    /// Initializes a new instance of the MongoDBService.
    /// </summary>
    /// <param name="connectionString">The MongoDB connection string.</param>
    /// <param name="databaseName">The name of the database to operate on.</param>
    public MongoDBService(string connectionString, string databaseName)
    {
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
    }

    /// <summary>
    /// Ensures that the database is set up with initial collections, documents, and indexes.
    /// </summary>
    public async Task EnsureDatabaseSetupAsync()
    {
        var collections = await _database.ListCollectionNames().ToListAsync();

        await CreateCollectionIfNotExistsAsync("accountHolders", EmployeeSchema);
        await InitializeEmployeesAsync();

        await CreateUsersAsync();
    }

    /// <summary>
    /// Creates indexes for the employees collection, including a unique index on username.
    /// </summary>
    public async Task CreateEmployeeIndexesAsync()
    {
        var employeeCollection = _database.GetCollection<AccountHolder>("accountHolders");
        var indexList = await employeeCollection.Indexes.ListAsync();
        var indexes = await indexList.ToListAsync();
        var usernameIndexExists = indexes.Any(index =>
            index["key"].AsBsonDocument.Contains("username"));

        if (!usernameIndexExists)
        {
            var usernameIndexKeysDefinition = Builders<AccountHolder>.IndexKeys.Ascending(employee => employee.Username);
            var usernameIndexModel = new CreateIndexModel<AccountHolder>(usernameIndexKeysDefinition, new CreateIndexOptions { Unique = true });

            await employeeCollection.Indexes.CreateOneAsync(usernameIndexModel);
        }
        else
        {
            Console.WriteLine("Index on 'username' already exists.");
        }
    }


    /// <summary>
    /// Creates a MongoDB collection with the specified name and schema if it does not already exist.
    /// </summary>
    /// <param name="collectionName">Name of the MongoDB collection to create.</param>
    /// <param name="schema">JSON schema to apply for collection validation.</param>
    private async Task CreateCollectionIfNotExistsAsync(string collectionName, BsonDocument schema)
    {
        var collectionList = _database.ListCollectionNames().ToListAsync().Result;
        if (!collectionList.Contains(collectionName))
        {
            await _database.CreateCollectionAsync(collectionName);
            if (schema != null)
            {
                var validator = new BsonDocument { { "$jsonSchema", schema } };
                var validationOptions = new BsonDocument
                {
                    { "collMod", collectionName },
                    { "validator", validator },
                    { "validationLevel", "moderate" },
                    { "validationAction", "warn" }
                };
                await _database.RunCommandAsync<BsonDocument>(validationOptions);
            }
        }
    }

    /// <summary>
    /// Initializes the employees collection with predefined employee documents if the collection is empty.
    /// </summary>
    public async Task InitializeEmployeesAsync()
    {
        var employeeCollection = _database.GetCollection<AccountHolder>("accountHolders");
        var exists = await employeeCollection.Find(_ => true).AnyAsync();
        if (!exists)
        {
            var employees = new List<AccountHolder>
        {
            new AccountHolder { Username = "Gapsch", Password = "12345678", IsLocked = false, FailedLoginAttempts = 0, Role = "Admin", Balance = 100 },
            new AccountHolder { Username = "Lukas", Password = "12345678", IsLocked = false, FailedLoginAttempts = 0, Role = "User", Balance = 100 },
            new AccountHolder { Username = "Goku", Password = "12345678", IsLocked = false, FailedLoginAttempts = 0, Role = "User", Balance = 100 },
            new AccountHolder { Username = "Gojo", Password = "12345678", IsLocked = false, FailedLoginAttempts = 0, Role = "User" , Balance = 100}
        };
            await employeeCollection.InsertManyAsync(employees);
            await CreateEmployeeIndexesAsync();
        }
    }



    private static readonly BsonDocument ServiceTypeSchema = new BsonDocument
{
    { "bsonType", "object" },
    { "required", new BsonArray { "Id", "name", "cost" } },
    { "properties", new BsonDocument
        {
            { "Id", new BsonDocument { { "bsonType", "string" }, { "description", "must be a string and is required" } } },
            { "name", new BsonDocument
                {
                    { "bsonType", "string" },
                    { "minLength", 1 },
                    { "maxLength", 100 },
                    { "description", "must be a string and is required" }
                }
            },
            { "cost", new BsonDocument
                {
                    { "bsonType", "decimal" },
                    { "minimum", 0 },
                    { "description", "must be a non-negative decimal and is required" }
                }
            }
        }
    }
};


    private static readonly BsonDocument ServicePrioritySchema = new BsonDocument
{
    { "bsonType", "object" },
    { "required", new BsonArray { "Id", "priorityName", "dayCount" } },
    { "properties", new BsonDocument
        {
            { "Id", new BsonDocument { { "bsonType", "string" }, { "description", "must be a string and is required" } } },
            { "priorityName", new BsonDocument
                {
                    { "bsonType", "string" },
                    { "minLength", 1 },
                    { "maxLength", 100 },
                    { "description", "must be a string and is required" }
                }
            },
            { "dayCount", new BsonDocument
                {
                    { "bsonType", "int" },
                    { "minimum", 0 },
                    { "description", "must be a non-negative integer and is required" }
                }
            }
        }
    }
};


    private static readonly BsonDocument ServiceOrderSchema = new BsonDocument
{
    { "bsonType", "object" },
    { "required", new BsonArray { "customerName", "email", "phoneNumber", "creationDate", "desiredPickupDate", "serviceType", "priority", "status" } },
    { "properties", new BsonDocument
        {
            { "customerName", new BsonDocument
                {
                    { "bsonType", "string" },
                    { "minLength", 1 },
                    { "maxLength", 255 },
                }
            },
            { "email", new BsonDocument
                {
                    { "bsonType", "string" },
                    { "pattern", "[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}" },
                }
            },
            { "phoneNumber", new BsonDocument
                {
                    { "bsonType", "string" },
                }
            },
        }
    }
};

        private static readonly BsonDocument EmployeeSchema = new BsonDocument
    {
        { "bsonType", "object" },
        { "required", new BsonArray { "username", "password", "isLocked", "failedLoginAttempts" } },
        { "properties", new BsonDocument
            {
                { "username", new BsonDocument { { "bsonType", "string" }, { "minLength", 3 }, { "description", "must be a non-empty string and is required" } } },
                { "password", new BsonDocument { { "bsonType", "string" }, { "minLength", 8 }, { "description", "must be a non-empty string and is required" } } },
                { "isLocked", new BsonDocument { { "bsonType", "bool" }, { "description", "must be a boolean and is required" } } },
                { "failedLoginAttempts", new BsonDocument { { "bsonType", "int" }, { "minimum", 0 }, { "description", "must be a non-negative integer and is required" } } }
            }
        }
    };


    

    /// <summary>
    /// Creates default user accounts for application access if they do not exist.
    /// </summary>
    public async Task CreateUsersAsync()
    {
        var adminDatabase = _client.GetDatabase("admin");

        var users = await adminDatabase.RunCommandAsync<BsonDocument>(new BsonDocument("usersInfo", 1));
        var usersArray = users["users"].AsBsonArray;

        var jetStreamApiMasterExists = usersArray.Any(u => u["user"].AsString == "JetStreamApiMaster" && u["db"].AsString == "admin");
        if (!jetStreamApiMasterExists)
        {
            var createUserCommand = new BsonDocument {
            { "createUser", "JetStreamApiMaster" },
            { "pwd", "SavePassword1234" },
            { "roles", new BsonArray {
                new BsonDocument { { "role", "dbOwner" }, { "db", "JetStreamAPI" } }
            }}
        };
            await adminDatabase.RunCommandAsync<BsonDocument>(createUserCommand);
        }

        var backendUserExists = usersArray.Any(u => u["user"].AsString == "BackendUser" && u["db"].AsString == "admin");
        if (!backendUserExists)
        {
            var createUserCommand = new BsonDocument {
            { "createUser", "BackendUser" },
            { "pwd", "BackendUserPassword123" },
            { "roles", new BsonArray {
                new BsonDocument { { "role", "readWrite" }, { "db", "JetStreamAPI" } }
            }}
        };
            await adminDatabase.RunCommandAsync<BsonDocument>(createUserCommand);
        }
    }
}