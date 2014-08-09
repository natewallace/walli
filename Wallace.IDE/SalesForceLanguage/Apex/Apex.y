/* Process with > gppg /nolines Apex.y */

%namespace SalesForceLanguage.Apex.Parser
%parsertype ApexParser
%YYSTYPE ApexSyntaxNode
%YYLTYPE ApexTextSpan
%partial 
%sharetokens
%start compilation_unit

/* Terminals (tokens returned by the scanner). */
%token IDENTIFIER   
%token WHITESPACE
%token NEWLINE
%token SOQL
%token SOSL

%token COMMENT_DOC
%token COMMENT_BLOCK
%token COMMENT_INLINE
%token COMMENT_DOCUMENTATION

%token KEYWORD_ABSTRACT
%token KEYWORD_AFTER
%token KEYWORD_ANNOTATE      
%token KEYWORD_BEFORE
%token KEYWORD_BLOB
%token KEYWORD_BOOLEAN
%token KEYWORD_BREAK
%token KEYWORD_CATCH
%token KEYWORD_CLASS
%token KEYWORD_CONTINUE
%token KEYWORD_DATE
%token KEYWORD_DATETIME
%token KEYWORD_DECIMAL
%token KEYWORD_DELETE
%token KEYWORD_DO
%token KEYWORD_DOUBLE
%token KEYWORD_ELSE
%token KEYWORD_ENUM
%token KEYWORD_EXTENDS
%token KEYWORD_FINAL
%token KEYWORD_FINALLY
%token KEYWORD_FOR
%token KEYWORD_GET
%token KEYWORD_GLOBAL
%token KEYWORD_ID
%token KEYWORD_IF
%token KEYWORD_IMPLEMENTS
%token KEYWORD_INSERT
%token KEYWORD_INTERFACE
%token KEYWORD_INTEGER
%token KEYWORD_LONG
%token KEYWORD_MERGE
%token KEYWORD_NEW
%token KEYWORD_ON
%token KEYWORD_OVERRIDE
%token KEYWORD_PRIVATE
%token KEYWORD_PROTECTED
%token KEYWORD_PUBLIC
%token KEYWORD_RETURN
%token KEYWORD_ROLLBACK
%token KEYWORD_SET
%token KEYWORD_STATIC
%token KEYWORD_STRING
%token KEYWORD_SUPER
%token KEYWORD_SYSTEM
%token KEYWORD_TESTMETHOD
%token KEYWORD_THIS
%token KEYWORD_THROW
%token KEYWORD_TRANSIENT
%token KEYWORD_TRIGGER
%token KEYWORD_TRY
%token KEYWORD_UNDELETE
%token KEYWORD_UPDATE
%token KEYWORD_UPSERT
%token KEYWORD_VIRTUAL
%token KEYWORD_VOID
%token KEYWORD_WEBSERVICE
%token KEYWORD_WHILE
%token KEYWORD_WITHSHARING
%token KEYWORD_WITHOUTSHARING

%token LITERAL_TRUE
%token LITERAL_FALSE
%token LITERAL_DOUBLE 
%token LITERAL_INTEGER
%token LITERAL_LONG  
%token LITERAL_NULL        
%token LITERAL_STRING
        
%token OPERATOR_ASSIGNMENT
%token OPERATOR_ASSIGNMENT_MAP
%token OPERATOR_ASSIGNMENT_ADDITION
%token OPERATOR_ASSIGNMENT_MULTIPLICATION
%token OPERATOR_ASSIGNMENT_SUBTRACTION
%token OPERATOR_ASSIGNMENT_DIVISION
%token OPERATOR_ASSIGNMENT_OR
%token OPERATOR_ASSIGNMENT_AND
%token OPERATOR_ASSIGNMENT_EXCLUSIVE_OR
%token OPERATOR_ASSIGNMENT_BITWISE_SHIFT_LEFT
%token OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT
%token OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT_UNSIGNED
%token OPERATOR_AND
%token OPERATOR_OR
%token OPERATOR_EQUALITY
%token OPERATOR_EQUALITY_EXACT
%token OPERATOR_LESS_THAN
%token OPERATOR_GREATER_THAN
%token OPERATOR_GREATER_THAN_A
%token OPERATOR_GREATER_THAN_B
%token OPERATOR_GREATER_THAN_C
%token OPERATOR_LESS_THAN_OR_EQUAL
%token OPERATOR_GREATER_THAN_OR_EQUAL
%token OPERATOR_INEQUALITY
%token OPERATOR_INEQUALITY_ALT
%token OPERATOR_INEQUALITY_EXACT
%token OPERATOR_ADDITION
%token OPERATOR_SUBTRACTION
%token OPERATOR_MULTIPLICATION
%token OPERATOR_DIVISION
%token OPERATOR_LOGICAL_COMPLEMENT
%token OPERATOR_INCREMENT
%token OPERATOR_DECREMENT
%token OPERATOR_BITWISE_AND
%token OPERATOR_BITWISE_OR
%token OPERATOR_BITWISE_EXCLUSIVE_OR
%token OPERATOR_BITWISE_SHIFT_LEFT
%token OPERATOR_QUESTION_MARK
%token OPERATOR_INSTANCEOF

