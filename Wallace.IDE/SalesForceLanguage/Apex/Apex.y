/* Process with > gppg /nolines Apex.y */

%namespace SalesForceLanguage.Apex.Parser
%parsertype ApexParser
%YYSTYPE ApexSyntaxNode
%YYLTYPE ApexTextSpan
%partial 
%sharetokens
%start Goal

/* Terminals (tokens returned by the scanner). */
%token IDENTIFIER   
%token WHITESPACE
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
%token ProductionAbstractMethodDeclaration
%token ProductionAccessorBody
%token ProductionAccessorDeclarations
%token ProductionAdditiveExpression
%token ProductionAndExpression
%token ProductionAnnotation
%token ProductionAnnotatedClassDeclaration
%token ProductionAnnotatedEnumDeclaration
%token ProductionAnnotatedFieldDeclaration
%token ProductionAnnotatedInterfaceDeclaration
%token ProductionAnnotatedMethodDeclaration
%token ProductionAnnotations
%token ProductionArgumentList
%token ProductionArrayAccess
%token ProductionArrayCreationExpression
%token ProductionArrayInitializer
%token ProductionArrayType
%token ProductionAssignment
%token ProductionAssignmentExpression
%token ProductionAssignmentOperator
%token ProductionMapArgumentAssignment
%token ProductionMapArgumentAssignmentList
%token ProductionBlock
%token ProductionBlockStatement
%token ProductionBlockStatements
%token ProductionBreakStatement
%token ProductionCatchClause
%token ProductionCatches
%token ProductionClassBody
%token ProductionClassBodyDeclaration
%token ProductionClassBodyDeclarations
%token ProductionClassDeclaration
%token ProductionClassInstanceCreationExpression
%token ProductionClassMemberDeclaration
%token ProductionCastExpression
%token ProductionCollectionType
%token ProductionCompilationUnit
%token ProductionConditionalAndExpression
%token ProductionConditionalExpression
%token ProductionConditionalOrExpression
%token ProductionConstantExpression
%token ProductionConstructorBody
%token ProductionConstructorDeclaration
%token ProductionConstructorDeclarator
%token ProductionContinueStatement
%token ProductionDimExpr
%token ProductionDimExprs
%token ProductionDims
%token ProductionDMLStatement
%token ProductionDoStatement
%token ProductionEmptyStatement
%token ProductionEnumBody
%token ProductionEnumDeclaration
%token ProductionEnumMemberDeclaration
%token ProductionEnumMemberDeclarations
%token ProductionEqualityExpression
%token ProductionExclusiveOrExpression
%token ProductionExplicitConstructorInvocation
%token ProductionExpression
%token ProductionExpressionStatement
%token ProductionExtendsInterfaces
%token ProductionFieldAccess
%token ProductionFieldDeclaration
%token ProductionFinally
%token ProductionForEachStatement
%token ProductionForEachStatementNoShortIf
%token ProductionForExpression
%token ProductionForInit
%token ProductionFormalParameter
%token ProductionFormalParameterList
%token ProductionForUpdate
%token ProductionForStatement
%token ProductionForStatementNoShortIf
%token ProductionGetAccessorDeclaration
%token ProductionGoal
%token ProductionIfThenElseStatement
%token ProductionIfThenElseStatementNoShortIf
%token ProductionIfThenStatement
%token ProductionInclusiveOrExpression
%token ProductionInterfaceBody
%token ProductionInterfaceDeclaration
%token ProductionInterfaceMemberDeclaration
%token ProductionInterfaceMemberDeclarations
%token ProductionInterfaces
%token ProductionInterfaceTypeList
%token ProductionLeftHandSide
%token ProductionLiteral
%token ProductionLocalVariableDeclaration
%token ProductionLocalVariableDeclarationStatement
%token ProductionMethodBody
%token ProductionMethodDeclaration
%token ProductionMethodDeclarator
%token ProductionMethodHeader
%token ProductionMethodInvocation
%token ProductionModifier
%token ProductionModifiers
%token ProductionMultiplicativeExpression
%token ProductionName
%token ProductionNonReservedIdentifier
%token ProductionPostDecrementExpression
%token ProductionPostIncrementExpression
%token ProductionPostfixExpression
%token ProductionPreDecrementExpression
%token ProductionPreIncrementExpression
%token ProductionPrimary
%token ProductionPrimaryNoNewArray
%token ProductionPrimitiveType
%token ProductionPropertyDeclaration
%token ProductionQualifiedName
%token ProductionReferenceType
%token ProductionRelationalExpression
%token ProductionRelationalOperator
%token ProductionReturnStatement
%token ProductionSetAccessorDeclaration
%token ProductionShiftExpression
%token ProductionSimpleName
%token ProductionStatement
%token ProductionStatementExpression
%token ProductionStatementExpressionList
%token ProductionStatementNoShortIf
%token ProductionStatementWithoutTrailingSubstatement
%token ProductionStaticInitializer
%token ProductionSuper
%token ProductionTemplateParameterList
%token ProductionTemplateParameters
%token ProductionThrowStatement
%token ProductionTriggerDeclaration
%token ProductionTriggerEvent
%token ProductionTriggerEvents
%token ProductionTriggerHeader
%token ProductionTryStatement
%token ProductionType
%token ProductionTypeDeclaration
%token ProductionUnaryExpression
%token ProductionUnaryExpressionNotPlusMinus
%token ProductionVariableDeclarator
%token ProductionVariableDeclaratorId
%token ProductionVariableDeclarators
%token ProductionVariableInitializer
%token ProductionVariableInitializers
%token ProductionWhileStatement
%token ProductionWhileStatementNoShortIf

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

/* Grammar ===============================================================================================================*/

Goal :					
	CompilationUnit 	{ GoalNode = Node(Tokens.ProductionGoal, $1); $$ = GoalNode; } ;

/* Lexical Structure =====================================================================================================*/

Literal :               
	LITERAL_TRUE	{ $$ = Node(Tokens.ProductionLiteral, $1); } |
	LITERAL_FALSE 	{ $$ = Node(Tokens.ProductionLiteral, $1); } |
	LITERAL_DOUBLE 	{ $$ = Node(Tokens.ProductionLiteral, $1); } |
	LITERAL_INTEGER { $$ = Node(Tokens.ProductionLiteral, $1); } |
	LITERAL_LONG	{ $$ = Node(Tokens.ProductionLiteral, $1); } |
	LITERAL_NULL	{ $$ = Node(Tokens.ProductionLiteral, $1); } |     
	LITERAL_STRING 	{ $$ = Node(Tokens.ProductionLiteral, $1); } ; 

/* Types, Values, and Variables ==========================================================================================*/

Type :					
	PrimitiveType 	{ $$ = Node(Tokens.ProductionType, $1); } |
	ReferenceType	{ $$ = Node(Tokens.ProductionType, $1); } ;
											
PrimitiveType :			
	KEYWORD_BLOB		{ $$ = Node(Tokens.ProductionPrimitiveType, $1); } |
	KEYWORD_BOOLEAN		{ $$ = Node(Tokens.ProductionPrimitiveType, $1); } |
	KEYWORD_DATE		{ $$ = Node(Tokens.ProductionPrimitiveType, $1); } |
	KEYWORD_DATETIME	{ $$ = Node(Tokens.ProductionPrimitiveType, $1); } |
	KEYWORD_DECIMAL		{ $$ = Node(Tokens.ProductionPrimitiveType, $1); } |
	KEYWORD_DOUBLE		{ $$ = Node(Tokens.ProductionPrimitiveType, $1); } |
	KEYWORD_ID			{ $$ = Node(Tokens.ProductionPrimitiveType, $1); } |
	KEYWORD_INTEGER		{ $$ = Node(Tokens.ProductionPrimitiveType, $1); } |
	KEYWORD_LONG		{ $$ = Node(Tokens.ProductionPrimitiveType, $1); } |
	KEYWORD_STRING		{ $$ = Node(Tokens.ProductionPrimitiveType, $1); } ;
														
