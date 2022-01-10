using System.Linq;
using System.Net.Http;
using System.Text;
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
    public class CreatePostTests : IClassFixture<TestingWebApplicationFactory<Startup>>
    {
        private readonly TestingWebApplicationFactory<Startup> factory;
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public CreatePostTests(TestingWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            this.dbContext = this.factory.Services.GetService<ApplicationDbContext>();
            this.dbContext.Database.EnsureClean();
            this.userManager = this.factory.Services.GetService<UserManager<ApplicationUser>>();
        }

        [Fact]
        public async void CreatesAPost()
        {
            var client = this.factory.CreateClient();
            var user = new ApplicationUser { UserName = "fred" };
            await this.userManager.CreateAsync(user, "Password123$");
            await RequestHelpers.Login(client, user, "Password123$");

            var response = await client.PostAsync("/api/posts", new StringContent(
                JsonSerializer.Serialize(new Post { Body = "Hello World", Cool = true }),
                Encoding.UTF8,
                "application/json"));

            response.EnsureSuccessStatusCode();
            Assert.Equal(1, this.dbContext.Posts.Count());
            Assert.Equal("Hello World", this.dbContext.Posts.OrderBy(p => p.Id).First().Body);
            Assert.True(this.dbContext.Posts.OrderBy(p => p.Id).First().Cool);
        }
    }
}