%token SEPARATOR_PARENTHESES_LEFT
%token SEPARATOR_PARENTHESES_RIGHT
%token SEPARATOR_BRACE_LEFT
%token SEPARATOR_BRACE_RIGHT
%token SEPARATOR_BRACKET_LEFT
%token SEPARATOR_BRACKET_RIGHT
%token SEPARATOR_BRACKET_EMPTY
%token SEPARATOR_SEMICOLON
%token SEPARATOR_COLON
%token SEPARATOR_COMMA
%token SEPARATOR_DOT 

%token RESERVED_ACTIVATE
%token RESERVED_ANY
%token RESERVED_ARRAY
%token RESERVED_ASC
%token RESERVED_AUTONOMOUS
%token RESERVED_BEGIN
%token RESERVED_BIGDECIMAL
%token RESERVED_BULK
%token RESERVED_BYTE
%token RESERVED_CAST
%token RESERVED_CHAR
%token RESERVED_COLLECT
%token RESERVED_COMMIT
%token RESERVED_CONST
%token RESERVED_DEFAULT
%token RESERVED_DESC
%token RESERVED_END
%token RESERVED_EXIT
%token RESERVED_EXPORT
%token RESERVED_FLOAT
%token RESERVED_FROM
%token RESERVED_GOTO
%token RESERVED_GROUP
%token RESERVED_HAVING
%token RESERVED_HINT
%token RESERVED_IMPORT
%token RESERVED_IN
%token RESERVED_INNER
%token RESERVED_INT
%token RESERVED_INTO
%token RESERVED_JOIN
%token RESERVED_LIKE
%token RESERVED_LIMIT
%token RESERVED_LOOP
%token RESERVED_NOT
%token RESERVED_NULLS
%token RESERVED_NUMBER
%token RESERVED_OBJECT
%token RESERVED_OF
%token RESERVED_ON
%token RESERVED_OR
%token RESERVED_OUTER
%token RESERVED_PACKAGE
%token RESERVED_PARALLEL
%token RESERVED_PRAGMA
%token RESERVED_RETRIEVE
%token RESERVED_RETURNING
%token RESERVED_SEARCH
%token RESERVED_SELECT
%token RESERVED_SHORT
%token RESERVED_SORT
%token RESERVED_STAT
%token RESERVED_SWITCH
%token RESERVED_SYNCHRONIZED
%token RESERVED_THEN
%token RESERVED_TRANSACTION
%token RESERVED_TYPE
%token RESERVED_USING
%token RESERVED_WHEN
%token RESERVED_WHERE

// Non terminals
%token grammar_accessor_body
%token grammar_accessor_declarations
%token grammar_additive_expression
%token grammar_and_expression
%token grammar_argument
%token grammar_argument_list
%token grammar_array_creation_expression
%token grammar_array_initializer
%token grammar_array_type
%token grammar_assignment
%token grammar_assignment_operator
%token grammar_attribute
%token grammar_attribute_argument_expression
%token grammar_attribute_arguments
%token grammar_attribute_section
%token grammar_attribute_sections
%token grammar_attributes
%token grammar_base_access
%token grammar_block
%token grammar_boolean_expression
%token grammar_break_statement
%token grammar_cast_expression
%token grammar_catch_clauses
%token grammar_class_base
%token grammar_class_body
%token grammar_class_declaration
%token grammar_class_member_declaration
%token grammar_class_member_declarations
%token grammar_class_type
%token grammar_compilation_unit
%token grammar_conditional_and_expression
%token grammar_conditional_expression
%token grammar_conditional_or_expression
%token grammar_constant_declaration
%token grammar_constant_declarator
%token grammar_constant_declarators
%token grammar_constant_expression
%token grammar_constructor_body
%token grammar_constructor_declaration
%token grammar_constructor_declarator
%token grammar_continue_statement
%token grammar_declaration_statement
%token grammar_dim_separators
%token grammar_do_statement
%token grammar_element_access
%token grammar_embedded_statement
%token grammar_empty_statement
%token grammar_enum_body
%token grammar_enum_declaration
%token grammar_enum_member_declaration
%token grammar_enum_member_declarations
%token grammar_enum_type
%token grammar_equality_expression
%token grammar_exclusive_or_expression
%token grammar_expression
%token grammar_expression_list
%token grammar_expression_statement
%token grammar_field_declaration
%token grammar_finally_clause
%token grammar_fixed_parameter
%token grammar_fixed_parameters
%token grammar_floating_point_type
%token grammar_for_condition
%token grammar_for_initializer
%token grammar_for_iterator
%token grammar_for_statement
%token grammar_foreach_statement
%token grammar_formal_parameter_list
%token grammar_general_catch_clause
%token grammar_get_accessor_declaration
%token grammar_identifier
%token grammar_if_statement
%token grammar_inclusive_or_expression
%token grammar_integral_type
%token grammar_interface_accessors
%token grammar_interface_base
%token grammar_interface_body
%token grammar_interface_declaration
%token grammar_interface_member_declaration
%token grammar_interface_member_declarations
%token grammar_interface_method_declaration
%token grammar_interface_property_declaration
%token grammar_interface_type
%token grammar_interface_type_list
%token grammar_invocation_expression
%token grammar_iteration_statement
%token grammar_jump_statement
%token grammar_literal
%token grammar_local_constant_declaration
%token grammar_local_variable_declaration
%token grammar_local_variable_declarator
%token grammar_local_variable_declarators
%token grammar_local_variable_initializer
%token grammar_member_access
%token grammar_method_body
%token grammar_method_declaration
%token grammar_method_header
%token grammar_modifier
%token grammar_modifiers
%token grammar_multiplicative_expression
%token grammar_named_argument
%token grammar_named_argument_list
%token grammar_namespace_or_type_name
%token grammar_non_array_type
%token grammar_non_reserved_identifier
%token grammar_numeric_type
%token grammar_object_creation_expression
%token grammar_parenthesized_expression
%token grammar_post_decrement_expression
%token grammar_post_increment_expression
%token grammar_pre_decrement_expression
%token grammar_pre_increment_expression
%token grammar_predefined_type
%token grammar_primary_expression
%token grammar_primary_no_array_creation_expression
%token grammar_property_declaration
%token grammar_qualified_name
%token grammar_rank_specifier
%token grammar_rank_specifiers
%token grammar_reference_type
%token grammar_relational_expression
%token grammar_return_statement
%token grammar_selection_statement
%token grammar_set_accessor_declaration
%token grammar_shift_expression
%token grammar_simple_name
%token grammar_simple_type
%token grammar_specific_catch_clause
%token grammar_specific_catch_clauses
%token grammar_statement
%token grammar_statement_expression
%token grammar_statement_expression_list
%token grammar_statement_list
%token grammar_static_constructor_body
%token grammar_static_constructor_declaration
%token grammar_struct_type
%token grammar_template_parameter_list
%token grammar_template_parameters
%token grammar_this_access
%token grammar_throw_statement
%token grammar_try_statement
%token grammar_type
%token grammar_type_declaration
%token grammar_type_name
%token grammar_unary_expression
%token grammar_value_type
%token grammar_variable_declarator
%token grammar_variable_declarators
%token grammar_variable_initializer
%token grammar_variable_initializer_list
%token grammar_while_statement