TemplateParameterList :				
	OPERATOR_LESS_THAN TemplateParameters OPERATOR_GREATER_THAN		{ $$ = Node(Tokens.ProductionTemplateParameterList, $1, $2, $3); } |
	OPERATOR_LESS_THAN TemplateParameters OPERATOR_GREATER_THAN_A	{ $$ = Node(Tokens.ProductionTemplateParameterList, $1, $2, $3); } |
	OPERATOR_LESS_THAN TemplateParameters OPERATOR_GREATER_THAN_B	{ $$ = Node(Tokens.ProductionTemplateParameterList, $1, $2, $3); } |
	OPERATOR_LESS_THAN TemplateParameters OPERATOR_GREATER_THAN_C	{ $$ = Node(Tokens.ProductionTemplateParameterList, $1, $2, $3); } ;
														
TemplateParameters :	
	Type									{ $$ = Node(Tokens.ProductionTemplateParameters, $1); } |
	TemplateParameters SEPARATOR_COMMA Type	{ $$ = Node(Tokens.ProductionTemplateParameters, $1, $2, $3); } ;
											
ReferenceType :				
	Name						{ $$ = Node(Tokens.ProductionReferenceType, $1); } |
	Name TemplateParameterList	{ $$ = Node(Tokens.ProductionReferenceType, $1, $2); } |
	ArrayType					{ $$ = Node(Tokens.ProductionReferenceType, $1); } ;		

ArrayType :					
	Type SEPARATOR_BRACKET_EMPTY	{ $$ = Node(Tokens.ProductionArrayType, $1, $2); } ;

/* Names =================================================================================================================*/

