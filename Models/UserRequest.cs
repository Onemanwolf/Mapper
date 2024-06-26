using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mapper.Models
{
    public class UserRequest
    {
        public string? Name { get; set; }


        public string? Email { get; set; }


        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email);
        }

        public bool NotValid()
        {
            return string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email);
        }

    }
}