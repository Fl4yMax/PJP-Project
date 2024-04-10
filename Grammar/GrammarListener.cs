﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TurboJanguage;

namespace Lab3
{
    public class GrammarListener : TurboJanguageBaseListener
    {

        public bool IsError { get; private set; } = false;
        private readonly Dictionary<string, MyType> _symbolTable = new();
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
                    return MyType.BOOL;
                }
                else if (lit_ctx.INT_LITERAL() is not null)
                {
                    return MyType.INT;
                }
                else if (lit_ctx.FLOAT_LITERAL() is not null)
                {
                    return MyType.FLOAT;
                }
                else
                {
                    return MyType.STRING;
                }
            }
            else if (prim_ctx.IDENTIFIER() is not null)
            {
                string name = prim_ctx.IDENTIFIER().GetText();
                if (_symbolTable.TryGetValue(name, out MyType type))
                {
                    return type;
                }
                else
                {
                    Errors.Add($"Error: Variable '{name}' hasn't been declared.");
                    Console.WriteLine($"Error: Variable '{name}' hasn't been declared.");
                }
            }

            return MyType.UNKNOWN;
        }

        public MyType ProcessUnaryExpression(TurboJanguageParser.Unary_expressionContext unary_ctx)
        {
            MyType currentType = MyType.UNKNOWN;
            if (unary_ctx.primary_expression() is TurboJanguageParser.Primary_expressionContext prim_ctx)
            {
                currentType = ProcessPrimaryExpression(prim_ctx);
                if (unary_ctx.LOGICAL_NOT() is not null)
                {
                    if (currentType == MyType.BOOL)
                    {
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
            MyType currentType = MyType.UNKNOWN;
            if (expr_ctx.primary_expression() is TurboJanguageParser.Primary_expressionContext prim_ctx)
            {
                return ProcessPrimaryExpression(prim_ctx);
            }
            else if (expr_ctx.unary_expression() is TurboJanguageParser.Unary_expressionContext unary_ctx)
            {
                currentType = ProcessUnaryExpression(unary_ctx);
                MyType primaryType = ProcessPrimaryExpression(unary_ctx.primary_expression());
                if (currentType == MyType.FLOAT && primaryType == MyType.INT || currentType == MyType.INT && primaryType == MyType.FLOAT)
                {
                    return MyType.FLOAT;
                }
                else if (currentType == primaryType)
                {
                    return currentType;
                }
            }
            else if (expr_ctx.calculation_expression() is TurboJanguageParser.Calculation_expressionContext calc_ctx)
            {
                return ProcessCalculationExpression(calc_ctx);
            }
            else if (expr_ctx.assignment_expression() is TurboJanguageParser.Assignment_expressionContext assign_ctx)
            {
                return ProcessAssignmentExpression(assign_ctx);
            }

            return MyType.UNKNOWN;
        }

        public MyType ProcessCalculationExpression(TurboJanguageParser.Calculation_expressionContext calc_ctx)
        {
            MyType typeA = MyType.UNKNOWN;
            MyType typeB = MyType.UNKNOWN;
            MyType result = MyType.UNKNOWN;

            TurboJanguageParser.Primary_expressionContext[] prim_ctxs = calc_ctx.primary_expression();
            TurboJanguageParser.OperatorContext[] op_ctxs = calc_ctx.@operator();

            if(prim_ctxs.Length == 1) { return ProcessPrimaryExpression(prim_ctxs[0]); }
            else
            {
                typeA = ProcessPrimaryExpression(prim_ctxs[0]);
                for (int i = 0; i < op_ctxs.Length; i++)
                {
                    typeB = ProcessPrimaryExpression(prim_ctxs[i + 1]);
                    result = ProcessOperator(typeA, typeB, op_ctxs[i]);
                }
            }

            if(result == MyType.UNKNOWN)
            {
                Errors.Add($"Type mismatch got '{result}' expected {typeA}.");
                Console.WriteLine($"Type mismatch got '{result}' expected {typeA}.");
            }

            return result;
        }

        public MyType ProcessAssignmentExpression(TurboJanguageParser.Assignment_expressionContext assign_ctx)
        {
            if(assign_ctx.IDENTIFIER() is not null)
            {
                string name = assign_ctx.IDENTIFIER().GetText();
                if (_symbolTable.TryGetValue(name, out MyType type))
                {
                    if(assign_ctx.expression() is TurboJanguageParser.ExpressionContext)
                    {
                        MyType result = ProcessExpression(assign_ctx.expression());
                        if(type == MyType.FLOAT && (result == MyType.INT || result == MyType.FLOAT))
                        {
                            return type;
                        }
                        else if(type != result)
                        {
                            Errors.Add($"Type mismatch got '{result}' expected {type}.");
                            Console.WriteLine($"Type mismatch got '{result}' expected {type}.");
                        }
                        else
                        {
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

        private MyType ProcessOperator(MyType A, MyType B, TurboJanguageParser.OperatorContext op_ctx) => op_ctx.GetText() switch
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
            "==" when A is not MyType.BOOL && B is not MyType.BOOL => MyType.BOOL,
            "!=" when A is not MyType.BOOL && B is not MyType.BOOL => MyType.BOOL,

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
    }
}
