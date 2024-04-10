using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Lab3
{
    internal class EvalListener : IParseTreeListener
    {
        public void EnterEveryRule(ParserRuleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void ExitEveryRule(ParserRuleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void VisitErrorNode(IErrorNode node)
        {

            throw new System.NotImplementedException();
        }

        public void VisitTerminal(ITerminalNode node)
        {
            throw new System.NotImplementedException();
        }
    }
}