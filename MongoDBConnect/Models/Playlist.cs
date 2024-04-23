using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace MongoDBConnect.Models;

public class Playlist 
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id {get; set;} = null!; //might be wierd based off how it is stored in db and table

    [BsonElement("user")]
    [JsonPropertyName("user")]
    public string user {get; set;} = null;

    [BsonElement("website-name")] //is based off of what the db field is called?
    [JsonPropertyName("website-name")] //same here
    public string WebsiteName {get; set;} = null!;

    [BsonElement("website-username")]
    [JsonPropertyName("website-username")]
    public string WebsiteUsername {get; set;} = null!;

    [BsonElement("website-password")]
    [JsonPropertyName("website-password")]
    public string WebsitePassword {get; set;} = null!;

}