NonReservedIdentifier :	
	KEYWORD_AFTER		{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	KEYWORD_BEFORE		{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	RESERVED_JOIN		{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	RESERVED_SORT		{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	KEYWORD_GET			{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	KEYWORD_ID			{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	KEYWORD_SET			{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	KEYWORD_TRIGGER		{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	KEYWORD_INSERT		{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	KEYWORD_UNDELETE	{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	KEYWORD_UPDATE		{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } |
	KEYWORD_UPSERT		{ $$ = Node(Tokens.ProductionNonReservedIdentifier, $1); } ;

Name :					
	SimpleName			{ $$ = Node(Tokens.ProductionName, $1); } |
	QualifiedName		{ $$ = Node(Tokens.ProductionName, $1); } ;
											
SimpleName :			
	IDENTIFIER				{ $$ = Node(Tokens.ProductionSimpleName, $1); } |
	NonReservedIdentifier	{ $$ = Node(Tokens.ProductionSimpleName, $1); } ;

QualifiedName :			
	Name SEPARATOR_DOT SimpleName		{ $$ = Node(Tokens.ProductionQualifiedName, $1, $2, $3); } |
	Name SEPARATOR_DOT KEYWORD_CLASS	{ $$ = Node(Tokens.ProductionQualifiedName, $1, $2, $3); } |
	Name SEPARATOR_DOT KEYWORD_NEW		{ $$ = Node(Tokens.ProductionQualifiedName, $1, $2, $3); } |
	Name SEPARATOR_DOT PrimitiveType	{ $$ = Node(Tokens.ProductionQualifiedName, $1, $2, $3); } ;

/* Packages ==============================================================================================================*/

CompilationUnit :		
	TypeDeclaration		{ $$ = Node(Tokens.ProductionCompilationUnit, $1); } |
	TriggerDeclaration	{ $$ = Node(Tokens.ProductionCompilationUnit, $1); } ;											
											
TypeDeclaration :		
	ClassDeclaration				{ $$ = Node(Tokens.ProductionTypeDeclaration, $1); } |
	AnnotatedClassDeclaration 		{ $$ = Node(Tokens.ProductionTypeDeclaration, $1); } |
	InterfaceDeclaration			{ $$ = Node(Tokens.ProductionTypeDeclaration, $1); } |
	AnnotatedInterfaceDeclaration	{ $$ = Node(Tokens.ProductionTypeDeclaration, $1); } |
	EnumDeclaration					{ $$ = Node(Tokens.ProductionTypeDeclaration, $1); } |
	AnnotatedEnumDeclaration		{ $$ = Node(Tokens.ProductionTypeDeclaration, $1); } ;

Modifiers :				
	Modifier			{ $$ = Node(Tokens.ProductionModifiers, $1); } |
	Modifiers Modifier	{ $$ = Node(Tokens.ProductionModifiers, $1, $2); } ;
											
Modifier :				
	KEYWORD_PUBLIC            { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_PROTECTED         { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_PRIVATE           { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_ABSTRACT          { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_STATIC            { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_GLOBAL            { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_OVERRIDE          { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_VIRTUAL           { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_TESTMETHOD        { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_WITHSHARING       { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_WITHOUTSHARING    { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_TRANSIENT		  { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_FINAL             { $$ = Node(Tokens.ProductionModifier, $1); } |
	KEYWORD_WEBSERVICE		  { $$ = Node(Tokens.ProductionModifier, $1); } ;

/* Class Declarations ====================================================================================================*/

AnnotatedClassDeclaration :		
	Annotations ClassDeclaration  { $$ = Node(Tokens.ProductionAnnotatedClassDeclaration, $1, $2); } ;

ClassDeclaration :		
				KEYWORD_CLASS SimpleName 					ClassBody { $$ = Node(Tokens.ProductionClassDeclaration, $1, $2, $3); } |
				KEYWORD_CLASS SimpleName 		Interfaces	ClassBody { $$ = Node(Tokens.ProductionClassDeclaration, $1, $2, $3, $4); } |
				KEYWORD_CLASS SimpleName Super 				ClassBody { $$ = Node(Tokens.ProductionClassDeclaration, $1, $2, $3, $4); } |
				KEYWORD_CLASS SimpleName Super	Interfaces	ClassBody { $$ = Node(Tokens.ProductionClassDeclaration, $1, $2, $3, $4, $5); } |
	Modifiers	KEYWORD_CLASS SimpleName 					ClassBody { $$ = Node(Tokens.ProductionClassDeclaration, $1, $2, $3, $4); } |
	Modifiers	KEYWORD_CLASS SimpleName 		Interfaces 	ClassBody { $$ = Node(Tokens.ProductionClassDeclaration, $1, $2, $3, $4, $5); } |
	Modifiers	KEYWORD_CLASS SimpleName Super				ClassBody { $$ = Node(Tokens.ProductionClassDeclaration, $1, $2, $3, $4, $5); } |
	Modifiers	KEYWORD_CLASS SimpleName Super	Interfaces	ClassBody { $$ = Node(Tokens.ProductionClassDeclaration, $1, $2, $3, $4, $5, $6); } ;										
											
Super :					
	KEYWORD_EXTENDS ReferenceType 			{ $$ = Node(Tokens.ProductionSuper, $1, $2); } ;

Interfaces :			
	KEYWORD_IMPLEMENTS InterfaceTypeList { $$ = Node(Tokens.ProductionInterfaces, $1, $2); } ;

InterfaceTypeList :		
	ReferenceType									{ $$ = Node(Tokens.ProductionInterfaceTypeList, $1); } |
	InterfaceTypeList SEPARATOR_COMMA ReferenceType	{ $$ = Node(Tokens.ProductionInterfaceTypeList, $1, $2, $3); } ;
											
ClassBody :				
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT 							{ $$ = Node(Tokens.ProductionClassBody, $1, $2); } |
	SEPARATOR_BRACE_LEFT ClassBodyDeclarations SEPARATOR_BRACE_RIGHT	{ $$ = Node(Tokens.ProductionClassBody, $1, $2, $3); } ;
											
ClassBodyDeclarations :	
	ClassBodyDeclaration 						{ $$ = Node(Tokens.ProductionClassBodyDeclarations, $1); } |
	ClassBodyDeclarations ClassBodyDeclaration	{ $$ = Node(Tokens.ProductionClassBodyDeclarations, $1, $2); } ;
											
ClassBodyDeclaration :	
	ClassMemberDeclaration	{ $$ = Node(Tokens.ProductionClassBodyDeclaration, $1); } |
	StaticInitializer		{ $$ = Node(Tokens.ProductionClassBodyDeclaration, $1); } |
	ConstructorDeclaration	{ $$ = Node(Tokens.ProductionClassBodyDeclaration, $1); } ;
											
ClassMemberDeclaration :	
	FieldDeclaration			{ $$ = Node(Tokens.ProductionClassMemberDeclaration, $1); } |
	AnnotatedFieldDeclaration	{ $$ = Node(Tokens.ProductionClassMemberDeclaration, $1); } |
	MethodDeclaration			{ $$ = Node(Tokens.ProductionClassMemberDeclaration, $1); } |
	AnnotatedMethodDeclaration	{ $$ = Node(Tokens.ProductionClassMemberDeclaration, $1); } |
	PropertyDeclaration			{ $$ = Node(Tokens.ProductionClassMemberDeclaration, $1); } |
	TypeDeclaration				{ $$ = Node(Tokens.ProductionClassMemberDeclaration, $1); } ;
	
											
/* Field Declarations ====================================================================================================*/

AnnotatedFieldDeclaration :	
	Annotations FieldDeclaration	{ $$ = Node(Tokens.ProductionAnnotatedFieldDeclaration, $1, $2); } ;

FieldDeclaration :		
	Type VariableDeclarators SEPARATOR_SEMICOLON			{ $$ = Node(Tokens.ProductionFieldDeclaration, $1, $2, $3); } |
	Modifiers Type VariableDeclarators SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionFieldDeclaration, $1, $2, $3, $4); } |
	error													{ Error(Tokens.ProductionFieldDeclaration, "Invalid field declaration."); } ;

VariableDeclarators :	
	VariableDeclarator										{ $$ = Node(Tokens.ProductionVariableDeclarators, $1); } |
	VariableDeclarators SEPARATOR_COMMA VariableDeclarator	{ $$ = Node(Tokens.ProductionVariableDeclarators, $1, $2, $3); } ;
											
VariableDeclarator :	
	VariableDeclaratorId											{ $$ = Node(Tokens.ProductionVariableDeclarator, $1); } |
	VariableDeclaratorId OPERATOR_ASSIGNMENT VariableInitializer	{ $$ = Node(Tokens.ProductionVariableDeclarator, $1, $2, $3); } ;
											
VariableDeclaratorId :	
	SimpleName															{ $$ = Node(Tokens.ProductionVariableDeclaratorId, $1); } ;
											
VariableInitializer :	
	Expression			{ $$ = Node(Tokens.ProductionVariableInitializer, $1); } |
	ArrayInitializer	{ $$ = Node(Tokens.ProductionVariableInitializer, $1); } ;
											
/* Trigger Declarations ==================================================================================================*/

TriggerDeclaration :	
	TriggerHeader MethodBody { $$ = Node(Tokens.ProductionTriggerDeclaration, $1, $2); } ;

TriggerHeader :			
	KEYWORD_TRIGGER SimpleName KEYWORD_ON SimpleName SEPARATOR_PARENTHESES_LEFT TriggerEvents SEPARATOR_PARENTHESES_RIGHT  { $$ = Node(Tokens.ProductionTriggerHeader, $1, $2, $3, $4, $5, $6, $7); } ;
				
TriggerEvents :			
	TriggerEvent								{ $$ = Node(Tokens.ProductionTriggerEvents, $1); } |
	TriggerEvents SEPARATOR_COMMA TriggerEvent	{ $$ = Node(Tokens.ProductionTriggerEvents, $1, $2, $3); } ;
				
TriggerEvent :			
	KEYWORD_BEFORE KEYWORD_INSERT	{ $$ = Node(Tokens.ProductionTriggerEvent, $1, $2); } |											
	KEYWORD_BEFORE KEYWORD_UPDATE	{ $$ = Node(Tokens.ProductionTriggerEvent, $1, $2); } |	
	KEYWORD_BEFORE KEYWORD_DELETE	{ $$ = Node(Tokens.ProductionTriggerEvent, $1, $2); } |	
	KEYWORD_AFTER KEYWORD_INSERT	{ $$ = Node(Tokens.ProductionTriggerEvent, $1, $2); } |											
	KEYWORD_AFTER KEYWORD_UPDATE	{ $$ = Node(Tokens.ProductionTriggerEvent, $1, $2); } |	
	KEYWORD_AFTER KEYWORD_DELETE	{ $$ = Node(Tokens.ProductionTriggerEvent, $1, $2); } |	
	KEYWORD_AFTER KEYWORD_UNDELETE	{ $$ = Node(Tokens.ProductionTriggerEvent, $1, $2); } ;
											
/* Property Declarations =================================================================================================*/

PropertyDeclaration :	
	Type SimpleName SEPARATOR_BRACE_LEFT AccessorDeclarations SEPARATOR_BRACE_RIGHT				{ $$ = Node(Tokens.ProductionPropertyDeclaration, $1, $2, $3, $4, $5); } |
	Modifiers Type SimpleName SEPARATOR_BRACE_LEFT AccessorDeclarations SEPARATOR_BRACE_RIGHT	{ $$ = Node(Tokens.ProductionPropertyDeclaration, $1, $2, $3, $4, $5, $6); } ;

AccessorDeclarations :	
	GetAccessorDeclaration							{ $$ = Node(Tokens.ProductionAccessorDeclarations, $1); } |
	GetAccessorDeclaration SetAccessorDeclaration	{ $$ = Node(Tokens.ProductionAccessorDeclarations, $1, $2); } |
	SetAccessorDeclaration							{ $$ = Node(Tokens.ProductionAccessorDeclarations, $1); } |
	SetAccessorDeclaration GetAccessorDeclaration	{ $$ = Node(Tokens.ProductionAccessorDeclarations, $1, $2); } ;

GetAccessorDeclaration :	
	KEYWORD_GET AccessorBody			{ $$ = Node(Tokens.ProductionGetAccessorDeclaration, $1, $2); } |
	Modifiers KEYWORD_GET AccessorBody	{ $$ = Node(Tokens.ProductionGetAccessorDeclaration, $1, $2, $3); } ;

SetAccessorDeclaration :	
	KEYWORD_SET AccessorBody			{ $$ = Node(Tokens.ProductionSetAccessorDeclaration, $1, $2); } |
	Modifiers KEYWORD_SET AccessorBody	{ $$ = Node(Tokens.ProductionSetAccessorDeclaration, $1, $2, $3); } ;
											
AccessorBody :			
	Block					{ $$ = Node(Tokens.ProductionAccessorBody, $1); } |
	SEPARATOR_SEMICOLON 	{ $$ = Node(Tokens.ProductionAccessorBody, $1); } ;
											
/* Method Declarations ===================================================================================================*/

AnnotatedMethodDeclaration :	
	Annotations MethodDeclaration { $$ = Node(Tokens.ProductionAnnotatedMethodDeclaration, $1, $2); } ;

MethodDeclaration :				
	MethodHeader MethodBody	{ $$ = Node(Tokens.ProductionMethodDeclaration, $1, $2); } ;

MethodHeader :			
	Modifiers Type MethodDeclarator				{ $$ = Node(Tokens.ProductionMethodHeader, $1, $2, $3); } |
	Type MethodDeclarator						{ $$ = Node(Tokens.ProductionMethodHeader, $1, $2); } |
	Modifiers KEYWORD_VOID MethodDeclarator		{ $$ = Node(Tokens.ProductionMethodHeader, $1, $2, $3); } |
	KEYWORD_VOID MethodDeclarator				{ $$ = Node(Tokens.ProductionMethodHeader, $1, $2); } ;
											
MethodDeclarator :		
	SimpleName SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT						{ $$ = Node(Tokens.ProductionMethodDeclarator, $1, $2, $3); } |
	SimpleName SEPARATOR_PARENTHESES_LEFT FormalParameterList SEPARATOR_PARENTHESES_RIGHT   { $$ = Node(Tokens.ProductionMethodDeclarator, $1, $2, $3, $4); } ;

FormalParameterList :	
	FormalParameter 									{ $$ = Node(Tokens.ProductionFormalParameterList, $1); } |
	FormalParameterList SEPARATOR_COMMA FormalParameter	{ $$ = Node(Tokens.ProductionFormalParameterList, $1, $2, $3); } ;
											
FormalParameter :		
	Type VariableDeclaratorId	{ $$ = Node(Tokens.ProductionFormalParameter, $1, $2); } ;

MethodBody :			
	Block				{ $$ = Node(Tokens.ProductionMethodBody, $1); } |
	SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionMethodBody, $1); } ;
											
/* Static Initializers ===================================================================================================*/

StaticInitializer :		
	KEYWORD_STATIC Block  { $$ = Node(Tokens.ProductionStaticInitializer, $1, $2); } ;

/* Constructor Declarations ==============================================================================================*/

ConstructorDeclaration :	
	ConstructorDeclarator ConstructorBody			{ $$ = Node(Tokens.ProductionConstructorDeclaration, $1, $2); } |
	Modifiers ConstructorDeclarator ConstructorBody	{ $$ = Node(Tokens.ProductionConstructorDeclaration, $1, $2, $3); } ;
											
ConstructorDeclarator :	
	Name SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT						{ $$ = Node(Tokens.ProductionConstructorDeclarator, $1, $2, $3); } |
	Name SEPARATOR_PARENTHESES_LEFT FormalParameterList SEPARATOR_PARENTHESES_RIGHT	{ $$ = Node(Tokens.ProductionConstructorDeclarator, $1, $2, $3, $4); } ;
											
ConstructorBody :		
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT													{ $$ = Node(Tokens.ProductionConstructorBody, $1, $2); } |
	SEPARATOR_BRACE_LEFT ExplicitConstructorInvocation SEPARATOR_BRACE_RIGHT					{ $$ = Node(Tokens.ProductionConstructorBody, $1, $2, $3); } |
	SEPARATOR_BRACE_LEFT BlockStatements SEPARATOR_BRACE_RIGHT									{ $$ = Node(Tokens.ProductionConstructorBody, $1, $2, $3); } |
	SEPARATOR_BRACE_LEFT ExplicitConstructorInvocation BlockStatements SEPARATOR_BRACE_RIGHT	{ $$ = Node(Tokens.ProductionConstructorBody, $1, $2, $3, $4); } ;
											
ExplicitConstructorInvocation :		
	KEYWORD_THIS SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON					{ $$ = Node(Tokens.ProductionExplicitConstructorInvocation, $1, $2, $3, $4); } |
	KEYWORD_THIS SEPARATOR_PARENTHESES_LEFT ArgumentList SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionExplicitConstructorInvocation, $1, $2, $3, $4, $5); } |
	KEYWORD_SUPER SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON				{ $$ = Node(Tokens.ProductionExplicitConstructorInvocation, $1, $2, $3, $4); } |
	KEYWORD_SUPER SEPARATOR_PARENTHESES_LEFT ArgumentList SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionExplicitConstructorInvocation, $1, $2, $3, $4, $5); } ;
											
/* Interface Declarations ================================================================================================*/

AnnotatedInterfaceDeclaration :		
	Annotations InterfaceDeclaration  { $$ = Node(Tokens.ProductionAnnotatedInterfaceDeclaration, $1, $2); } ;

InterfaceDeclaration :	
				KEYWORD_INTERFACE SimpleName 					InterfaceBody	{ $$ = Node(Tokens.ProductionInterfaceDeclaration, $1, $2, $3); } |
                KEYWORD_INTERFACE SimpleName ExtendsInterfaces	InterfaceBody	{ $$ = Node(Tokens.ProductionInterfaceDeclaration, $1, $2, $3, $4); } |
	Modifiers 	KEYWORD_INTERFACE SimpleName 					InterfaceBody	{ $$ = Node(Tokens.ProductionInterfaceDeclaration, $1, $2, $3, $4); } |
	Modifiers 	KEYWORD_INTERFACE SimpleName ExtendsInterfaces	InterfaceBody	{ $$ = Node(Tokens.ProductionInterfaceDeclaration, $1, $2, $3, $4, $5); } ;

ExtendsInterfaces :		
	KEYWORD_EXTENDS Name					{ $$ = Node(Tokens.ProductionExtendsInterfaces, $1, $2); } |
	ExtendsInterfaces SEPARATOR_COMMA Name	{ $$ = Node(Tokens.ProductionExtendsInterfaces, $1, $2, $3); } ;
											
InterfaceBody :			
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT								{ $$ = Node(Tokens.ProductionInterfaceBody, $1, $2); } |
	SEPARATOR_BRACE_LEFT InterfaceMemberDeclarations SEPARATOR_BRACE_RIGHT	{ $$ = Node(Tokens.ProductionInterfaceBody, $1, $2, $3); } ;
											
InterfaceMemberDeclarations :	
	InterfaceMemberDeclaration								{ $$ = Node(Tokens.ProductionInterfaceMemberDeclarations, $1); } |
	InterfaceMemberDeclarations InterfaceMemberDeclaration	{ $$ = Node(Tokens.ProductionInterfaceMemberDeclarations, $1, $2); } ;
											
InterfaceMemberDeclaration :	
	AbstractMethodDeclaration	{ $$ = Node(Tokens.ProductionInterfaceMemberDeclaration, $1); } ;

AbstractMethodDeclaration :		
	MethodHeader SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionAbstractMethodDeclaration, $1, $2); } ;

/* Enum Declarations =====================================================================================================*/

AnnotatedEnumDeclaration :	
	Annotations EnumDeclaration { $$ = Node(Tokens.ProductionAnnotatedEnumDeclaration, $1, $2); } ;

EnumDeclaration :		
	KEYWORD_ENUM SimpleName EnumBody 			{ $$ = Node(Tokens.ProductionEnumDeclaration, $1, $2, $3); } |
	Modifiers KEYWORD_ENUM SimpleName EnumBody 	{ $$ = Node(Tokens.ProductionEnumDeclaration, $1, $2, $3, $4); } ;

EnumBody :				
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT 							{ $$ = Node(Tokens.ProductionEnumBody, $1, $2); } |
	SEPARATOR_BRACE_LEFT EnumMemberDeclarations SEPARATOR_BRACE_RIGHT 	{ $$ = Node(Tokens.ProductionEnumBody, $1, $2, $3); } ;

EnumMemberDeclarations :	
	EnumMemberDeclaration											{ $$ = Node(Tokens.ProductionEnumMemberDeclarations, $1); } |
	EnumMemberDeclarations SEPARATOR_COMMA EnumMemberDeclaration 	{ $$ = Node(Tokens.ProductionEnumMemberDeclarations, $1, $2, $3); } ;
											
EnumMemberDeclaration :		
	SimpleName  { $$ = Node(Tokens.ProductionEnumMemberDeclaration, $1); };

/* Arrays ================================================================================================================*/

ArrayInitializer :		
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT						{ $$ = Node(Tokens.ProductionArrayInitializer, $1, $2); } |
	SEPARATOR_BRACE_LEFT VariableInitializers SEPARATOR_BRACE_RIGHT	{ $$ = Node(Tokens.ProductionArrayInitializer, $1, $2, $3); } ;
											
VariableInitializers :	
	VariableInitializer											{ $$ = Node(Tokens.ProductionVariableInitializers, $1); } |
	VariableInitializers SEPARATOR_COMMA VariableInitializer	{ $$ = Node(Tokens.ProductionVariableInitializers, $1, $2, $3); } ;

/* Blocks and Statements =================================================================================================*/

Block :					
	SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT					{ $$ = Node(Tokens.ProductionBlock, $1, $2); } |
	SEPARATOR_BRACE_LEFT BlockStatements SEPARATOR_BRACE_RIGHT	{ $$ = Node(Tokens.ProductionBlock, $1, $2, $3); } ;

BlockStatements :		
	BlockStatement					{ $$ = Node(Tokens.ProductionBlockStatements, $1); } |
	BlockStatements BlockStatement	{ $$ = Node(Tokens.ProductionBlockStatements, $1, $2); } ;
											
BlockStatement :		
	LocalVariableDeclarationStatement	{ $$ = Node(Tokens.ProductionBlockStatement, $1); } |
	Statement							{ $$ = Node(Tokens.ProductionBlockStatement, $1); } ;
											
LocalVariableDeclarationStatement :		
	LocalVariableDeclaration SEPARATOR_SEMICOLON				{ $$ = Node(Tokens.ProductionLocalVariableDeclarationStatement, $1, $2); } |
	Modifiers LocalVariableDeclaration SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionLocalVariableDeclarationStatement, $1, $2, $3); } ;

LocalVariableDeclaration :	
	Type VariableDeclarators	{ $$ = Node(Tokens.ProductionLocalVariableDeclaration, $1, $2); } ;											

Statement :				
	StatementWithoutTrailingSubstatement	{ $$ = Node(Tokens.ProductionStatement, $1); } |
	IfThenStatement							{ $$ = Node(Tokens.ProductionStatement, $1); } |
	IfThenElseStatement						{ $$ = Node(Tokens.ProductionStatement, $1); } |
	WhileStatement							{ $$ = Node(Tokens.ProductionStatement, $1); } |
	ForStatement							{ $$ = Node(Tokens.ProductionStatement, $1); } |
	ForEachStatement						{ $$ = Node(Tokens.ProductionStatement, $1); } ;											
											
StatementNoShortIf :	
	StatementWithoutTrailingSubstatement	{ $$ = Node(Tokens.ProductionStatementNoShortIf, $1); } |
	IfThenElseStatementNoShortIf			{ $$ = Node(Tokens.ProductionStatementNoShortIf, $1); } |
	WhileStatementNoShortIf					{ $$ = Node(Tokens.ProductionStatementNoShortIf, $1); } |
	ForStatementNoShortIf					{ $$ = Node(Tokens.ProductionStatementNoShortIf, $1); } |
	ForEachStatementNoShortIf				{ $$ = Node(Tokens.ProductionStatementNoShortIf, $1); } ;
											
StatementWithoutTrailingSubstatement :		
	Block								{ $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } |
	LocalVariableDeclarationStatement   { $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } |
	EmptyStatement						{ $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } |
	ExpressionStatement					{ $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } |
	DoStatement							{ $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } |
	BreakStatement						{ $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } |
	ContinueStatement					{ $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } |
	ReturnStatement						{ $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } |
	ThrowStatement						{ $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } |
	TryStatement						{ $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } |
	DMLStatement						{ $$ = Node(Tokens.ProductionStatementWithoutTrailingSubstatement, $1); } ;
											
EmptyStatement :		
	SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionEmptyStatement, $1); } ;

ExpressionStatement :	
	StatementExpression SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionExpressionStatement, $1, $2); } |
	MethodInvocation Block					{ $$ = Node(Tokens.ProductionExpressionStatement, $1, $2); } |
	error SEPARATOR_SEMICOLON				{ Error(Tokens.ProductionExpressionStatement, "Invalid expression."); } |
	error									{ Error(Tokens.ProductionExpressionStatement, "';' expected."); } ;

StatementExpression :	
	Assignment						{ $$ = Node(Tokens.ProductionStatementExpression, $1); } |
	PreIncrementExpression			{ $$ = Node(Tokens.ProductionStatementExpression, $1); } |
	PreDecrementExpression			{ $$ = Node(Tokens.ProductionStatementExpression, $1); } |
	PostIncrementExpression			{ $$ = Node(Tokens.ProductionStatementExpression, $1); } |
	PostDecrementExpression			{ $$ = Node(Tokens.ProductionStatementExpression, $1); } |
	MethodInvocation				{ $$ = Node(Tokens.ProductionStatementExpression, $1); } |
	ClassInstanceCreationExpression	{ $$ = Node(Tokens.ProductionStatementExpression, $1); } ;
											
IfThenStatement :				
	KEYWORD_IF SEPARATOR_PARENTHESES_LEFT Expression SEPARATOR_PARENTHESES_RIGHT Statement	{ $$ = Node(Tokens.ProductionIfThenStatement, $1, $2, $3, $4, $5); };

IfThenElseStatement :			
	KEYWORD_IF SEPARATOR_PARENTHESES_LEFT Expression SEPARATOR_PARENTHESES_RIGHT StatementNoShortIf KEYWORD_ELSE Statement	{ $$ = Node(Tokens.ProductionIfThenElseStatement, $1, $2, $3, $4, $5, $6); } ;

IfThenElseStatementNoShortIf :	
	KEYWORD_IF SEPARATOR_PARENTHESES_LEFT Expression SEPARATOR_PARENTHESES_RIGHT StatementNoShortIf KEYWORD_ELSE StatementNoShortIf	{ $$ = Node(Tokens.ProductionIfThenElseStatementNoShortIf, $1, $2, $3, $4, $5, $6, $7); } ;

WhileStatement :				
	KEYWORD_WHILE SEPARATOR_PARENTHESES_LEFT Expression SEPARATOR_PARENTHESES_RIGHT Statement	{ $$ = Node(Tokens.ProductionWhileStatement, $1, $2, $3, $4, $5); } ;

WhileStatementNoShortIf :		
	KEYWORD_WHILE SEPARATOR_PARENTHESES_LEFT Expression SEPARATOR_PARENTHESES_RIGHT StatementNoShortIf	{ $$ = Node(Tokens.ProductionWhileStatementNoShortIf, $1, $2, $3, $4, $5); } ;

DoStatement :					
	KEYWORD_DO Statement KEYWORD_WHILE SEPARATOR_PARENTHESES_LEFT Expression SEPARATOR_PARENTHESES_RIGHT SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionDoStatement, $1, $2, $3, $4, $5, $6, $7); } ;

ForEachStatement :				
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT Type SimpleName SEPARATOR_COLON Expression SEPARATOR_PARENTHESES_RIGHT Statement 	{ $$ = Node(Tokens.ProductionForEachStatement, $1, $2, $3, $4, $5, $6, $7, $8); } ;

ForEachStatementNoShortIf :		
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT Type SimpleName SEPARATOR_COLON Expression SEPARATOR_PARENTHESES_RIGHT ForStatementNoShortIf 	{ $$ = Node(Tokens.ProductionForEachStatementNoShortIf, $1, $2, $3, $4, $5, $6, $7, $8); } ;

ForStatement :					
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT ForInit	SEPARATOR_SEMICOLON ForExpression	SEPARATOR_SEMICOLON ForUpdate	SEPARATOR_PARENTHESES_RIGHT Statement	{ $$ = Node(Tokens.ProductionForStatement, $1, $2, $3, $4, $5, $6, $7, $8, $9); } ;
											
ForStatementNoShortIf :			
	KEYWORD_FOR SEPARATOR_PARENTHESES_LEFT ForInit	SEPARATOR_SEMICOLON ForExpression	SEPARATOR_SEMICOLON ForUpdate	SEPARATOR_PARENTHESES_RIGHT StatementNoShortIf	{ $$ = Node(Tokens.ProductionForStatementNoShortIf, $1, $2, $3, $4, $5, $6, $7, $8, $9); } ;
											
ForInit :				
								{ $$ = Node(Tokens.ProductionForInit); } |
	StatementExpressionList		{ $$ = Node(Tokens.ProductionForInit, $1); } |
	LocalVariableDeclaration	{ $$ = Node(Tokens.ProductionForInit, $1); } ;
											
ForExpression :			
				{ $$ = Node(Tokens.ProductionForExpression); } |
	Expression	{ $$ = Node(Tokens.ProductionForExpression, $1); } ;
											
ForUpdate :				
							{ $$ = Node(Tokens.ProductionForUpdate); } |
	StatementExpressionList	{ $$ = Node(Tokens.ProductionForUpdate, $1); } ;

StatementExpressionList :	
	StatementExpression											{ $$ = Node(Tokens.ProductionStatementExpressionList, $1); } |
	StatementExpressionList SEPARATOR_COMMA StatementExpression	{ $$ = Node(Tokens.ProductionStatementExpressionList, $1, $2, $3); } ;
											
BreakStatement :		
	KEYWORD_BREAK SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionBreakStatement, $1, $2); } ;

ContinueStatement :		
	KEYWORD_CONTINUE SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionContinueStatement, $1, $2); } ;

