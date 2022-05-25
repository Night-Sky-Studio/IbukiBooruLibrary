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

namespace IbukiBooruLibrary;

public class BooruException : Exception {
    public BooruException(string message) : base(message) { }
}

public class Booru : Extension {
    private Engine _ScriptEngine { get; }

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
        
    public Booru(string Script) {
        _ScriptEngine = new Engine(config => config.AllowClr(typeof(URL).Assembly))
            .Execute(Script);

        JsValue extension = _ScriptEngine.Evaluate("Extension");
            
        try {
            ObjectInstance ExtensionObject = extension.AsObject();

            Name = ExtensionObject.Get("name").AsString();
            ApiType = ExtensionObject.Get("api_type").AsString();
            BaseURL = ExtensionObject.Get("base_url").AsString();
            TagsSeparator = ExtensionObject.Get("tags_separator").AsString();
        } catch (Exception) {
            throw new NotSupportedException("Can't create extension using provided script!");
        }
    }
}
