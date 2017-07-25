using PSOTSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.PSOKit
{
    // 用于将待解决问题翻译为PSOKit模块可以理解的问题

    public class Translate
    {

        public static Solution Solve(IEnumerable<MenuOrder> menuOrders, IEnumerable<Worker> workers)
        {
            ProblemInstance instance = GetInstance(menuOrders, workers);
            Evaluator evaluator = EvaluatorFactory.getMakespanEvaluator();
            Optimizer optimizer = OptimizerFactory.getPSOOptimizer();   //粒子群优化器，可以调用其他的

            return optimizer.minimize(instance, evaluator);
        }

        public static ProblemInstance GetInstance(IEnumerable<MenuOrder> menuOrders, IEnumerable<Worker> workers)
        {
            ProblemInstance problemInstance = new ProblemInstance();

            problemInstance.n = menuOrders.Count();
            problemInstance.m = workers.Count();

            JobDetails[] job_details = new JobDetails[problemInstance.n];
            List<MenuOrder> menuOrders_list = menuOrders.ToList();

            for(int i = 0; i < job_details.Length; i++)
            {
                job_details[i] = new JobDetails(Convert.ToDouble(100 * menuOrders_list[i].JD), Convert.ToDouble(100 * menuOrders_list[i].WD));
            }
            problemInstance.job_details = job_details;

            return problemInstance;
        }


    }



}