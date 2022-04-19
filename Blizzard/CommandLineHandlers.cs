using Antlr4.Runtime;
using System;
using System.Diagnostics;
using System.IO;

namespace Blizzard;

/// <summary>
/// Contains all the handlers for command line commands
/// </summary>
public static class CommandLineHandlers
{
    /// <summary>
    /// The handler for the <c>Blizzard run</c> command
    /// </summary>
    /// <remarks>
    /// Parses the Blizzard Source File <c>inputFile</c> and executes the code
    /// </remarks>
    public readonly static Action<FileInfo> runCommandHandler = delegate (FileInfo inputFile)
    {
        // Setup the lexer and parser
        var antlrInputFileStream = new AntlrFileStream(inputFile.FullName);
        var lexer = new blizzardLexer(antlrInputFileStream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new blizzardParser(tokens);

        // Visit the tree with the parsed input
        var visitor = new BlizzardVisitor();
        visitor.Visit(parser.program());
    };

    /// <summary>
    /// The handler for the <c>Blizzard update</c> command
    /// </summary>
    /// <remarks>
    /// Calls the BlizzardInstaller to update the runtime in a separate process, kills the current process.
    /// </remarks>
    public static readonly Action<bool> updateCommandHandler = delegate (bool verbose)
    {
        // Get the current Blizzard version to determine whether or not an update is needed (handled in the installer)
        var currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version ?? new Version("0.0.0"); // 0.0.0 forces an update if current version cannot be determined

        try
        {
            // Call the BlizzardInstaller to update to the latest version
            // Kill the current process so the file is not in use during update
            var updateProcess = new ProcessStartInfo("BlizzardInstaller", $"--current {currentVersion} --target latest -v {verbose}")
            {
                UseShellExecute = true
            };
            Process.Start(updateProcess);
            Process.GetCurrentProcess().Kill();
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine(ex.ToString());
            Console.WriteLine(ex.StackTrace);
#else
                if (verbose)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
                else
                {
                    Console.WriteLine("There was an error running the installer. Check if it exists in the current directory.");
                    Console.WriteLine("If needed you can download the installer from https://github.com/BlizzardLang/Blizzard_Installer/releases/latest");
                    Console.WriteLine("Try running the command with verbose mode enabled to see the problem.");
                }
#endif
        }
    };
}
