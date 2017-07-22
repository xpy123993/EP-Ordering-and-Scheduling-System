using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class User
    {

        public int ID { get; set; }
        [Display(Name = "用户名")]
        public string Name { get; set; }
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "角色")]
        public string Roles { get; set; }

    }

    public class UserContext: DbContext
    {

        public DbSet<User> Users { get; set; }

        public void AddUser(string username, string password)
        {
            User user = new User() { Name = username, Password = password, Roles = "user"  };
            Users.Add(user);
            this.SaveChanges();
        }

        public bool ValidateUser(string userName, string password)
        {
            return Users
                .Any(u => u.Name == userName && u.Password == password);
        }

        public string GetRoles(string userName)
        {
            List<string> ret = new List<string>();
            foreach(User user in Users)
            {
                if (user.Name.Equals(userName))
                {
                    return user.Roles;
                }
            }
            return "";
        }

        public User GetByNameAndPassword(string name, string password)
        {
            return Users
                .FirstOrDefault(u => u.Name == name && u.Password == password);
        }

        public bool ChangePassword(string name, string password, string newpassword)
        {
            User user = GetByNameAndPassword(name, password);
            if (user == null)
                return false;
            user.Password = newpassword;
            SaveChanges();
            return true;
        }

        
    }
}