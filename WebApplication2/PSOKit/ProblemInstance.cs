using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSOTSP
{
    /*
     * 
     * 问题描述类
     * 
     * 成员参数
     * 
     * m: int   资源数目
     * n: int   任务数目
     * 
     * job_details:  JobDetails[N]   任务地理位置描述 
     * 
     * */


    public class ProblemFactory
    {
        public static ProblemInstance getRandomProblemInstance(int m, int n, 
            int unique_position_num, double max_range)
        {
            ProblemInstance problemInstance = new ProblemInstance();
            double max_x = Math.Sin(3.1415926 / 4) * max_range;
            double max_y = max_x;

            problemInstance.m = m;
            problemInstance.n = n;

            JobDetails[] u_jobDetails = new JobDetails[unique_position_num];

            for(int i = 0; i < unique_position_num; i++)
            {
                u_jobDetails[i] = new JobDetails();
                u_jobDetails[i].x = MathUtils.u(-(int)max_x, (int)max_x);
                u_jobDetails[i].y = MathUtils.u(-(int)max_y, (int)max_y);
            }

            problemInstance.job_details = new JobDetails[n];
            for(int i = 0; i < n; i++)
                problemInstance.job_details[i] = u_jobDetails[i % unique_position_num];
            

            return problemInstance;
        }

        public static ProblemInstance getCustomProblemInstance()
        {
            ProblemInstance problemInstance = new ProblemInstance();

            problemInstance.m = 2;      //有几个骑士
            problemInstance.n = 5;      //有几个地理坐标

            problemInstance.job_details = new JobDetails[problemInstance.n];

            //因为n = 5，所以下面有5个坐标，最好在[-50, 50]之间，保证显示效果
            int pointer = 0;
            problemInstance.job_details[pointer++] = new JobDetails(10, 10);
            problemInstance.job_details[pointer++] = new JobDetails(8, 10);
            problemInstance.job_details[pointer++] = new JobDetails(6, 10);
            problemInstance.job_details[pointer++] = new JobDetails(4, 10);
            problemInstance.job_details[pointer++] = new JobDetails(2, 10);

            return problemInstance;
        }

    }
    public class ProblemInstance
    {
        public int n = 0;
        public int m = 0;

        public JobDetails[] job_details = new JobDetails[0];

        public override string ToString()
        {
            string text = "任务数：" + n + "\n资源数：" + m + "\n";
            for(int i = 0; i < job_details.Length; i++)
            {
                text += "任务" + i + ": (" + job_details[i].x + "," + job_details[i].y + ")\n";
            }
            return text;
        }
    }

    public class JobDetails
    {
        public double x = 0;
        public double y = 0;

        public JobDetails() { }
        public JobDetails(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

}
