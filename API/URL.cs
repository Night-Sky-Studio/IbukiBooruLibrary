using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace IbukiBooruLibrary.API;

/// <summary>
/// Since we got bamboozled by availability of HttpUtility.ParseQueryString, we're going to do it ourselves.
/// </summary>
public class QueryString {
    private Dictionary<string, string> _query { get; }

    public QueryString(string query) {
        if (query == "")
            _query = new Dictionary<string, string>();
        else
            _query = query.Split('&')
                .ToDictionary(
                    c => c.Split('=')[0], 
                    c => Uri.UnescapeDataString(c.Split('=')[1])
                );
    }

    public void Add(string key, string value) {
        _query.Add(key, value);
    }

    public void Remove(string key) {
        _query.Remove(key);
    }
    
    public override string ToString() {
        string result = "";
        
        for (int i = 0; i < _query.Count; i++) {
            KeyValuePair<string, string> kvp = _query.ElementAt(i);
            result += $"{kvp.Key}={kvp.Value}";
            if (i < _query.Count - 1) {
                result += "&";
            }
        }
        
        return result;
    }
}

/// <summary>
/// Custom URL class for scripts
/// Technically, it's missing proper URLBuilder implementation for C#
/// </summary>
public class URL {
    private QueryString _query { get; } = new QueryString("");
    private UriBuilder _uri { get; set; } = new UriBuilder();

    private string _baseUrl { get; set; }
    private string _path { get; set; }
        
    public Uri Uri => _uri.Uri;

    public override string ToString() => _uri.Uri.AbsoluteUri;

    private void UpdateInternalUri() {
        _uri = new UriBuilder(_baseUrl) {
            Path = _path,
            Query = _query.ToString()
        };
    }
        
    public URL() {
        _baseUrl = "";
        _path = "";
        UpdateInternalUri();
    }

    public URL(string baseUrl) {
        _baseUrl = baseUrl;
        _path = "";
        UpdateInternalUri();
    }

    public URL(string baseUrl, string path) {
        _baseUrl = baseUrl;
        _path = path;
        UpdateInternalUri();
    }
        
    public URL(string baseUrl, string path, Dictionary<string, string> args) {
        _baseUrl = baseUrl;
        _path = path;

        foreach (var pair in args) {
            _query.Add(pair.Key, pair.Value);
        }

        UpdateInternalUri();
    }
        
    public void AppendQueryParam(string Key, string Value) {
        _query.Add(Key, Value);
        UpdateInternalUri();
    }

    /// Appends & or ? symbols automatically
    public void AppendString(string value) {
        if (_uri.Query == "") 
            _uri.Query += $"?{value}";
        else
            _uri.Query += $"&{value}";
    }
}