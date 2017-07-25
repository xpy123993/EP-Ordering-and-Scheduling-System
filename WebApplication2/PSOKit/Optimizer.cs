using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSOTSP
{

    /*
     * 
     * 用于进行任务调度及路径规划
     * 
     * Solution minimize(ProblemInstance instance, Evaluator evaluator)
     * 
     * 参数
     * instance:ProblemInstance     问题描述
     * evaluator:Evaluator          目标函数
     * 
     * 返回值：
     * job_ids: array[m]            每个外卖骑士的任务序列
     * 
     * */

    public interface Optimizer
    {
        Solution minimize(ProblemInstance instance, Evaluator evaluator);
    }

    public class OptimizerFactory
    {
        public static Optimizer getGreedyOptimizer()
        {
            return new GreedyOptimizer();
        }

        public static Optimizer getImprovedGreedyOptimizer()
        {
            return new ImprovedGreedyOptimizer();
        }

        public static Optimizer getPSOOptimizer()
        {
            return new PSOOptimizer();
        }

        public static Optimizer getScannerOptimizer()
        {
            return new ScannerOptimizer();
        }

        public static Optimizer getGeneticAlgorithmOptimizer()
        {
            return new GeneticAlgorithmOptimizer();
        }
    }

    public class SolutionUtils
    {
        public static Solution getRandomSolution(ProblemInstance problem)
        {
            Solution solution = new Solution(problem.m);
            List<int> order_ids = new List<int>(problem.n);
            for (int i = 0; i < problem.n; i++)
                order_ids.Add(i);

            int selected_id = 0;

            for(int i = 0; i < problem.n; i++)
            {
                selected_id = order_ids[MathUtils.u(0, order_ids.Count)];
                order_ids.Remove(selected_id);
                solution.job_ids[MathUtils.u(0, problem.m)].Add(selected_id);
            }
            return solution;
        }
    }

    public class Solution
    {
        public List<int>[] job_ids = new List<int>[0];
        public string tag = "";
        public Solution(int m)
        {
            job_ids = new List<int>[m];
            for (int i = 0; i < m; i++)
                job_ids[i] = new List<int>();
        }
        public Solution(Solution solution)
        {
            job_ids = new List<int>[solution.job_ids.Length];
            for (int i = 0; i < job_ids.Length; i++)
                job_ids[i] = new List<int>(solution.job_ids[i]);
        }

        public override string ToString()
        {
            String text = "";
            for(int i = 0; i < job_ids.Length; i++)
            {
                text += "任务序列-骑士" + i + ":\n";
                foreach (int value in job_ids[i])
                    text += value + " ";
                text += "\n";
            }
            return text;
        }
    }

    public class GreedyOptimizer: Optimizer
    {
        public Solution minimize(ProblemInstance instance, Evaluator evaluator)
        {
            Solution solution = new Solution(instance.m);
            Cost cost = evaluator.evaluate(instance, solution);

            int current_worker = 0;

            for(int i = 0; i < instance.n; i++)
            {
                current_worker = MathUtils.argmin(cost.costs);
                solution.job_ids[current_worker].Add(i);
                cost = evaluator.evaluate(instance, solution);
            }
            solution.tag = "贪心算法";
            return solution;
        }
    }

    public class ImprovedGreedyOptimizer : Optimizer
    {
        public Solution minimize(ProblemInstance instance, Evaluator evaluator)
        {
            Solution solution = new Solution(instance.m);
            Cost cost = evaluator.evaluate(instance, solution);

            int current_worker = 0;
            double[] worker_costs = new double[instance.m];
            int min_worker_cost_index = 0;

            for (int i = 0; i < instance.n; i++)
            {
                min_worker_cost_index = 0;

                for (int j = 0; j < instance.m; j++)
                {
                    solution.job_ids[j].Add(i);
                    cost = evaluator.evaluate(instance, solution);
                    worker_costs[j] = cost.costs[MathUtils.argmax(cost.costs)];
                    solution.job_ids[j].Remove(i);

                    if (worker_costs[min_worker_cost_index] > worker_costs[j])
                        min_worker_cost_index = j;
                }
                current_worker = min_worker_cost_index;
                solution.job_ids[current_worker].Add(i);
                cost = evaluator.evaluate(instance, solution);
            }
            solution.tag = "改进贪心算法";
            return solution;
        }
    }

    public class ScannerOptimizer: Optimizer
    {
        private ProblemInstance problem;
        private Evaluator evaluator;

        public double angles(JobDetails jobDetails)
        {
            if (jobDetails.x >= 0)
                return Math.Asin(jobDetails.y / (Math.Sqrt(jobDetails.x * jobDetails.x + jobDetails.y * jobDetails.y)));
            return 3.14 - Math.Asin(jobDetails.y / (Math.Sqrt(jobDetails.x * jobDetails.x + jobDetails.y * jobDetails.y)));
        }

        public Solution minimize(ProblemInstance problem, Evaluator evaluator)
        {
            this.problem = problem;
            this.evaluator = evaluator;
            Solution solution = new Solution(problem.m);

            List<int> order_ids = new List<int>();
            for(int i = 0; i < problem.n; i++)
            {
                order_ids.Add(i);
            }
            order_ids.Sort((x, y) =>
            {
                JobDetails job_x = problem.job_details[x];
                JobDetails job_y = problem.job_details[y];

                double d = angles(job_x) - angles(job_y);
                if (d < 0) return -1;
                if (d == 0) return 0;
                return 1;
            });

            int jobPerMachine = 1 + problem.n / problem.m, machine_id;
            for(int i = 0; i < problem.n; i++)
            {
                machine_id = i / jobPerMachine;
                solution.job_ids[machine_id].Add(order_ids[i]);
            }
            solution.tag = "扫描算法";
            return solution;
        }
    }

    public class PSOOptimizer: Optimizer
    {

        public const double learnRate = 1e-1;
        public const double inertia = 1e-1;
        public const double p_best_rate = 0.4;
        public const double g_best_rate = 0.4;
        public const double cross_over_rate = 1e-1;

        public const int population_size = 100;
        public const int iteration_times = 1000;

        private class DataPack
        {
            public int[] data = new int[0];

            public double getValue(ProblemInstance problem, Evaluator evaluator)
            {
                return evaluator.evaluate(problem, getSolution(problem)).totalCost;
            }

            public DataPack(DataPack dataPack)
            {
                data = new int[dataPack.data.Length];
                for (int i = 0; i < data.Length; i++)
                    data[i] = dataPack.data[i];
            }
            public DataPack() { }
            public static DataPack parseDataPack(ProblemInstance problem, Solution solution)
            {
                DataPack dataPack = new DataPack();
                dataPack.data = new int[problem.m + problem.n - 1];
                int pointer = 0;
                for(int i = 0; i < problem.m; i++)
                {
                    if (i > 0) dataPack.data[pointer++] = -1;
                    for (int j = 0; j < solution.job_ids[i].Count; j++)
                        dataPack.data[pointer++] = solution.job_ids[i][j];
                }

                return dataPack;
            }
            public Solution getSolution(ProblemInstance problem)
            {
                int machine_id = 0;
                Solution solution = new Solution(problem.m);
                for(int i = 0; i < data.Length; i++)
                {
                    if (data[i] == -1)
                        machine_id++;
                    else
                        solution.job_ids[machine_id].Add(data[i]);
                }
                return solution;
            }
        }
        private class Particle
        {
            private DataPack position, velocity;
            private DataPack p_best;
            private double p_best_value;

            private ProblemInstance problem;
            private Evaluator evaluator;

            public Particle(ProblemInstance problem, Evaluator evaluator, Solution solution)
            {
                this.problem = problem;
                this.evaluator = evaluator;

                position = DataPack.parseDataPack(problem, solution);
                velocity = DataPack.parseDataPack(problem, solution);

                p_best = new DataPack(position);
                p_best_value = p_best.getValue(problem, evaluator);
            }

            public Particle(ProblemInstance problem, Evaluator evaluator)
            {
                this.problem = problem;
                this.evaluator = evaluator;

                position = DataPack.parseDataPack(problem, SolutionUtils.getRandomSolution(problem));
                velocity = DataPack.parseDataPack(problem, SolutionUtils.getRandomSolution(problem));

                p_best = new DataPack(position);
                p_best_value = p_best.getValue(problem, evaluator);
            }

            public void mutation(List<Particle> swarm)
            {
                int selected_id = MathUtils.u(0, swarm.Count);
                this.velocity = new DataPack(swarm[selected_id].velocity);
            }

            public void update(ref DataPack g_best, ref double g_best_value, List<Particle> swarm)
            {
                if (p_best_value < g_best_value)
                {
                    g_best_value = p_best_value;
                    g_best = new DataPack(p_best);
                }

                DataPack v_p_best = substract(p_best, position);
                DataPack v_g_best = substract(g_best, position);

                DataPack r0 = generate_binary_vector(inertia);
                DataPack r1 = generate_binary_vector(p_best_rate);
                DataPack r2 = generate_binary_vector(g_best_rate);

                if (MathUtils.probability(cross_over_rate))
                    mutation(swarm);

                DataPack v0 = multiply(r0, velocity);
                DataPack v1 = multiply(r1, v_p_best);
                DataPack v2 = multiply(r2, v_g_best);

                position = mix(position, v0);
                position = mix(position, v1);
                position = mix(position, v2);

                double temp_value = position.getValue(problem, evaluator);
                if (temp_value < p_best_value)
                {
                    p_best_value = temp_value;
                    p_best = new DataPack(position);
                    if (temp_value < g_best_value)
                    {
                        g_best_value = temp_value;
                        g_best = new DataPack(position);
                    }
                }
            }

            private DataPack mix(DataPack a, DataPack b)
            {
                DataPack result = new DataPack(a);
                for (int i = 0; i < result.data.Length; i++)
                {
                    if (b.data[i] != -2 && MathUtils.probability(learnRate))
                    {
                        result.data[MathUtils.indexOf(result.data, b.data[i])] = result.data[i];
                        result.data[i] = b.data[i];
                    }
                }
                return result;
            }

            private DataPack generate_binary_vector(double p)
            {
                DataPack result = new DataPack();
                result.data = new int[problem.m + problem.n - 1];
                for (int i = 0; i < result.data.Length; i++)
                {
                    if (MathUtils.probability(p))
                        result.data[i] = 1;
                    else
                        result.data[i] = 0;
                }
                return result;
            }

            private DataPack multiply(DataPack binary_vector, DataPack velocity)
            {
                DataPack result = new DataPack();
                result.data = new int[problem.m + problem.n - 1];
                for (int i = 0; i < result.data.Length; i++)
                {
                    if (binary_vector.data[i] == 1)
                        result.data[i] = velocity.data[i];
                    else
                        result.data[i] = -2;
                }
                return result;
            }

            private DataPack substract(DataPack better_solution, DataPack solution)
            {
                DataPack result = new DataPack();
                result.data = new int[better_solution.data.Length];
                for (int i = 0; i < result.data.Length; i++)
                {
                    if (better_solution.data[i] == solution.data[i])
                        result.data[i] = -2;
                    else
                        result.data[i] = better_solution.data[i];
                }
                return result;
            }
        }

        public Solution minimize(ProblemInstance problem, Evaluator evaluator)
        {

            DataPack g_best = DataPack.parseDataPack(problem, SolutionUtils.getRandomSolution(problem));
            double g_best_value = g_best.getValue(problem, evaluator);

            double last_value = -1;

            List<Particle> particles = new List<Particle>();
            for (int i = 0; i < population_size; i++)
                particles.Add(new Particle(problem, evaluator));

            Optimizer optimizer = new ImprovedGreedyOptimizer();
            particles.Add(new Particle(problem, evaluator, optimizer.minimize(problem, evaluator)));

            optimizer = new ScannerOptimizer();
            particles.Add(new Particle(problem, evaluator, optimizer.minimize(problem, evaluator)));

            for (int i = 0; i < iteration_times; i++)
            {
                foreach (Particle particle in particles)
                    particle.update(ref g_best, ref g_best_value, particles);
                if(last_value != g_best_value)
                {
                    last_value = g_best_value;
                    Console.WriteLine("OPT = " + last_value.ToString());
                }
            }
            Solution solution = g_best.getSolution(problem);
            solution.tag = "粒子群优化算法";
            return solution;
        }
    }

    public class GeneticAlgorithmOptimizer : Optimizer
    {

        public const double learnRate = 1e-1;
        public const double inertia = 1e-1;
        public const double p_best_rate = 0.4;
        public const double g_best_rate = 0.4;
        public const double cross_over_rate = 1e-1;

        public const int population_size = 100;
        public const int iteration_times = 1000;

        class DataPack
        {
            public int[] data = new int[0];

            public double getValue(ProblemInstance problem, Evaluator evaluator)
            {
                return evaluator.evaluate(problem, getSolution(problem)).totalCost;
            }

            public DataPack(DataPack dataPack)
            {
                data = new int[dataPack.data.Length];
                for (int i = 0; i < data.Length; i++)
                    data[i] = dataPack.data[i];
            }
            public DataPack() { }
            public static DataPack parseDataPack(ProblemInstance problem, Solution solution)
            {
                DataPack dataPack = new DataPack();
                dataPack.data = new int[problem.m + problem.n - 1];
                int pointer = 0;
                for (int i = 0; i < problem.m; i++)
                {
                    if (i > 0) dataPack.data[pointer++] = -1;
                    for (int j = 0; j < solution.job_ids[i].Count; j++)
                        dataPack.data[pointer++] = solution.job_ids[i][j];
                }

                return dataPack;
            }
            public Solution getSolution(ProblemInstance problem)
            {
                int machine_id = 0;
                Solution solution = new Solution(problem.m);
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == -1)
                        machine_id++;
                    else
                        solution.job_ids[machine_id].Add(data[i]);
                }
                return solution;
            }
        }
        
        private DataPack crossover(DataPack dataPack1, DataPack dataPack2)
        {
            DataPack result = new DataPack(dataPack1);
            for (int i = 0; i < result.data.Length; i++)
            {
                if (MathUtils.probability(learnRate))
                {
                    result.data[MathUtils.indexOf(result.data, dataPack2.data[i])] = result.data[i];
                    result.data[i] = dataPack2.data[i];
                }
            }
            return result;
        }

        private DataPack mutation(DataPack dataPack)
        {
            DataPack result = new DataPack(dataPack);

            int rand1 = MathUtils.u(0, dataPack.data.Length);
            int rand2 = MathUtils.u(0, dataPack.data.Length);

            result.data[rand1] = dataPack.data[rand2];
            result.data[rand2] = dataPack.data[rand1];

            return result;
        }

        private void sort(List<DataPack> chrs, ProblemInstance problem, Evaluator evaluator)
        {
            chrs.Sort((x, y) =>
            {
                double cost_x = x.getValue(problem, evaluator);
                double cost_y = y.getValue(problem, evaluator);
                if (cost_x == cost_y) return 0;
                return cost_x < cost_y ? -1 : 1;
            });
        }

        public Solution minimize(ProblemInstance problem, Evaluator evaluator)
        {

            List<DataPack> chrs = new List<DataPack>();
            for(int i = 0; i < population_size; i++)
                chrs.Add(DataPack.parseDataPack(problem, SolutionUtils.getRandomSolution(problem)));

            Optimizer optimizer = new ImprovedGreedyOptimizer();
            chrs.Add(DataPack.parseDataPack(problem, optimizer.minimize(problem, evaluator)));

            optimizer = new ScannerOptimizer();
            chrs.Add(DataPack.parseDataPack(problem, optimizer.minimize(problem, evaluator)));

            sort(chrs, problem, evaluator);

            double last_value = -1, g_best_value;

            for(int i = 0; i < iteration_times; i++)
            {
                for(int j = 0; j < population_size; j ++)
                {
                    DataPack dataPack = chrs[j];
                    if (MathUtils.probability(cross_over_rate))
                        chrs.Add(crossover(dataPack, chrs[MathUtils.u(0, chrs.Count)]));
                    else
                        chrs.Add(mutation(dataPack));
                }
                sort(chrs, problem, evaluator);
                chrs = chrs.GetRange(0, population_size);
                g_best_value = chrs[0].getValue(problem, evaluator);

                if (last_value != g_best_value)
                {
                    last_value = g_best_value;
                    Console.WriteLine("OPT = " + last_value.ToString());
                }
            }

            sort(chrs, problem, evaluator);
            Solution solution = chrs[0].getSolution(problem);
            solution.tag = "遗传算法";
            return solution;
        }
    }
}
