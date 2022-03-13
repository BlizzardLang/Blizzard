using System.CommandLine;

namespace Blizzard
{
    /// <summary>
    /// Handles the parsing of command line arguments and flags
    /// </summary>
    internal static class ArgumentParser
    {
        /// <summary>
        /// Registers all the command line arguments with the root command
        /// </summary>
        public static void Register()
        {
            rootCommand.Add(inputFileArgument);
        }

        /// <summary>
        /// Sets the handler method for the parsed data
        /// </summary>
        /// <param name="handler">The handler to use the parsed data</param>
        public static void SetHandler(Action<FileInfo> handler)
            => rootCommand.SetHandler(handler, inputFileArgument);

        /// <summary>
        /// Parses the command line arguments
        /// </summary>
        /// <param name="args">The arguments to parse</param>
        public static void Invoke(string[] args)
            => rootCommand.Invoke(args);



        private static readonly RootCommand rootCommand = new(
            description: "The command line tool for the Blizzard Programming Language."
        );

        private static readonly Argument inputFileArgument = new Argument<FileInfo>(
            name: "Input File",
            description: "The path to the input Blizzard Source File to execute."
        ).ExistingOnly();
    }
}
