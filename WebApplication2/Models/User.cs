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
}