using System;
using System.Collections.Generic;
using Jint;
using Jint.Native;
using Jint.Native.Array;

namespace IbukiBooruLibrary.Classes; 

public class BooruPost {
    public int ID { get; }
    public string PreviewFileURL { get; }
    public string LargeFileURL { get; }
    public string DirectURL { get; }
    public Tags PostTags { get; }
    
    public Information PostInformation { get; }

    private List<Tag>? ProcessTags(JsValue? ParsedTags) {
        if (ParsedTags == null) return null;
        try {
            ArrayInstance TagsArray = ParsedTags.AsArray();

            List<Tag> result = new List<Tag>();

            for (int i = 0; i < TagsArray.Length; i++) {
                result.Add(new Tag(TagsArray[i].AsString()));
            }

            return result;
        } catch (Exception) {
            return null;
        }
    }
        
    public BooruPost(JsValue ParsedPost) {
        ID = Convert.ToInt32(ParsedPost.Get("ID").AsNumber());
        PreviewFileURL = ParsedPost.Get("PreviewFileURL").AsString();
        LargeFileURL = ParsedPost.Get("LargeFileURL").AsString();
        DirectURL = ParsedPost.Get("DirectURL").AsString();

        PostTags = new Tags() {
            CopyrightTags = ProcessTags(ParsedPost.Get("Tags").Get("CopyrightTags")),
            CharacterTags = ProcessTags(ParsedPost.Get("Tags").Get("CharacterTags")),
            SpeciesTags = ProcessTags(ParsedPost.Get("Tags").Get("SpeciesTags")),
            ArtistTags = ProcessTags(ParsedPost.Get("Tags").Get("ArtistTags")),
            LoreTags = ProcessTags(ParsedPost.Get("Tags").Get("LoreTags")),
            GeneralTags = ProcessTags(ParsedPost.Get("Tags").Get("GeneralTags")),
            MetaTags = ProcessTags(ParsedPost.Get("Tags").Get("MetaTags")),
        };

        PostInformation = new Information() {
            UploaderID = Convert.ToInt32(ParsedPost.Get("Information").Get("UploaderID").AsNumber()),
            PostScore = new Score() {
                UpVotes = Convert.ToInt32(ParsedPost.Get("Information").Get("Score").Get("UpVotes").AsNumber()),
                DownVotes = Convert.ToInt32(ParsedPost.Get("Information").Get("Score").Get("DownVotes").AsNumber()),
                FavoritesCount = Convert.ToInt32(ParsedPost.Get("Information").Get("Score").Get("FavoritesCount").AsNumber())
            },
            Source = ParsedPost.Get("Information").Get("Source").AsString(),
            ParentID = ParsedPost.Get("Information").Get("ParentID").ToObject() != null ? Convert.ToInt32(ParsedPost.Get("Information").Get("ParentID").AsNumber()) : null,
            HasChildren = ParsedPost.Get("Information").Get("HasChildren").AsBoolean(),
            CreatedAt = DateTime.Parse(ParsedPost.Get("Information").Get("UploadedAt").AsString()),
            UploadedAt = DateTime.Parse(ParsedPost.Get("Information").Get("UploadedAt").AsString()),
            FileExtension = ParsedPost.Get("Information").Get("FileExtension").AsString(),
            FileSize = Convert.ToInt32(ParsedPost.Get("Information").Get("FileSize").AsNumber()),
            ImageWidth = Convert.ToInt32(ParsedPost.Get("Information").Get("ImageWidth").AsNumber()),
            ImageHeight = Convert.ToInt32(ParsedPost.Get("Information").Get("ImageHeight").AsNumber())
        };
    }

    public override string ToString() {
        return ID.ToString();
    }
}