ReturnStatement :		
	KEYWORD_RETURN SEPARATOR_SEMICOLON				{ $$ = Node(Tokens.ProductionReturnStatement, $1, $2); } |
	KEYWORD_RETURN Expression SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionReturnStatement, $1, $2, $3); } ;
											
ThrowStatement :		
	KEYWORD_THROW Expression SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionThrowStatement, $1, $2, $3); } ;

TryStatement :			
	KEYWORD_TRY Block Catches			{ $$ = Node(Tokens.ProductionTryStatement, $1, $2, $3); } |
	KEYWORD_TRY Block Catches Finally	{ $$ = Node(Tokens.ProductionTryStatement, $1, $2, $3, $4); } |
	KEYWORD_TRY Block Finally			{ $$ = Node(Tokens.ProductionTryStatement, $1, $2, $3); } ;
											
Catches :				
	CatchClause			{ $$ = Node(Tokens.ProductionCatches, $1); } |
	Catches CatchClause	{ $$ = Node(Tokens.ProductionCatches, $1, $2); } ;
											
CatchClause :			
	KEYWORD_CATCH SEPARATOR_PARENTHESES_LEFT FormalParameter SEPARATOR_PARENTHESES_RIGHT Block	{ $$ = Node(Tokens.ProductionCatchClause, $1, $2, $3, $4, $5); } ;

