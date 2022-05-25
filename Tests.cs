using System.Collections.Generic;
using System.IO;
using IbukiBooruLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IbukiBooruLibrary; 

[TestClass]
public class Tests {
    private string TestScript { get; } = File.ReadAllText(@"Extensions\Danbooru.js");
    private Booru? Danbooru { get; set; }
    
    [TestMethod]
    public void TestTypeCreation() {
        Danbooru ??= new Booru(TestScript);
        Assert.IsNotNull(Danbooru);
    }

    [TestMethod]
    public void TestGetPost() { 
        Danbooru ??= new Booru(TestScript);
        BooruPost Post = Danbooru.GetPost(1).GetAwaiter().GetResult();
        Assert.IsNotNull(Post);
    }

    [TestMethod]
    public void TestGetPostException() {
        Danbooru ??= new Booru(TestScript);
        Assert.ThrowsException<BooruException>(() => Danbooru.GetPost(-1).GetAwaiter().GetResult());
    }

    [TestMethod]
    public void TestGetPostID() {
        Danbooru ??= new Booru(TestScript);
        BooruPost Post = Danbooru.GetPost(1).GetAwaiter().GetResult();
        Assert.AreEqual(Post.ID, 1);
    }

    [TestMethod]
    public void TestGetPosts() {
        Danbooru ??= new Booru(TestScript);
        List<BooruPost> Posts = Danbooru.GetPosts(1, 5).GetAwaiter().GetResult();
        // We can expect up to two deleted posts
        Assert.AreEqual(Posts.Count, 5, 2);
    }

    [TestMethod]
    public void TestSearch() {
        Danbooru ??= new Booru(TestScript);
        List<BooruPost> Posts = Danbooru.GetPosts(1, 5, "rating:safe").GetAwaiter().GetResult();
        Assert.AreEqual(Posts.Count, 5, 2);
    }

    [TestMethod]
    public void TestGetPostRelationships() {
        Danbooru ??= new Booru(TestScript);
        List<BooruPost> Posts = Danbooru.GetPostChildren(5352491).GetAwaiter().GetResult();
        Assert.AreEqual(Posts.Count, 5);
    }
}