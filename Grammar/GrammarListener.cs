using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TurboJanguage;

namespace Lab3
{
    public class GrammarListener : TurboJanguageBaseListener
    {
        private readonly Dictionary<string, MyType> _symbolTable = new();
        private readonly Dictionary<string, MyType> _scopeSymbolTable = new();
        public ParseTreeProperty<MyType> types = new();
        public HashSet<string> Errors { get; } = new();

        public override void EnterEveryRule([NotNull] ParserRuleContext context)
        {
            base.EnterEveryRule(context);
            Console.WriteLine($"\u001b[96m{context.GetType().Name.Replace("Context", ""),-20}\u001b[0m\t\u001b[90m{context.GetText()}\u001b[0m");
            if (context.GetType().Name == "Type")
            {
                Console.WriteLine(context.GetText());
            }
        }

        public override void EnterDo_while_statement([NotNull] TurboJanguageParser.Do_while_statementContext context)
        {
            base.EnterDo_while_statement(context);

            context.statement().while_statement();
        }

        public override void EnterType([NotNull] TurboJanguageParser.TypeContext context)
        {
            base.EnterType(context);

            string type = context.GetText();
            switch (type)
            {
                case "int":
                    break;
                case "float":
                    break;
                case "string":
                    break;
                case "bool":
                    break;
            }
        }

        public override void EnterDeclaration(TurboJanguageParser.DeclarationContext context)
        {
            MyType type = GetMyType(context.type().GetText());
            foreach (var variable in context.variable_list().IDENTIFIER())
            {
                string variableName = variable.GetText();
                if (!_symbolTable.ContainsKey(variableName))
                {
                    _symbolTable.Add(variableName, type);
                    types.Put(context, type);
                }
                else
                {
                    Errors.Add($"Error: Variable '{variableName}' has already been declared.");
                    Console.WriteLine($"Error: Variable '{variableName}' has already been declared.");
                }
            }
        }

        public override void EnterExpression([NotNull] TurboJanguageParser.ExpressionContext context)
        {
            base.EnterExpression(context);

            ProcessExpression(context);
        }

        public MyType ProcessPrimaryExpression(TurboJanguageParser.Primary_expressionContext prim_ctx)
        {

            if (prim_ctx.literal() is TurboJanguageParser.LiteralContext lit_ctx)
            {
                if (lit_ctx.BOOL_LITERAL() is not null)
                {
                    types.Put(prim_ctx, MyType.BOOL);
                    return MyType.BOOL;

                }
                else if (lit_ctx.INT_LITERAL() is not null)
                {
                    types.Put(prim_ctx, MyType.INT);
                    return MyType.INT;
                }
                else if (lit_ctx.FLOAT_LITERAL() is not null)
                {
                    types.Put(prim_ctx, MyType.FLOAT);
                    return MyType.FLOAT;
                }
                else
                {
                    types.Put(prim_ctx, MyType.STRING);
                    return MyType.STRING;
                }
            }
            else if (prim_ctx.IDENTIFIER() is not null)
            {
                string name = prim_ctx.IDENTIFIER().GetText();
                if (_symbolTable.TryGetValue(name, out MyType type))
                {
                    types.Put(prim_ctx, type);
                    return type;
                }
                else
                {
                    Errors.Add($"Error: Variable '{name}' hasn't been declared.");
                    Console.WriteLine($"Error: Variable '{name}' hasn't been declared.");
                }
            }
            else if(prim_ctx.expression() is TurboJanguageParser.ExpressionContext expr_ctx) 
            {
                MyType type = ProcessExpression(expr_ctx);
                types.Put(expr_ctx, type);
                return type;
            }

            return MyType.UNKNOWN;
        }

