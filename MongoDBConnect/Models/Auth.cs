using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace MongoDBConnect.Models;

public class Auth {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id {get; set;} = null!;

    [BsonElement("passwordAuth")]
    [JsonPropertyName("passwordAuth")]
    public string PasswordAuth {get; set;} = null!;

    [BsonElement("userAuth")]
    [JsonPropertyName("userAuth")]
    public string UserAuth {get; set;} = null!;
}