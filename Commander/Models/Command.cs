using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Commander.Models
{
    public class Command
    {
        public int Id { get; set; }
        [Required]
        public string HowTo { get; set; }
        [Required]
        [MaxLength(250)]
        public string Line { get; set; }
        [Required]
        public string Platform { get; set; }
    }
}