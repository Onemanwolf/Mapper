using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mapper.Models{
public class User
{
    public string? Name { get; set; }
    public string? Email { get; set; }

    public User FactorMethodUser(string name, string email)
    { var user = new User{
        Name = name,
        Email = email,};

        

        return user;
    }
}}