/* Precedences */
%right OPERATOR_ASSIGNMENT OPERATOR_ASSIGNMENT_MAP OPERATOR_ASSIGNMENT_ADDITION OPERATOR_ASSIGNMENT_SUBTRACTION OPERATOR_ASSIGNMENT_MULTIPLICATION OPERATOR_ASSIGNMENT_DIVISION OPERATOR_ASSIGNMENT_AND OPERATOR_ASSIGNMENT_EXCLUSIVE_OR OPERATOR_ASSIGNMENT_BITWISE_SHIFT_LEFT OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT_UNSIGNED
%right OPERATOR_QUESTION_MARK SEPARATOR_COLON
%left OPERATOR_OR
%left OPERATOR_AND
%left OPERATOR_BITWISE_OR
%left OPERATOR_BITWISE_EXCLUSIVE_OR
%left OPERATOR_BITWISE_AND
%left OPERATOR_EQUALITY OPERATOR_EQUALITY_EXACT OPERATOR_INEQUALITY OPERATOR_INEQUALITY_ALT OPERATOR_INEQUALITY_EXACT
%left OPERATOR_LESS_THAN OPERATOR_GREATER_THAN OPERATOR_LESS_THAN_OR_EQUAL OPERATOR_GREATER_THAN_OR_EQUAL OPERATOR_INSTANCEOF
%left OPERATOR_BITWISE_SHIFT_LEFT
%left OPERATOR_ADDITION OPERATOR_SUBTRACTION
%left OPERATOR_MULTIPLICATION OPERATOR_DIVISION
%right OPERATOR_INCREMENT OPERATOR_DECREMENT OPERATOR_LOGICAL_COMPLEMENT

%%
 
identifier:
	KEYWORD_ANNOTATE IDENTIFIER |
	non_reserved_identifier |
	IDENTIFIER ;

non_reserved_identifier:
    KEYWORD_AFTER |
    KEYWORD_BEFORE |
    RESERVED_JOIN |
    RESERVED_SORT |
    KEYWORD_GET |
    KEYWORD_ID |
    KEYWORD_SET |
    KEYWORD_TRIGGER |
    KEYWORD_INSERT |
    KEYWORD_UNDELETE |
    KEYWORD_UPDATE |
    KEYWORD_UPSERT ;

literal:
	LITERAL_TRUE |
	LITERAL_FALSE |
	LITERAL_DOUBLE |
	LITERAL_INTEGER |
	LITERAL_LONG |
	LITERAL_NULL |
	LITERAL_STRING ;

type_name:
	namespace_or_type_name ;

namespace_or_type_name:
	qualified_name |
	qualified_name template_parameter_list |
	namespace_or_type_name SEPARATOR_DOT identifier |
	namespace_or_type_name SEPARATOR_DOT identifier template_parameter_list ;

qualified_name:
	identifier |
	qualified_name SEPARATOR_DOT identifier ;

type:
	value_type |
	reference_type ;

value_type:
	struct_type |
	enum_type ;

struct_type:
	type_name |
	simple_type ;

simple_type:
	numeric_type |
	KEYWORD_ID |
	KEYWORD_BLOB |
	KEYWORD_DATE |
	KEYWORD_DATETIME |
	KEYWORD_BOOLEAN ;

numeric_type:
	integral_type |
	floating_point_type |
	KEYWORD_DECIMAL ;

integral_type:
	KEYWORD_INTEGER |
	KEYWORD_LONG ;

floating_point_type:
	KEYWORD_DOUBLE ;

enum_type:
	type_name ;

reference_type:
	class_type |
	interface_type |
	array_type ;

class_type:
	type_name |
	KEYWORD_STRING ;

interface_type:
	type_name ;

