using MongoDB.Bson;

namespace NoSQLSkiServiceManager.Interfaces
{
    public interface IEntity
    {
        ObjectId Id { get; set; }
    }

}
