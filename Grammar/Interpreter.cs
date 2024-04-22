using Lab3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    internal class Interpreter
    {
        private Stack<string> _stack;
        private readonly List<string> _instructions;
        private Dictionary<string, Variable> _variables;
        private int _ip;

        public Interpreter()
        {
            _stack = new();
            _ip = 0;
           // var fileName = "C:\\Users\\gabri\\Desktop\\Programming\\PJP\\C#\\Grammar\\Grammar\\InstructionSetTest.txt";
            var fileName = "C:\\Users\\gabri\\Desktop\\Programming\\PJP\\C#\\Grammar\\Grammar\\TurboJanguageInstr.txt";
            _instructions = File.ReadAllLines(fileName).ToList();
            _variables = [];

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Instruction count: " + _instructions.Count);
            Console.ResetColor();

            while(_instructions.Count > _ip)
            {
                ExecuteInstruction(_instructions[_ip].Split(' ', 2));
                _ip++;
            }
        }


        private void ExecuteInstruction(string[] s)
        {
            string rest = "";
            string result = "";
            if (s.Length > 1) 
            {
                rest = s[1];
            }
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine($"Current instruction: {s[0]} {rest}");
            //Console.ResetColor();
            switch(s[0])
            {
                case "add":
                    {
                        string[] a = _stack.Pop().Split(' ', 2);
                        string[] b = _stack.Pop().Split(' ', 2);
                        if (a[0] == "I" && b[0] == "I")
                        {
                            result = a[0] + " " + (int.Parse(a[1]) + int.Parse(b[1])).ToString();
                            _stack.Push(result);
                            break;
                        }
                        result = a[0] + " " + (float.Parse(a[1]) + float.Parse(b[1])).ToString();
                        _stack.Push(result);
                        break;
                    }
                case "sub":
                    {
                        string[] b = _stack.Pop().Split(' ', 2);
                        string[] a = _stack.Pop().Split(' ', 2);
                        
                        if (a[0] == "I" && b[0] == "I")
                        {
                            result = a[0] + " " + (int.Parse(a[1]) - int.Parse(b[1])).ToString();
                            _stack.Push(result);
                            break;
                        }
                        result = a[0] + " " + (float.Parse(a[1]) - float.Parse(b[1])).ToString();
                        _stack.Push(result);
                        break;
                    }
                case "mul":
                    {
                        string[] a = _stack.Pop().Split(' ', 2);
                        string[] b = _stack.Pop().Split(' ', 2);
                        if (a[0] == "I" && b[0] == "I")
                        {
                            result = a[0] + " " + (int.Parse(a[1]) * int.Parse(b[1])).ToString();
                            _stack.Push(result);
                            break;
                        }
                        result = a[0] + " " + (float.Parse(a[1]) * float.Parse(b[1])).ToString();
                        _stack.Push(result);
                        break;
                    }
                case "div":
                    {
                        string[] b = _stack.Pop().Split(' ', 2);
                        string[] a = _stack.Pop().Split(' ', 2);
                        
                        if (a[0] == "I" && b[0] == "I")
                        {
                            result = a[0] + " " + (int.Parse(a[1]) / int.Parse(b[1])).ToString();
                            _stack.Push(result);
                            break;
                        }
                        result = a[0] + " " + (float.Parse(a[1]) / float.Parse(b[1])).ToString();
                        _stack.Push(result);
                        break;
                    }
                case "mod":
                    {
                        string[] b = _stack.Pop().Split(' ', 2);
                        string[] a = _stack.Pop().Split(' ', 2);

                        result = a[0] + " " + (int.Parse(a[1]) % int.Parse(b[1])).ToString();
                        _stack.Push(result);
                        break;
                    }
                case "uminus":
                    {
                        string[] a = _stack.Pop().Split(' ', 2);

                        if (a[0] == "I")
                        {
                            result = "I " + (-int.Parse(a[1])).ToString();
                            _stack.Push(result);
                            break;
                        }
                        result = "F " + (-float.Parse(a[1])).ToString();
                        _stack.Push(result);
                        break;
                    }
                case "concat":
                    {
                        string[] b = _stack.Pop().Split(' ', 2);
                        string[] a = _stack.Pop().Split(' ', 2);

                        a[1] = a[1].TrimEnd('\"');
                        b[1] = b[1].TrimStart('\"');

                        result = a[0] + " " + a[1] + b[1];
                        _stack.Push(result);
                        break;
                    }
                case "and":
                    {
                        string[] a = _stack.Pop().Split(' ', 2);
                        string[] b = _stack.Pop().Split(' ', 2);

                        result = a[0] + " " + (bool.Parse(a[1]) && bool.Parse(b[1])).ToString().ToLower();
                        _stack.Push(result);
                        break;
                    }
                case "or":
                    {
                        string[] a = _stack.Pop().Split(' ', 2);
                        string[] b = _stack.Pop().Split(' ', 2);

                        result = a[0] + " " + (bool.Parse(a[1]) || bool.Parse(b[1])).ToString().ToLower();
                        _stack.Push(result);
                        break;
                    }
                case "gt":
                    {
                        string[] b = _stack.Pop().Split(' ', 2);
                        string[] a = _stack.Pop().Split(' ', 2);
                        

                        if (a[0] == "I")
                        {
                            result = "B " + (int.Parse(a[1]) > int.Parse(b[1])).ToString().ToLower();
                            _stack.Push(result);
                            break;
                        }
                        result = "B " + (float.Parse(a[1]) > float.Parse(b[1])).ToString().ToLower();
                        _stack.Push(result);
                        break;
                    }
                case "lt":
                    {
                        string[] b = _stack.Pop().Split(' ', 2);
                        string[] a = _stack.Pop().Split(' ', 2);

                        if (a[0] == "I")
                        {
                            result = "B " + (int.Parse(a[1]) < int.Parse(b[1])).ToString().ToLower();
                            _stack.Push(result);
                            break;
                        }
                        result = "B " + (float.Parse(a[1]) < float.Parse(b[1])).ToString().ToLower();
                        _stack.Push(result);
                        break;
                    }
                case "eq":
                    {
                        string[] b = _stack.Pop().Split(' ', 2);
                        string[] a = _stack.Pop().Split(' ', 2);

                        if (a[0] == "I")
                        {
                            result = "B " + (int.Parse(a[1]) == int.Parse(b[1])).ToString().ToLower();
                            _stack.Push(result);
                            break;
                        }
                        if (a[0] == "F")
                        {
                            result = "B " + (float.Parse(a[1]) == float.Parse(b[1])).ToString().ToLower();
                            _stack.Push(result);
                            break;
                        }
                        if (a[0] == "S")
                        {
                            a[1] = a[1].TrimStart('\"');
                            a[1] = a[1].TrimEnd('\"');
                            b[1] = b[1].TrimStart('\"');
                            b[1] = b[1].TrimEnd('\"');
                            result = "B " + (a[1] == b[1]).ToString().ToLower();
                            _stack.Push(result);
                            break;
                        }
                        result = "B " + (float.Parse(a[1]) == float.Parse(b[1])).ToString().ToLower();
                        _stack.Push(result);
                        break;
                    }
                case "not":
                    {
                        string[] a = _stack.Pop().Split(' ', 2);
                        bool b = !(bool.Parse(a[1]));
                        _stack.Push("B " + b.ToString().ToLower());

                        break;
                    }
                case "itof":
                    {
                        string[] a = _stack.Pop().Split(' ', 2);
                        _stack.Push("F " + float.Parse(a[1]).ToString());
             
                        break; 
                    }
                case "push":
                    _stack.Push(rest);
                    Console.WriteLine(rest);
                    break;
                case "pop":
                    _stack.Pop();
                    break;
                case "load":
                    {
                        Variable v;
                        rest = rest.TrimEnd();
                        if (_variables.TryGetValue(rest, out v))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Loading variable... {v.type} : {v.value.ToString()}");
                            _stack.Push(ConvertTypeToString(v.type) + " " + v.value.ToString());
                        }
                        break;
                    }
                case "save":
                    {
                        string[] a = _stack.Pop().Split(' ', 2);
                        Variable v; 
                        if (!_variables.TryGetValue(rest, out v))
                        {
                            MyType t = ConvertStringToType(a[0]);
                            v = new(t, a[1]);
                            _variables.Add(rest, v);
                        }
                        else
                        {
                            v.value = a[1];
                        }
                        break;
                    }
                case "jmp":
                    {
                        _ip = _instructions.IndexOf($"label {rest}");

                        break;
                    }
                case "fjmp":
                    if (!bool.Parse(_stack.Pop().Split(" ", 2)[1]))
                    {
                        _ip = _instructions.IndexOf($"label {rest}");
                    }
                    break;
                case "print":
                    {
                        int size = int.Parse(rest);
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Print size:  {size} Stack size: {_stack.Count}");
                        List<string> toPrint = [];
                        for(int i = 0; i < size; i++)
                        {
                            if (_stack.Count > 0)
                            {
                                string r = _stack.Pop();
                                toPrint.Add(r);
                                //Console.WriteLine("To print " + r);
                            }
                        }
                        toPrint.Reverse();

                        foreach(string str in toPrint)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            string[] res = str.Split(' ', 2);
                            if (res[0] == "F")
                            {
                                if (float.Parse(res[1]) == Math.Floor(float.Parse(res[1])))
                                {
                                    res[1] += ".0";
                                }
                            }
                            Console.WriteLine("Value: " + res[1]);
                            Console.ResetColor();
                        }
                        Console.Write("\n");
                        break;
                    }
                case "read":
                    {
                        while (true)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine($"Waiting for input expecting {rest}");
                            string? str = Console.ReadLine();
                            Console.ResetColor();
                            ArgumentNullException.ThrowIfNull(str);

                            if (rest == "S")
                            {
                                _stack.Push("S " + str);
                                break;
                            }
                            if (rest == "I")
                            {
                                int value;
                                if (int.TryParse(str, out value))
                                {
                                    _stack.Push("I " + str);
                                    break;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Wrong Input try again");
                                    Console.ResetColor();
                                }
                            }
                            else if (rest == "F")
                            {
                                float value;
                                if (float.TryParse(str, out value))
                                {
                                    _stack.Push("F " + str);
                                    break;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Wrong Input try again");
                                    Console.ResetColor();
                                }
                            }
                            else
                            {
                                bool value;
                                if (bool.TryParse(str, out value))
                                {
                                    _stack.Push("I " + str);
                                    break;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Wrong Input try again");
                                    Console.ResetColor();
                                }
                            }
                        }
                        break;
                    }
            }
        }

        private MyType ConvertStringToType(string type) => type switch
        {
            "S" => MyType.STRING,
            "I" => MyType.INT,
            "F" => MyType.FLOAT,
            "B" => MyType.BOOL
        };

        private string ConvertTypeToString(MyType type) => type switch
        {
            MyType.STRING => "S",
            MyType.INT => "I",
            MyType.FLOAT => "F",
            MyType.BOOL => "B"
        };

        private void CalculateBinaryOp(string op)
        {

        }
    }
}
