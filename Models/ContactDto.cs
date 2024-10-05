using System.ComponentModel.DataAnnotations;

namespace CsvReaderApp.Models
{
    public class ContactDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public bool Married { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public decimal Salary { get; set; }
    }
}
