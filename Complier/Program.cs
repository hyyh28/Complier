using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Complier
{
    public enum TokenType
    {
        Comment,
        Keyword,
        Identifier,
        Operator,
        Delimiters,
        Numbers,
        Error
    }
    public struct Token
    {
        public int RowNum { get; set; }
        public int Position { get; set; }
        public string TokenString { get; set; }
        public TokenType Type { get; set; }

        public Token(int row, int p, string token,TokenType mytype)
        {
            RowNum = row;
            Position = p;
            TokenString = token;
            Type = mytype;
        }

        public void PrintToken()
        {
            Console.WriteLine("Token: " + TokenString + " Line: " + (int)(RowNum+1) + " Position: " + Position + " Type: " + Type.ToString());
        }
    }
    internal class Program
    {
        public static Regex Keywords = new Regex(@"int|real|if|then|else|while");
        public static Regex Identifier = new Regex(@"(([A-Z]|[a-z])+)([0-9]*)");
        //public static Regex Operator = new Regex(@"\+|\-|\*|\/|\=|\==|\<|\<=|\>|\>=|\!=");
        //public static Regex Delimiters = new Regex(@"{|[|(|)]|}|;");
        public static Regex Numbers = new Regex(@"-?[0-9]*[.]*[0-9]*");
        public static List<Token> lexicla_Analysis(string path)
        {
            List<Token> result = new List<Token>();
            try
            {
                using (var sw = new StreamReader(path))
                {
                    string lineString;
                    int line_num = 0;
                    while ((lineString = sw.ReadLine()) != null)
                    {
                        Console.WriteLine(line_num+1 + ": " + lineString);
                        int first = 0;
                        while (lineString[first] == ' ')
                        {
                            first++;
                        }
                        int i = first;
                        while(i < lineString.Length)
                        {
                            if ((i + 1) < lineString.Length && (lineString[i] == '/' && lineString[i + 1] == '/'))
                            {
                                Token addToken = new Token(line_num,i,lineString.Substring(i),TokenType.Comment);
                                result.Add(addToken);
                                break;
                            }
                            else if (lineString[i] == '+' || lineString[i] == '-' || lineString[i] == '*'
                                     || lineString[i] == '/' || lineString[i] == '<' || lineString[i] == '>' ||
                                     lineString[i] == '=' || lineString[i] == '!')
                            {
                                if ((i + 1) < lineString.Length && lineString[i + 1] == '=')
                                {
                                    Token addToken = new Token(line_num,i,lineString.Substring(i,2),TokenType.Operator);
                                    result.Add(addToken);
                                    i += 2;
                                }
                                else
                                {
                                    Token addToken = new Token(line_num,i,lineString.Substring(i,1),TokenType.Operator);
                                    result.Add(addToken);
                                    i++;
                                }
                            }
                            else if (lineString[i] == '(' || lineString[i] == ')' || lineString[i] == '{'
                                     || lineString[i] == '}' || lineString[i] == ';')
                            {
                                Token addToken = new Token(line_num,i,lineString.Substring(i,1),TokenType.Delimiters);
                                result.Add((addToken));
                                i++;
                            }
                            else if (lineString[i] == ' ')
                            {
                                i++;
                            }
                            else
                            {
                                int start = i;
                                for (int j = start+1; j < lineString.Length; j++)
                                {
                                    if (lineString[j] == ' ' || lineString[j] == ';' || lineString[j] == '{'
                                        || lineString[j] == '}' || lineString[j] == '(' || lineString[j] == ')'
                                        || lineString[j] == '+' || lineString[j] == '-' || lineString[j] == '*'
                                        || lineString[j] == '/' || lineString[j] == '>' || lineString[j] == '<'
                                        || lineString[j] == '=' || lineString[j] == '!')
                                    {
                                        int end = j;
                                        TokenType myType;
                                        string addString = lineString.Substring(start, end - start);
                                        if(Keywords.IsMatch(addString)) myType = TokenType.Keyword;
                                        else if(Identifier.IsMatch(addString)) myType = TokenType.Identifier;
                                        else if(Numbers.IsMatch(addString)) myType = TokenType.Numbers;
                                        else myType = TokenType.Error;
                                        Token addToken = new Token(line_num,start,addString,myType);
                                        result.Add(addToken);
                                        i = end;
                                        break;
                                    }
                                }
                            }
                        }
                        line_num++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return result;
        }
        public static void Main(string[] args)
        {
            List<Token> result = lexicla_Analysis("./test.txt");
            foreach (var tok in result)
            {
                tok.PrintToken();
            }
        }
    }
}