using Microsoft.VisualStudio.TestTools.UnitTesting;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;

namespace Blizzard.Tests;

/// <summary>
/// Contains the tests for the <see cref="blizzardParser"/>.
/// Confirms that the <see cref="BlizzardVisitor"/> correctly handles the parse tree.
/// </summary>
[TestClass]
public class BlizzardParserTests
{
    /// <summary>
    /// The Blizzard parse tree visitor
    /// </summary>
    private readonly BlizzardVisitor Visitor = new();

    /// <summary>
    /// Gets the parse tree for a given program input
    /// </summary>
    /// <param name="text">The program input to parse</param>
    /// <returns>The <see cref="blizzardParser"/> for <paramref name="text"/></returns>
    public static blizzardParser GetParser(string text)
    {
        var input = new AntlrInputStream(text);
        var lexer = new blizzardLexer(input);
        var tokens = new CommonTokenStream(lexer);
        return new blizzardParser(tokens);
    }

    /// <summary>
    /// Tests that literals are correctly parsed
    /// </summary>
    /// <param name="input">The literal to parse</param>
    /// <param name="expected">The expected parse result</param>
    [DataTestMethod]
    [DataRow("\"John Smith\"", "John Smith")]
    [DataRow("34", 34)]
    [DataRow("42.15", 42.15)]
    public void TestLiteral(string input, object expected)
    {
        var actual = Visitor.Visit(GetParser(input).literal());
        Assert.AreEqual(expected, actual);
    }

    #region VariableDeclaration

    /// <summary>
    /// Tests that variable declarations are correctly parsed
    /// </summary>
    /// <param name="input">The program input to parse</param>
    /// <param name="expected">The expected parse result</param>
    [DataTestMethod]
    [DynamicData(nameof(VariableDeclarationTestData), DynamicDataSourceType.Property)]
    public void TestVariableDeclaration(string input, object expected)
    {
        var @var = GetParser(input).variableDeclaration();
        var actual = Visitor.Visit(@var);
        Assert.AreEqual(expected, actual); // Assert correctly parsed
        Assert.IsTrue(Visitor.Variables.ContainsKey(@var.IDENTIFIER().GetText())); // Assert is added to Variables list
    }

    /// <summary>
    /// The test data for variable declarations
    /// </summary>
    private static IEnumerable<object[]> VariableDeclarationTestData
    {
        get
        {
            yield return new object[]
            {
                "str name = \"John Smith\"",
                new Variable("name", VariableType.STR, "John Smith")
            };

            yield return new object[]
            {
                "int age = 34",
                new Variable("age", VariableType.INT, 34)
            };
            
            yield return new object[]
            {
                "dec salary = 42.15",
                new Variable("salary", VariableType.DEC, 42.15)
            };
        }
    }

    #endregion

    /// <summary>
    /// Tests that function calls are correctly parsed
    /// </summary>
    /// <param name="input">The function call to parse</param>
    /// <param name="expected">The expected console output for evaluating the function <paramref name="input"/></param>
    [DataTestMethod]
    [DataRow("WRITE(\"Hello World!\")", "Hello World!")]
    [DataRow("WRITE(\"Howdy,\", \"World!\")", "Howdy, World!")]
    [DataRow("WRITELN(\"Hello World! (With a new line)\")", "Hello World! (With a new line)\r\n")]
    public void TestFunctionCall_WRITE(string input, object expected)
    {
        // Redirect stdout to sw to capture the write and writeln functions
        var sw = new System.IO.StringWriter();
        sw.NewLine = "\r\n"; // Set the line endings for consistent testing
        Console.SetOut(sw);

        Visitor.Visit(GetParser(input).functionCall());
        Assert.AreEqual(expected, sw.ToString());
    }

    #region Expression

    /// <summary>
    /// Tests that many expressions are parsed correctly
    /// </summary>
    /// <param name="input">The program input to parse</param>
    /// <param name="expected">The expected parse result</param>
    [DataTestMethod]
    [DynamicData(nameof(ParenthesesExpressionTestData), DynamicDataSourceType.Property)]
    [DynamicData(nameof(MulDivExpressionTestData), DynamicDataSourceType.Property)]
    [DynamicData(nameof(AddSubExpressionTestData), DynamicDataSourceType.Property)]
    public void TestExpressionsWithoutOwnParserRule(string input, object expected)
    {
        var actual = Visitor.Visit(GetParser(input).expression());
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// The test data for parentheses expressions
    /// </summary>
    private static IEnumerable<object[]> ParenthesesExpressionTestData
    {
        get
        {
            yield return new object[] { "(3 + 5) * 2", 16 };
            yield return new object[] { "2 * (3 + (2 - 7))", -4 };
        }
    }

    /// <summary>
    /// The test data for multiplication or division expressions
    /// </summary>
    private static IEnumerable<object[]> MulDivExpressionTestData
    {
        get
        {
            yield return new object[] { "3 * 7.5", 22.5 };
            yield return new object[] { "9 * -6 / 2 + 3", -24 };
        }
    }

    /// <summary>
    /// The test data for addition or subtraction expressions
    /// </summary>
    private static IEnumerable<object[]> AddSubExpressionTestData
    {
        get
        {
            yield return new object[] { "12 + 16 - 22", 6 };
            yield return new object[] { "3.2 - 0.6 + 12", 14.6 };
        }
    }

    #endregion

    /// <summary>
    /// Tests that identifiers are correctly parsed
    /// </summary>
    /// <param name="input">The program to parse</param>
    /// <param name="expected">The expected console output for evaluating the functions in <paramref name="input"/></param>
    [DataTestMethod]
    [DataRow("int x=2;WRITE(x);", "2")]
    [DataRow("int x=2; str y=\"x is \" + x;WRITE(y);", "x is 2")]
    [DataRow("int x = 2;dec y = x - 3.2;WRITE(y);", "-1.2000000000000002")] // Decimal operations off in most languages
    public void TestIdentifierExpression(string input, object expected)
    {
        // Redirect stdout to sw to capture the write and writeln functions
        var sw = new System.IO.StringWriter();
        sw.NewLine = "\r\n"; // Set the line endings for consistent testing
        Console.SetOut(sw);

        Visitor.Visit(GetParser(input).program());
        Assert.AreEqual(expected, sw.ToString());
    }
}
