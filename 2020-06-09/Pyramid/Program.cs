using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyramid
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] pyramid = ConstructPyramid(8,8);

            PrintPyramid(pyramid);

            Console.ReadLine();
        }

        static void PrintPyramid(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    int number = matrix[i, j];

                    if (number < 10)
                    {
                        Console.Write(' ');
                    }
                    Console.Write("{0} ", number);
                }
                Console.Write("\n");
            }
        }

        static int[,] ConstructLayer(int[,] pyramid, int step)
        {
            int width = pyramid.GetLength(0);
            int height = pyramid.GetLength(1);

            for(int i = step; i<width-step; i++)
            {
                for(int j = step; j<height-step; j++)
                {
                    pyramid[i, j]++;
                }
            }

            return pyramid;
        }

        static int[,] ConstructPyramid(int width, int height)
        {
            int[,] returned = new int[width, height];

            float xCenter = width / 2.0f;
            float yCenter = height / 2.0f;
            float smallestCenter = Math.Min(xCenter, yCenter);

            for(int i = 1; i<smallestCenter; i++)
            {
                returned = ConstructLayer(returned, i);
            }


            return returned;
        }
    }
}
