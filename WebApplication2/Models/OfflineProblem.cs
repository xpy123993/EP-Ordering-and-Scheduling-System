using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class OfflineProblem
    {

        [Display(Name = "骑士数量")]
        public int WorkerNum { get; set; }

        [Display(Name = "每个骑士最多派遣次数")]
        public int ScheduleMaxTime { get; set; }

        [Display(Name = "最早订单时间")]
        public DateTime StartTime { get; set; }

        [Display(Name = "最晚订单时间")]
        public DateTime EndTime { get; set; }

        [Display(Name = "订单完成时限(分钟)")]
        public int DueDate { get; set; }

    }

    public class Position
    {
        public double JD, WD;
        public string Address;
        public Position(double JD, double WD, string Address)
        {
            this.JD = JD;
            this.WD = WD;
            this.Address = Address;
        }
    }

    public class CoordinateRes
    {
        private List<Position> data = new List<Position>();
        private Random random = new Random();

        public CoordinateRes()
        {
            data.Add(new Position(121.827606, 39.092586, "中国邮政"));  //中国邮政
            data.Add(new Position(121.830013, 39.091578, "时代城55号"));  //时代城55号
            data.Add(new Position(121.820276, 39.091382, "南昌科技中心"));  //南昌科技中心
            data.Add(new Position(121.819917, 39.090038, "大连理工大学综合楼"));  //大连理工大学综合楼
            data.Add(new Position(121.826528, 39.087266, "大连仁合商务旅馆"));  //大连仁合商务旅馆
            data.Add(new Position(121.829762, 39.088050, "知语山"));  //知语山
            data.Add(new Position(121.824767, 39.083905, "松岚工业园"));  //松岚工业园
            data.Add(new Position(121.838314, 39.092194, "大连财经学院"));  //大连财经学院
            data.Add(new Position(121.821246, 39.081665, "XX公司"));  //XX公司
            data.Add(new Position(121.839428, 39.082505, "XX公司"));  //XX公司
            data.Add(new Position(121.831774, 39.081160, "顺通驾校"));  //顺通驾校
            data.Add(new Position(121.828504, 39.076959, "爱家超市"));  //爱家超市
            data.Add(new Position(121.831846, 39.081300, "顺通驾校"));  //顺通驾校
        }

        public Position GetRandomPosition()
        {
            

            return data[random.Next(data.Count)];
        }
    }

    public class ProblemUtils
    {
        public static List<MenuOrder> GetTestOrders()
        {
            CoordinateRes cr = new CoordinateRes();
            List<MenuOrder> menuOrders = new List<MenuOrder>();

            DateTime current_time = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "07:00");

            Random random = new Random();

            int cur_id = 0;

            for(int i = 0; i < 5; i++)
            {
                Position position = cr.GetRandomPosition();

                MenuOrder menuOrder = new MenuOrder()
                {
                    Address = position.Address,
                    ID = cur_id++,

                    JD = position.JD,
                    WD = position.WD ,
                    OrderDate = current_time.AddMinutes(random.Next(30))
                , OrderList = "0",
                    Status = "未分配",
                    Username = "Virtual User",
                    Text = ""
                };
                menuOrders.Add(menuOrder);
            }

            current_time = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "11:00");
            for (int i = 0; i < 5; i++)
            {
                Position position = cr.GetRandomPosition();

                MenuOrder menuOrder = new MenuOrder()
                {
                    Address = position.Address,
                    ID = cur_id++,

                    JD = position.JD,
                    WD = position.WD,
                    OrderDate = current_time.AddMinutes(random.Next(30))
                ,
                    OrderList = "0",
                    Status = "未分配",
                    Username = "Virtual User",
                    Text = ""
                };
                menuOrders.Add(menuOrder);
            }

            return menuOrders;

        }

        public static List<MenuOrder> GetTestOrders_Random()
        {
            Random random = new Random();



            //121.82333,39.088169
            double min_jd = 121.82333 - 0.05;
            double max_jd = 121.82333 + 0.05;

            double min_wd = 39.088169 - 0.02;
            double max_wd = 39.088169 + 0.02;

            double jd, wd;

            List<MenuOrder> menuOrders = new List<MenuOrder>();

            DateTime current_time = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "07:00");

            int cur_id = 0;

            for (int i = 0; i < 5; i++)
            {
                jd = (random.NextDouble()) * (max_jd - min_jd) + min_jd;
                wd = (random.NextDouble()) * (max_wd - min_wd) + min_wd;

                MenuOrder menuOrder = new MenuOrder()
                {
                    Address = "Fake Address " + cur_id.ToString(),
                    ID = cur_id++,

                    JD = jd,
                    WD = wd,
                    OrderDate = current_time
                ,
                    OrderList = "0",
                    Status = "未分配",
                    Username = "Virtual User",
                    Text = ""
                };
                menuOrders.Add(menuOrder);
            }

            current_time = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "11:00");
            for (int i = 0; i < 5; i++)
            {
                jd = (random.NextDouble()) * (max_jd - min_jd) + min_jd;
                wd = (random.NextDouble()) * (max_wd - min_wd) + min_wd;

                MenuOrder menuOrder = new MenuOrder()
                {
                    Address = "Fake Address " + cur_id.ToString(),
                    ID = cur_id++,

                    JD = jd,
                    WD = wd,
                    OrderDate = current_time
                ,
                    OrderList = "0",
                    Status = "未分配",
                    Username = "Virtual User",
                    Text = ""
                };
                menuOrders.Add(menuOrder);
            }

            return menuOrders;

        }

        public static List<MenuOrder> GetOrders(DateTime startTime, DateTime endTime)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<MenuOrder> menuOrders = db.MenuOrders.ToList();
            List<MenuOrder> retOrders = new List<MenuOrder>();
            foreach(MenuOrder menuOrder in menuOrders)
            {
                if(DateTime.Compare(startTime, menuOrder.OrderDate) <= 0 && DateTime.Compare(endTime, menuOrder.OrderDate) >= 0)
                {
                    retOrders.Add(menuOrder);
                }
            }
            return retOrders;
        }
    }
}