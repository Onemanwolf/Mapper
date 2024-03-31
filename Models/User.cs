using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mapper.Models
{
    public class User
    {


        public string Id { get; set; }

        public string formId { get; set; }


        public string? Name { get; set; }


        public string? Email { get; set; }

        public User FactorMethodUser(string name, string email)
        {
            var id = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = id,
                Name = name,
                Email = email,
                formId = id
            };



            return user;
        }
    }
}