Finally :				
	KEYWORD_FINALLY Block	{ $$ = Node(Tokens.ProductionFinally, $1, $2); } ;

DMLStatement :			
	KEYWORD_INSERT Expression SEPARATOR_SEMICOLON  	{ $$ = Node(Tokens.ProductionDMLStatement, $1, $2, $3); } |
	KEYWORD_UPDATE Expression SEPARATOR_SEMICOLON  	{ $$ = Node(Tokens.ProductionDMLStatement, $1, $2, $3); } |
	KEYWORD_UPSERT Expression SEPARATOR_SEMICOLON  	{ $$ = Node(Tokens.ProductionDMLStatement, $1, $2, $3); } |
	KEYWORD_DELETE Expression SEPARATOR_SEMICOLON  	{ $$ = Node(Tokens.ProductionDMLStatement, $1, $2, $3); } |
	KEYWORD_UNDELETE Expression SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionDMLStatement, $1, $2, $3); } |
	KEYWORD_MERGE Expression SEPARATOR_SEMICOLON	{ $$ = Node(Tokens.ProductionDMLStatement, $1, $2, $3); } ;

/* Expressions ===========================================================================================================*/

Primary :
	PrimaryNoNewArray			{ $$ = Node(Tokens.ProductionPrimary, $1); } |
	ArrayCreationExpression		{ $$ = Node(Tokens.ProductionPrimary, $1); } ;

