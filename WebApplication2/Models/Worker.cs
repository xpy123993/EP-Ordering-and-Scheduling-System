using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Worker
    {

        public int ID { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "手机号码")]
        public string PhoneNumber { get; set; }

        [Display(Name = "调度次数")]
        public int ScheduleTimes { get; set; }

        [Display(Name = "下次可用时间")]
        public DateTime ReadyTime { get; set; }

    }
}