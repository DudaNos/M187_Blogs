using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Data.Entities;
using WebApplication1.Dtos;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly BlogsContext _blogsContext;
        private readonly HtmlSanitizer _htmlSanitizer;

        public BlogsController(BlogsContext blogsContext)
        {
            _blogsContext = blogsContext;
            _htmlSanitizer = new HtmlSanitizer();
        }

        /// <summary>
        /// Get All Blogs saved in the database
        /// </summary>
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var results = _blogsContext.Blogs.ToList();

            return Ok(results);
        }

        /// <summary>
        /// Gets a blog by it's specific Id
        /// </summary>
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var blog = _blogsContext.Blogs.Single(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return Ok(blog);
        }

        // POST api/values
        /// <summary>
        /// Gets a JSON Format of a Blog (Title and Content) Both can be in HTML
        /// and used in a rich text formatter
        /// </summary>
        [HttpPost]
        public IActionResult Post([FromBody] BlogDto blog)
        {
            var blogEntity = new Blog
            {
                // HTMLSanitizer extracts all tags that should not be allowed to exist
                Title = _htmlSanitizer.Sanitize(blog.Title),
                Content = _htmlSanitizer.Sanitize(blog.Content)
            };

            _blogsContext.Add(blogEntity);
            _blogsContext.SaveChanges();

            return Created("Get", new  { id = blogEntity.Id });
        }
    }
}