template_parameter_list:
	OPERATOR_LESS_THAN template_parameters OPERATOR_GREATER_THAN   |
    OPERATOR_LESS_THAN template_parameters OPERATOR_GREATER_THAN_A |
    OPERATOR_LESS_THAN template_parameters OPERATOR_GREATER_THAN_B |
    OPERATOR_LESS_THAN template_parameters OPERATOR_GREATER_THAN_C ;

template_parameters:
	type |
	template_parameters SEPARATOR_COMMA type ;

array_type:
	non_array_type rank_specifiers ;

non_array_type:
	type ;

rank_specifiers:
	rank_specifier |
	rank_specifiers rank_specifier ;

rank_specifier:
	SEPARATOR_BRACKET_EMPTY |
	SEPARATOR_BRACKET_LEFT dim_separators SEPARATOR_BRACKET_RIGHT ;

dim_separators:
	SEPARATOR_COMMA |
	dim_separators SEPARATOR_COMMA ;

argument_list:
	argument |
	argument_list SEPARATOR_COMMA argument ;

argument:
	expression ;

primary_expression:
	primary_no_array_creation_expression |
	array_creation_expression ;

primary_no_array_creation_expression:
	literal |
	simple_name |
	parenthesized_expression |
	member_access |
	invocation_expression |
	element_access |
	this_access |
	base_access |
	post_increment_expression |
	post_decrement_expression |
	object_creation_expression ;

simple_name:
	identifier ;

parenthesized_expression:
	SEPARATOR_PARENTHESES_LEFT expression SEPARATOR_PARENTHESES_RIGHT ;

member_access:
	qualified_name |
	primary_expression SEPARATOR_DOT identifier |
	predefined_type SEPARATOR_DOT identifier ;

predefined_type:
	KEYWORD_DATE |
	KEYWORD_DATETIME |
	KEYWORD_BOOLEAN |
	KEYWORD_BLOB |
	KEYWORD_DECIMAL |
	KEYWORD_DOUBLE |
	KEYWORD_INTEGER |
	KEYWORD_LONG |
	KEYWORD_STRING |
	KEYWORD_ID ;

invocation_expression:
	primary_expression SEPARATOR_PARENTHESES_LEFT argument_list SEPARATOR_PARENTHESES_RIGHT |
	primary_expression SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT ;

element_access:
	identifier SEPARATOR_BRACKET_LEFT expression_list SEPARATOR_BRACKET_RIGHT |
	primary_no_array_creation_expression SEPARATOR_BRACKET_LEFT expression_list SEPARATOR_BRACKET_RIGHT ;

expression_list:
	expression |
	expression_list SEPARATOR_COMMA expression ;

this_access:
	KEYWORD_THIS ;

base_access:
	KEYWORD_SUPER SEPARATOR_DOT identifier |
	KEYWORD_SUPER SEPARATOR_BRACKET_LEFT expression_list SEPARATOR_BRACKET_RIGHT ;

post_increment_expression:
	primary_expression OPERATOR_INCREMENT ;

post_decrement_expression:
	primary_expression OPERATOR_DECREMENT ;

object_creation_expression:
	KEYWORD_NEW type SEPARATOR_PARENTHESES_LEFT argument_list SEPARATOR_PARENTHESES_RIGHT |
	KEYWORD_NEW type SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT |
	KEYWORD_NEW type SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT |
	KEYWORD_NEW type SEPARATOR_BRACE_LEFT argument_list SEPARATOR_BRACE_RIGHT ;

array_creation_expression:
	KEYWORD_NEW non_array_type SEPARATOR_BRACKET_LEFT expression_list SEPARATOR_BRACKET_RIGHT rank_specifiers array_initializer |
	KEYWORD_NEW non_array_type SEPARATOR_BRACKET_LEFT expression_list SEPARATOR_BRACKET_RIGHT rank_specifiers |
	KEYWORD_NEW non_array_type SEPARATOR_BRACKET_LEFT expression_list SEPARATOR_BRACKET_RIGHT array_initializer |
	KEYWORD_NEW non_array_type SEPARATOR_BRACKET_LEFT expression_list SEPARATOR_BRACKET_RIGHT |
	KEYWORD_NEW array_type array_initializer ;

unary_expression:
	primary_expression |
	OPERATOR_ADDITION unary_expression |
	OPERATOR_SUBTRACTION unary_expression |
	OPERATOR_LOGICAL_COMPLEMENT unary_expression |
	OPERATOR_MULTIPLICATION unary_expression |
	pre_increment_expression |
	pre_decrement_expression |
	cast_expression ;

pre_increment_expression:
	OPERATOR_INCREMENT unary_expression ;

pre_decrement_expression:
	OPERATOR_DECREMENT unary_expression ;

cast_expression:
	SEPARATOR_PARENTHESES_LEFT type SEPARATOR_PARENTHESES_RIGHT unary_expression |
	SEPARATOR_PARENTHESES_LEFT type SEPARATOR_PARENTHESES_RIGHT SEPARATOR_PARENTHESES_LEFT identifier SEPARATOR_PARENTHESES_RIGHT ;

multiplicative_expression:
	unary_expression |
	multiplicative_expression OPERATOR_MULTIPLICATION unary_expression |
	multiplicative_expression OPERATOR_DIVISION unary_expression ;

additive_expression:
	multiplicative_expression |
	additive_expression OPERATOR_ADDITION multiplicative_expression |
	additive_expression OPERATOR_SUBTRACTION multiplicative_expression ;

