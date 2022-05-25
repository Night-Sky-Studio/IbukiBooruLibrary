using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IbukiBooruLibrary.Classes;
public static class Network {
    private const string UserAgent = "HttpClient/10.0 (.NET) IbukiBooruLibrary/1.0.0 Ibuki/1.0.0.0";

    private static string FormatBooruString(string source, Dictionary<string, string> args) {
        foreach (Match match in Regex.Matches(source, @"{\s*(.*?)\s*}")) {       /// "{MATCH}"
            string token = match.Value.Replace("{", "").Replace("}", "");           /// "TOKEN"
            if (match.Value == "{}" || args[token] == "")
                source = source
                    .Replace(match.Value, args[token])
                    .Remove(source.IndexOf(token, StringComparison.Ordinal) - 2, 1);
            else
                source = source.Replace(match.Value, args[token]);
        }
        return source;
    }
    
    [Obsolete("Was used before, useless, since we use JS scripts now")]
    public static Uri FormatBooruUri(Uri baseURI, Dictionary<string, string> args) {
        string resultURL = Uri.UnescapeDataString(baseURI.AbsoluteUri);

        resultURL = FormatBooruString(resultURL, args);

        return new Uri(resultURL);
    }

    /// <summary>
    /// Asynchronously gets contents of an HTML file at the address
    /// </summary>
    /// <param name="URL">Target URL</param>
    /// <returns>Contents of response as string</returns>
    public static async Task<string> GET(string URL) {
        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        Uri URI = new Uri(URL);

        HttpResponseMessage Response = await Client.GetAsync(URI);

        if (Response.StatusCode != HttpStatusCode.OK)
            throw new HttpRequestException(await Response.Content.ReadAsStringAsync());

        return await Response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Asynchronously gets contents of an HTML file at the address
    /// </summary>
    /// <param name="uri">Uri representation of target URL</param>
    /// <returns>Contents of response as string</returns>
    public static async Task<string> GET(Uri uri) {
        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);

        HttpResponseMessage Response = await Client.GetAsync(uri);

        return await Response.Content.ReadAsStringAsync();
    }
}