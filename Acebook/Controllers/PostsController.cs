using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acebook.DbContext;
using Acebook.IdentityAuth;
using Acebook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Acebook.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        // GET: /api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPost()
        {
            return await this.context.Posts
                .Include(x => x.User)
                .Select(p => p.ToDto()).ToListAsync();
        }

        // GET: /api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var post = await this.context.Posts
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return post.ToDto();
        }

        // GET: /api/Posts/Cool
        [HttpGet("Cool")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetCoolPosts()
        {
            return await this.context.Posts
                        .Include(x => x.User)
                        .Where(x => x.Cool == true)
                        .Select(p => p.ToDto()).ToListAsync();
        }

        // GET: /api/Posts/Uncool
        [HttpGet("Uncool")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetUnCoolPosts()
        {
            return await this.context.Posts
                        .Include(x => x.User)
                        .Where(x => x.Cool == false)
                        .Select(p => p.ToDto()).ToListAsync();
        }

        // POST: /api/Posts
        [HttpPost]
        public async Task<ActionResult<PostDto>> PostPost(PostDto postDto)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(User);
            var post = new Post
            {
                UserId = user.Id,
                Body = postDto.Body,
                Cool = postDto.Cool,
            };

            this.context.Posts.Add(post);
            await this.context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post.ToDto());
        }

        // DELETE: /api/Posts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost(int id)
        {
            var post = await this.context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            this.context.Posts.Remove(post);
            await this.context.SaveChangesAsync();

            return NoContent();
        }
    }
}
