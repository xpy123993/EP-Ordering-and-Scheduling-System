using PSOTSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.PSOKit;

namespace WebApplication2.Controllers
{
    public class OutputSolution
    {
        public IEnumerable<Worker> workers;
        public IEnumerable<MenuOrder> menuOrders;
        public Solution solution;
    }


    public class AssignmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private IQueryable<MenuOrder> GetUnassignedMenuOrders()
        {
            return db.MenuOrders.Where(item => item.Status.Equals("未分配"));
        }

        private IQueryable<Worker> GetUnassignedWorkers()
        {
            return db.Workers.Where(item => item.ReadyTime.CompareTo(DateTime.Now) < 0);
        }
        // 为javascript的getJSON提供依据: 访问 /./Assignment/QueryWorkers       获得闲置外卖骑士
        public ActionResult QueryWorkers()
        {
            return Json(GetUnassignedWorkers(), JsonRequestBehavior.AllowGet);
        }
        // 为javascript的getJSON提供依据: 访问 /./Assignment/QueryMenuOrders    获得未分配订单
        public ActionResult QueryMenuOrders()
        {
            return Json(GetUnassignedMenuOrders(), JsonRequestBehavior.AllowGet);
        }

        // GET: Assignment
        public ActionResult Index()
        {
            IQueryable<MenuOrder> menuOrders = GetUnassignedMenuOrders();
            IQueryable<Worker> workers = GetUnassignedWorkers();

            Solution solution = Translate.Solve(menuOrders, workers);

            OutputSolution data = new OutputSolution()
            {
                workers = workers,
                menuOrders = menuOrders,
                solution = solution
            };

            return View(data);
        }
    }
}