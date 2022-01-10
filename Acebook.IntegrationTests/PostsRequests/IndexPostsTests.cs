using System.Collections.Generic;
using System.Text.Json;
using Acebook.DbContext;
using Acebook.IdentityAuth;
using Acebook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TestSupport.EfHelpers;
using Xunit;

namespace Acebook.IntegrationTests.PostsRequests
{
    public class IndexPostsTests : IClassFixture<TestingWebApplicationFactory<Startup>>
    {
        private readonly TestingWebApplicationFactory<Startup> factory;
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public IndexPostsTests(TestingWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            this.dbContext = this.factory.Services.GetService<ApplicationDbContext>();
            this.dbContext.Database.EnsureClean();
            this.userManager = this.factory.Services.GetService<UserManager<ApplicationUser>>();
        }

        [Fact]
        public async void GetsListOfPosts()
        {
            var client = this.factory.CreateClient();
            var user = new ApplicationUser { UserName = "fred" };
            await this.userManager.CreateAsync(user, "Password123$");
            await RequestHelpers.Login(client, user, "Password123$");
            var post1 = new Post { Id = 1, UserId = user.Id, Body = "Hello World" };
            var post2 = new Post { Id = 2, UserId = user.Id, Body = "Hello World 2" };
            this.dbContext.Posts.AddRange(post1, post2);
            await this.dbContext.SaveChangesAsync();

            var response = await client.GetAsync("/api/posts");

            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            var responseString = await response.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<List<PostDto>>(responseString);
            Assert.Equal(2, posts.Count);
            Assert.Equal("Hello World", posts[0].Body);
            Assert.Equal("fred", posts[0].User.Username);
            Assert.Equal("Hello World 2", posts[1].Body);
        }

        [Fact]
        public async void GetCoolPosts()
        {
            var client = this.factory.CreateClient();
            var user = new ApplicationUser { UserName = "fred" };
            await this.userManager.CreateAsync(user, "Password123$");
            await RequestHelpers.Login(client, user, "Password123$");
            var post1 = new Post { Id = 3, UserId = user.Id, Body = "Hello World", Cool = true };
            var post2 = new Post { Id = 4, UserId = user.Id, Body = "Hello World 2", Cool = true };
            var post3 = new Post { Id = 5, UserId = user.Id, Body = "Hello World 3", Cool = false };
            this.dbContext.Posts.AddRange(post1, post2, post3);
            await this.dbContext.SaveChangesAsync();

            var response = await client.GetAsync("/api/posts/Cool");

            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            var responseString = await response.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<List<PostDto>>(responseString);
            Assert.Equal(2, posts.Count);
            Assert.Equal("Hello World", posts[0].Body);
            Assert.Equal("fred", posts[0].User.Username);
            Assert.Equal("Hello World 2", posts[1].Body);
        }

        [Fact]
        public async void GetUnCoolPosts()
        {
            var client = this.factory.CreateClient();
            var user = new ApplicationUser { UserName = "fred" };
            await this.userManager.CreateAsync(user, "Password123$");
            await RequestHelpers.Login(client, user, "Password123$");
            var post1 = new Post { Id = 6, UserId = user.Id, Body = "Hello World", Cool = true };
            var post2 = new Post { Id = 7, UserId = user.Id, Body = "Hello World 2", Cool = false };
            var post3 = new Post { Id = 8, UserId = user.Id, Body = "Hello World 3", Cool = false };
            this.dbContext.Posts.AddRange(post1, post2, post3);
            await this.dbContext.SaveChangesAsync();

            var response = await client.GetAsync("/api/posts/Uncool");

            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            var responseString = await response.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<List<PostDto>>(responseString);
            Assert.Equal(2, posts.Count);
            Assert.Equal("Hello World 2", posts[0].Body);
            Assert.Equal("fred", posts[0].User.Username);
            Assert.Equal("Hello World 3", posts[1].Body);
        }
    }
}
