using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using TurboJanguage;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab3
{
    internal class InstructionSetListener : TurboJanguageBaseListener
    {
        public List<string> Instructions { get; private set; } = [];
        private Stack<Dictionary<string, MyType>> _symbolTable = new();
        private int counter = 0;
        public override void EnterStatement([NotNull] TurboJanguageParser.StatementContext context)
        {
            base.EnterStatement(context);

            string type = "";
            string value = "";

            if (context.write_statement() is TurboJanguageParser.Write_statementContext write_ctx)
            {
                if (write_ctx.expression() is TurboJanguageParser.ExpressionContext[] expr_ctxs)
                {
                    int i = 0;
                    foreach (TurboJanguageParser.ExpressionContext expr_ctx in expr_ctxs)
                    {
                        i++;
                        EvaluateExpr(expr_ctx);
                    }
                    Instructions.Add($"print {i}");
                }
            }
            else if (context.read_statement() is TurboJanguageParser.Read_statementContext read_ctx)
            {
                if (read_ctx.variable_list() is TurboJanguageParser.Variable_listContext var_ctx)
                {
                    foreach (var ID in var_ctx.IDENTIFIER())
                    {
                        MyType t;
                        string id = ID.GetText();
                        if (_symbolTable.Count > 0 && _symbolTable.Peek().TryGetValue(id, out t))
                        {
                            type = ConvertType(t);
                            Instructions.Add($"read {type}");
                            Instructions.Add($"save {id}");
                        }
                    }
                }

            }
            else if (context.expression() is TurboJanguageParser.ExpressionContext expr_ctx)
            {
                EvaluateExpr(expr_ctx);
            }
            else if(context.if_statement() is TurboJanguageParser.If_statementContext if_ctx)
            {
                if(if_ctx.expression() is TurboJanguageParser.ExpressionContext expr)
                {
                    EvaluateExpr(expr);
                    Instructions.Add($"fjmp {counter}");
                }
            }
        }

        public override void ExitStatement([NotNull] TurboJanguageParser.StatementContext context)
        {
            base.ExitStatement(context);
            if (context.Parent is TurboJanguageParser.If_statementContext if_ctx)
            {
                if (if_ctx.statement(0) == context)
                {
                    Instructions.Add($"jmp {counter + 1}");
                    Instructions.Add($"label {counter}");
                }
            }
        }

        public override void ExitIf_statement([NotNull] TurboJanguageParser.If_statementContext context)
        {
            base.ExitIf_statement(context);

            counter++;
            Instructions.Add($"label {counter}");
            counter++;
        }

        public MyType EvaluateExpr(TurboJanguageParser.ExpressionContext expr_ctx)
        {
            MyType type;
            MyType typeB;
            if (expr_ctx.primary_expression() is TurboJanguageParser.Primary_expressionContext prim_ctx)
            {
                type = EvaluatePrimaryExpr(prim_ctx);
            }
            else if (expr_ctx.unary_expression() is TurboJanguageParser.Unary_expressionContext unary_ctx)
            {
                type = EvaluateUnaryExpr(unary_ctx);
            }
            else if (expr_ctx.assignment_expression() is TurboJanguageParser.Assignment_expressionContext assign_ctx)
            {
                type = EvaluateAssignExpr(assign_ctx);

            }
            else
            {
                type = EvaluateExpr(expr_ctx.expression(0));
                typeB = EvaluateExpr(expr_ctx.expression(1));
                string op = expr_ctx.@operator.Text;

                if (type is MyType.INT && typeB is MyType.FLOAT)
                {
                    Instructions.Insert(Instructions.Count - 1, "itof");
                }
                if (type is MyType.FLOAT && typeB is MyType.INT)
                {
                    Instructions.Add("itof");
                }
                    

                switch (op)
                {
                    case "*":
                        Instructions.Add("mul");
                        break;
                    case "/":
                        Instructions.Add("div");
                        break;
                    case "+":
                        Instructions.Add("add");
                        break;
                    case "-":
                        Instructions.Add("sub");
                        break;
                    case "&&":
                        Instructions.Add("and");
                        break;
                    case "||":
                        Instructions.Add("or");
                        break;
                    case "%":
                        Instructions.Add("mod");
                        break;
                    case ".":
                        Instructions.Add("concat");
                        break;
                    case "==":
                        Instructions.Add("eq");
                        break;
                    case "!=":
                        Instructions.Add("eq");
                        Instructions.Add("not");
                        break;
                    case "<":
                        Instructions.Add("lt");
                        break;
                    case ">":
                        Instructions.Add("gt");
                        break;
                }
            }
            return type;
        }

        public override void ExitAssignment_expression([NotNull] TurboJanguageParser.Assignment_expressionContext context)
        {
            base.ExitAssignment_expression(context);

            if (context.expression().assignment_expression() is null)
            {
                Instructions.Add("pop");
            }
        }
        public override void EnterWhile_statement([NotNull] TurboJanguageParser.While_statementContext context)
        {
            Instructions.Add($"label {counter}");
            EvaluateExpr(context.expression());
            Instructions.Add($"fjmp {counter + 1}");
        }

        public override void ExitWhile_statement([NotNull] TurboJanguageParser.While_statementContext context)
        {
            Instructions.Add($"jmp {counter}");
            counter++;
            Instructions.Add($"label {counter}");
            counter++;
        }

        public MyType EvaluatePrimaryExpr(TurboJanguageParser.Primary_expressionContext prim_ctx)
        {
            if(prim_ctx.IDENTIFIER() is not null)
            {
                string ID = prim_ctx.IDENTIFIER().GetText();
                string type = "";
                MyType t;

                _symbolTable.Peek().TryGetValue(ID, out t);
                type = ConvertType(t);
                Instructions.Add($"load {ID}");

                return t;
            }
            else if(prim_ctx.literal() is TurboJanguageParser.LiteralContext lit_ctx)
            {
                if(lit_ctx.INT_LITERAL() is not null)
                {
                    Instructions.Add($"push I {lit_ctx.INT_LITERAL().GetText()}");
                    return MyType.INT;
                }
                else if(lit_ctx.FLOAT_LITERAL() is not null)
                {
                    Instructions.Add($"push F {lit_ctx.FLOAT_LITERAL().GetText()}");
                    return MyType.FLOAT;
                }
                else if(lit_ctx.STRING_LITERAL() is not null)
                {
                    Instructions.Add($"push S {lit_ctx.STRING_LITERAL().GetText()}");
                    return MyType.STRING;
                }
                else
                {
                    Instructions.Add($"push B {lit_ctx.BOOL_LITERAL().GetText()}");
                    return MyType.BOOL;
                }
            }
            else
            {
                return EvaluateExpr(prim_ctx.expression());
            }
        }

        private MyType EvaluateUnaryExpr(TurboJanguageParser.Unary_expressionContext unary_ctx)
        {
            MyType type = MyType.UNKNOWN;
            if(unary_ctx.expression() is TurboJanguageParser.ExpressionContext expr)
            {
                type = EvaluateExpr(expr);
            }
            
            if(unary_ctx.LOGICAL_NOT() is not null)
            {
                Instructions.Add("not");
            }
            else
            {
                Instructions.Add("uminus");
            }
            return type;
        }

        private MyType EvaluateAssignExpr(TurboJanguageParser.Assignment_expressionContext assign_ctx)
        {
            MyType t = MyType.UNKNOWN;
            MyType t2 = MyType.UNKNOWN;
            
            if (assign_ctx.IDENTIFIER() is not null)
            {
                
                string ID = assign_ctx.IDENTIFIER().GetText();
                t2 = EvaluateExpr(assign_ctx.expression());
                _symbolTable.Peek().TryGetValue(ID, out t);
                if (t == MyType.FLOAT && t2 == MyType.INT)
                {
                    Instructions.Add("itof");
                }
                Instructions.Add($"save {ID}");
                Instructions.Add($"load {ID}");
            }
            return t;
        }

        public override void EnterDeclaration([NotNull] TurboJanguageParser.DeclarationContext context)
        {
            base.EnterDeclaration(context);

            if(_symbolTable.Count == 0)
            {
                _symbolTable.Push(new Dictionary<string, MyType>());
            }

            MyType t = GetMyType(context.type().GetText());
            string type;
            
            foreach (var variable in context.variable_list().IDENTIFIER())
            {
                if(!_symbolTable.Peek().ContainsKey(variable.GetText()))
                {
                    _symbolTable.Peek().Add(variable.GetText(), t);
                    type = ConvertType(t);
                    switch(type)
                    {
                        case "S":
                            Instructions.Add($"push {type} \"\"");
                            break;
                        case "I":
                            Instructions.Add($"push {type} 0");
                            break;
                        case "F":
                            Instructions.Add($"push {type} 0.0");
                            break;
                        case "B":
                            Instructions.Add($"push {type} false");
                            break;
                    }
                    Instructions.Add($"save {variable}");
                }
            }
        }

        private string ConvertType(MyType type) => type switch
        {
            MyType.STRING => "S",
            MyType.INT => "I",
            MyType.FLOAT => "F",
            MyType.BOOL => "B"
        };

        private MyType GetMyType(string type) => type switch
        {
            "int" => MyType.INT,
            "float" => MyType.FLOAT,
            "string" => MyType.STRING,
            "bool" => MyType.BOOL,
            _ => MyType.UNKNOWN,
        };

        public void printStack()
        {
            foreach(string s in Instructions)
            {
                Console.WriteLine(s);
            }
        }
    }
}
