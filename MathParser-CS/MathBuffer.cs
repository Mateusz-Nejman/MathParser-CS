using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nejman.MathParser
{
    public class MathBuffer : IDisposable
    {
        private readonly string signs = "+-*x/";
        public string Buffer { get; private set; } = "";
        private readonly MathParser parser;
        private readonly EvalParser evalParser;
        public MathBuffer(string baseBuffer = "")
        {
            parser = new MathParser();
            evalParser = new EvalParser();
            Buffer = baseBuffer;
        }

        public string Add(char text)
        {
            return Add(text.ToString());
        }

        private string Add(string text)
        {
            string validChars = $"1234567890()+-*x/{MathParser.SQRT}{MathParser.POW}.{MathParser.PI}%";
            if (validChars.Contains(text))
            {
                if (Buffer.Length >= 1)
                {
                    string leftSide = parser.LeftSide(Buffer, Buffer.Length);
                    char lastChar = Buffer[Buffer.Length - 1];
                    if (text == "." && leftSide.Contains("."))
                        text = "";
                    else if (text == "." && (signs.Contains(lastChar) || lastChar == MathParser.POW || lastChar == MathParser.POW))
                        text = "0.";
                    else if (signs.Contains(text) && signs.Contains(lastChar))
                    {
                        Buffer = Buffer.Substring(0, Buffer.Length - 1) + text;
                        text = "";
                    }
                    else if (signs.Contains(text) && (lastChar == MathParser.SQRT || lastChar == MathParser.POW))
                        text = "";
                    else if (leftSide == "0" && text == "0")
                        text = "";
                    else if (signs.Contains(text) && lastChar == '.')
                        text = "";
                    else if ((lastChar == MathParser.SQRT || lastChar == MathParser.POW) && (text.Contains(MathParser.POW) || text.Contains(MathParser.SQRT)))
                        text = "";
                    else if ((lastChar == '%' && text == "%"))
                        text = "";
                }
                else
                {
                    if (text == ".")
                        text = "0.";
                }
            }
            else
                text = "";
            Buffer += text;

            return Buffer;
        }


        public string ChangeToFunction(string text = "", bool changeToFunctions = true)
        {
            string temp = text.Length == 0 ? Buffer : text;
            temp = temp.Replace('x', '*').Replace(MathParser.PI.ToString(),Math.PI.ToString());

            for(int a = 0; a < temp.Length; a++)
            {
                if(temp[a] == MathParser.SQRT)
                {
                    string leftSide = parser.LeftSide(temp, a, true);
                    string rightSide = parser.RightSide(temp, a, true);
                    string leftTemp = temp.Substring(0, a - leftSide.Length);
                    string rightTemp = temp.Substring(a + 1 + rightSide.Length);

                    leftSide = leftSide.Length == 0 ? "1" : leftSide;

                    temp = leftTemp + "[" + leftSide + ";" + rightSide + "]" + rightTemp;
                }
                else if(temp[a] == MathParser.POW)
                {
                    string leftSide = parser.LeftSide(temp, a, true);
                    string rightSide = parser.RightSide(temp, a, true);
                    string leftTemp = temp.Substring(0, a - leftSide.Length);
                    string rightTemp = temp.Substring(a + 1 + rightSide.Length);
                    //Console.WriteLine("Pow "+leftTemp);

                    temp = leftTemp + "<" + leftSide + "," + rightSide + ">" + rightTemp;
                }
                else if(temp[a] == '%')
                {
                    string leftSide = parser.LeftSide(temp, a, true);
                    string leftTemp = temp.Substring(0, a - leftSide.Length);
                    string rightTemp = temp.Substring(a + 1);
                    string val = "100";

                    if(a >= leftSide.Length+1)
                    {
                        val = parser.LeftSide(temp, a - leftSide.Length - 1, true);
                        //Console.WriteLine("Val " + val);
                    }

                    temp = leftTemp + "{" + leftSide + "'" + val + "}" + rightTemp;
                }
            }

            
            if(changeToFunctions)
                temp = temp.Replace(";", "*Math.Sqrt(").Replace("[","").Replace("]", ")").Replace("<", "Math.Pow(").Replace(">", ")").Replace("{","(").Replace("}",")").Replace("'","/100.0*");
            return temp;
        }

        public double Eval()
        {
            string mathString = ChangeToFunction("", false);
            Dictionary<string, double> cache = new Dictionary<string, double>();

            if (cache == null)
                cache = new Dictionary<string, double>();

            Dictionary<char, char> middleSeparators = new Dictionary<char, char>
            {
                { '[', ';' }, //sqrt
                { '<', ',' }, //pow
                { '{', '\'' } //percent
            };

            Dictionary<char, char> endBrackets = new Dictionary<char, char>
            {
                { '[', ']' },
                { '<', '>' },
                { '{', '}' }
            };

            while (true)
            {
                if (!mathString.Contains('[') && !mathString.Contains('<') && !mathString.Contains('{'))
                    break;


                for (int a = 0; a < mathString.Length; a++)
                {
                    if (endBrackets.ContainsKey(mathString[a]))
                    {
                        string content = parser.GetSingleBracketContentRight(mathString, a, mathString[a], endBrackets[mathString[a]], true);
                        //Console.WriteLine("With brackets: " + parser.GetSingleBracketContentRight(text, a, text[a], endBrackets[text[a]], true));
                        string withoutBrackets = content.Substring(1, content.Length - 2);
                        bool clear = true;
                        for (int b = 0; b < withoutBrackets.Length; b++)
                        {
                            if (endBrackets.ContainsKey(withoutBrackets[b]))
                            {
                                clear = false;
                            }
                        }
                        if (clear)
                        {
                            if (!cache.ContainsKey(content))
                            {
                                var table = new DataTable();
                                var loDataColumn = new DataColumn("Eval");
                                table.Columns.Add(loDataColumn);
                                table.Rows.Add(0);

                                char startBracket = content[0];
                                char separator = middleSeparators[startBracket];
                                char endBracket = endBrackets[startBracket];

                                string leftSide = withoutBrackets.Split(separator)[0];
                                string rightSide = withoutBrackets.Split(separator)[1];

                                double leftVal = evalParser.Calculate(leftSide);
                                double rightVal = evalParser.Calculate(rightSide);

                                double val = 0;

                                if (startBracket == '[')
                                    val = leftVal * Math.Sqrt(rightVal);
                                else if (startBracket == '<')
                                    val = Math.Pow(leftVal, rightVal);
                                else if (startBracket == '{')
                                    val = leftVal / 100.0 * rightVal;

                                cache.Add(content, val);
                                mathString = mathString.Replace(content, val.ToString().Replace(",", "."));
                            }
                        }
                        else
                        {
                        }
                    }
                }
            }
            return evalParser.Calculate(mathString);
        }

        public override string ToString()
        {
            return Buffer;
        }

        public void Dispose()
        {
            evalParser?.Dispose();
        }

        private class EvalParser
        {
            private readonly DataTable dataTable;

            public EvalParser()
            {
                dataTable = new DataTable();
                var dataColumn = new DataColumn("Eval");
                dataTable.Columns.Add(dataColumn);
                dataTable.Rows.Add(0);
            }

            public double Calculate(string expression)
            {
                dataTable.Columns.Clear();
                var dataColumn = new DataColumn("Eval", typeof(double), expression);
                dataTable.Columns.Add(dataColumn);
                return (double)dataTable.Rows[0]["Eval"];
            }

            public void Dispose()
            {
                dataTable?.Dispose();
            }
        }
    }
}
