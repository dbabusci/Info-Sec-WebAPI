using MongoDBConnect.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics.Eventing.Reader;

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
    /*
    public async Task CreateAsync(Playlist playlist) 
    {
        await _PlaylistCollection.InsertOneAsync(playlist);
        return;
    }

    public async Task<List<Playlist>> GetAsync()
    {
        return await _PlaylistCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task AddToPlaylistAsync(string id, string user, string websiteName, string websiteUsername, string websitePassword)
    {
        FilterDefinition<Playlist> filter = Builders<Playlist>.Filter.Eq("ID", id); //in db or playliost
        UpdateDefinition<Playlist> updateUser = Builders<Playlist>.Update.AddToSet<string>("User", websiteName);
        UpdateDefinition<Playlist> updateName = Builders<Playlist>.Update.AddToSet<string>("WebsiteName", websiteName);
        UpdateDefinition<Playlist> updateUsername = Builders<Playlist>.Update.AddToSet<string>("WebsiteUsername", websiteUsername); //will these even work
        UpdateDefinition<Playlist> updatePassword = Builders<Playlist>.Update.AddToSet<string>("WebsitePasswords", websitePassword);
        await _PlaylistCollection.UpdateOneAsync(filter, updateUser);
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
    */

    public async Task<List<Playlist>> GetAsync() => await _PlaylistCollection.Find(_ => true).ToListAsync();

    public async Task<Playlist?> GetAsync(string id) => await _PlaylistCollection.Find(x => x._id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Playlist newEntry) => await _PlaylistCollection.InsertOneAsync(newEntry);

    public async Task UpdateAsync(string id, Playlist updatedEntry) => await _PlaylistCollection.ReplaceOneAsync(x => x._id == id, updatedEntry);

    public async Task DeleteAsync(string id) => await _PlaylistCollection.DeleteOneAsync(x => x._id == id);
}