PrimaryNoNewArray :		
	Literal																{ $$ = Node(Tokens.ProductionPrimaryNoNewArray, $1); } |
	KEYWORD_THIS														{ $$ = Node(Tokens.ProductionPrimaryNoNewArray, $1); } |
	SEPARATOR_PARENTHESES_LEFT Expression SEPARATOR_PARENTHESES_RIGHT	{ $$ = Node(Tokens.ProductionPrimaryNoNewArray, $1, $2, $3); } |
	ClassInstanceCreationExpression										{ $$ = Node(Tokens.ProductionPrimaryNoNewArray, $1); } |
	FieldAccess															{ $$ = Node(Tokens.ProductionPrimaryNoNewArray, $1); } |
	MethodInvocation													{ $$ = Node(Tokens.ProductionPrimaryNoNewArray, $1); } |
	ArrayAccess															{ $$ = Node(Tokens.ProductionPrimaryNoNewArray, $1); } |
	SOQL																{ $$ = Node(Tokens.ProductionPrimaryNoNewArray, $1); } |
	SOSL																{ $$ = Node(Tokens.ProductionPrimaryNoNewArray, $1); } ;
											
ClassInstanceCreationExpression :	
	KEYWORD_NEW Type SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT					{ $$ = Node(Tokens.ProductionClassInstanceCreationExpression, $1, $2, $3, $4); } |
	KEYWORD_NEW Type SEPARATOR_PARENTHESES_LEFT ArgumentList SEPARATOR_PARENTHESES_RIGHT	{ $$ = Node(Tokens.ProductionClassInstanceCreationExpression, $1, $2, $3, $4, $5); } |
	KEYWORD_NEW Type SEPARATOR_BRACE_LEFT SEPARATOR_BRACE_RIGHT								{ $$ = Node(Tokens.ProductionClassInstanceCreationExpression, $1, $2, $3, $4); } |
	KEYWORD_NEW Type SEPARATOR_BRACE_LEFT ArgumentList SEPARATOR_BRACE_RIGHT   { $$ = Node(Tokens.ProductionClassInstanceCreationExpression, $1, $2, $3, $4, $5); } |
	KEYWORD_NEW Type SEPARATOR_BRACE_LEFT MapArgumentAssignmentList SEPARATOR_BRACE_RIGHT   { $$ = Node(Tokens.ProductionClassInstanceCreationExpression, $1, $2, $3, $4, $5); } ;

MapArgumentAssignmentList :			
	MapArgumentAssignment											{ $$ = Node(Tokens.ProductionMapArgumentAssignmentList, $1); } |
	MapArgumentAssignmentList SEPARATOR_COMMA MapArgumentAssignment	{ $$ = Node(Tokens.ProductionMapArgumentAssignmentList, $1, $2, $3); } ;

MapArgumentAssignment :				
	Expression OPERATOR_ASSIGNMENT_MAP Expression  { $$ = Node(Tokens.ProductionMapArgumentAssignment, $1, $2, $3); } ;
											
ArgumentList :						
	Expression								{ $$ = Node(Tokens.ProductionArgumentList, $1); } |
	ArgumentList SEPARATOR_COMMA Expression	{ $$ = Node(Tokens.ProductionArgumentList, $1, $2, $3); } ;
											
ArrayCreationExpression :			
	KEYWORD_NEW Type DimExprs																									{ $$ = Node(Tokens.ProductionArrayCreationExpression, $1, $2, $3); } |
	KEYWORD_NEW Type DimExprs Dims																								{ $$ = Node(Tokens.ProductionArrayCreationExpression, $1, $2, $3, $4); } ;
											
DimExprs :							
	DimExpr				{ $$ = Node(Tokens.ProductionDimExprs, $1); } |
	DimExprs DimExpr	{ $$ = Node(Tokens.ProductionDimExprs, $1, $2); } ;
											
DimExpr :							
	SEPARATOR_BRACKET_LEFT Expression SEPARATOR_BRACKET_RIGHT	{ $$ = Node(Tokens.ProductionDimExpr, $1, $2, $3); } ;

