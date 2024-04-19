using MongoDBConnect.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDBConnect.Services;

public class ApplicationServices 
{
    private readonly IMongoCollection<Playlist> _PlaylistCollection;  
}