using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Mono.Web;

namespace IbukiBooruLibrary.API; 

/// <summary>
/// Custom URL class for scripts
/// Technically, it's missing proper URLBuilder implementation for C#
/// </summary>
public class URL {
    private NameValueCollection _query { get; } = HttpUtility.ParseQueryString(string.Empty);
    private UriBuilder _uri { get; set; } = new UriBuilder();

    private string _baseUrl { get; set; }
    private string _path { get; set; }
        
    public Uri Uri => _uri.Uri;
    //public string href => _uri.Uri.AbsoluteUri;
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

    public void AppendString(string value) {
        _uri.Query += $"&{value}";
    }
}