grammar blizzard;

//////////////////////////////////////////////////////////////////////////////////////////
// Parser Rules
//////////////////////////////////////////////////////////////////////////////////////////

program: statement* EOF;
statement: (expression | variableDeclaration) ';';

expression
    : literal                           #literalExpression
    | functionCall                      #functionCallExpression
    | '(' expression ')'                #parenthesesExpression
    | expression MUL_DIV expression     #MulDivExpression
    | expression ADD_SUB expression     #AddSubExpression
    | IDENTIFIER                        #identifierExpression
    ;

literal: STRING | INTEGER | DECIMAL;
variableDeclaration: TYPE IDENTIFIER '=' expression;
functionCall: IDENTIFIER '(' (expression (',' expression)*)? ')';



//////////////////////////////////////////////////////////////////////////////////////////
// Lexer Rules
//////////////////////////////////////////////////////////////////////////////////////////

TYPE: 'str' | 'int' | 'dec';                // The valid variable types
STRING: '"' ~'"'* '"';                      // Matches a string literal
INTEGER: '-'? [0-9]+;                       // Matches an integer literal
DECIMAL: '-'? [0-9]* '.' [0-9]+;            // Matches a decimal literal

IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;         // Name identifier for variables and functions
MUL_DIV: '*' | '/';                         // The multiplication and division operators
ADD_SUB: '+' | '-';                         // The addition and subtraction operators



/*
 * Ignore comments and irrelevant whitespace
 */
SINGLE_LINE_COMMENT: '//' .*? ('\r'? '\n') -> skip;
MULTI_LINE_COMMENT: '/*' .*? '*/' -> skip;
WHITESPACE: [ \t\r\n] -> skip;
