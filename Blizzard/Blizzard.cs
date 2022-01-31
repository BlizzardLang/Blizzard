using Antlr4.Runtime;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Blizzard.Tests")]
namespace Blizzard;

class Blizzard
{
    public static void Main(string[] args)
    {
        #if DEBUG
        var input = new AntlrFileStream("Grammar/example.bzz");
        #else
        if (args.Length == 0)
        {
            Console.WriteLine("Missing required argument `input`. Please specify a file to execute.");
            return;
        }
        var input = new AntlrFileStream(args[0]);
        #endif

        // Setup the lexer and parser
        var lexer = new blizzardLexer(input);
        var tokens = new CommonTokenStream(lexer);
        var parser = new blizzardParser(tokens);

        // Visit the tree with the parsed input
        var visitor = new BlizzardVisitor();
        visitor.Visit(parser.program());
    }
}
