using System;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using Newtonsoft.Json;

namespace IbukiBooruLibrary.Classes; 

public class Extension {
    public string Name { get; set; } = "";
    public string Kind { get; set; } = "";
    public string ApiType { get; set; } = "";
    public string BaseURL { get; set; } = "";
    [JsonIgnore]
    public Uri BaseURI => new Uri(BaseURL);
    public string TagsSeparator { get; set; } = "";

    protected Extension() { }
        
    public Extension(JsValue obj) {
        try {
            ObjectInstance? evaluated = obj.AsObject();

            Name = evaluated.Get("name").AsString();
            Kind = evaluated.Get("kind").AsString();
            ApiType = evaluated.Get("api_type").AsString();
            BaseURL = evaluated.Get("base_url").AsString();
            TagsSeparator = evaluated.Get("tags_separator").AsString();
        } catch (Exception) {
            throw new NotSupportedException("Unsupported extension!");
        }

    }
}