using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Course_Project.Domain.Models;
namespace Course_Project.Application.Services
{
    public class UserService
    {
        public UserService() { }

        public User Create(string Email,string Login)
        {
            return new User
            {
                Email = Email,
                UserName = Login,
                LockoutEnabled =false
            };
        }
    }
}
