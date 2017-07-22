using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class MenuComment
    {
        public int ID { get; set; }
        [Display(Name = "订单号")]
        public int OrderID { get; set; }
        [Display(Name = "评论人")]
        public string Username { get; set; }
        [Display(Name = "评论日期")]
        public DateTime CommentDate { get; set; }
        [Display(Name = "商品编号")]
        public int ItemID { get; set; }
        [Display(Name = "商品名")]
        public string MenuItem { get; set; }
        [Display(Name = "分数")]
        public int Rating { get; set; }
        [Display(Name = "评论")]
        public string Text { get; set; }
    }

    public class ItemCommentModel
    {
        public MenuItem menuItem { get; set; }
        public List<MenuComment> menuComments { get; set; }
    }

    public class UserCommentModel
    {
        public MenuOrder menuOrder { get; set; }
        public List<MenuComment> menuComments { get; set; }
    }
}