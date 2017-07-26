using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Models
{
    public class ApplicationDbContext: DbContext
    {

        public DbSet<User> Users { get; set; }

        public DbSet<Worker> Workers { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<MenuOrder> MenuOrders { get; set; }

        public DbSet<MenuComment> MenuComments { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public void AddUser(string username, string password)
        {
            User user = new User() { Name = username, Password = password, Roles = "user" };
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
            foreach (User user in Users)
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