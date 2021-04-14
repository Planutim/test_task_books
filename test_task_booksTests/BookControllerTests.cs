using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using test_task_books.Models;
using Xunit;

namespace test_task_booksTests
{
    public class BookControllerTests
    {
        HttpClient client = new HttpClient();

        [Theory]
        [InlineData("J.K.Rowling","Harry Potter", "1994-01-01")]
        [InlineData("War and Peace", "Lev Tolstoy", "1912-01-01")]
        public async void ShouldCreateABook(string author, string name, string published)
        {
            //Arrange

            Book book = new Book
            {
                Author = author,
                Name = name,
                Published = DateTime.Parse(published)
            };
            var jsonString = JsonConvert.SerializeObject(book);
            var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            //Act
            var response = await client.PostAsync("https://localhost:5001/api/books",jsonContent);
            //Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void ShouldFailCreateBook()
        {
            //Arrange
            Book book = new Book();

            var jsonString = JsonConvert.SerializeObject(book);
            var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            //Act
            var response = await client.PostAsync("https://localhost:5001/api/books", jsonContent);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void ShouldDeleteBook()
        {
            //Arrange
            Book book = new Book
            {
                Author = "author",
                Name = "name",
                Published = DateTime.Parse("2000-01-01")
            };

            var jsonString = JsonConvert.SerializeObject(book);
            var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            //Act
            var response = await client.PostAsync("https://localhost:5001/api/books", jsonContent);
        }
    }
}