shift_expression:
	additive_expression |
	shift_expression OPERATOR_BITWISE_SHIFT_LEFT additive_expression |
	shift_expression OPERATOR_GREATER_THAN_A OPERATOR_GREATER_THAN_B additive_expression |
	shift_expression OPERATOR_GREATER_THAN_A OPERATOR_GREATER_THAN_B OPERATOR_GREATER_THAN_C additive_expression ;

relational_expression:
	shift_expression |
	relational_expression OPERATOR_LESS_THAN shift_expression |
	relational_expression OPERATOR_GREATER_THAN shift_expression |
	relational_expression OPERATOR_LESS_THAN_OR_EQUAL shift_expression |
	relational_expression OPERATOR_GREATER_THAN_OR_EQUAL shift_expression |
	relational_expression OPERATOR_INSTANCEOF type ;

equality_expression:
	relational_expression |
	equality_expression OPERATOR_EQUALITY relational_expression |
	equality_expression OPERATOR_EQUALITY_EXACT relational_expression |
	equality_expression OPERATOR_INEQUALITY relational_expression |
	equality_expression OPERATOR_INEQUALITY_ALT relational_expression |
	equality_expression OPERATOR_INEQUALITY_EXACT relational_expression ;

and_expression:
	equality_expression |
	and_expression OPERATOR_BITWISE_AND equality_expression ;

exclusive_or_expression:
	and_expression |
	exclusive_or_expression OPERATOR_BITWISE_EXCLUSIVE_OR and_expression ;

inclusive_or_expression:
	exclusive_or_expression |
	inclusive_or_expression OPERATOR_BITWISE_OR exclusive_or_expression ;

conditional_and_expression:
	inclusive_or_expression |
	conditional_and_expression OPERATOR_AND inclusive_or_expression ;

conditional_or_expression:
	conditional_and_expression |
	conditional_or_expression OPERATOR_OR conditional_and_expression ;

conditional_expression:
	conditional_or_expression |
	conditional_or_expression OPERATOR_QUESTION_MARK expression SEPARATOR_COLON expression ;

assignment:
	unary_expression assignment_operator expression ;

assignment_operator:
	OPERATOR_ASSIGNMENT |
	OPERATOR_ASSIGNMENT_MAP |
	OPERATOR_ASSIGNMENT_ADDITION |
	OPERATOR_ASSIGNMENT_MULTIPLICATION |
	OPERATOR_ASSIGNMENT_SUBTRACTION |
	OPERATOR_ASSIGNMENT_DIVISION |
	OPERATOR_ASSIGNMENT_OR |
	OPERATOR_ASSIGNMENT_AND |
	OPERATOR_ASSIGNMENT_EXCLUSIVE_OR |
	OPERATOR_ASSIGNMENT_BITWISE_SHIFT_LEFT |
	OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT |
	OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT_UNSIGNED ;

expression:
	conditional_expression |
	assignment |
	SOQL |
	SOSL ;

constant_expression:
	expression ;

boolean_expression:
	expression ;

statement:
	declaration_statement |
	embedded_statement |
	error SEPARATOR_SEMICOLON { Error(); } ;

embedded_statement:
	block |
	empty_statement |
	expression_statement |
	selection_statement |
	iteration_statement |
	jump_statement |
	try_statement ;

block:
	SEPARATOR_BRACE_LEFT statement_list SEPARATOR_BRACE_RIGHT |
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT ;

statement_list:
	statement |
	statement_list statement ;

empty_statement:
	SEPARATOR_SEMICOLON ;

declaration_statement:
	local_variable_declaration SEPARATOR_SEMICOLON |
	local_constant_declaration SEPARATOR_SEMICOLON ;

local_variable_declaration:
	type local_variable_declarators ;

local_variable_declarators:
	local_variable_declarator |
	local_variable_declarators SEPARATOR_COMMA local_variable_declarator ;

local_variable_declarator:
	identifier |
	identifier OPERATOR_ASSIGNMENT local_variable_initializer ;

local_variable_initializer:
	expression |
	array_initializer ;

local_constant_declaration:
	KEYWORD_FINAL type constant_declarators ;

constant_declarators:
	constant_declarator |
	constant_declarators SEPARATOR_COMMA constant_declarator ;

constant_declarator:
	identifier OPERATOR_ASSIGNMENT constant_expression ;

expression_statement:
	statement_expression SEPARATOR_SEMICOLON ;

statement_expression:
	invocation_expression |
	object_creation_expression |
	assignment |
	post_increment_expression |
	post_decrement_expression |
	pre_increment_expression |
	pre_decrement_expression ;

selection_statement:
	if_statement ;

if_statement:
	KEYWORD_IF SEPARATOR_PARENTHESES_LEFT boolean_expression SEPARATOR_PARENTHESES_RIGHT embedded_statement |
	KEYWORD_IF SEPARATOR_PARENTHESES_LEFT boolean_expression SEPARATOR_PARENTHESES_RIGHT embedded_statement KEYWORD_ELSE embedded_statement ;

boolean_expression:
	expression ;

iteration_statement:
	while_statement |
	do_statement |
	for_statement |
	foreach_statement ;

while_statement:
	KEYWORD_WHILE SEPARATOR_PARENTHESES_LEFT boolean_expression SEPARATOR_PARENTHESES_RIGHT embedded_statement ;

do_statement:
	KEYWORD_DO embedded_statement KEYWORD_WHILE SEPARATOR_PARENTHESES_LEFT boolean_expression SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON ;

