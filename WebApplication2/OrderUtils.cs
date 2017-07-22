using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2
{

    public class OrderUtils
    {

        private static OrderItem GetOrderItemFromMenuItem(MenuItem menuItem, uint ItemNumber)
        {
            OrderItem orderItem = new OrderItem() { ItemID = menuItem.ID, ItemNumber = ItemNumber };
            orderItem.SellNumber = GetTotalNumberThisMonth(menuItem.ID);
            orderItem.Name = menuItem.Name;
            orderItem.Type = menuItem.Type;
            orderItem.ImagePath = menuItem.ID.ToString();
            orderItem.Price = menuItem.Price;
            orderItem.Description = menuItem.Description;
            orderItem.Rating = GetRatingThisMonth(menuItem.ID);
            if (orderItem.Description.Length > 18)
                orderItem.Description = orderItem.Description.Substring(0, 16) + " ...";

            return orderItem;
        }



        private static MenuItem GetMenuItemByID(int menuID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.MenuItems.Find(menuID);
        }

        private static void AddMenuItem(List<OrderItem> orderItems, MenuItem menuItem)
        {
            bool isExist = false;
            foreach (OrderItem orderItem in orderItems)
            {
                if (orderItem.ItemID == (menuItem.ID))
                {
                    isExist = true;
                    orderItem.ItemNumber += 1;
                }
            }
            if (!isExist)
            {
                OrderItem orderItem = GetOrderItemFromMenuItem(menuItem, 1);
                orderItems.Add(orderItem);
            }
        }


        public static OrderModel GetOrderModel(MenuOrder menuOrder)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            OrderModel orderModel = new OrderModel() { menuOrder = menuOrder, orderItems = GetOrderDetails(menuOrder.OrderList) };
            orderModel.cost = 0;
            foreach(OrderItem orderItem in orderModel.orderItems)
            {
                MenuItem menuItem = db.MenuItems.FirstOrDefault(item => item.ID == orderItem.ItemID);
                if (orderItem.ItemNumber > 0)
                    orderModel.cost += menuItem.Price * orderItem.ItemNumber;
            }
            return orderModel;
        }

        public static List<OrderItem> GetOrderDetails(string MenuOrder)
        {
            List<OrderItem> orderItems = new List<OrderItem>();
            string[] array_ids = MenuOrder.Split(',');

            foreach (string s_id in array_ids)
                AddMenuItem(orderItems, GetMenuItemByID(int.Parse(s_id)));

            return orderItems;
        }

        private static int GetNumberInMenuOrder(MenuOrder menuOrder, int itemID)
        {
            string[] s_ids = menuOrder.OrderList.Split(',');
            string item_id = itemID.ToString();
            int count = 0;
            foreach(string s_id in s_ids)
            {
                if (s_id.Equals(item_id)) count++;

            }
            return count;
        }

        private static int GetTotalNumberThisMonth(int itemID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<MenuOrder> menuOrders = db.MenuOrders.ToList();
            int count = 0;
            foreach(MenuOrder menuOrder in menuOrders)
            {
                if(DateTime.Now.Subtract(menuOrder.OrderDate).Days <= 30)
                    count += GetNumberInMenuOrder(menuOrder, itemID);
            }
            return count;
        }

        private static float GetRatingThisMonth(int itemID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<MenuComment> menuComments = db.MenuComments.ToList();
            int count = 0, rating = 0;
            foreach(MenuComment menuComment in menuComments)
            {
                if (menuComment.ItemID == itemID && DateTime.Now.Subtract(menuComment.CommentDate).Days < 30)
                {
                    rating += menuComment.Rating;
                    count++;
                }
            }
            return rating;
        }

        public static Dictionary<string, List<OrderItem>> GetBlankOrder()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Dictionary<string, List<OrderItem>> blankOrder = new Dictionary<string, List<OrderItem>>();

            List<MenuItem> menuItems = db.MenuItems.OrderBy(item => item.Type).ThenBy(item => item.Name).ToList();
            foreach (MenuItem menuItem in db.MenuItems)
            {
                OrderItem orderItem = GetOrderItemFromMenuItem(menuItem, 0);

                if (!blankOrder.ContainsKey(orderItem.Type))
                    blankOrder.Add(orderItem.Type, new List<OrderItem>());
                blankOrder[orderItem.Type].Add(orderItem);
                
            }
            return blankOrder;
        }

        public static int SaveOrder(string username, string orderlist)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MenuOrder order = db.MenuOrders.Add(new MenuOrder() { OrderDate = DateTime.Now, OrderList = orderlist, Status = "未分配", Username = username });
            List<OrderItem> orderItems = GetOrderDetails(orderlist);

            foreach(OrderItem orderItem in orderItems)
            {
                orderItem.OrderID = order.ID;
            }
            db.OrderItems.AddRange(orderItems);
            db.SaveChanges();
            return order.ID;
        }

        public static bool DeleteOrder(int orderID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MenuOrder menuOrder = db.MenuOrders.FirstOrDefault(u => u.ID == orderID);
            if (menuOrder != null)
                db.MenuOrders.Remove(menuOrder);
            return menuOrder != null;
        }

    }

}