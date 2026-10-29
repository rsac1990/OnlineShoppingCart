namespace OnlineShopping.Domain.Cart.Data;

using MongoDB.Driver;
using OnlineShopping.Domain.Cart.Business.Entities;
using MongoDB.Bson.Serialization;

public class DBContext
{
    private readonly IMongoDatabase _database;

    static DBContext()
    {
        BsonClassMap.RegisterClassMap<Entity>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(e => e.Id);
        });

        BsonClassMap.RegisterClassMap<Cart>(cm =>
        {
            cm.AutoMap();
            cm.MapField("_items").SetElementName("Items");
        });

        BsonClassMap.RegisterClassMap<Item>(cm =>
        {
            cm.AutoMap();
        });
    }

    public DBContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Cart> Carts => _database.GetCollection<Cart>("Carts");
}