for_statement:
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT                 SEPARATOR_SEMICOLON               SEPARATOR_SEMICOLON              SEPARATOR_PARENTHESES_RIGHT embedded_statement |
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT                 SEPARATOR_SEMICOLON               SEPARATOR_SEMICOLON for_iterator SEPARATOR_PARENTHESES_RIGHT embedded_statement |
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT                 SEPARATOR_SEMICOLON for_condition SEPARATOR_SEMICOLON              SEPARATOR_PARENTHESES_RIGHT embedded_statement |
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT                 SEPARATOR_SEMICOLON for_condition SEPARATOR_SEMICOLON for_iterator SEPARATOR_PARENTHESES_RIGHT embedded_statement |
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT for_initializer SEPARATOR_SEMICOLON               SEPARATOR_SEMICOLON              SEPARATOR_PARENTHESES_RIGHT embedded_statement |
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT for_initializer SEPARATOR_SEMICOLON               SEPARATOR_SEMICOLON for_iterator SEPARATOR_PARENTHESES_RIGHT embedded_statement |
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT for_initializer SEPARATOR_SEMICOLON for_condition SEPARATOR_SEMICOLON              SEPARATOR_PARENTHESES_RIGHT embedded_statement |
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT for_initializer SEPARATOR_SEMICOLON for_condition SEPARATOR_SEMICOLON for_iterator SEPARATOR_PARENTHESES_RIGHT embedded_statement ;

for_initializer:
	local_variable_declaration |
	statement_expression_list ;

for_condition:
	boolean_expression ;

for_iterator:
	statement_expression_list ;

statement_expression_list:
	statement_expression |
	statement_expression_list SEPARATOR_COMMA statement_expression ;

foreach_statement:
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT type identifier SEPARATOR_COLON expression SEPARATOR_PARENTHESES_RIGHT embedded_statement ;

jump_statement:
	break_statement |
	continue_statement |
	return_statement |
	throw_statement ;

break_statement:
	KEYWORD_BREAK SEPARATOR_SEMICOLON ;

continue_statement:
	KEYWORD_CONTINUE SEPARATOR_SEMICOLON ;

return_statement:
	KEYWORD_RETURN expression SEPARATOR_SEMICOLON |
	KEYWORD_RETURN SEPARATOR_SEMICOLON ;

throw_statement:
	KEYWORD_THROW expression SEPARATOR_SEMICOLON ;

try_statement:
	KEYWORD_TRY block catch_clauses |
	KEYWORD_TRY block finally_clause |
	KEYWORD_TRY block catch_clauses finally_clause ;

catch_clauses:
	specific_catch_clauses general_catch_clause |
	general_catch_clause specific_catch_clauses |
	general_catch_clause |
	specific_catch_clauses ;

specific_catch_clauses:
	specific_catch_clause |
	specific_catch_clauses specific_catch_clause ;

specific_catch_clause:
	KEYWORD_CATCH SEPARATOR_PARENTHESES_LEFT class_type identifier SEPARATOR_PARENTHESES_RIGHT block |
	KEYWORD_CATCH SEPARATOR_PARENTHESES_LEFT class_type SEPARATOR_PARENTHESES_RIGHT block ;

general_catch_clause:
	KEYWORD_CATCH block ;

finally_clause:
	KEYWORD_FINALLY block ;

compilation_unit:
	|
	type_declaration ;

type_declaration:
	class_declaration |
	interface_declaration |
	enum_declaration ;

class_declaration:
	                     KEYWORD_CLASS identifier            class_body |
	                     KEYWORD_CLASS identifier            class_body SEPARATOR_SEMICOLON |
	                     KEYWORD_CLASS identifier class_base class_body |
	                     KEYWORD_CLASS identifier class_base class_body SEPARATOR_SEMICOLON |
	           modifiers KEYWORD_CLASS identifier            class_body |
	           modifiers KEYWORD_CLASS identifier            class_body SEPARATOR_SEMICOLON |
	           modifiers KEYWORD_CLASS identifier class_base class_body |
	           modifiers KEYWORD_CLASS identifier class_base class_body SEPARATOR_SEMICOLON |
	attributes           KEYWORD_CLASS identifier            class_body |
	attributes           KEYWORD_CLASS identifier            class_body SEPARATOR_SEMICOLON |
	attributes           KEYWORD_CLASS identifier class_base class_body |
	attributes           KEYWORD_CLASS identifier class_base class_body SEPARATOR_SEMICOLON |
	attributes modifiers KEYWORD_CLASS identifier            class_body |
	attributes modifiers KEYWORD_CLASS identifier            class_body SEPARATOR_SEMICOLON |
	attributes modifiers KEYWORD_CLASS identifier class_base class_body |
	attributes modifiers KEYWORD_CLASS identifier class_base class_body SEPARATOR_SEMICOLON ;

class_base:
	KEYWORD_EXTENDS class_type |
	KEYWORD_IMPLEMENTS interface_type_list |
	KEYWORD_EXTENDS class_type KEYWORD_IMPLEMENTS interface_type_list ;

interface_type_list:
	interface_type |
	interface_type_list SEPARATOR_COMMA interface_type ;

class_body:
	SEPARATOR_BRACE_LEFT class_member_declarations SEPARATOR_BRACE_RIGHT |
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT ;

class_member_declarations:
	class_member_declaration |
	class_member_declarations class_member_declaration ;

