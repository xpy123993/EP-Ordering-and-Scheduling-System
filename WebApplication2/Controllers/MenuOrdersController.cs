using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{

    // 管理员视图的订单控制器，管理员可以修改订单状态，删除订单

    public class MenuOrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MenuOrders
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("superadmin"))
                return View(db.MenuOrders.ToList());
            if (User.IsInRole("admin"))
            {
                List<MenuOrder> menuOrders = new List<MenuOrder>();
                foreach(MenuOrder menuOrder in db.MenuOrders)
                {
                    if (DateTime.Now.Date == menuOrder.OrderDate.Date)
                        menuOrders.Add(menuOrder);
                }
                return View(menuOrders);   
            }
            if (User.IsInRole("user"))
            {
                List<MenuOrder> menuOrders = db.MenuOrders.Where(item => item.Username.Equals(User.Identity.Name)).ToList();
                return View(menuOrders);
            }
            return View();
        }

        // GET: MenuOrders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuOrder menuOrder = db.MenuOrders.Find(id);
            if (menuOrder == null)
            {
                return HttpNotFound();
            }
            return View(OrderUtils.GetOrderModel(menuOrder));
        }

        // GET: MenuOrders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MenuOrders/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Username,OrderList,OrderDate,Status")] MenuOrder menuOrder)
        {
            if (ModelState.IsValid)
            {
                db.MenuOrders.Add(menuOrder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menuOrder);
        }

        // GET: MenuOrders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuOrder menuOrder = db.MenuOrders.Find(id);
            if (menuOrder == null)
            {
                return HttpNotFound();
            }
            return View(menuOrder);
        }

        // POST: MenuOrders/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Username,OrderList,OrderDate,Status")] MenuOrder menuOrder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuOrder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menuOrder);
        }

        // GET: MenuOrders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuOrder menuOrder = db.MenuOrders.Find(id);
            if (menuOrder == null)
            {
                return HttpNotFound();
            }
            return View(menuOrder);
        }

        // POST: MenuOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuOrder menuOrder = db.MenuOrders.Find(id);
            db.MenuOrders.Remove(menuOrder);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
