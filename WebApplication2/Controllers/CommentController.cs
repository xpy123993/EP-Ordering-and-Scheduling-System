using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class CommentController : Controller
    {
        // 用户视图的菜品评论控制器，对应“用户评论”操作逻辑
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comment
        // 打开一个具体菜品的相关评论，ID为菜品在数据库中的编号
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.MenuItem menuItem = db.MenuItems.Find(id);
            if (menuItem == null)
            {
                return HttpNotFound();
            }
            return View(CommentUtils.GetCommentModel(menuItem));
        }
        // “添加评论”功能，返回一张空白评论表，评论内容为对应订单的所有菜品
        public ActionResult OrderComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuOrder menuOrder = db.MenuOrders.Find(id);
            UserCommentModel commentModel = CommentUtils.GetBlankCommentModel(menuOrder);
            Session["OrderDetails"] = commentModel;
            if (menuOrder == null)
            {
                return HttpNotFound();
            }
            return View(commentModel);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public ActionResult OrderComment(string str, string returnUrl)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            UserCommentModel commentModel = (UserCommentModel)Session["OrderDetails"];

            foreach (MenuComment menuComment in commentModel.menuComments)
            {
                string value = Request["rating-" + menuComment.ItemID];
                if (value.Equals("good"))
                    menuComment.Rating = 1;
                else if (value.Equals("bad"))
                    menuComment.Rating = -1;
                else
                    menuComment.Rating = 0;
                menuComment.Text = Request["input-" + menuComment.ItemID];
                db.MenuComments.Add(menuComment);
            }

            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}