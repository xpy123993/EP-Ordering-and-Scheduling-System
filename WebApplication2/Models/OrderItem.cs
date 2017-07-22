using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class OrderItem
    {

        public int ID { get; set; }

        [Display(Name = "订单号")]
        public int OrderID { get; set; }
        [Display(Name = "商品编号")]
        public int ItemID { get; set; }
        [Display(Name = "商品类别")]
        public string Type { get; set; }
        [Display(Name = "商品名称")]
        public string Name { get; set; }
        [Display(Name = "商品描述")]
        public string Description { get; set; }
        [Display(Name = "图片")]
        public string ImagePath { get; set; }
        [Display(Name = "单价")]
        public decimal Price { get; set; }
        [Display(Name = "订购数量")]
        public uint ItemNumber { get; set; }
        [Display(Name = "月销售量")]
        public int SellNumber { get; set; }
        [Display(Name = "好评数")]
        public float Rating { get; set; }
        
    }

    public class OrderModel
    {
        public MenuOrder menuOrder;
        public List<OrderItem> orderItems;
        public decimal cost;
    }


}