Dims :								
	SEPARATOR_BRACKET_EMPTY			{ $$ = Node(Tokens.ProductionDims, $1); } |
	Dims SEPARATOR_BRACKET_EMPTY	{ $$ = Node(Tokens.ProductionDims, $1, $2); } ;
											
FieldAccess :						
	Primary SEPARATOR_DOT SimpleName			{ $$ = Node(Tokens.ProductionFieldAccess, $1, $2, $3); } |											
	Primary SEPARATOR_DOT KEYWORD_CLASS			{ $$ = Node(Tokens.ProductionFieldAccess, $1, $2, $3); } |
	Primary SEPARATOR_DOT KEYWORD_NEW			{ $$ = Node(Tokens.ProductionFieldAccess, $1, $2, $3); } |
	Primary SEPARATOR_DOT PrimitiveType			{ $$ = Node(Tokens.ProductionFieldAccess, $1, $2, $3); } |											
	KEYWORD_SUPER SEPARATOR_DOT SimpleName		{ $$ = Node(Tokens.ProductionFieldAccess, $1, $2, $3); } |
	KEYWORD_SUPER SEPARATOR_DOT KEYWORD_CLASS	{ $$ = Node(Tokens.ProductionFieldAccess, $1, $2, $3); } |
	Primary SEPARATOR_DOT KEYWORD_NEW			{ $$ = Node(Tokens.ProductionFieldAccess, $1, $2, $3); } |
	KEYWORD_SUPER SEPARATOR_DOT PrimitiveType	{ $$ = Node(Tokens.ProductionFieldAccess, $1, $2, $3); } ;
											
Annotations :						
	Annotation				{ $$ = Node(Tokens.ProductionAnnotations, $1); } |
	Annotations Annotation	{ $$ = Node(Tokens.ProductionAnnotations, $1, $2); } ;
											
Annotation :						
	KEYWORD_ANNOTATE SimpleName																				{ $$ = Node(Tokens.ProductionAnnotation, $1, $2); } |
	KEYWORD_ANNOTATE SimpleName SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT 						{ $$ = Node(Tokens.ProductionAnnotation, $1, $2, $3, $4); } |
	KEYWORD_ANNOTATE SimpleName SEPARATOR_PARENTHESES_LEFT VariableInitializers SEPARATOR_PARENTHESES_RIGHT	{ $$ = Node(Tokens.ProductionAnnotation, $1, $2, $3, $4, $5); } ;
											
MethodInvocation :					
	Name SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT													{ $$ = Node(Tokens.ProductionMethodInvocation, $1, $2, $3); } |
	Name SEPARATOR_PARENTHESES_LEFT ArgumentList SEPARATOR_PARENTHESES_RIGHT									{ $$ = Node(Tokens.ProductionMethodInvocation, $1, $2, $3, $4); } |
	PrimitiveType SEPARATOR_DOT SimpleName SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT               { $$ = Node(Tokens.ProductionMethodInvocation, $1, $2, $3, $4, $5); } |
	PrimitiveType SEPARATOR_DOT SimpleName SEPARATOR_PARENTHESES_LEFT ArgumentList SEPARATOR_PARENTHESES_RIGHT	{ $$ = Node(Tokens.ProductionMethodInvocation, $1, $2, $3, $4, $5, $6); } |
	Primary SEPARATOR_DOT SimpleName SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT						{ $$ = Node(Tokens.ProductionMethodInvocation, $1, $2, $3, $4, $5); } |
	Primary SEPARATOR_DOT SimpleName SEPARATOR_PARENTHESES_LEFT ArgumentList SEPARATOR_PARENTHESES_RIGHT		{ $$ = Node(Tokens.ProductionMethodInvocation, $1, $2, $3, $4, $5, $6); } |
	KEYWORD_SUPER SEPARATOR_DOT SimpleName SEPARATOR_PARENTHESES_LEFT SEPARATOR_PARENTHESES_RIGHT				{ $$ = Node(Tokens.ProductionMethodInvocation, $1, $2, $3, $4, $5); } |
	KEYWORD_SUPER SEPARATOR_DOT SimpleName SEPARATOR_PARENTHESES_LEFT ArgumentList SEPARATOR_PARENTHESES_RIGHT	{ $$ = Node(Tokens.ProductionMethodInvocation, $1, $2, $3, $4, $5, $6); } ;
										
ArrayAccess :						
	Name SEPARATOR_BRACKET_LEFT Expression SEPARATOR_BRACKET_RIGHT					{ $$ = Node(Tokens.ProductionArrayAccess, $1, $2, $3, $4); } |
	PrimaryNoNewArray SEPARATOR_BRACKET_LEFT Expression SEPARATOR_BRACKET_RIGHT		{ $$ = Node(Tokens.ProductionArrayAccess, $1, $2, $3, $4); } ;
											
PostfixExpression :					
	Primary															{ $$ = Node(Tokens.ProductionPostfixExpression, $1); } |
	Name															{ $$ = Node(Tokens.ProductionPostfixExpression, $1); } |
	PostIncrementExpression											{ $$ = Node(Tokens.ProductionPostfixExpression, $1); } |
	PostDecrementExpression											{ $$ = Node(Tokens.ProductionPostfixExpression, $1); } ;
											
PostIncrementExpression :			
	PostfixExpression OPERATOR_INCREMENT	{ $$ = Node(Tokens.ProductionPostIncrementExpression, $1, $2); } ;

PostDecrementExpression :			
	PostfixExpression OPERATOR_DECREMENT	{ $$ = Node(Tokens.ProductionPostDecrementExpression, $1, $2); } ;

UnaryExpression :					
	PreIncrementExpression													{ $$ = Node(Tokens.ProductionUnaryExpression, $1); } |
	PreDecrementExpression													{ $$ = Node(Tokens.ProductionUnaryExpression, $1); } |
	OPERATOR_ADDITION UnaryExpression										{ $$ = Node(Tokens.ProductionUnaryExpression, $1, $2); } |
	OPERATOR_SUBTRACTION UnaryExpression									{ $$ = Node(Tokens.ProductionUnaryExpression, $1, $2); } |
	UnaryExpressionNotPlusMinus												{ $$ = Node(Tokens.ProductionUnaryExpression, $1); } ;
											
PreIncrementExpression :			
	OPERATOR_INCREMENT UnaryExpression	{ $$ = Node(Tokens.ProductionPreIncrementExpression, $1, $2); } ;

PreDecrementExpression :			
	OPERATOR_DECREMENT UnaryExpression	{ $$ = Node(Tokens.ProductionPreDecrementExpression, $1, $2); } ;

UnaryExpressionNotPlusMinus :       
	PostfixExpression							{ $$ = Node(Tokens.ProductionUnaryExpressionNotPlusMinus, $1); } |
	OPERATOR_LOGICAL_COMPLEMENT UnaryExpression	{ $$ = Node(Tokens.ProductionUnaryExpressionNotPlusMinus, $1, $2); } |
	CastExpression                              { $$ = Node(Tokens.ProductionUnaryExpressionNotPlusMinus, $1); } ;
											
CastExpression :					
	SEPARATOR_PARENTHESES_LEFT PrimitiveType SEPARATOR_PARENTHESES_RIGHT UnaryExpression													{ $$ = Node(Tokens.ProductionCastExpression, $1, $2, $3, $4); } |
	SEPARATOR_PARENTHESES_LEFT ReferenceType SEPARATOR_PARENTHESES_RIGHT UnaryExpressionNotPlusMinus										{ $$ = Node(Tokens.ProductionCastExpression, $1, $2, $3, $4); } |
	SEPARATOR_PARENTHESES_LEFT PrimitiveType SEPARATOR_PARENTHESES_RIGHT SEPARATOR_PARENTHESES_LEFT IDENTIFIER SEPARATOR_PARENTHESES_RIGHT  { $$ = Node(Tokens.ProductionCastExpression, $1, $2, $3, $4, $5, $6); } |
    SEPARATOR_PARENTHESES_LEFT ReferenceType SEPARATOR_PARENTHESES_RIGHT SEPARATOR_PARENTHESES_LEFT IDENTIFIER SEPARATOR_PARENTHESES_RIGHT  { $$ = Node(Tokens.ProductionCastExpression, $1, $2, $3, $4, $5, $6); } ;
											
