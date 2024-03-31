using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mapper.Models
{
    public class UserUpdateRequest
    {
        public string Id { get; set; }

        public string formId { get; set; }


        public string? Name { get; set; }


        public string? Email { get; set; }
    }
}