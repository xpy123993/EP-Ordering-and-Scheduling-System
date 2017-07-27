using PSOTSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //return db.Workers;
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

            if (menuOrders.Count() == 0)
                return Content("无待分配订单");
            if (workers.Count() == 0)
                return Content("无可用人员");

            Solution solution = Translate.Solve(menuOrders, workers);

            OutputSolution data = new OutputSolution()
            {
                workers = workers,
                menuOrders = menuOrders,
                solution = solution
            };
            
            return View(data);
        }

        [HttpPost]
        public ActionResult Index(int? id)
        {
            StringBuilder sb = new StringBuilder();

            IQueryable<Worker> workers = GetUnassignedWorkers();
            IQueryable<MenuOrder> menuOrders = GetUnassignedMenuOrders();

            if (workers.Count() == 0) return Content("无闲置骑士");
            if (menuOrders.Count() == 0) return Content("无待分配订单");

            //修改工作人员下次可用时间，增加调度次数
            for(int i = 0; i < workers.Count(); i++)
            {
                int current_worker_second = int.Parse(Request["i_worker_" + i]);
                if (current_worker_second == 0) continue;
                workers.ToList()[i].ReadyTime = DateTime.Now.AddSeconds(current_worker_second);
                workers.ToList()[i].ScheduleTimes++;
                db.Entry(workers.ToList()[i]).State = System.Data.Entity.EntityState.Modified;
            }

            //将订单标记为“已分配”
            foreach(MenuOrder menuOrder in menuOrders)
            {
                menuOrder.Status = "已分配";
                db.Entry(menuOrder).State = System.Data.Entity.EntityState.Modified;
            }


            db.SaveChanges();


            return RedirectToAction("Index", "Workers");
        }
    }
}