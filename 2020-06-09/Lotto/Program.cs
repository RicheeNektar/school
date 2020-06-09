using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotto
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] selection = new int[] { 3, 10, 17, 24, 31, 38 };

            PrintNumbers(selection);
            
            Console.ReadLine();
        }

        static void PrintNumbers(int[] selection)
        {
            for(int i = 1; i<50; i++)
            {
                bool isSelected = selection.Any(number => number == i);
                
                if(isSelected)
                {
                    Console.Write(" X ");
                }
                else
                {
                    if(i < 10)
                    {
                        Console.Write(' ');
                    }

                    Console.Write("{0} ", i);
                }

                if (i % 7 == 0)
                {
                    Console.Write('\n');
                }
            }
        }
    }
}
