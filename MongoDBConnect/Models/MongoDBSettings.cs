namespace MongoDBConnect.Models;

public class MongoDBSettings{
    public string Connection {get; set;} = null!;
    public string DatabaseName {get; set;} = null!;
    public string CollectionName {get; set;} = null!;
}