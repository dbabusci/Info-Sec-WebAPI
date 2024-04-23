using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace MongoDBConnect.Models;

public class Playlist 
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ID {get; set;} = null!; //might be wierd based off how it is stored in db and table

    [BsonElement("website-name")] //is baded off of what the db field is called?
    [JsonPropertyName("website-name")] //same here
    public List<string> WebsiteName {get; set;} = null!;

    [BsonElement("website-username")]
    [JsonPropertyName("website-username")]
    public List<string> WebsiteUsername {get; set;} = null!;

    [BsonElement("website-password")]
    [JsonPropertyName("website-password")]
    public List<string> WebsitePassword {get; set;} = null!;

}