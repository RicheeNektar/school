using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixBildung
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] a = new int[] { 2, 4 };
            int[] b = new int[] { 3, 5, 7 };

            PrintMatrix(CalculateMatrix(a, b));
            Console.ReadLine();
        }

        static void PrintMatrix(int[,] matrix)
        {
            for(int i = 0; i<matrix.GetLength(0); i++)
            {
                for(int j = 0; j<matrix.GetLength(1); j++)
                {
                    int number = matrix[i, j];

                    if(number<10)
                    {
                        Console.Write(' ');
                    }
                    Console.Write("{0} ", number);
                }
                Console.Write("\n");
            }
        }

        static int[,] CalculateMatrix(int[] a, int[] b)
        {
            int[,] returned = new int[a.Length, b.Length];

            for(int i = 0; i<a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    returned[i, j] = a[i] * b[j];
                }
            }

            return returned;
        }
    }
}
