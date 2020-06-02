using Nejman.MathParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class Program
    {
        public const string SQRT = "√";
        static void Main(string[] args)
        {
            string mathString = $"2+2{SQRT}4^2+5%";

            MathBuffer buffer = new MathBuffer(mathString);

            Console.WriteLine(mathString);
            Console.WriteLine(buffer.ChangeToFunction());
            Console.WriteLine(buffer.Eval());
            Console.ReadKey();
        }
    }
}