MultiplicativeExpression :			
	UnaryExpression														{ $$ = Node(Tokens.ProductionMultiplicativeExpression, $1); } |
	MultiplicativeExpression OPERATOR_MULTIPLICATION UnaryExpression	{ $$ = Node(Tokens.ProductionMultiplicativeExpression, $1, $2, $3); } |
	MultiplicativeExpression OPERATOR_DIVISION UnaryExpression			{ $$ = Node(Tokens.ProductionMultiplicativeExpression, $1, $2, $3); } ;
											
AdditiveExpression :				
	MultiplicativeExpression											{ $$ = Node(Tokens.ProductionAdditiveExpression, $1); } |
	AdditiveExpression OPERATOR_ADDITION MultiplicativeExpression		{ $$ = Node(Tokens.ProductionAdditiveExpression, $1, $2, $3); } |
	AdditiveExpression OPERATOR_SUBTRACTION MultiplicativeExpression	{ $$ = Node(Tokens.ProductionAdditiveExpression, $1, $2, $3); } ;
											
ShiftExpression :					
	AdditiveExpression																							{ $$ = Node(Tokens.ProductionRelationalExpression, $1); } |
	ShiftExpression OPERATOR_BITWISE_SHIFT_LEFT AdditiveExpression												{ $$ = Node(Tokens.ProductionShiftExpression, $1, $2, $3); } |
	ShiftExpression OPERATOR_GREATER_THAN_A OPERATOR_GREATER_THAN_B AdditiveExpression							{ $$ = Node(Tokens.ProductionShiftExpression, $1, $2, $3, $4); } |
	ShiftExpression OPERATOR_GREATER_THAN_A OPERATOR_GREATER_THAN_B OPERATOR_GREATER_THAN_C AdditiveExpression	{ $$ = Node(Tokens.ProductionShiftExpression, $1, $2, $3, $4, $5); } ;
											
RelationalExpression :				
	ShiftExpression														{ $$ = Node(Tokens.ProductionRelationalExpression, $1); } |
	RelationalExpression RelationalOperator ShiftExpression				{ $$ = Node(Tokens.ProductionRelationalExpression, $1, $2, $3); } |
	RelationalExpression RelationalOperator ShiftExpression				{ $$ = Node(Tokens.ProductionRelationalExpression, $1, $2, $3); } |
	RelationalExpression OPERATOR_INSTANCEOF Type						{ $$ = Node(Tokens.ProductionRelationalExpression, $1, $2, $3); } ;

RelationalOperator:
	OPERATOR_LESS_THAN				{ $$ = Node(Tokens.ProductionRelationalOperator, $1); } |
	OPERATOR_GREATER_THAN			{ $$ = Node(Tokens.ProductionRelationalOperator, $1); } |
	OPERATOR_LESS_THAN_OR_EQUAL		{ $$ = Node(Tokens.ProductionRelationalOperator, $1); } |
	OPERATOR_GREATER_THAN_OR_EQUAL	{ $$ = Node(Tokens.ProductionRelationalOperator, $1); } ;
											
EqualityExpression :				
	RelationalExpression												{ $$ = Node(Tokens.ProductionRelationalExpression, $1); } |
	EqualityExpression OPERATOR_EQUALITY RelationalExpression			{ $$ = Node(Tokens.ProductionEqualityExpression, $1, $2, $3); } |
	EqualityExpression OPERATOR_EQUALITY_EXACT RelationalExpression		{ $$ = Node(Tokens.ProductionEqualityExpression, $1, $2, $3); } |
	EqualityExpression OPERATOR_INEQUALITY RelationalExpression			{ $$ = Node(Tokens.ProductionEqualityExpression, $1, $2, $3); } |										
	EqualityExpression OPERATOR_INEQUALITY_ALT RelationalExpression		{ $$ = Node(Tokens.ProductionEqualityExpression, $1, $2, $3); } |
	EqualityExpression OPERATOR_INEQUALITY_EXACT RelationalExpression	{ $$ = Node(Tokens.ProductionEqualityExpression, $1, $2, $3); } ;
											
AndExpression :						
	EqualityExpression                                      { $$ = Node(Tokens.ProductionAndExpression, $1); } |
	AndExpression OPERATOR_BITWISE_AND EqualityExpression   { $$ = Node(Tokens.ProductionAndExpression, $1, $2, $3); } ;
											
ExclusiveOrExpression :				
	AndExpression														{ $$ = Node(Tokens.ProductionExclusiveOrExpression, $1); } |
	ExclusiveOrExpression OPERATOR_BITWISE_EXCLUSIVE_OR AndExpression	{ $$ = Node(Tokens.ProductionExclusiveOrExpression, $1, $2, $3); } ;
											
InclusiveOrExpression :				
	ExclusiveOrExpression											{ $$ = Node(Tokens.ProductionInclusiveOrExpression, $1); } |
	InclusiveOrExpression OPERATOR_BITWISE_OR ExclusiveOrExpression	{ $$ = Node(Tokens.ProductionInclusiveOrExpression, $1, $2, $3); } ;
											
ConditionalAndExpression :			
	InclusiveOrExpression										{ $$ = Node(Tokens.ProductionConditionalAndExpression, $1); } |
	ConditionalAndExpression OPERATOR_AND InclusiveOrExpression	{ $$ = Node(Tokens.ProductionConditionalAndExpression, $1, $2, $3); } ;
											
ConditionalOrExpression :			
	ConditionalAndExpression																		{ $$ = Node(Tokens.ProductionConditionalOrExpression, $1); } |
	ConditionalOrExpression OPERATOR_OR ConditionalAndExpression									{ $$ = Node(Tokens.ProductionConditionalOrExpression, $1, $2, $3); } ;
											
ConditionalExpression :			
	ConditionalOrExpression                                                                         { $$ = Node(Tokens.ProductionConditionalExpression, $1); } |
	ConditionalOrExpression OPERATOR_QUESTION_MARK Expression SEPARATOR_COLON ConditionalExpression	{ $$ = Node(Tokens.ProductionConditionalExpression, $1, $2, $3, $4, $5); } ;
											
AssignmentExpression :			
	ConditionalExpression     { $$ = Node(Tokens.ProductionAssignmentExpression, $1); } |
	Assignment                { $$ = Node(Tokens.ProductionAssignmentExpression, $1); } ;
										
Assignment :					
	Name AssignmentOperator Expression			{ $$ = Node(Tokens.ProductionAssignment, $1, $2, $3); } |
	KEYWORD_ID AssignmentOperator Expression	{ $$ = Node(Tokens.ProductionAssignment, $1, $2, $3); } |
	FieldAccess AssignmentOperator Expression	{ $$ = Node(Tokens.ProductionAssignment, $1, $2, $3); } |
    ArrayAccess AssignmentOperator Expression	{ $$ = Node(Tokens.ProductionAssignment, $1, $2, $3); } ;
											
AssignmentOperator :				
	OPERATOR_ASSIGNMENT									{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_MAP								{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_ADDITION						{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_MULTIPLICATION					{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_SUBTRACTION						{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_DIVISION                        { $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_OR								{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_AND								{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_EXCLUSIVE_OR					{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_BITWISE_SHIFT_LEFT				{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT				{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } |
	OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT_UNSIGNED	{ $$ = Node(Tokens.ProductionAssignmentOperator, $1); } ;
											
Expression :
	AssignmentExpression												{ $$ = Node(Tokens.ProductionExpression, $1); } |
	SOQL																{ $$ = Node(Tokens.ProductionExpression, $1); } |
	SOSL																{ $$ = Node(Tokens.ProductionExpression, $1); } ;

%%