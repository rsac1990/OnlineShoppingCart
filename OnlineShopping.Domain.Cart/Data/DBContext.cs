namespace OnlineShopping.Domain.Cart.Data;

using Business.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

public class DBContext
{
    private readonly IMongoDatabase _database;

    static DBContext()
    {
        BsonClassMap.RegisterClassMap<Cart>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(c => c.Id);
            cm.MapField("_items").SetElementName("Items");
        });

        BsonClassMap.RegisterClassMap<Item>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(i => i.Id);
        });
    }

    public DBContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Cart> Carts => _database.GetCollection<Cart>("Carts");
}