class_member_declaration:
	constant_declaration |
	field_declaration |
	method_declaration |
	property_declaration |
	constructor_declaration |
	static_constructor_declaration |
	type_declaration |
	error SEPARATOR_SEMICOLON { Error(); } |
	error SEPARATOR_BRACE_RIGHT { Error(); } ;

constant_declaration:
	           modifiers KEYWORD_FINAL type constant_declarators SEPARATOR_SEMICOLON |
	                     KEYWORD_FINAL type constant_declarators SEPARATOR_SEMICOLON |
	attributes modifiers KEYWORD_FINAL type constant_declarators SEPARATOR_SEMICOLON |
	attributes           KEYWORD_FINAL type constant_declarators SEPARATOR_SEMICOLON ;

constant_declarators:
	constant_declarator |
	constant_declarators SEPARATOR_COMMA constant_declarator ;

constant_declarator:
	identifier OPERATOR_ASSIGNMENT constant_expression ;

field_declaration:
	           modifiers type variable_declarators SEPARATOR_SEMICOLON |
	                     type variable_declarators SEPARATOR_SEMICOLON |
	attributes modifiers type variable_declarators SEPARATOR_SEMICOLON |
	attributes           type variable_declarators SEPARATOR_SEMICOLON ;

variable_declarators:
	variable_declarator |
	variable_declarators SEPARATOR_COMMA variable_declarator ;

variable_declarator:
	identifier |
	identifier OPERATOR_ASSIGNMENT variable_initializer ;

variable_initializer:
	expression |
	array_initializer ;

method_declaration:
	method_header method_body ;

method_header:
	                     type         identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT |
	                     type         identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT |
	           modifiers type         identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT |
	           modifiers type         identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT |
	attributes           type         identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT |
	attributes           type         identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT |
	attributes modifiers type         identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT |
	attributes modifiers type         identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT |
	                     KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT |
	                     KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT |
	           modifiers KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT |
	           modifiers KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT |
	attributes           KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT |
	attributes           KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT |
	attributes modifiers KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT |
	attributes modifiers KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT ;

method_body:
	block |
	SEPARATOR_SEMICOLON ;

formal_parameter_list:
	fixed_parameters ;

fixed_parameters:
	fixed_parameter |
	fixed_parameters SEPARATOR_COMMA fixed_parameter ;

fixed_parameter:
	type identifier ;

property_declaration:
	                     type identifier SEPARATOR_BRACE_LEFT accessor_declarations SEPARATOR_BRACE_RIGHT |
	           modifiers type identifier SEPARATOR_BRACE_LEFT accessor_declarations SEPARATOR_BRACE_RIGHT |
	attributes           type identifier SEPARATOR_BRACE_LEFT accessor_declarations SEPARATOR_BRACE_RIGHT |
	attributes modifiers type identifier SEPARATOR_BRACE_LEFT accessor_declarations SEPARATOR_BRACE_RIGHT ;

accessor_declarations:
	get_accessor_declaration set_accessor_declaration |
	get_accessor_declaration |
	set_accessor_declaration get_accessor_declaration |
	set_accessor_declaration ;

get_accessor_declaration:
	attributes KEYWORD_GET accessor_body |
	           KEYWORD_GET accessor_body ;

set_accessor_declaration:
	attributes KEYWORD_SET accessor_body |
	           KEYWORD_SET accessor_body ;

accessor_body:
	block |
	SEPARATOR_SEMICOLON ;

constructor_declaration:
	                     constructor_declarator constructor_body |
	           modifiers constructor_declarator constructor_body |
	attributes           constructor_declarator constructor_body |
	attributes modifiers constructor_declarator constructor_body ;

constructor_declarator:
	identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT |
	identifier SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT ;

constructor_body:
	block |
	SEPARATOR_SEMICOLON ;

static_constructor_declaration:
	attributes KEYWORD_STATIC static_constructor_body |
	           KEYWORD_STATIC static_constructor_body ;

static_constructor_body:
	block |
	SEPARATOR_SEMICOLON ;

array_initializer:
	SEPARATOR_BRACE_LEFT variable_initializer_list SEPARATOR_BRACE_RIGHT |
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT |
	SEPARATOR_BRACE_LEFT variable_initializer_list SEPARATOR_COMMA SEPARATOR_BRACE_RIGHT ;

variable_initializer_list:
	variable_initializer |
	variable_initializer_list SEPARATOR_COMMA variable_initializer ;

variable_initializer:
	expression |
	array_initializer ;

interface_declaration:
	                     KEYWORD_INTERFACE identifier                interface_body |
	                     KEYWORD_INTERFACE identifier                interface_body SEPARATOR_SEMICOLON |
	                     KEYWORD_INTERFACE identifier interface_base interface_body |
	                     KEYWORD_INTERFACE identifier interface_base interface_body SEPARATOR_SEMICOLON |
	           modifiers KEYWORD_INTERFACE identifier                interface_body |
	           modifiers KEYWORD_INTERFACE identifier                interface_body SEPARATOR_SEMICOLON |
	           modifiers KEYWORD_INTERFACE identifier interface_base interface_body |
	           modifiers KEYWORD_INTERFACE identifier interface_base interface_body SEPARATOR_SEMICOLON |
	attributes           KEYWORD_INTERFACE identifier                interface_body |
	attributes           KEYWORD_INTERFACE identifier                interface_body SEPARATOR_SEMICOLON |
	attributes           KEYWORD_INTERFACE identifier interface_base interface_body |
	attributes           KEYWORD_INTERFACE identifier interface_base interface_body SEPARATOR_SEMICOLON |
	attributes modifiers KEYWORD_INTERFACE identifier                interface_body |
	attributes modifiers KEYWORD_INTERFACE identifier                interface_body SEPARATOR_SEMICOLON |
	attributes modifiers KEYWORD_INTERFACE identifier interface_base interface_body |
	attributes modifiers KEYWORD_INTERFACE identifier interface_base interface_body SEPARATOR_SEMICOLON ;
 
