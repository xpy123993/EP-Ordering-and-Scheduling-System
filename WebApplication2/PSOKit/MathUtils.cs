using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSOTSP
{
    public class MathUtils
    {
        private static Random random = new Random();

        public static int argmin(double[] data)
        {
            int min_index = 0;
            for(int i = 1; i < data.Length; i++)
            {
                if (data[min_index] > data[i])
                    min_index = i;
            }
            return min_index;
        }

        public static int argmax(double[] data)
        {
            int max_index = 0;
            for(int i = 1; i < data.Length; i++)
            {
                if (data[max_index] < data[i])
                    max_index = i;
            }
            return max_index;
        }

        public static int u(int min, int max)
        {
            int size = max - min;
            return min + random.Next(size);
        }

        public static bool probability(double p)
        {
            return p >= random.NextDouble();
        }

        public static int indexOf(int[] data, int value)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if (value == data[i])
                    return i;
            }
            return -1;
        }
    }
}
