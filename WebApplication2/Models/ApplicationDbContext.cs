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

        public DbSet<Worker> Workers { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<MenuOrder> MenuOrders { get; set; }

        public DbSet<MenuComment> MenuComments { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

    }
}