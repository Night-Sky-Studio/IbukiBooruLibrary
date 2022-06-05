using Newtonsoft.Json;

namespace IbukiBooruLibrary.Classes; 

public class Account {
    [JsonIgnore] private const string SALT = "choujin-steiner--";
    public int ID { get; set; }
    public bool IsAPIKey { get; set; }
    public bool IsActive { get; set; }
    public string Username { get; set; } = "";
    [JsonProperty] public string EncryptedPassword { get; private set; } = "";
    [JsonIgnore] public string Password { 
        get => StringEncryptor.Decrypt(EncryptedPassword, SALT); 
        set => EncryptedPassword = StringEncryptor.Encrypt(value, SALT);
    }
}