

%namespace SalesForceLanguage.Apex.Parser
%scannertype ApexLexer
%option stack caseInsensitive unicode codePage:utf-8

%{	
   // User code   
%}

LineTerminator			\r|\n|\r\n
InputCharacter			[^\r\n]

WhiteSpace				{LineTerminator}|[ \t\f]
CommentDoc				"/**"([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+"/"
CommentBlock			"/*"([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+"/"
CommentLine				"//"{InputCharacter}*{LineTerminator}?

Letter					[A-Za-z]
Digit					[0-9]
LetterOrDigit			{Letter}|{Digit}
Identifier				{Letter}({LetterOrDigit}*_+{LetterOrDigit})*{LetterOrDigit}*
IntegerLiteral			{Digit}+
RealLiteral				{Digit}?\.?{Digit}+
LongLiteral				{IntegerLiteral}[lL]
DoubleLiteral			{RealLiteral}[dD]?

SOQLStart				\[{WhiteSpace}*SELECT
SOSLStart				\[{WhiteSpace}*FIND

StringDelimiter			\'
StringCharacter			[^\r\n\'\\]

WithSharing				WITH{WhiteSpace}*SHARING
WithoutSharing 			WITHOUT{WhiteSpace}*SHARING

%x STRING 
%x SOQL
%x SOSL
%x SOQL_STRING
%x INNER_BRACKET

%%

%{	
	// Prolog code
	PreviousNode = yylval;

	if (_pushBackQueue.Count > 0)
	{
		yylval = _pushBackQueue.Dequeue();
		return ((int)yylval.Token);
	}
%}

<INITIAL> {

	"abstract"			{ return Symbol(Tokens.KEYWORD_ABSTRACT); }
	"after"				{ return Symbol(Tokens.KEYWORD_AFTER); }
	"@"					{ return Symbol(Tokens.KEYWORD_ANNOTATE); }    
	"before"			{ return Symbol(Tokens.KEYWORD_BEFORE); }
	"blob"				{ return Symbol(Tokens.KEYWORD_BLOB); }
	"boolean"			{ return Symbol(Tokens.KEYWORD_BOOLEAN); }
	"break"				{ return Symbol(Tokens.KEYWORD_BREAK); }
	"catch"				{ return Symbol(Tokens.KEYWORD_CATCH); }
	"class"				{ return Symbol(Tokens.KEYWORD_CLASS); }
	"continue"			{ return Symbol(Tokens.KEYWORD_CONTINUE); }
	"date"				{ return Symbol(Tokens.KEYWORD_DATE); }
	"datetime"			{ return Symbol(Tokens.KEYWORD_DATETIME); }
	"decimal"			{ return Symbol(Tokens.KEYWORD_DECIMAL); }
	"delete"			{ return Symbol(Tokens.KEYWORD_DELETE); }
	"do"				{ return Symbol(Tokens.KEYWORD_DO); }
	"double"			{ return Symbol(Tokens.KEYWORD_DOUBLE); }
	"else"				{ return Symbol(Tokens.KEYWORD_ELSE); }
	"enum"				{ return Symbol(Tokens.KEYWORD_ENUM); }
	"extends"			{ return Symbol(Tokens.KEYWORD_EXTENDS); }
	"final"				{ return Symbol(Tokens.KEYWORD_FINAL); }
	"finally"			{ return Symbol(Tokens.KEYWORD_FINALLY); }
	"for"				{ return Symbol(Tokens.KEYWORD_FOR); }
	"get"				{ return Symbol(Tokens.KEYWORD_GET); }
	"global"			{ return Symbol(Tokens.KEYWORD_GLOBAL); }
	"id"				{ return Symbol(Tokens.KEYWORD_ID); }
	"if"				{ return Symbol(Tokens.KEYWORD_IF); }
	"implements"        { return Symbol(Tokens.KEYWORD_IMPLEMENTS); }
	"insert"			{ return Symbol(Tokens.KEYWORD_INSERT); }
	"interface"			{ return Symbol(Tokens.KEYWORD_INTERFACE); }
	"integer"			{ return Symbol(Tokens.KEYWORD_INTEGER); }
	"long"				{ return Symbol(Tokens.KEYWORD_LONG); }
	"merge"				{ return Symbol(Tokens.KEYWORD_MERGE); }
	"new"				{ return Symbol(Tokens.KEYWORD_NEW); }
	"on"				{ return Symbol(Tokens.KEYWORD_ON); }
	"override"			{ return Symbol(Tokens.KEYWORD_OVERRIDE); }
	"private"			{ return Symbol(Tokens.KEYWORD_PRIVATE); }
	"protected"			{ return Symbol(Tokens.KEYWORD_PROTECTED); }
	"public"			{ return Symbol(Tokens.KEYWORD_PUBLIC); }
	"return"			{ return Symbol(Tokens.KEYWORD_RETURN); }
	"rollback"			{ return Symbol(Tokens.KEYWORD_ROLLBACK); }
	"set"				{ return Symbol(Tokens.KEYWORD_SET); }
	"static"			{ return Symbol(Tokens.KEYWORD_STATIC); }
	"string"			{ return Symbol(Tokens.KEYWORD_STRING); }
	"super"				{ return Symbol(Tokens.KEYWORD_SUPER); }
	"testmethod"        { return Symbol(Tokens.KEYWORD_TESTMETHOD); }
	"this"				{ return Symbol(Tokens.KEYWORD_THIS); }
	"throw"				{ return Symbol(Tokens.KEYWORD_THROW); }
	"transient"			{ return Symbol(Tokens.KEYWORD_TRANSIENT); }
	"trigger"			{ return Symbol(Tokens.KEYWORD_TRIGGER); }
	"try"				{ return Symbol(Tokens.KEYWORD_TRY); }
	"undelete"			{ return Symbol(Tokens.KEYWORD_UNDELETE); }
	"update"			{ return Symbol(Tokens.KEYWORD_UPDATE); }
	"upsert"			{ return Symbol(Tokens.KEYWORD_UPSERT); }
	"virtual"			{ return Symbol(Tokens.KEYWORD_VIRTUAL); }
	"void"				{ return Symbol(Tokens.KEYWORD_VOID); }
	"webservice"        { return Symbol(Tokens.KEYWORD_WEBSERVICE); }
	"while"				{ return Symbol(Tokens.KEYWORD_WHILE); }
	{WithSharing}       { return Symbol(Tokens.KEYWORD_WITHSHARING); }
	{WithoutSharing}    { return Symbol(Tokens.KEYWORD_WITHOUTSHARING); }
  
	"("					{ return Symbol(Tokens.SEPARATOR_PARENTHESES_LEFT); }
	")"					{ return Symbol(Tokens.SEPARATOR_PARENTHESES_RIGHT); }
	"{"               	{ return Symbol(Tokens.SEPARATOR_BRACE_LEFT); }
	"}"               	{ return Symbol(Tokens.SEPARATOR_BRACE_RIGHT); }
	"["               	{ return Symbol(Tokens.SEPARATOR_BRACKET_LEFT); }
	"]"               	{ return Symbol(Tokens.SEPARATOR_BRACKET_RIGHT); }
	"["{WhiteSpace}*"]"	{ return Symbol(Tokens.SEPARATOR_BRACKET_EMPTY); }
	";"               	{ return Symbol(Tokens.SEPARATOR_SEMICOLON); }
	":"					{ return Symbol(Tokens.SEPARATOR_COLON); }
	","               	{ return Symbol(Tokens.SEPARATOR_COMMA); }
	"."               	{ return Symbol(Tokens.SEPARATOR_DOT); } 
  
	"="                 { return Symbol(Tokens.OPERATOR_ASSIGNMENT); }
	"=>"				{ return Symbol(Tokens.OPERATOR_ASSIGNMENT_MAP); }
	">"                 { return Symbol(Tokens.OPERATOR_GREATER_THAN); }
	"<"                 { return Symbol(Tokens.OPERATOR_LESS_THAN); }
	"!"                 { return Symbol(Tokens.OPERATOR_LOGICAL_COMPLEMENT); }
	"?"                 { return Symbol(Tokens.OPERATOR_QUESTION_MARK); }
	"=="                { return Symbol(Tokens.OPERATOR_EQUALITY); }
	"==="               { return Symbol(Tokens.OPERATOR_EQUALITY_EXACT); }
	"<="                { return Symbol(Tokens.OPERATOR_LESS_THAN_OR_EQUAL); }
	">="                { return Symbol(Tokens.OPERATOR_GREATER_THAN_OR_EQUAL); }
	"!="                { return Symbol(Tokens.OPERATOR_INEQUALITY); }
	"<>"                { return Symbol(Tokens.OPERATOR_INEQUALITY_ALT); }
	"!=="               { return Symbol(Tokens.OPERATOR_INEQUALITY_EXACT); }
	"&&"                { return Symbol(Tokens.OPERATOR_AND); }
	"||"                { return Symbol(Tokens.OPERATOR_OR); }
	"++"                { return Symbol(Tokens.OPERATOR_INCREMENT); }
	"--"                { return Symbol(Tokens.OPERATOR_DECREMENT); }
	"+"                 { return Symbol(Tokens.OPERATOR_ADDITION); }
	"-"                 { return Symbol(Tokens.OPERATOR_SUBTRACTION); }
	"*"                 { return Symbol(Tokens.OPERATOR_MULTIPLICATION); }
	"/"                 { return Symbol(Tokens.OPERATOR_DIVISION); }
	"&"                 { return Symbol(Tokens.OPERATOR_BITWISE_AND); }
	"|"                 { return Symbol(Tokens.OPERATOR_BITWISE_OR); }
	"^"                 { return Symbol(Tokens.OPERATOR_BITWISE_EXCLUSIVE_OR); }
	"<<"                { return Symbol(Tokens.OPERATOR_BITWISE_SHIFT_LEFT); }
	">>"                { return SymbolShiftRight(); }
	">>>"				{ return SymbolShiftRightUnsigned(); }
	"+="                { return Symbol(Tokens.OPERATOR_ASSIGNMENT_ADDITION); }
	"-="                { return Symbol(Tokens.OPERATOR_ASSIGNMENT_SUBTRACTION); }
	"*="                { return Symbol(Tokens.OPERATOR_ASSIGNMENT_MULTIPLICATION); }
	"/="                { return Symbol(Tokens.OPERATOR_ASSIGNMENT_DIVISION); }
	"&="                { return Symbol(Tokens.OPERATOR_ASSIGNMENT_AND); }
	"|="                { return Symbol(Tokens.OPERATOR_ASSIGNMENT_OR); }
	"^="                { return Symbol(Tokens.OPERATOR_ASSIGNMENT_EXCLUSIVE_OR); }
	"<<="               { return Symbol(Tokens.OPERATOR_ASSIGNMENT_BITWISE_SHIFT_LEFT); }
	">>="               { return Symbol(Tokens.OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT); }
	">>>="              { return Symbol(Tokens.OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT_UNSIGNED); } 
	"instanceof"        { return Symbol(Tokens.OPERATOR_INSTANCEOF); }
	
	"activate"			{ return Symbol(Tokens.RESERVED_ACTIVATE); }
	"any"				{ return Symbol(Tokens.RESERVED_ANY); }
	"array"				{ return Symbol(Tokens.RESERVED_ARRAY); }
	"asc"				{ return Symbol(Tokens.RESERVED_ASC); }
	"autonomous"        { return Symbol(Tokens.RESERVED_AUTONOMOUS); }
	"begin"				{ return Symbol(Tokens.RESERVED_BEGIN); }
	"bigdecimal"		{ return Symbol(Tokens.RESERVED_BIGDECIMAL); }
	"bulk"				{ return Symbol(Tokens.RESERVED_BULK); }
	"byte"				{ return Symbol(Tokens.RESERVED_BYTE); }
	"cast"				{ return Symbol(Tokens.RESERVED_CAST); }
	"char"				{ return Symbol(Tokens.RESERVED_CHAR); }
	"collect"			{ return Symbol(Tokens.RESERVED_COLLECT); }
	"commit"			{ return Symbol(Tokens.RESERVED_COMMIT); }
	"const"				{ return Symbol(Tokens.RESERVED_CONST); }
	"default"			{ return Symbol(Tokens.RESERVED_DEFAULT); }
	"desc"				{ return Symbol(Tokens.RESERVED_DESC); }
	"end"				{ return Symbol(Tokens.RESERVED_END); }
	"exit"				{ return Symbol(Tokens.RESERVED_EXIT); }
	"export"			{ return Symbol(Tokens.RESERVED_EXPORT); }
	"float"				{ return Symbol(Tokens.RESERVED_FLOAT); }
	"from"				{ return Symbol(Tokens.RESERVED_FROM); }
	"goto"				{ return Symbol(Tokens.RESERVED_GOTO); }
	"group"				{ return Symbol(Tokens.RESERVED_GROUP); }
	"having"			{ return Symbol(Tokens.RESERVED_HAVING); }
	"hint"				{ return Symbol(Tokens.RESERVED_HINT); }
	"import"			{ return Symbol(Tokens.RESERVED_IMPORT); }
	"in"				{ return Symbol(Tokens.RESERVED_IN); }
	"inner"				{ return Symbol(Tokens.RESERVED_INNER); }
	"int"				{ return Symbol(Tokens.RESERVED_INT); }
	"into"				{ return Symbol(Tokens.RESERVED_INTO); }
	"join"				{ return Symbol(Tokens.RESERVED_JOIN); }
	"like"				{ return Symbol(Tokens.RESERVED_LIKE); }
	"limit"				{ return Symbol(Tokens.RESERVED_LIMIT); }
	"loop"				{ return Symbol(Tokens.RESERVED_LOOP); }
	"not"				{ return Symbol(Tokens.RESERVED_NOT); }
	"nulls"				{ return Symbol(Tokens.RESERVED_NULLS); }
	"number"			{ return Symbol(Tokens.RESERVED_NUMBER); }
	"of"				{ return Symbol(Tokens.RESERVED_OF); }
	"or"				{ return Symbol(Tokens.RESERVED_OR); }
	"outer"				{ return Symbol(Tokens.RESERVED_OUTER); }
	"package"			{ return Symbol(Tokens.RESERVED_PACKAGE); }
	"parallel"			{ return Symbol(Tokens.RESERVED_PARALLEL); }
	"pragma"			{ return Symbol(Tokens.RESERVED_PRAGMA); }
	"retrieve"			{ return Symbol(Tokens.RESERVED_RETRIEVE); }
	"returning"			{ return Symbol(Tokens.RESERVED_RETURNING); }
	"search"			{ return Symbol(Tokens.RESERVED_SEARCH); }
	"select"			{ return Symbol(Tokens.RESERVED_SELECT); }
	"short"				{ return Symbol(Tokens.RESERVED_SHORT); }
	"sort"				{ return Symbol(Tokens.RESERVED_SORT); }
	"stat"				{ return Symbol(Tokens.RESERVED_STAT); }
	"switch"			{ return Symbol(Tokens.RESERVED_SWITCH); }
	"synchronized"		{ return Symbol(Tokens.RESERVED_SYNCHRONIZED); }
	"then"				{ return Symbol(Tokens.RESERVED_THEN); }
	"transaction"		{ return Symbol(Tokens.RESERVED_TRANSACTION); }
	"using"				{ return Symbol(Tokens.RESERVED_USING); }
	"when"				{ return Symbol(Tokens.RESERVED_WHEN); }
	"where"				{ return Symbol(Tokens.RESERVED_WHERE); }	
   
    "true"				{ return Symbol(Tokens.LITERAL_TRUE); }
	"false"				{ return Symbol(Tokens.LITERAL_FALSE); }
	{IntegerLiteral}    { return Symbol(Tokens.LITERAL_INTEGER); }  
	{LongLiteral}       { return Symbol(Tokens.LITERAL_LONG); }
	{RealLiteral}       { return Symbol(Tokens.LITERAL_DOUBLE); }
	{DoubleLiteral}     { return Symbol(Tokens.LITERAL_DOUBLE); }

	{WhiteSpace}+		{ Symbol(Tokens.WHITESPACE); }

	{Identifier}		{ return Symbol(Tokens.IDENTIFIER); } 

	{CommentDoc}		{ Symbol(Tokens.COMMENT_DOC); }
	{CommentBlock}		{ Symbol(Tokens.COMMENT_BLOCK); }
	{CommentLine}		{ Symbol(Tokens.COMMENT_INLINE); }

	{StringDelimiter}   { PushState(STRING, Tokens.LITERAL_STRING, false); }
	{SOQLStart}			{ PushState(SOQL, Tokens.SOQL, false); }
	{SOSLStart}			{ PushState(SOSL, Tokens.SOSL, false); }
}

<STRING> {
	{StringDelimiter}	{ UpdateState(); if (PopState()) return GetCurrentToken(); }
  
	{StringCharacter}+	{ UpdateState(); }
  
	"\\b"				{ UpdateState(); }
	"\\t"               { UpdateState(); }
	"\\n"               { UpdateState(); }
	"\\f"               { UpdateState(); }
	"\\r"               { UpdateState(); }	
	"\\'"               { UpdateState(); }
	"\\\\"              { UpdateState(); }
  
	\\.					{ UpdateState(); if (PopState()) return (int)Tokens.error; }
	{LineTerminator}	{ UpdateState(); if (PopState()) return (int)Tokens.error; }
}

<SOQL> {
	{StringDelimiter}	{ PushState(STRING, Tokens.LITERAL_STRING, true); }
	\[					{ PushState(INNER_BRACKET, Tokens.WHITESPACE, true); }
	[^\]\[]				{ }
	\]					{ if (PopState()) return GetCurrentToken(); }
}

<SOSL> {
	{StringDelimiter}	{ PushState(STRING, Tokens.LITERAL_STRING, true); }
	\[					{ PushState(INNER_BRACKET, Tokens.WHITESPACE, true); }
	[^\]\[]				{ }
	\]					{ if (PopState()) return GetCurrentToken(); }
}

<INNER_BRACKET> {
	[^\]]				{ }
	\]					{ PopState(); }
}

<<EOF>>					{ return Symbol(Tokens.EOF); }

.|\n					{ return Symbol(Tokens.error); }

%%