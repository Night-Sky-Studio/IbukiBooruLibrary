using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IbukiBooruLibrary.API;
using IbukiBooruLibrary.Classes;
using Jint;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;
using Newtonsoft.Json;

namespace IbukiBooruLibrary;

public class BooruException : Exception {
    public BooruException(string message) : base(message) { }
}

public class Booru : Extension {
    [JsonProperty]
    private string _Script { get; set; } = "";
    
    //private bool _NeedsInitialization { get; set; } = false;
    private Engine _ScriptEngine { get; }

    [JsonProperty]
    private int _ActiveAccountID;
    [JsonIgnore]
    public int ActiveAccountID {
        get => _ActiveAccountID;
        set => _ActiveAccountID = Accounts.TryGetValue(_ActiveAccountID, out _) ? value : -1;
    }
    [JsonIgnore]
    public Account? ActiveAccount => Accounts.TryGetValue(ActiveAccountID, out Account result) ? result : null;
    public Dictionary<int, Account> Accounts { get; } = new Dictionary<int, Account>();

    private string GetPostsURL(int page, int limit, string search, string auth) {
        return _ScriptEngine.Invoke("GetPostsURL", page, limit, search, auth).AsString();
    }

    private string GetPostURL(int id) {
        return _ScriptEngine.Invoke("GetPostURL", id).AsString();
    }

    private string GetUserFavoritesURL(int page, int limit, string username, string auth) {
        return _ScriptEngine.Invoke("GetUserFavoritesURL", page, limit, username, auth).AsString();
    }

    private string GetPostChildrenURL(int id, string auth) {
        return _ScriptEngine.Invoke("GetPostChildrenURL", id, auth).AsString();
    }

    private async Task<List<BooruPost>> GetPostsInternal(string url = "") {
        try {
            ArrayInstance PostsArray = _ScriptEngine.Invoke(ApiType == "json" ? "ParsePostsJSON" : "ParsePostXML", await Network.GET(url)).AsArray();

            return PostsArray.Select(t => new BooruPost(t)).ToList();
        } catch (HttpRequestException e) {
            JsValue error = _ScriptEngine.Invoke(ApiType == "json" ? "ParseErrorJSON" : "ParseErrorXML", e.Message);
            throw new BooruException($"{error.Get("error").AsString()}: {error.Get("message").AsString()}");
        }
    }
    
    public async Task<List<BooruPost>> GetPosts(int page = 1, int limit = 20, string search = "", string auth = "") {
        return await GetPostsInternal(GetPostsURL(page, limit, search, auth));
    }

    public async Task<List<BooruPost>> GetPostChildren(int id, string auth = "") {
        return await GetPostsInternal(GetPostChildrenURL(id, auth));
    }
    
    public async Task<List<BooruPost>> GetUserFavorites(int page = 1, int limit = 20, string username = "", string auth = "") {
        return await GetPostsInternal(GetUserFavoritesURL(page, limit, username, auth));
    }

    public async Task<BooruPost> GetPost(int id) {
        try {
            JsValue Post = _ScriptEngine.Invoke(ApiType == "json" ? "ParsePostJSON" : "ParsePostXML", await Network.GET(GetPostURL(id)));

            return new BooruPost(Post);
        } catch (Exception e) {
            JsValue error = _ScriptEngine.Invoke(ApiType == "json" ? "ParseErrorJSON" : "ParseErrorXML", e.Message);
            throw new BooruException($"{error.Get("error").AsString()}: {error.Get("message").AsString()}");
        }
    }
    
    #region Constructors
    /// <summary>
    /// Parameterless constructor for settings loading.
    /// </summary>
    /// <remarks>
    /// Since we use serialization of settings, we can not just deserialize our
    /// json back to settings object, the JavaScript engine won't be properly initialized
    /// and the extension script will not be loaded.
    /// 
    /// That solves the problem. We can create an instance of Booru, we just have to initialize script later in the code.
    /// </remarks>
    public Booru() {
        //_NeedsInitialization = true;
        _ScriptEngine = new Engine(config => config.AllowClr(typeof(URL).Assembly));
    }

    /// <summary>
    /// Initializes the extension script.
    /// </summary>
    /// <remarks>
    /// We can be sure that our object is created here and
    /// we have a <see cref="_Script"/> property deserialized.
    /// </remarks>
    public void Initialize() {
        try {
            _ScriptEngine.Execute(_Script);
            
            JsValue extension = _ScriptEngine.Evaluate("Extension");
                
            ObjectInstance ExtensionObject = extension.AsObject();

            Name = ExtensionObject.Get("name").AsString();
            Kind = ExtensionObject.Get("kind").AsString();
            ApiType = ExtensionObject.Get("api_type").AsString();
            BaseURL = ExtensionObject.Get("base_url").AsString();
            TagsSeparator = ExtensionObject.Get("tags_separator").AsString();
        } catch (Exception) {
            throw new NotSupportedException("Can't create extension using provided script!");
        }
    }
    
    public Booru(string Script) {
        //try {
            if (Script != "") _Script = Script;

            _ScriptEngine = new Engine(config => config.AllowClr(typeof(URL).Assembly));

            Initialize();
            //     .Execute(_Script);
            //
            // JsValue extension = _ScriptEngine.Evaluate("Extension");
            //
            // ObjectInstance ExtensionObject = extension.AsObject();
            //
            // Name = ExtensionObject.Get("name").AsString();
            // Kind = ExtensionObject.Get("kind").AsString();
            // ApiType = ExtensionObject.Get("api_type").AsString();
            // BaseURL = ExtensionObject.Get("base_url").AsString();
            // TagsSeparator = ExtensionObject.Get("tags_separator").AsString();
            // } catch (Exception) {
            //     throw new NotSupportedException("Can't create extension using provided script!");
            // }
    }
    #endregion
    
    #region Operators
    protected bool Equals(Booru other) {
        return _Script == other._Script;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Booru)obj);
    }

    public override int GetHashCode() {
        return _Script.GetHashCode();
    }
    public static bool operator ==(Booru a, Booru b) {
        return a._Script == b._Script;
    }
    
    public static bool operator !=(Booru a, Booru b) {
        return a._Script != b._Script;
    }
    #endregion
}
