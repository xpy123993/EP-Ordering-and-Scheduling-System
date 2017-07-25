using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSOTSP
{
    /*
     * 
     *  评估当前方案的代价
     *  Cost evaluate(ProblemInstance instance, Solution solution)
     *  
     *  函数将返回该方案的代价
     *  返回值:
     *  totalCost:double    本方案的总代价
     *  costs:double[m]     每个资源（外卖骑士）的代价
     * 
     * */

    public interface Evaluator
    {
        Cost evaluate(ProblemInstance instance, Solution solution);
    }

    public class EvaluatorFactory
    {
        public static Evaluator getDistanceEvaluator()
        {
            return new DistanceEvaluator();
        }
        public static Evaluator getMakespanEvaluator()
        {
            return new MakespanEvaluator();
        }
    }

    public class Cost
    {
        public double totalCost = 0;
        public double[] costs = new double[0];
        public string tag = "";
    }

    public class DistanceEvaluator: Evaluator
    {
        public double distance(JobDetails details1, JobDetails details2)
        {
            double d_x = details1.x - details2.x;
            double d_y = details1.y - details2.y;
            return Math.Sqrt(d_x * d_x + d_y * d_y);
        }
        public double distanceToZero(JobDetails details)
        {
            double d_x = details.x;
            double d_y = details.y;
            return Math.Sqrt(d_x * d_x + d_y * d_y);
        }
        public double getDistanceOfWorker(int workerID, ProblemInstance instance, Solution solution)
        {
            List<int> job_sequence = solution.job_ids[workerID];
            if (job_sequence.Count == 0) return 0;
            double sum = distanceToZero(instance.job_details[job_sequence[0]]);
            for (int i = 1; i < job_sequence.Count; i++)
            {
                sum += distance(instance.job_details[job_sequence[i - 1]], instance.job_details[job_sequence[i]]);
            }
            sum += distanceToZero(instance.job_details[job_sequence[job_sequence.Count - 1]]);
            return sum;
        }

        public Cost evaluate(ProblemInstance instance, Solution solution)
        {
            Cost cost = new Cost();

            cost.costs = new double[instance.m];
            cost.totalCost = 0;

            for(int i = 0; i < instance.m; i++)
            {
                cost.costs[i] = getDistanceOfWorker(i, instance, solution);
                cost.totalCost += cost.costs[i];
            }

            cost.tag = "总路径长度";
            return cost;
        }
    }

    public class MakespanEvaluator : Evaluator
    {
        public double distance(JobDetails details1, JobDetails details2)
        {
            double d_x = details1.x - details2.x;
            double d_y = details1.y - details2.y;
            return Math.Sqrt(d_x * d_x + d_y * d_y);
        }
        public double distanceToZero(JobDetails details)
        {
            double d_x = details.x;
            double d_y = details.y;
            return Math.Sqrt(d_x * d_x + d_y * d_y);
        }
        public double getDistanceOfWorker(int workerID, ProblemInstance instance, Solution solution)
        {
            List<int> job_sequence = solution.job_ids[workerID];
            if (job_sequence.Count == 0) return 0;
            double sum = distanceToZero(instance.job_details[job_sequence[0]]);
            for (int i = 1; i < job_sequence.Count; i++)
            {
                sum += distance(instance.job_details[job_sequence[i - 1]], instance.job_details[job_sequence[i]]);
            }
            sum += distanceToZero(instance.job_details[job_sequence[job_sequence.Count - 1]]);
            return sum;
        }

        public Cost evaluate(ProblemInstance instance, Solution solution)
        {
            Cost cost = new Cost();

            cost.costs = new double[instance.m];
            cost.totalCost = 0;

            for (int i = 0; i < instance.m; i++)
            {
                cost.costs[i] = getDistanceOfWorker(i, instance, solution);
                if (cost.totalCost < cost.costs[i])
                    cost.totalCost = cost.costs[i];
            }

            cost.tag = "调度总时间";

            return cost;
        }
    }
}
