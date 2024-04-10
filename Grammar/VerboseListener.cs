using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace Lab3
{
    internal class VerboseListener : IAntlrErrorListener<IToken>
    {
        public void SyntaxError(TextWriter output, [NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            IList<string> stack = ((Parser)recognizer).GetRuleInvocationStack();
            stack.Reverse();
            Console.Error.WriteLine("rule stack: " + string.Join(", ", stack));
            Console.Error.WriteLine("line " + line + ":" + charPositionInLine + " at " + offendingSymbol + ": " + msg);
        }
    }
}