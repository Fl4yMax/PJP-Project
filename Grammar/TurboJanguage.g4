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
     | read_statement
     | write_statement
     | (expression)? SEMI
     | block
     | if_statement
     | while_statement
     | do_while_statement;

write_statement : 'write' expression (COMMA expression)* SEMI;

read_statement : 'read' variable_list SEMI;

declaration : type variable_list;

variable_list : IDENTIFIER (COMMA IDENTIFIER)*;

block : LBRACE statement* RBRACE;

if_statement : 'if' LPAR expression RPAR statement ('else' statement)?;

while_statement : 'while' LPAR expression RPAR statement;

do_while_statement : 'do' statement 'while' LPAR expression RPAR SEMI;

expression : primary_expression
             | unary_expression
             | assignment_expression
             | expression operator=(MULT | DIV) expression
             | expression operator=(PLUS | MINUS) expression
             | expression operator=MOD expression
             | expression operator=AND expression
             | expression operator=OR expression
             | expression operator=CONCAT expression
             | expression operator=(EQUAL | NOT_EQUAL | LESS_THAN | GREATER_THAN) expression;

assignment_expression : IDENTIFIER ASSIGN expression;

unary_expression : LOGICAL_NOT expression
                 | MINUS expression;

primary_expression : literal
                   | IDENTIFIER
                   | LPAR expression RPAR;

literal : INT_LITERAL
        | FLOAT_LITERAL
        | BOOL_LITERAL
        | STRING_LITERAL;