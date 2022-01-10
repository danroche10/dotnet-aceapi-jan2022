using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Acebook.Models
{
    public class PostDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("body")]
        [Required]
        public string Body { get; set; }

        [JsonPropertyName("user")]
        public ApplicationUserDto User { get; set; }

        [JsonPropertyName("cool")]
        [Required]
        public bool Cool { get; set; }
    }
}
