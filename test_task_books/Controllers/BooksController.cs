using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using test_task_books.Models;

namespace test_task_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookContext db;
        private readonly ILogger<BooksController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public BooksController(BookContext context, ILogger<BooksController> logger,IHttpClientFactory clientFactory)
        {
            db = context;
            _logger = logger;
            _clientFactory = clientFactory;
        }


        [HttpGet]
        public IEnumerable<Book> GetBooks([FromQuery] string sortBy = null, [FromQuery] string orderBy = null)
        {

            Func<Book, object> orderByFunc = null;
            switch (sortBy)
            {
                case "id":
                    orderByFunc = p => p.Id; break;
                case "author":
                    orderByFunc = p => p.Author; break;
                case "name":
                    orderByFunc = p => p.Name; break;
                case "published":
                    orderByFunc = p => p.Published; break;
                default:
                    orderByFunc = p => p.Id; break;
            }
            new ArgumentException("testo", "wtf");

            if (orderBy == "desc")
            {
                return db.Books.OrderByDescending(orderByFunc).ToList();
            }
            else
            {
                return db.Books.OrderBy(orderByFunc).ToList();
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            if (ModelState.IsValid)
            {
                var addedBook = db.Books.Add(book);
                try
                {
                    await db.SaveChangesAsync();
                    return Ok(book);
                }
                catch
                {
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            _logger.LogInformation("book id  is {0}", id);
            var book = await db.Books.FirstOrDefaultAsync(book => book.Id == id);
            if(book != null)
            {
                db.Books.Remove(book);
                try
                {
                    await db.SaveChangesAsync();
                    return NoContent();
                }
                catch
                {
                    return StatusCode(500);
                }
            }
            return NotFound();
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllBooks()
        {
            _logger.LogInformation("Deleting all books");
            //var books = await db.Books.ToListAsync();
            try
            {
                //db.Books.RemoveRange(books);
                await db.Database.ExecuteSqlRawAsync("TRUNCATE \"Books\" RESTART IDENTITY");
                //var result = db.Books.FromSqlRaw("TRUNCATE \"Books\" RESTART IDENTITY");
                return NoContent();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public int SomeShit(int id)
        {
            return id;
        }

        [HttpPost("populate")]
        public async Task<IActionResult> PopulateBooks()
        {
            string uri = "https://gist.githubusercontent.com/nanotaboada/6396437/raw/a572b6cd69f8847e75249e0d177b324417eeee1c/books.json";
            var client = _clientFactory.CreateClient();

            try
            {
                await db.Database.ExecuteSqlRawAsync("TRUNCATE \"Books\" RESTART IDENTITY");
                var response = await client.GetAsync(uri);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    JObject json = JObject.Parse(jsonString);

                    JArray books = json["books"] as JArray;

                    var formattedBooks = books.Select(book =>
                    {
                        return new Book
                        {
                            Author = book["author"].ToString(),
                            Name = book["title"].ToString(),
                            Published = DateTime.Parse(book["published"].ToString())

                        };
                    });

                    await db.Books.AddRangeAsync(formattedBooks);
                    await db.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(500);
            }

            return NoContent();
        }
    }
}
