using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{

    // 离线计算控制器，用于返回计算器以及查询订单信息等

    public class CItem
    {
        public string Name { get; set; }
        public string TAG { get; set; }
        public string ScheduleTime { get; set; }
        public string Duration { get; set; }
        public string Distance { get; set; }
        public string TotalLate { get; set; }
        public List<int> Route { get; set; }
    }

    public class CResult
    {
        [Display(Name = "结果编号")]
        public int ID { get; set; }

        [Display(Name = "总拖延时长(分钟)")]
        public double TotalLate { get; set; }

        [Display(Name = "总长度(公里)")]
        public double TotalDistance { get; set; }

        [Display(Name = "总时间(分钟)")]
        public double TotalDuration { get; set; }

        public List<CItem> Items { get; set; }

    }

    public class ResultRequest
    {
        public string ScheduleDetails { get; set; }
        public string ScheduleDurations { get; set; }
        public string ScheduleDistances { get; set; }
        public string ScheduleTimes { get; set; }
        public string ScheduleTotalLates { get; set; }
    }

    public class OfflineCalController : Controller
    {
        // GET: OfflineCal
        public ActionResult Index()
        {
            return RedirectToAction("Entrance");
        }

        public ActionResult Entrance()
        {
            OfflineProblem op = new OfflineProblem();
            op.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "08:00");
            op.EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "21:00");
            return View(op);
        }

        [HttpPost]
        public ActionResult Entrance([Bind(Include = "WorkerNum, ScheduleMaxTime, StartTime, EndTime, DueDate")] OfflineProblem op)
        {
            //return Content("递交内容:工作人数：" + op.WorkerNum + ",最大调度次数:" + op.ScheduleMaxTime + ",开始时间:" + op.StartTime + ",结束时间：" + op.EndTime);

            Session.Add("history", new List<CResult>());



            return RedirectToAction("Calculator", new { workerNum = op.WorkerNum, scheduleMaxTime = op.ScheduleMaxTime, startTime = op.StartTime, endTime = op.EndTime , dueDate = op.DueDate});
        }


        public int ScheduleMaxTime, WorkerNum;


        public ActionResult Calculator(int workerNum, int scheduleMaxTime, DateTime startTime, DateTime endTime, int dueDate)
        {
            OfflineProblem op = new OfflineProblem();
            op.StartTime = startTime;
            op.EndTime = endTime;
            op.ScheduleMaxTime = scheduleMaxTime;
            op.WorkerNum = workerNum;
            op.DueDate = dueDate;
            Session["ScheduleMaxTime"] = op.ScheduleMaxTime;
            Session["WorkerNum"] = op.WorkerNum;
            Session["StartTime"] = op.StartTime;
            Session["EndTime"] = op.EndTime;

            return View(op);
        }

        //public ActionResult Calculator(int workerNum, int scheduleMaxTime, DateTime startTime, DateTime endTime)
        //{
        //    OfflineProblem op;
        //    //if (workerNum != null)
        //        op = new OfflineProblem() { WorkerNum = (int)workerNum, ScheduleMaxTime = scheduleMaxTime, StartTime = startTime, EndTime = endTime };
        //    //else
        //    //    op = new OfflineProblem();
        //    //op.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "08:00");
        //    //op.EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "21:00");
        //    //op.ScheduleMaxTime = 5;
        //    //op.WorkerNum = 10;
        //    return View(op);

        //}


        private List<CResult> getHistory() { return (List<CResult>)Session["history"]; }
        private void resetHistory()
        {
            Session.Remove("history");
            Session.Add("history", new List<CResult>());
        }

        

        [HttpPost]
        public ActionResult Calculator([Bind(Include = "ScheduleDetails, ScheduleDurations, ScheduleDistances, ScheduleTimes, ScheduleTotalLates")] ResultRequest resultRequest)
        {
            Session["ScheduleDetails"] = resultRequest.ScheduleDetails;
            Session["ScheduleDurations"] = resultRequest.ScheduleDurations;
            Session["ScheduleDistances"] = resultRequest.ScheduleDistances;
            Session["ScheduleTimes"] = resultRequest.ScheduleTimes;
            Session["ScheduleTotalLates"] = resultRequest.ScheduleTotalLates;
            return RedirectToAction("CalculateResult");
        }


        public ActionResult RouteQuery()
        {
            CResult current_model = (CResult)Session["current_model"];

            string js_str = "function update_routes(){";

            foreach(CItem item in current_model.Items)
            {
                js_str += "routes[\"" + item.TAG + "\"] = new Array();" + "\n";

                foreach(int value in item.Route)
                    js_str += "routes[\"" + item.TAG + "\"].push(" + value + ");" + "\n";

            }

            js_str += "}";

            return JavaScript(js_str);
        }

        public ActionResult CalculateResult(int? id)
        {
            if(id != null)
            {
 
                if (0 <= id && id < getHistory().Count)
                {
                    CResult currentModel = getHistory()[(int)id];

                    Session["current_model"] = currentModel;


                    return View(currentModel);


                }
            }


            if (Session["ScheduleDetails"] == null)
                return RedirectToAction("Calculator");


            string scheduleDetails = Session["ScheduleDetails"].ToString();
            string scheduleDurations = Session["ScheduleDurations"].ToString();
            string scheduleDistances = Session["ScheduleDistances"].ToString();
            string scheduleTimes = Session["ScheduleTimes"].ToString();
            string scheduleTotalLates = Session["ScheduleTotalLates"].ToString();

            int pointer = 0;
            int last_pointer = 0;

            string one_array = "";

            Dictionary<string, List<int>> our_dic = new Dictionary<string, List<int>>();

            ScheduleMaxTime = int.Parse(Session["ScheduleMaxTime"].ToString());
            WorkerNum = int.Parse(Session["WorkerNum"].ToString());

            List<CResult> history = (List<CResult>)Session["history"];

            CResult model = new CResult();
            model.Items = new List<CItem>();

            int pointer_duration = 0;

            string[] durations = scheduleDurations.Split(',');
            string[] distances = scheduleDistances.Split(',');
            string[] times = scheduleTimes.Split(',');
            string[] totallates = scheduleTotalLates.Split(',');

            model.TotalLate = model.TotalDuration = model.TotalDistance = 0;
            model.ID = history.Count;

            for (int i = 0; i < ScheduleMaxTime; i++)
            {
                for (int j = 0; j < WorkerNum; j++)
                {
                    while (scheduleDetails[pointer] != ':')
                        pointer++;

                    if (last_pointer != pointer)
                        one_array = scheduleDetails.Substring(last_pointer, pointer - last_pointer);
                    else
                        one_array = "";

                    CItem modelItem = new CItem()
                    {
                        Name = string.Format("骑士{0}", j),
                        TAG = string.Format("{0}_{1}", j, i),
                        Duration = durations[pointer_duration],
                        Distance = distances[pointer_duration],
                        ScheduleTime = times[pointer_duration],
                        TotalLate = totallates[pointer_duration],
                        Route = GetList(one_array)
                      
                    };

                    
                    model.TotalLate += double.Parse(totallates[pointer_duration]);
                    model.TotalDuration += double.Parse(durations[pointer_duration]);
                    model.TotalDistance += double.Parse(distances[pointer_duration]);
                    pointer_duration++;

                    Session["S" + i + "_" + j] = one_array;
                    our_dic.Add("S" + i + "_" + j, GetList(one_array));

                    pointer += 1;
                    last_pointer = pointer;

                    model.Items.Add(modelItem);
                }
            }

            Session["current_model"] = model;
            history.Add(model);

            return View(model);
        }

        public ActionResult GetHistory()
        {
            return Json(getHistory(), JsonRequestBehavior.AllowGet);
        }

        //<!-TODO AUTH
        public ActionResult TestQuery()
        {
            List<MenuOrder> data = ProblemUtils.GetOrders((DateTime)Session["StartTime"], (DateTime)Session["EndTime"]);
            Session["cal_data"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        /*
         * ResultTable
         * 返回辅助计算的历史纪录，ID为排序方式
         * 
         * id:
         * 0 - 根据结果总路程大小进行排序
         * 1 - 根据结果总时间长短进行排序
         * 2 - 根据结果总拖延时间长短进行排序
         * null - 根据结果编号进行排序
         * 
         * */

        public ActionResult ResultTable(int? id)
        {
            List<CResult> results = getHistory();
            if(id != null)
            {
                switch (id)
                {
                    case 0:
                        results.Sort(delegate (CResult a, CResult b)
                        {
                            return a.TotalDistance.CompareTo(b.TotalDistance);
                        });
                        break;
                    case 1:
                        results.Sort(delegate (CResult a, CResult b)
                        {
                            return a.TotalDuration.CompareTo(b.TotalDuration);
                        });
                        break;
                    case 2:
                        results.Sort(delegate (CResult a, CResult b)
                        {
                            return a.TotalLate.CompareTo(b.TotalLate);
                        });
                        break;
                    case 3:
                        results.Sort(delegate (CResult a, CResult b)
                        {
                            return -a.TotalDistance.CompareTo(b.TotalDistance);
                        });
                        break;
                    case 4:
                        results.Sort(delegate (CResult a, CResult b)
                        {
                            return -a.TotalDuration.CompareTo(b.TotalDuration);
                        });
                        break;
                    case 5:
                        results.Sort(delegate (CResult a, CResult b)
                        {
                            return -a.TotalLate.CompareTo(b.TotalLate);
                        });
                        break;
                    case 6:
                        results.Sort(delegate (CResult a, CResult b)
                        {
                            return a.ID.CompareTo(b.ID);
                        });
                        break;
                    case 7:
                        results.Sort(delegate (CResult a, CResult b)
                        {
                            return -a.ID.CompareTo(b.ID);
                        });
                        break;

                    default:
                        Console.WriteLine("ERR AURGUMENT IN RESULTTABLE REQUEST");
                        break;
                }
                return View(results);
            }


            return View(getHistory());
        }

        public List<int> GetList(string str)
        {
            List<int> data = new List<int>();
            if (str.Length == 0) return data;
            string[] arr = str.Split(',');
            foreach (string a in arr)
                data.Add(int.Parse(a));
            return data;
        }

        public ActionResult GetOrderInfo()
        {
            List<MenuOrder> data = (List<MenuOrder>)Session["cal_data"];
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetManualResult()
        {
            string str = Session["str"].ToString();

            int pointer = 0;
            int last_pointer = 0;

            string one_array = "";

            Dictionary<string, List<int>> our_dic = new Dictionary<string, List<int>>();

            ScheduleMaxTime = int.Parse(Session["ScheduleMaxTime"].ToString());
            WorkerNum = int.Parse(Session["WorkerNum"].ToString());

            for (int i = 0; i < ScheduleMaxTime; i++)
            {
                for (int j = 0; j < WorkerNum; j++)
                {
                    while (str[pointer] != ':')
                        pointer++;

                    if (last_pointer != pointer)
                        one_array = str.Substring(last_pointer, pointer - last_pointer);
                    else
                        one_array = "";

                    Session["S" + i + "_" + j] = one_array;
                    our_dic.Add("S" + i + "_" + j, GetList(one_array));

                    pointer += 1;
                    last_pointer = pointer;
                }
            }
            return Json(our_dic, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult RealtimeCalculate(string str)
        {
            int pointer = 0;
            int last_pointer = 0;

            string one_array = "";

            List<List<List<int>>> job_data = new List<List<List<int>>>();

            ScheduleMaxTime = int.Parse(Session["ScheduleMaxTime"].ToString());
            WorkerNum = int.Parse(Session["WorkerNum"].ToString());


            for (int i = 0; i < ScheduleMaxTime; i++)
            {
                job_data.Add(new List<List<int>>());
                for (int j = 0; j < WorkerNum; j++)
                {
                    job_data[i].Add(new List<int>());
                    while (str[pointer] != ':')
                        pointer++;

                    if (last_pointer != pointer)
                        one_array = str.Substring(last_pointer, pointer - last_pointer);
                    else
                        one_array = "";

                    job_data[i][j].AddRange(GetList(one_array));

                    Session["S" + i + "_" + j] = one_array;

                    pointer += 1;
                    last_pointer = pointer;
                }
            }


            return Content(str);
        }

        public ActionResult RandomGenerate()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            db.MenuOrders.AddRange(ProblemUtils.GetTestOrders());
            db.SaveChanges();
            return Content("随机订单生成完毕");
        }
    }
}