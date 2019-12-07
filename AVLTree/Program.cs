using System;
using System.Collections.Generic;
using System.Linq;

namespace AVLTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = int.Parse(Console.ReadLine());
            var ress = new List<int>();
            foreach (var _ in Enumerable.Range(0, t))
            {
                var inputs = Console.ReadLine().Split().Select(int.Parse).ToArray();
                Array.Sort(inputs);
                var diffMinMed = inputs[1] - inputs[0];
                var res = diffMinMed;
                inputs[2] -= diffMinMed;
                inputs[1] -= diffMinMed;
                var n = Math.Min(inputs[2] - inputs[0], inputs[0]);
                inputs[2] -= n * 2;
                inputs[0] -= n;
                inputs[1] -= n;

                res += 2 * n;

                if (inputs[2] == inputs[0])
                {
                    res += (inputs[2] / 2) * 3;
                    res += inputs[2] % 2;
                }
                else
                {
                    res += inputs[0] % 2;
                }

                ress.Add(res);
            }

            foreach (var i in ress)
            {
                Console.WriteLine(i);
            }
        }
    }
}
