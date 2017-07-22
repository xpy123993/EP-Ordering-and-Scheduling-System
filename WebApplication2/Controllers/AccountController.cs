using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    
    public class AccountController : Controller
    {
        //这个是数据库，repository你可以理解为一张数据表
        private UserContext repository = new UserContext();

        public ActionResult Login()
        {
            return View();  //返回Login.cshtml，详情请阅读csharp mvc
        }

        //登录时POST方法更加安全，因此仅仅定义POST登陆方法
        [HttpPost]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            //在Login.cshtml上点击了“递交”按钮后的处理事件，model中包含用户在该页面填写的信息
            if (ModelState.IsValid)
            {
                if (repository.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    if (!String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "用户名或密码不正确！");
            }
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                repository.AddUser(model.UserName, model.Password);
                return RedirectToAction("Login", "Account");
            }
            ModelState.AddModelError("", "输入信息有误");
            return View(model);
        }
        [Authorize]
        public ActionResult ChangeUserInfo()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangeUserInfo(ChangePasswordModel model, string returnUrl)
        {
            if (ModelState.IsValid && repository.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                ModelState.AddModelError("", "修改成功");
            else
                ModelState.AddModelError("", "输入信息有误");
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}