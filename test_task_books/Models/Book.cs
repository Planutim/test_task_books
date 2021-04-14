using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace test_task_books.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Нет автора")]
        [MaxLength(25)]
        public string Author { get; set; }
        [Required(ErrorMessage ="Нет названия")]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required(ErrorMessage ="Нет даты публикации")]
        public DateTime? Published { get; set; }
    }
}
