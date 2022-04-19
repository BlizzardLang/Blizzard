using System.CommandLine;
using System.IO;

namespace Blizzard;

/// <summary>
/// Handles the parsing of command line arguments and flags
/// </summary>
internal class ArgumentParser
{
    /// <summary>
    /// Register all the required commands, arguments, options, and handlers for the command line
    /// </summary>
    public ArgumentParser Register()
    {
        RegisterCommands();
        RegisterHandlers();

        return this;
    }

    /// <summary>
    /// Registers all the command line commands, arguments, and options
    /// </summary>
    private static void RegisterCommands()
    {
        rootCommand.Add(runCommand);
        rootCommand.Add(updateCommand);

        runCommand.Add(inputFileArgument);

        updateCommand.Add(updateCommandVerboseFlag);
    }

    /// <summary>
    /// Registers all the handlers for the commands
    /// </summary>
    private static void RegisterHandlers()
    {
        runCommand.SetHandler(CommandLineHandlers.runCommandHandler, inputFileArgument);
        updateCommand.SetHandler(CommandLineHandlers.updateCommandHandler, updateCommandVerboseFlag);
    }

    /// <summary>
    /// Parses the command line arguments
    /// </summary>
    /// <param name="args">The arguments to parse</param>
    public void Invoke(string[] args)
        => rootCommand.Invoke(args);



    private static readonly RootCommand rootCommand = new(
        description: "The command line tool for the Blizzard Programming Language."
    );

    #region Run Command

    private static readonly Command runCommand = new(
        name: "run",
        description: "Execute a Blizzard Source File."
    );

    private static readonly Argument inputFileArgument = new Argument<FileInfo>(
        name: "Input File",
        description: "The path to the input Blizzard Source File to execute."
    ).ExistingOnly();

    #endregion
    #region Update Command

    private static readonly Command updateCommand = new(
        name: "update",
        description: "Updates Blizzard to the latest version."
    );

    private static readonly Option<bool> updateCommandVerboseFlag = new(
        aliases: new[] { "--verbose", "-v" },
        description: "Show verbose output with more details about the update process."
    );

    #endregion
}
