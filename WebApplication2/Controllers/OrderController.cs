using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class OrderController : Controller
    {

        // 用户视图的订单控制器，用于用户购买等操作

        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View(OrderUtils.GetBlankOrder());
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public ActionResult Create(string str, string returnUrl)
        {

            List<MenuItem> menuItems = db.MenuItems.ToList();

            decimal cost = 0;
            string orderlist = "";

            foreach(MenuItem menuItem in menuItems)
            {
                int count = int.Parse(Request["input-" + menuItem.ID]);

                while (count > 0)
                {
                    if (cost > 0) orderlist += ",";
                    cost += menuItem.Price;
                    orderlist += menuItem.ID;
                    count--;
                }
            }

            int order_id = OrderUtils.SaveOrder(User.Identity.Name, orderlist);


            return RedirectToAction("Confirm", "Order", new { id = order_id});
        }

        public ActionResult Confirm(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                id = 30;
            }
            MenuOrder menuOrder = db.MenuOrders.Find(id);
            if (menuOrder == null)
            {
                return HttpNotFound();
            }
            return View(OrderUtils.GetOrderModel(menuOrder));
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public ActionResult Confirm(string str, string returnUrl)
        {

            int id = int.Parse(Request["order_id"]);
            MenuOrder menuOrder = db.MenuOrders.Find(id);

            menuOrder.Address = Request["address"] == null ? "" : Request["address"];
            menuOrder.JD = double.Parse(Request["JD"]);
            menuOrder.WD = double.Parse(Request["WD"]);
            menuOrder.Text = Request["text"] == null ? "" : Request["text"];
            db.Entry(menuOrder).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "MenuOrders");
        }

        public ActionResult Delete(int? id)
        {
            MenuOrder menuOrder = db.MenuOrders.Find(id);
            db.MenuOrders.Remove(menuOrder);
            db.SaveChanges();
            return JavaScript("alert('success');");
        }

    }
}