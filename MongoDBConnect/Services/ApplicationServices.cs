using MongoDBConnect.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDBConnect.Services;

public class ApplicationServices 
{
    private readonly IMongoCollection<Playlist> _PlaylistCollection;

    public ApplicationServices(IOptions<MongoDBSettings> mongoDBSettings) 
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.Connection); //was connection URI in guide dc
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _PlaylistCollection = database.GetCollection<Playlist>(mongoDBSettings.Value.CollectionName);
    }

    public async Task CreateAsync(Playlist playlist) 
    {
        await _PlaylistCollection.InsertOneAsync(playlist);
        return;
    }

    public async Task<List<Playlist>> GetAsync()
    {
        return await _PlaylistCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task AddToPlaylistAsync(string id, string websiteName, string websiteUsername, string websitePassword)
    {
        FilterDefinition<Playlist> filter = Builders<Playlist>.Filter.Eq("ID", id); //in db or playliost
        UpdateDefinition<Playlist> updateName = Builders<Playlist>.Update.AddToSet<string>("WebsiteName", websiteName);
        UpdateDefinition<Playlist> updateUsername = Builders<Playlist>.Update.AddToSet<string>("WebsiteUsername", websiteUsername); //will these even work
        UpdateDefinition<Playlist> updatePassword = Builders<Playlist>.Update.AddToSet<string>("WebsitePasswords", websitePassword);
        await _PlaylistCollection.UpdateOneAsync(filter, updateName);
        await _PlaylistCollection.UpdateOneAsync(filter, updateUsername);//huh wierd idk
        await _PlaylistCollection.UpdateOneAsync(filter, updatePassword);
        return;
    }

    public async Task DeleteAsync(string id) 
    {
        FilterDefinition<Playlist> filter = Builders<Playlist>.Filter.Eq("ID", id); //in db or playliost
        await _PlaylistCollection.DeleteOneAsync(filter);
        return;
    }
}