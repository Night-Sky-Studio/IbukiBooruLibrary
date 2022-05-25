using System;
using System.Collections.Generic;

namespace IbukiBooruLibrary.Classes; 

public class Tag {
    public Tag(string tag_name) {
        this.tag_name = tag_name;
    }

    public string tag_name { get; }
    public string tag_display => tag_name.Replace("_", " ");
}
    
public class Tags {
    public List<Tag>? CopyrightTags { get; set; }
    public List<Tag>? CharacterTags { get; set; }
    public List<Tag>? SpeciesTags { get; set; }
    public List<Tag>? ArtistTags { get; set; }
    public List<Tag>? LoreTags { get; set; }
    public List<Tag>? GeneralTags { get; set; }
    public List<Tag>? MetaTags { get; set; }
}

public class Score {
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public int OverallScore => UpVotes - DownVotes;
    public int FavoritesCount { get; set; }
}

public class Information {
    public int UploaderID { get; set; }
    public Score? PostScore { get; set; }
    public string? Source { get; set; }
    public int? ParentID { get; set; }
    public bool HasChildren { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? FileExtension { get; set; }
    public int FileSize { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
}