        public MyType ProcessUnaryExpression(TurboJanguageParser.Unary_expressionContext unary_ctx)
        {
            MyType currentType = MyType.UNKNOWN;
            if (unary_ctx.expression() is TurboJanguageParser.ExpressionContext expr_ctx)
            {
                currentType = ProcessExpression(expr_ctx);
                if (unary_ctx.LOGICAL_NOT() is not null)
                {
                    if (currentType == MyType.BOOL)
                    {
                        types.Put(unary_ctx, currentType);
                        return currentType;
                    }
                    else
                    {
                        Errors.Add($"Error: Type mismatch got '{currentType}' expected BOOL");
                        Console.WriteLine($"Error: Type mismatch got '{currentType}' expected BOOL");
                    }
                }
                else if (unary_ctx.MINUS() is not null)
                {
                    if (currentType == MyType.INT || currentType == MyType.FLOAT)
                    {
                        types.Put(unary_ctx, currentType);
                        return currentType;
                    }
                    else
                    {
                        Errors.Add($"Error: Type mismatch got '{currentType}' expected INT or FLOAT");
                        Console.WriteLine($"Error: Type mismatch got '{currentType}' expected INT or FLOAT");
                    }
                }
            }
            return MyType.UNKNOWN;
        }
        public MyType ProcessExpression(TurboJanguageParser.ExpressionContext expr_ctx)
        {
            if (expr_ctx.primary_expression() is TurboJanguageParser.Primary_expressionContext prim_ctx)
            {
                return ProcessPrimaryExpression(prim_ctx);
            }
            else if (expr_ctx.unary_expression() is TurboJanguageParser.Unary_expressionContext unary_ctx)
            {
                return ProcessUnaryExpression(unary_ctx);
            }
            else if (expr_ctx.assignment_expression() is TurboJanguageParser.Assignment_expressionContext assign_ctx)
            {
                return ProcessAssignmentExpression(assign_ctx);
            }
            else
            {

                MyType typeA = ProcessExpression(expr_ctx.expression(0));
                types.Put(expr_ctx.expression(0), typeA);
                MyType typeB = ProcessExpression(expr_ctx.expression(1));
                MyType result = MyType.UNKNOWN;
                Console.WriteLine("EXPR 0 " + expr_ctx.expression(0).GetText() + typeA);
                Console.WriteLine("EXPR 1 " + expr_ctx.expression(1).GetText() + typeB);
                types.Put(expr_ctx.expression(1), typeB);
                string op = expr_ctx.@operator.Text;
                result = ProcessOperator(typeA, typeB, op);
                //types.Put(expr_ctx, result);
                //for (int i = 1; i < expr_ctx.expression().Length; i++)
                //{
                //    MyType typeB = ProcessExpression(expr_ctx.expression(i));
                //    types.Put(expr_ctx.expression(0), typeB);
                //    string op = expr_ctx.@operator.Text;
                //    typeA = ProcessOperator(typeA, typeB, op);
                    
                //}
                if(result == MyType.UNKNOWN)
                {
                    Errors.Add($"Type mismatch got {result}.");
                    Console.WriteLine($"Type mismatch got {result}.");
                }

                
                return result;
            }
        }

        public MyType ProcessAssignmentExpression(TurboJanguageParser.Assignment_expressionContext assign_ctx)
        {
            if(assign_ctx.IDENTIFIER() is not null)
            {
                string name = assign_ctx.IDENTIFIER().GetText();
                if (_symbolTable.TryGetValue(name, out MyType type))
                {
                    if(assign_ctx.expression() is TurboJanguageParser.ExpressionContext expr)
                    {
                        MyType result = ProcessExpression(expr);
                        if(type == MyType.FLOAT && (result == MyType.INT || result == MyType.FLOAT))
                        {
                            types.Put(assign_ctx.expression(), result);
                            return type;
                        }
                        else if(type != result)
                        {
                            Errors.Add($"Type mismatch got '{result}' expected {type}.");
                            Console.WriteLine($"Type mismatch got '{result}' expected {type}.");
                        }
                        else
                        {
                            types.Put(assign_ctx.expression(), result);
                            Console.WriteLine(assign_ctx.GetText());
                            return type;
                        }
                    }
                }
                else
                {
                    Errors.Add($"Error: Variable '{name}' hasn't been declared.");
                    Console.WriteLine($"Error: Variable '{name}' hasn't been declared.");
                }
            }
            return MyType.UNKNOWN;
        }

