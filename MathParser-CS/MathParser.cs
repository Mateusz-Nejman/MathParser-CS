using System;
using System.Linq;

namespace Nejman.MathParser
{
    public class MathParser
    {
        public const char SQRT = '√';
        public const char POW = '^';
        public string LeftSide(string text, int startIndex, bool withBracket = false)
        {
            string validChars = "1234567890.,";

            if(text[startIndex - 1] == ')' || text[startIndex - 1] == ']')
            {
                return GetBracketContentLeft(text, startIndex, withBracket);
            }
            else
            {
                int leftIndex = 0;
                string left = "";
                for(int a = startIndex - 1; a >= 0; a--)
                {
                    if (validChars.Contains(text[a]))
                        left = text[a] + left;
                    else
                    {
                        leftIndex = a;
                        break;
                    }
                }

                return left;
            }
        }

        public string RightSide(string text, int startIndex, bool withBracket = false)
        {
            string validChars = "1234567890.,";

            if (text[startIndex + 1] == '(' || text[startIndex+1] == '[')
            {
                return GetBracketContentRight(text, startIndex, withBracket);
            }
            else
            {
                int rightIndex = 0;
                string right = "";
                for (int a = startIndex + 1; a < text.Length; a++)
                {
                    if (validChars.Contains(text[a]))
                        right += text[a];
                    else
                    {
                        rightIndex = a;
                        break;
                    }
                }

                return right;
            }
        }

        public string GetBracketContentLeft(string text, int startIndex, bool withBracket = false)
        {
            int bracketId = 0;

            for(int a = startIndex; a > 0; a--)
            {
                if (text[a] == ')' || text[a] == ']')
                    bracketId++;
                else if(text[a] == '(' || text[a] == '[')
                {
                    bracketId--;
                    if (bracketId == 0)
                        return text.Substring(a + (withBracket ? 0 : 1), startIndex - a - (withBracket ? 0 : 2)); //t: 0; f: 1:2
                }
            }

            return "";
        }

        public string GetBracketContentRight(string text, int startIndex, bool withBracket = false)
        {
            int bracketId = 0;

            for (int a = startIndex; a < text.Length; a++)
            {
                if (text[a] == '(' || text[a] == '[')
                    bracketId++;
                else if (text[a] == ')' || text[a] == ']')
                {
                    bracketId--;
                    if (bracketId == 0)
                        return text.Substring(startIndex + (withBracket ? 1 : 2), a - startIndex - (withBracket ? 0 : 2)); //t: 0; f: 1:2
                }
            }

            return "";
        }

    }
}