interface_base:
	KEYWORD_IMPLEMENTS interface_type_list ;

interface_body:
	SEPARATOR_BRACE_LEFT interface_member_declarations SEPARATOR_BRACE_RIGHT |
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT ;

interface_member_declarations:
	interface_member_declaration |
	interface_member_declarations interface_member_declaration ;

interface_member_declaration:
	interface_method_declaration |
	interface_property_declaration ;

interface_method_declaration:
	           type         identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON |
	           type         identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON |
	attributes type         identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON |
	attributes type         identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON |
	           KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON |
	           KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON |
	attributes KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT                       SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON |
	attributes KEYWORD_VOID identifier SEPARATOR_PARENTHESES_LEFT formal_parameter_list SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON ;

interface_property_declaration:
	attributes type identifier SEPARATOR_BRACE_LEFT interface_accessors SEPARATOR_BRACE_RIGHT |
	           type identifier SEPARATOR_BRACE_LEFT interface_accessors SEPARATOR_BRACE_RIGHT ;

interface_accessors:
	attributes KEYWORD_GET SEPARATOR_SEMICOLON |
		       KEYWORD_GET SEPARATOR_SEMICOLON |
	attributes KEYWORD_SET SEPARATOR_SEMICOLON |
	           KEYWORD_SET SEPARATOR_SEMICOLON |
	           KEYWORD_GET SEPARATOR_SEMICOLON            KEYWORD_SET SEPARATOR_SEMICOLON |
	           KEYWORD_GET SEPARATOR_SEMICOLON attributes KEYWORD_SET SEPARATOR_SEMICOLON |
	attributes KEYWORD_GET SEPARATOR_SEMICOLON            KEYWORD_SET SEPARATOR_SEMICOLON |
	attributes KEYWORD_GET SEPARATOR_SEMICOLON attributes KEYWORD_SET SEPARATOR_SEMICOLON |
	           KEYWORD_SET SEPARATOR_SEMICOLON            KEYWORD_GET SEPARATOR_SEMICOLON |
	           KEYWORD_SET SEPARATOR_SEMICOLON attributes KEYWORD_GET SEPARATOR_SEMICOLON |
	attributes KEYWORD_SET SEPARATOR_SEMICOLON            KEYWORD_GET SEPARATOR_SEMICOLON |
	attributes KEYWORD_SET SEPARATOR_SEMICOLON attributes KEYWORD_GET SEPARATOR_SEMICOLON ;

enum_declaration:
	                     KEYWORD_ENUM identifier enum_body |
	                     KEYWORD_ENUM identifier enum_body SEPARATOR_SEMICOLON |
	           modifiers KEYWORD_ENUM identifier enum_body |
	           modifiers KEYWORD_ENUM identifier enum_body SEPARATOR_SEMICOLON |
	attributes           KEYWORD_ENUM identifier enum_body |
	attributes           KEYWORD_ENUM identifier enum_body SEPARATOR_SEMICOLON |
	attributes modifiers KEYWORD_ENUM identifier enum_body |
	attributes modifiers KEYWORD_ENUM identifier enum_body SEPARATOR_SEMICOLON ;

enum_body:
	SEPARATOR_BRACE_LEFT enum_member_declarations SEPARATOR_BRACE_RIGHT |
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT |
	SEPARATOR_BRACE_LEFT enum_member_declarations SEPARATOR_COMMA SEPARATOR_BRACE_RIGHT ;

enum_member_declarations:
	enum_member_declaration |
	enum_member_declarations SEPARATOR_COMMA enum_member_declaration ;

enum_member_declaration:
	identifier ;

modifiers:
	modifier |
	modifiers modifier ;

modifier:
	KEYWORD_PUBLIC |
	KEYWORD_PROTECTED |
	KEYWORD_PRIVATE |
	KEYWORD_ABSTRACT |
	KEYWORD_STATIC |
	KEYWORD_GLOBAL |
	KEYWORD_OVERRIDE |
	KEYWORD_VIRTUAL |
	KEYWORD_TESTMETHOD |
	KEYWORD_WITHSHARING |
	KEYWORD_WITHOUTSHARING |
	KEYWORD_WEBSERVICE ;

attributes:
	attribute_sections ;

attribute_sections:
	attribute_section |
	attribute_sections attribute_section ;

attribute_section:
	KEYWORD_ANNOTATE identifier |
	KEYWORD_ANNOTATE identifier attribute_arguments ;

attribute_arguments:
	SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT |
	SEPARATOR_PARENTHESES_LEFT named_argument_list SEPARATOR_PARENTHESES_RIGHT ;

named_argument_list:
	named_argument |
	named_argument_list SEPARATOR_COMMA named_argument ;

named_argument:
	identifier OPERATOR_ASSIGNMENT attribute_argument_expression ;

attribute_argument_expression:
	expression ;

%%