        private MyType ProcessOperator(MyType A, MyType B, string op) => op switch
        {

            "+" when A is MyType.INT && B is MyType.INT => MyType.INT,
            "+" when A is MyType.FLOAT && (B is MyType.INT || B is MyType.FLOAT) => MyType.FLOAT,
            "+" when A is MyType.INT && B is MyType.FLOAT => MyType.FLOAT,

            "-" when A is MyType.INT && B is MyType.INT => MyType.INT,
            "-" when A is MyType.FLOAT && (B is MyType.INT || B is MyType.FLOAT) => MyType.FLOAT,
            "-" when A is MyType.INT && B is MyType.FLOAT => MyType.FLOAT,

            "*" when A is MyType.INT && B is MyType.INT => MyType.INT,
            "*" when A is MyType.FLOAT && (B is MyType.INT || B is MyType.FLOAT) => MyType.FLOAT,
            "*" when A is MyType.INT && B is MyType.FLOAT => MyType.FLOAT,

            "/" when A is MyType.INT && B is MyType.INT => MyType.INT,
            "/" when A is MyType.FLOAT && (B is MyType.INT || B is MyType.FLOAT) => MyType.FLOAT,
            "/" when A is MyType.INT && B is MyType.FLOAT => MyType.FLOAT,

            "%" when A is MyType.INT && B is MyType.INT => MyType.INT,

            "&&" when A is MyType.BOOL && B is MyType.BOOL => MyType.BOOL,

            "||" when A is MyType.BOOL && B is MyType.BOOL => MyType.BOOL,

            "==" when A is MyType.BOOL && B is MyType.BOOL => MyType.BOOL,
            "==" when A is MyType.STRING && B is MyType.STRING => MyType.BOOL,
            "==" when A is MyType.INT && B is MyType.INT => MyType.BOOL,
            "==" when A is MyType.FLOAT && B is MyType.FLOAT => MyType.BOOL,
            "==" when A is MyType.FLOAT && B is MyType.INT => MyType.BOOL,
            "==" when A is MyType.INT && B is MyType.FLOAT => MyType.BOOL,

            "!=" when A is MyType.BOOL && B is MyType.BOOL => MyType.BOOL,
            "!=" when A is MyType.FLOAT && B is MyType.FLOAT => MyType.BOOL,
            "!=" when A is MyType.INT && B is MyType.INT => MyType.BOOL,
            "!=" when A is MyType.STRING && B is MyType.STRING => MyType.BOOL,
            "!=" when A is MyType.FLOAT && B is MyType.INT => MyType.BOOL,
            "!=" when A is MyType.INT && B is MyType.FLOAT => MyType.BOOL,

            "<" when (A is MyType.FLOAT || A is MyType.INT) && (B is MyType.INT || B is MyType.FLOAT) => MyType.BOOL,
            "<" when A is MyType.BOOL && B is MyType.BOOL => MyType.BOOL,
            ">" when (A is MyType.FLOAT || A is MyType.INT) && (B is MyType.INT || B is MyType.FLOAT) => MyType.BOOL,
            ">" when A is MyType.BOOL && B is MyType.BOOL => MyType.BOOL,
            "." when A is MyType.STRING && B is MyType.STRING => MyType.STRING,
            _ => MyType.UNKNOWN,
        };

        private MyType GetMyType(string type) => type switch
        {
            "int" => MyType.INT,
            "float" => MyType.FLOAT,
            "string" => MyType.STRING,
            "bool" => MyType.BOOL,
            _ => MyType.UNKNOWN,
        };

        public bool IsError()
        {
            if(Errors.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
