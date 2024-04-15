using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
//using Grammar;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using TurboJanguage;

namespace Lab3
{

	class Program
	{
		static void Main(string[] args)
		{
            //try
            //{
            //	StreamReader r = new StreamReader(new FileStream("G1.TXT", FileMode.Open));

            //	GrammarReader inp = new GrammarReader(r);
            //	var grammar = inp.Read();
            //	grammar.dump();

            //	GrammarOps gr = new GrammarOps(grammar);

            //	// First step, computes nonterminals that can be rewritten as empty word
            //	foreach (Nonterminal nt in gr.EmptyNonterminals)
            //	{
            //		Console.Write(nt.Name + " ");
            //	}
            //	Console.WriteLine();

            //             foreach (var (rule, first) in gr.Firsts)
            //             {
            //                 Console.WriteLine($"FIRST[{rule}] = {string.Join(',', first)}");
            //             }
            //             Console.WriteLine();
            //         }
            //catch (GrammarException e)
            //{
            //	Console.WriteLine($"{e.LineNumber}: Error -  {e.Message}");
            //}
            //catch (IOException e)
            //{
            //	Console.WriteLine(e);
            //}

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var fileName = "C:\\Users\\gabri\\Desktop\\Programming\\PJP\\C#\\Grammar\\Grammar\\Input2.txt";
            Console.WriteLine("Parsing: " + fileName);
            var inputFile = new StreamReader(fileName);
            AntlrInputStream input = new AntlrInputStream(inputFile);
            TurboJanguageLexer lexer = new TurboJanguageLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            TurboJanguageParser parser = new TurboJanguageParser(tokens);
            InstructionSetListener instructionCreator = new();

            parser.AddErrorListener(new VerboseListener());

            IParseTree tree = parser.program();

            if (parser.NumberOfSyntaxErrors == 0)
            {
                //Console.WriteLine(tree.ToStringTree(parser));
                ParseTreeWalker walker = new ParseTreeWalker();
                GrammarListener gListen = new GrammarListener();
                walker.Walk(gListen, tree);

                if(!gListen.IsError())
                {
                    ParseTreeWalker walkerInstructions = new ParseTreeWalker();
                    
                    walkerInstructions.Walk(instructionCreator, tree);
                    fileName = "TurboJanguageInstr.txt";
                    using var fileStream = File.Create(fileName);
                    using var outputFile = new StreamWriter(fileStream);
                    outputFile.Write(string.Join('\n', instructionCreator.Instructions));
                    instructionCreator.printStack();
                }
            }
        }
	}
}