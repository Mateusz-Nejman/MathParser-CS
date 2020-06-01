using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nejman.MathParser
{
    public class MathBuffer
    {
        private readonly string signs = "+-*x/";
        public string Buffer { get; private set; } = "";
        private readonly MathParser parser;
        public MathBuffer(string baseBuffer = "")
        {
            parser = new MathParser();
            Buffer = baseBuffer;
        }

        public string Add(char text)
        {
            return Add(text.ToString());
        }

        private string Add(string text)
        {
            string validChars = $"1234567890()+-*x/{MathParser.SQRT}{MathParser.POW}.";
            if (validChars.Contains(text))
            {
                if (Buffer.Length > 1)
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
                }
            }
            else
                text = "";
            Buffer += text;

            return Buffer;
        }

        public string ChangeSqrtToResult(string text = "")
        {
            string temp = text.Length == 0 ? Buffer : text;
            var loDataTable = new DataTable();
            var loDataColumn = new DataColumn("Eval", typeof(double), "");
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);

            while (true)
            {
                if (temp.Contains(MathParser.SQRT))
                {
                    int currentIndex = temp.IndexOf(MathParser.SQRT);
                    string leftSide = parser.LeftSide(temp, currentIndex, true);
                    string rightSide = parser.RightSide(temp, currentIndex, true);
                    string leftTemp = temp.Substring(0, currentIndex - leftSide.Length);
                    string rightTemp = temp.Substring(currentIndex + 1 + rightSide.Length);

                    if (leftSide.Length > 0)
                        leftSide += "*";

                    loDataColumn = new DataColumn("Eval", typeof(double), rightSide);
                    loDataTable.Columns.RemoveAt(0);
                    loDataTable.Columns.Add(loDataColumn);

                    double rightVal = (double)(loDataTable.Rows[0]["Eval"]);

                    temp = leftTemp + leftSide + Math.Sqrt(rightVal)+ rightTemp;
                }
                else
                    break;
            }

            loDataColumn.Dispose();
            loDataTable.Dispose();

            return temp;
        }

        public string ChangePowToResult(string text = "")
        {
            string temp = text.Length == 0 ? Buffer : text;
            var loDataTable = new DataTable();
            var loDataColumn = new DataColumn("Eval", typeof(double), "");
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            //return (double)(loDataTable.Rows[0]["Eval"]);
            while (true)
            {
                if (temp.Contains(MathParser.POW))
                {
                    int currentIndex = temp.IndexOf(MathParser.POW);
                    string leftSide = parser.LeftSide(temp, currentIndex);
                    string rightSide = parser.RightSide(temp, currentIndex);
                    string leftTemp = temp.Substring(0, currentIndex - leftSide.Length);
                    string rightTemp = temp.Substring(currentIndex + 1 + rightSide.Length);

                    loDataColumn = new DataColumn("Eval", typeof(double), leftSide);
                    loDataTable.Columns.RemoveAt(0);
                    loDataTable.Columns.Add(loDataColumn);

                    double leftVal = (double)(loDataTable.Rows[0]["Eval"]);

                    loDataColumn = new DataColumn("Eval", typeof(double), rightSide);
                    loDataTable.Columns.RemoveAt(0);
                    loDataTable.Columns.Add(loDataColumn);

                    double rightVal = (double)(loDataTable.Rows[0]["Eval"]);

                    temp = leftTemp + Math.Pow(leftVal,rightVal) + rightTemp;
                }
                else
                    break;
            }

            loDataColumn.Dispose();
            loDataTable.Dispose();

            return temp;
        }

        public string ChangeSqrtToFunction(string text = "")
        {
            string temp = text.Length == 0 ? Buffer : text;
            while(true)
            {
                if (temp.Contains(MathParser.SQRT))
                {
                    int currentIndex = temp.IndexOf(MathParser.SQRT);
                    string leftSide = parser.LeftSide(temp, currentIndex,true);
                    string rightSide = parser.RightSide(temp, currentIndex,true);
                    string leftTemp = temp.Substring(0, currentIndex - leftSide.Length);
                    string rightTemp = temp.Substring(currentIndex + 1 + rightSide.Length);

                    if (leftSide.Length > 0)
                        leftSide += "*";

                    temp = leftTemp + leftSide + "[" + rightSide + "]" + rightTemp;
                }
                else
                    break;
            }

            temp = temp.Replace("[", "Math.Sqrt(").Replace("]", ")");

            return temp;
        }

        public string ChangePowToFunction(string text = "")
        {
            string temp = text.Length == 0 ? Buffer : text;
            while (true)
            {
                if (temp.Contains(MathParser.POW))
                {
                    int currentIndex = temp.IndexOf(MathParser.POW);
                    string leftSide = parser.LeftSide(temp, currentIndex,true);
                    string rightSide = parser.RightSide(temp, currentIndex,true);
                    string leftTemp = temp.Substring(0, currentIndex - leftSide.Length);
                    string rightTemp = temp.Substring(currentIndex + 1 + rightSide.Length);
                    

                    temp = leftTemp + "[" + leftSide + ","+rightSide+"]" + rightTemp;
                }
                else
                    break;
            }

            temp = temp.Replace("[", "Math.Pow(").Replace("]", ")");

            return temp;
        }

        public double Eval()
        {
            string mathString = ChangeSqrtToResult(ChangePowToResult(Buffer));
            var loDataTable = new DataTable();
            var loDataColumn = new DataColumn("Eval", typeof(double), mathString);
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            double val = (double)(loDataTable.Rows[0]["Eval"]);
            loDataColumn.Dispose();
            loDataTable.Dispose();
            return val;
        }

        public override string ToString()
        {
            return Buffer;
        }
    }
}
