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
            string mathString = $"2+(3+8){SQRT}(2+2)*5^4^5";

            MathBuffer buffer = new MathBuffer(mathString);

            Console.WriteLine(mathString);
            Console.WriteLine(buffer.ChangePowToFunction(buffer.ChangeSqrtToFunction()));
            Console.WriteLine(buffer.ChangePowToResult(buffer.ChangeSqrtToResult()));
            Console.WriteLine(buffer.Eval());
            Console.ReadKey();
        }
    }
}
