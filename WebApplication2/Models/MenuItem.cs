using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class MenuItem
    {
        
        public int ID { get; set; }

        [Display(Name = "商品类别")]
        public string Type { get; set; }

        [Display(Name = "商品名称")]
        public string Name { get; set; }

        [Display(Name = "商品描述")]
        public string Description { get; set; }

        [Display(Name = "单价")]
        public decimal Price { get; set; }

    }

    public class MenuOrder
    {

        public int ID { get; set; }

        [Display(Name = "用户名")]
        public string Username { get; set; }
        [Display(Name = "商品编号")]
        public string OrderList { get; set; }  //1, 2, 2, 3
        [Display(Name = "购买日期")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "当前状态")]
        public string Status { get; set; }
        [Display(Name = "备注")]
        public string Text { get; set; }

        [Display(Name = "订购地址")]
        public string Address { get; set; }
        
        public decimal JD { get; set; }
        public decimal WD { get; set; }
    }

    
}