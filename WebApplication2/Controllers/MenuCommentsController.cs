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
    public class MenuCommentsController : Controller
    {
        // 管理员视图的菜品评论控制器，用于对用户评论进行查看和删除操作


        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MenuComments
        public ActionResult Index()
        {
            return View(db.MenuComments.ToList());
        }

        // GET: MenuComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuComment menuComment = db.MenuComments.Find(id);
            if (menuComment == null)
            {
                return HttpNotFound();
            }
            return View(menuComment);
        }

        // GET: MenuComments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MenuComments/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OrderID,Username,Rating,Text")] MenuComment menuComment)
        {
            if (ModelState.IsValid)
            {
                db.MenuComments.Add(menuComment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menuComment);
        }

        // GET: MenuComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuComment menuComment = db.MenuComments.Find(id);
            if (menuComment == null)
            {
                return HttpNotFound();
            }
            return View(menuComment);
        }

        // POST: MenuComments/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OrderID,Username,Rating,Text")] MenuComment menuComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menuComment);
        }

        // GET: MenuComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuComment menuComment = db.MenuComments.Find(id);
            if (menuComment == null)
            {
                return HttpNotFound();
            }
            return View(menuComment);
        }

        // POST: MenuComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuComment menuComment = db.MenuComments.Find(id);
            db.MenuComments.Remove(menuComment);
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
