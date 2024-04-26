using MongoDBConnect.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;
using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace MongoDBConnect.Services;

public class ApplicationServices 
{
    private readonly IMongoCollection<Playlist> _PlaylistCollection;
    private static byte[] Key;
    private static byte[] IV;

    public ApplicationServices(IOptions<MongoDBSettings> mongoDBSettings) 
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.Connection); //was connection URI in guide dc
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _PlaylistCollection = database.GetCollection<Playlist>(mongoDBSettings.Value.CollectionName);

        using(Aes aesAlg = Aes.Create())
        {
            Key = aesAlg.Key;
            IV = aesAlg.IV;
        } 
    }

    static byte[] EncryptString(string PlainText, byte[] Key, byte[] IV) 
    {
        byte[] encrypted;

        using(Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key; 
            aesAlg.IV = IV;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using(MemoryStream m = new MemoryStream())
            {
                using(CryptoStream c = new CryptoStream(m, encryptor, CryptoStreamMode.Write))
                {
                    using(StreamWriter s = new StreamWriter(c))
                    {
                        s.Write(PlainText);
                    }
                    encrypted = m.ToArray();
                }
            }
        }
        return encrypted;
    }

    static string DecryptString(byte[] CipherText, byte[] Key, byte[] IV)
    {
        if(CipherText == null) {
            Console.WriteLine("Null in decrypt");
        }

        string PlainText = "";

        using(Aes aesAlg =  Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using(MemoryStream m = new MemoryStream(CipherText))
            {
                using(CryptoStream c = new CryptoStream(m, decryptor, CryptoStreamMode.Read))
                {
                    using(StreamReader s = new StreamReader(c))
                    {
                        PlainText = s.ReadToEnd();
                    }
                }
            }
        }
        return PlainText;
    }

    public async Task<List<Playlist>> GetAsync() {
        return await _PlaylistCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Playlist?> GetAsync(string id) => await _PlaylistCollection.Find(x => x._id == id).FirstOrDefaultAsync();

    public async Task<List<Playlist>> GetUserEntriesAsync(string user) {
        List<Playlist> avail = await _PlaylistCollection.Find(x => x.user == user).ToListAsync();
        byte[] s;
        for(int i = 0; i < avail.Count; ++i) {
            s = Convert.FromBase64String(avail[i].WebsitePassword);
            avail[i].WebsitePassword = DecryptString(s, Key, IV);
        }
        return avail;
    }

    //Clean this up when functional
    public async Task CreateAsync(Playlist newEntry) {
        string s = newEntry.WebsitePassword;
        //Console.WriteLine(s);
        byte[] b = EncryptString(s, Key, IV);
        //Console.WriteLine(b);
        string r = Convert.ToBase64String(b);
        newEntry.WebsitePassword = r;
        //Console.WriteLine(newEntry.WebsitePassword);
        await _PlaylistCollection.InsertOneAsync(newEntry);
    } 

    public async Task UpdateAsync(string id, Playlist updatedEntry) => await _PlaylistCollection.ReplaceOneAsync(x => x._id == id, updatedEntry);

    public async Task DeleteAsync(string id) => await _PlaylistCollection.DeleteOneAsync(x => x._id == id);
}