using MongoDBConnect.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;
using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace MongoDBConnect.Services;

public class AuthServices 
{
    private readonly IMongoCollection<Auth> _AuthCollection;
    private static byte[] Key;
    private static byte[] IV;

    public AuthServices(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.Connection); //was connection URI in guide dc
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _AuthCollection = database.GetCollection<Auth>(mongoDBSettings.Value.CollectionName); //Change to fit auth collection

        using(Aes aesAlg = Aes.Create())
        {
            Key = aesAlg.Key;
            IV = aesAlg.IV;
        }
    }

    //EncryptString and DecryptString are ctrl c + v from ApplicationServices
    //Maybe make them their own class but thats low on the todo
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

    //Login
    public async Task<List<Auth>> LoginAsync(string user, string pass) //maybe make bool whether person is found
    {
        List<Auth> userExists = await _AuthCollection.Find(x => x.UserAuth == user && x.PasswordAuth == pass).ToListAsync();
        return userExists;
    }

    //Register
    public async Task RegisterAsync(Auth newUser) 
    {
        string s = newUser.PasswordAuth;
        byte[] b = EncryptString(s, Key, IV);
        string r = Convert.ToBase64String(b);
        newUser.PasswordAuth = r;
        await _AuthCollection.InsertOneAsync(newUser);
    }
    
    //Get all
    public async Task<List<Auth>> GetAsync() {
        return await _AuthCollection.Find(_ => true).ToListAsync();
    }
}