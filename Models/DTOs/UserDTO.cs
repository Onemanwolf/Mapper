using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;

namespace Mapper.Models.DTOs
{
    public class UserDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("formId")]
        public string formId { get; set; } 

        [JsonProperty("name")]

        public string? Name { get; set; }
        [JsonProperty("email")]
        public string? Email { get; set; }
    }
}