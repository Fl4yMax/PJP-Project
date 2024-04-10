grammar TurboJanguage;

// Whitespace and comments
COMMENT : '//' ~[\r\n]* -> skip;
WS : [ \t\r\n]+ -> skip;

// Lexer rules
INT_LITERAL  : [0-9]+;
FLOAT_LITERAL : [0-9]+ '.' [0-9]+;
BOOL_LITERAL : 'true' | 'false';
STRING_LITERAL: '"' ~["\r\n]* '"';

// Identifiers
IDENTIFIER : [a-zA-Z][a-zA-Z0-9]*;

type : 'int' | 'float' | 'bool' | 'string';

// Punctuation
SEMI  : ';';
COMMA  : ',';
LPAR : '(';
RPAR : ')';
LBRACE : '{';
RBRACE : '}';

ASSIGN : '=';

PLUS  : '+';
MINUS  : '-';
MULT  : '*';
DIV   : '/';
MOD   : '%';

AND : '&&';
OR : '||';
LOGICAL_NOT : '!';
EQUAL  : '==';
NOT_EQUAL: '!=';
LESS_THAN: '<';
GREATER_THAN: '>';
CONCAT : '.';

// Parser rules
program : statement+ EOF;

statement : declaration SEMI
     | 'read' variable_list SEMI
     | 'write' expression (COMMA expression)* SEMI
     | (expression)? SEMI
     | block
     | if_statement
     | while_statement
     | do_while_statement;

declaration : type variable_list;

variable_list : IDENTIFIER (COMMA IDENTIFIER)*;

block : LBRACE statement* RBRACE;

if_statement : 'if' LPAR expression RPAR statement ('else' statement)?;

while_statement : 'while' LPAR expression RPAR statement;

do_while_statement : 'do' statement 'while' LPAR expression RPAR SEMI;

expression : primary_expression
             | unary_expression
             | calculation_expression
             | assignment_expression;

operator: PLUS | MINUS | DIV | MULT | MOD | CONCAT | AND | OR | EQUAL | NOT_EQUAL | LESS_THAN | GREATER_THAN;

calculation_expression : primary_expression (operator primary_expression)*;

assignment_expression : IDENTIFIER ASSIGN expression;

unary_expression : LOGICAL_NOT primary_expression
                 | MINUS primary_expression;

primary_expression : literal
                   | IDENTIFIER
                   | LPAR expression RPAR;

literal : INT_LITERAL
        | FLOAT_LITERAL
        | BOOL_LITERAL
        | STRING_LITERAL;