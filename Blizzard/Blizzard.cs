using Antlr4.Runtime;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Blizzard.Tests")]
namespace Blizzard;

class Blizzard
{
    public static void Main(string[] args)
    {
#if DEBUG
        // Use the default Grammar/example.bzz file in DEBUG mode if no options are specified
        // Allows debugging in an IDE without building and running from the command line
        if (args.Length == 0) {
            args = new[] { "Grammar/example.bzz" };
        }
#endif

        ArgumentParser.Register();
        ArgumentParser.SetHandler((FileInfo inputFile) => {
            // Setup the lexer and parser
            var antlrInputFileStream = new AntlrFileStream(inputFile.FullName);
            var lexer = new blizzardLexer(antlrInputFileStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new blizzardParser(tokens);

            // Visit the tree with the parsed input
            var visitor = new BlizzardVisitor();
            visitor.Visit(parser.program());
        });
        ArgumentParser.Invoke(args);
    }
}
