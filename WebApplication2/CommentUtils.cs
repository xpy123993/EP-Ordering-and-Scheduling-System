using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2
{
    public class CommentUtils
    {

        public static ItemCommentModel GetCommentModel(MenuItem menuItem)
        {
            ItemCommentModel commentModel = new ItemCommentModel();
            commentModel.menuItem = menuItem;
            ApplicationDbContext db = new ApplicationDbContext();

            List<MenuComment> menuComments = db.MenuComments.Where(item => item.ItemID == menuItem.ID).ToList();
            commentModel.menuComments = new List<MenuComment>();
            foreach(MenuComment menuComment in menuComments)
            {
                if (DateTime.Now.Subtract(menuComment.CommentDate).Days < 90)
                    commentModel.menuComments.Add(menuComment);
            }
            return commentModel;
        }



        public static UserCommentModel GetBlankCommentModel(MenuOrder menuOrder)
        {
            List<OrderItem> orderItems = OrderUtils.GetOrderDetails(menuOrder.OrderList);
            UserCommentModel commentModel = new UserCommentModel();
            commentModel.menuOrder = menuOrder;
            commentModel.menuComments = new List<MenuComment>();

            ApplicationDbContext db = new ApplicationDbContext();

            foreach(OrderItem orderItem in orderItems)
            {
                MenuComment menuComment = new MenuComment();
                menuComment.CommentDate = DateTime.Now;
                menuComment.ItemID = orderItem.ItemID;
                menuComment.MenuItem = db.MenuItems.FirstOrDefault(item => item.ID == orderItem.ItemID).Name;
                menuComment.OrderID = menuOrder.ID;
                menuComment.Username = menuOrder.Username;
                commentModel.menuComments.Add(menuComment);
            }

            return commentModel;
        }

        private static int BuyTimes(List<MenuOrder> menuOrders, int ItemID)
        {

            int count = 0;

            foreach(MenuOrder menuOrder in menuOrders)
            {
                if(DateTime.Now.Subtract(menuOrder.OrderDate).Days < 3)
                {
                    string[] s_ids = menuOrder.OrderList.Split(',');

                    foreach (string s_id in s_ids)
                    {
                        if (ItemID.ToString().Equals(s_id))
                            count++;
                    }
                }
            }
            return count;
        }

        public static bool CheckCommentPermission(string Name, int ItemID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<MenuComment> menuComments = db.MenuComments.Where(item => item.Username.Equals(Name)).ToList();
            List<MenuOrder> menuOrders = db.MenuOrders.Where(item => item.Username.Equals(Name)).ToList();

            int count = BuyTimes(menuOrders, ItemID), comment_count = 0;

            foreach(MenuComment menuComment in menuComments)
            {
                if (DateTime.Now.Subtract(menuComment.CommentDate).Days < 3 && DateTime.Now.Subtract(menuComment.CommentDate).Hours > 1)
                    comment_count++;
            }
            return count > comment_count;
        }

        public static bool CheckOrderCommentPermission(string Name, int OrderID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            

            MenuOrder menuOrder = db.MenuOrders.FirstOrDefault(item => item.ID == OrderID);
            bool outdated = DateTime.Now.Subtract(menuOrder.OrderDate).Days > 3;
            bool commented = db.MenuComments.Where(item => item.OrderID == OrderID).Count() > 0;
            bool completed = menuOrder.Status.Equals("已完成");
            return !outdated && !commented && completed;
        }

    }
}