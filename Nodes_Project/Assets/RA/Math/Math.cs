using System.Collections.Generic;

namespace RA.Math
{
    public static class BasicMath
    {
        public static int[] GetDivers(int n)
        {
            List<int> r =new List<int>();
            for (int i = 0; i < n; i++)
            {
                if(n%i > 0)
                {
                    r.Add(i);
                }
            }

            return r.ToArray();
        }
    }
}
