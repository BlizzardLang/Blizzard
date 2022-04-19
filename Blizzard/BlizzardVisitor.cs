using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blizzard;

/// <summary>
/// Handles the implementation for visiting each node
/// </summary>
public class BlizzardVisitor : blizzardBaseVisitor<object>
{
    /// <summary>
    /// Maps the variable names to their variables
    /// </summary>
    internal Dictionary<string, Variable> Variables = new();
    /// <summary>
    /// Maps the default function names to their functions
    /// </summary>
    internal Dictionary<string, Function> Functions = BuiltinFunctions.GetDict();

    /// <summary>
    /// Override visitor for literals
    /// </summary>
    /// <param name="context">The literal parser context</param>
    /// <returns>The literal as the correctly parsed type</returns>
    /// <exception cref="NotImplementedException">Thrown when the variable type of the literal is unrecognized.
    ///     This should never get thrown as literals will not get parsed as other types.</exception>
    public override object VisitLiteral([NotNull] blizzardParser.LiteralContext context)
    {
        if (context.STRING() is { } s)
            return s.GetText()[1..^1]; // Exclude the quotation marks

        if (context.INTEGER() is { } i)
            return int.Parse(i.GetText());

        if (context.DECIMAL() is { } d)
            return double.Parse(d.GetText());

        throw new NotImplementedException("Unexpected variable type encountered.");
    }

    /// <summary>
    /// Override visitor for variable declarations. Adds the variable to <see cref="Variables"/>.
    /// </summary>
    /// <param name="context">The variable declaration parser context</param>
    /// <returns>The <see cref="Variable"/> representation of the parsed variable</returns>
    public override object VisitVariableDeclaration([NotNull] blizzardParser.VariableDeclarationContext context)
    {
        var type = Variable.VariableTypeFrom(context.TYPE().GetText());
        var name = context.IDENTIFIER().GetText();
        object value = Visit(context.expression());

        var variable = new Variable(name, type, value);

        // Add the variable to our Dictionary
        Variables.Add(name, variable);
        return variable;
    }

    /// <summary>
    /// Override visitor for function calls
    /// </summary>
    /// <param name="context">The function call parser context</param>
    /// <returns>The result of computing the called function</returns>
    public override object VisitFunctionCall([NotNull] blizzardParser.FunctionCallContext context)
    {
        var name = context.IDENTIFIER().GetText();
        var @params = context.expression().Select(e => Visit(e)).ToArray();
        return Functions[name].Exec(@params);
    }

    /// <summary>
    /// Override visitor for parentheses expressions
    /// </summary>
    /// <param name="context">The parentheses expression parser context</param>
    /// <returns>The result of visiting the expression wrapped in parentheses</returns>
    public override object VisitParenthesesExpression([NotNull] blizzardParser.ParenthesesExpressionContext context)
    {
        return Visit(context.expression());
    }

    /// <summary>
    /// Override visitor for multiplication or division expressions
    /// </summary>
    /// <param name="context">The multiplication or division expression parser context</param>
    /// <returns>The computed multiplication or division result</returns>
    /// <exception cref="NotImplementedException">Thrown when the operation cannot be performed on the types</exception>
    public override object VisitMulDivExpression([NotNull] blizzardParser.MulDivExpressionContext context)
    {
        var LHS = Visit(context.expression()[0]);
        var RHS = Visit(context.expression()[1]);
        var op = context.MUL_DIV().GetText();

        if (LHS is string || RHS is string)
            throw new NotImplementedException($"Operation `{op}` doesn't exist on type `str`");

        else if (LHS is double || RHS is double)
        {
            var ld = double.Parse($"{LHS}");
            var rd = double.Parse($"{RHS}");

            return op == "*" ? ld * rd : ld / rd;
        }

        else if (LHS is int || RHS is int)
        {
            var li = int.Parse($"{LHS}");
            var ri = int.Parse($"{RHS}");

            return op == "*" ? li * ri : li / ri;
        }

        throw new NotImplementedException($"Cannot determine types for operation `{op}`");
    }

    /// <summary>
    /// Override visitor for addition or subtraction expressions
    /// </summary>
    /// <param name="context">The addition or subtraction expression parser context</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException">Thrown when the operation cannot be performed on the types</exception>
    public override object VisitAddSubExpression([NotNull] blizzardParser.AddSubExpressionContext context)
    {
        var LHS = Visit(context.expression()[0]);
        var RHS = Visit(context.expression()[1]);
        var op = context.ADD_SUB().GetText();

        if (LHS is string || RHS is string)
        {
            return op == "+"
                ? $"{LHS}{RHS}"
                : throw new NotImplementedException("Operation `-` doesn't exist on type `str`");
        }

        else if (LHS is double || RHS is double)
        {
            var ld = double.Parse($"{LHS}");
            var rd = double.Parse($"{RHS}");

            return op == "+" ? ld + rd : ld - rd;
        }

        else if (LHS is int || RHS is int)
        {
            var li = int.Parse($"{LHS}");
            var ri = int.Parse($"{RHS}");

            return op == "+" ? li + ri : li - ri;
        }

        throw new NotImplementedException($"Cannot determine types for operation `{op}`");
    }
    
    /// <summary>
    /// Override visitor for identifier expressions
    /// </summary>
    /// <param name="context">The identifier expression context</param>
    /// <returns>The value of the variable associated with <paramref name="context"/></returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the identifier is not defined</exception>
    public override object VisitIdentifierExpression([NotNull] blizzardParser.IdentifierExpressionContext context)
    {
        return Variables.ContainsKey(context.IDENTIFIER().GetText())
            ? Variables[context.IDENTIFIER().GetText()].Value
            : throw new DirectoryNotFoundException($"Unknown identifier `{context.IDENTIFIER().GetText()}`");
    }
}
