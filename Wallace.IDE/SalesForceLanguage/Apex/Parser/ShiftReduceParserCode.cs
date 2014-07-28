// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace QUT.Gppg
{
    /// <summary>
    /// Abstract class for GPPG shift-reduce parsers.
    /// Parsers generated by GPPG derive from this base
    /// class, overriding the abstract Initialize() and
    /// DoAction() methods.
    /// </summary>
    /// <typeparam name="TValue">Semantic value type</typeparam>
    /// <typeparam name="TSpan">Location type</typeparam>
#if EXPORT_GPPG
    public abstract class ShiftReduceParser<TValue, TSpan>
#else
    public abstract class ShiftReduceParser<TValue, TSpan>
#endif
 where TSpan : IMerge<TSpan>, new()
    {
        private AbstractScanner<TValue, TSpan> scanner;
        /// <summary>
        /// The abstract scanner for this parser.
        /// </summary>
        protected AbstractScanner<TValue, TSpan> Scanner {
            get { return scanner; }
            set { scanner = value; }
        }

        /// <summary>
        /// Constructor for base class
        /// </summary>
        /// <param name="scanner">Scanner instance for this parser</param>
        protected ShiftReduceParser(AbstractScanner<TValue, TSpan> scanner)
        {
            this.scanner = scanner;
        }

        // ==============================================================
        //                    TECHNICAL EXPLANATION.
        //   Why the next two fields are not exposed via properties.
        // ==============================================================
        // These fields are of the generic parameter types, and are
        // frequently instantiated as struct types in derived classes.
        // Semantic actions are defined in the derived classes and refer
        // to instance fields of these structs.  Is such cases the code
        // "get_CurrentSemanticValue().myField = blah;" will fail since
        // the getter pushes the value of the field, not the reference.
        // So, in the presence of properties, gppg would need to encode
        // such field accesses as ... 
        //  "tmp = get_CurrentSemanticValue(); // Fetch value
        //   tmp.myField = blah;               // update
        //   set_CurrentSemanticValue(tmp); "  // Write update back.
        // There is no issue if TValue is restricted to be a ref type.
        // The same explanation applies to scanner.yylval.
        // ==============================================================
        /// <summary>
        /// The current value of the "$$" symbolic variable in the parser
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        protected TValue CurrentSemanticValue;

        /// <summary>
        /// The current value of the "@$" symbolic variable in the parser
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        protected TSpan CurrentLocationSpan;
        protected int NextToken;

        private TSpan LastSpan;
        private State FsaState;
        private bool recovering;
        private int tokensSinceLastError;

        private PushdownPrefixState<State> StateStack = new PushdownPrefixState<State>();
        private PushdownPrefixState<TValue> valueStack = new PushdownPrefixState<TValue>();
        private PushdownPrefixState<TSpan> locationStack = new PushdownPrefixState<TSpan>();

        /// <summary>
        /// The stack of semantic value (YYSTYPE) values.
        /// </summary>
        protected PushdownPrefixState<TValue> ValueStack { get { return valueStack; } }

        /// <summary>
        /// The stack of location value (YYLTYPE) varlues.
        /// </summary>
        protected PushdownPrefixState<TSpan> LocationStack { get { return locationStack; } }

        private int errorToken;
        private int endOfFileToken;
        private string[] nonTerminals;
        private State[] states;
        private Rule[] rules;

        /// <summary>
        /// Initialization method to allow derived classes
        /// to insert the rule list into this base class.
        /// </summary>
        /// <param name="rules">The array of Rule objects</param>
        protected void InitRules(Rule[] rules) { this.rules = rules; }

      /// <summary>
      /// Initialization method to allow derived classes to
      /// insert the states table into this base class.
      /// </summary>
      /// <param name="states">The pre-initialized states table</param>
        protected void InitStates(State[] states) { this.states = states; }

      /// <summary>
      /// OBSOLETE FOR VERSION 1.4.0
      /// </summary>
      /// <param name="size"></param>
        protected void InitStateTable(int size) { states = new State[size]; }

        /// <summary>
        /// Initialization method to allow derived classes
        /// to insert the special value for the error and EOF tokens.
        /// </summary>
        /// <param name="err">The error state ordinal</param>
        /// <param name="end">The EOF stat ordinal</param>
        protected void InitSpecialTokens(int err, int end)
        {
            errorToken = err;
            endOfFileToken = end;
        }

        /// <summary>
        /// Initialization method to allow derived classes to
        /// insert the non-terminal symbol names into this base class.
        /// </summary>
        /// <param name="names">Non-terminal symbol names</param>
        protected void InitNonTerminals(string[] names) { nonTerminals = names; }

        #region YYAbort, YYAccept etcetera.
        [Serializable]
        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
        // Reason for FxCop message suppression -
        // This exception cannot escape from the local context
        private class AcceptException : Exception
        {
            internal AcceptException() { }
            protected AcceptException(SerializationInfo i, StreamingContext c) : base(i, c) { }
        }
        [Serializable]
        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
        // Reason for FxCop message suppression -
        // This exception cannot escape from the local context
        private class AbortException : Exception
        {
            internal AbortException() { }
            protected AbortException(SerializationInfo i, StreamingContext c) : base(i, c) { }
        }
        [Serializable]
        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
        // Reason for FxCop message suppression -
        // This exception cannot escape from the local context
        private class ErrorException : Exception
        {
            internal ErrorException() { }
            protected ErrorException(SerializationInfo i, StreamingContext c) : base(i, c) { }
        }

        // The following methods are only called from within
        // a semantic action. The thrown exceptions can never
        // propagate outside the ShiftReduceParser class in 
        // which they are nested.

        /// <summary>
        /// Force parser to terminate, returning "true"
        /// </summary>
        protected static void YYAccept() { throw new AcceptException(); }

        /// <summary>
        /// Force parser to terminate, returning "false"
        /// </summary>
        protected static void YYAbort() { throw new AbortException(); }

        /// <summary>
        /// Force parser to terminate, returning
        /// "false" if error recovery fails.
        /// </summary>
        protected static void YYError() { throw new ErrorException(); }

        /// <summary>
        /// Check if parser in error recovery state.
        /// </summary>
        protected bool YYRecovering { get { return recovering; } }
        #endregion

        /// <summary>
        /// Abstract base method. ShiftReduceParser calls this
        /// to initialize the base class data structures.  Concrete
        /// parser classes must override this method.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Main entry point of the Shift-Reduce Parser.
        /// </summary>
        /// <returns>True if parse succeeds, else false for
        /// unrecoverable errors</returns>
        public bool Parse()
        {
            Initialize();	// allow derived classes to instantiate rules, states and nonTerminals

            NextToken = 0;
            FsaState = states[0];

            StateStack.Push(FsaState);
            valueStack.Push(CurrentSemanticValue);
            LocationStack.Push(CurrentLocationSpan);

            while (true)
            {
#if TRACE_ACTIONS
                Console.Error.WriteLine("Entering state {0} ", FsaState.number);
                DisplayStack();
#endif
                int action = FsaState.defaultAction;

                if (FsaState.ParserTable != null)
                {
                    if (NextToken == 0)
                    {
                        // We save the last token span, so that the location span
                        // of production right hand sides that begin or end with a
                        // nullable production will be correct.
                        LastSpan = scanner.yylloc;
                        NextToken = scanner.yylex();
#if TRACE_ACTIONS
                       Console.Error.WriteLine( "Reading: Next token is {0}", TerminalToString( NextToken ) );
#endif
                    }
#if TRACE_ACTIONS
                    else 
                        Console.Error.WriteLine( "Next token is still {0}", TerminalToString( NextToken ) );
#endif
                    if (FsaState.ParserTable.ContainsKey(NextToken))
                        action = FsaState.ParserTable[NextToken];
                }

                if (action > 0)         // shift
                {
                    Shift(action);
                }
                else if (action < 0)   // reduce
                {
                    try
                    {
                        Reduce(-action);
                        if (action == -1)	// accept
                            return true;
                    }
                    catch (Exception x)
                    {
                        if (x is AbortException)
                            return false;
                        else if (x is AcceptException)
                            return true;
                        else if (x is ErrorException && !ErrorRecovery())
                            return false;
                        else
                            throw;  // Rethrow x, preserving information.

                    }
                }
                else if (action == 0)   // error
                    if (!ErrorRecovery())
                        return false;
            }
        }

        private void Shift(int stateIndex)
        {
#if TRACE_ACTIONS
				Console.Error.Write("Shifting token {0}, ", TerminalToString(NextToken));
#endif
            FsaState = states[stateIndex];

            valueStack.Push(scanner.yylval);
            StateStack.Push(FsaState);
            LocationStack.Push(scanner.yylloc);

            if (recovering)
            {
                if (NextToken != errorToken)
                    tokensSinceLastError++;

                if (tokensSinceLastError > 5)
                    recovering = false;
            }

            if (NextToken != endOfFileToken)
                NextToken = 0;
        }

        private void Reduce(int ruleNumber)
        {
#if TRACE_ACTIONS
				DisplayRule(ruleNumber);
#endif
            Rule rule = rules[ruleNumber];
            //
            //  Default actions for unit productions.
            //
            if (rule.RightHandSide.Length == 1)
            {
                CurrentSemanticValue = valueStack.TopElement();    // Default action: $$ = $1;
                CurrentLocationSpan = LocationStack.TopElement(); // Default action "@$ = @1;
            }
            else
            {
                if (rule.RightHandSide.Length == 0)
                {
                    // Create a new blank value.
                    // Explicit semantic action may mutate this value
                    CurrentSemanticValue = default(TValue);
                    // The location span for an empty production will start with the
                    // beginning of the next lexeme, and end with the finish of the
                    // previous lexeme.  This gives the correct behaviour when this
                    // nonsense value is used in later Merge operations.
                    CurrentLocationSpan = (scanner.yylloc != null && LastSpan != null ?
                        scanner.yylloc.Merge(LastSpan) :
                        default(TSpan));
                }
                else
                {
                    // Default action: $$ = $1;
                    CurrentSemanticValue = valueStack.TopElement();
                    //  Default action "@$ = @1.Merge(@N)" for location info.
                    TSpan at1 = LocationStack[LocationStack.Depth - rule.RightHandSide.Length];
                    TSpan atN = LocationStack[LocationStack.Depth - 1];
                    CurrentLocationSpan = 
                        ((at1 != null && atN != null) ? at1.Merge(atN) : default(TSpan));
                }
            }

            DoAction(ruleNumber);

            for (int i = 0; i < rule.RightHandSide.Length; i++)
            {
                StateStack.Pop();
                valueStack.Pop();
                LocationStack.Pop();
            }
            FsaState = StateStack.TopElement();

            if (FsaState.Goto.ContainsKey(rule.LeftHandSide))
                FsaState = states[FsaState.Goto[rule.LeftHandSide]];

            StateStack.Push(FsaState);
            valueStack.Push(CurrentSemanticValue);
            LocationStack.Push(CurrentLocationSpan);
        }

        /// <summary>
        /// Execute the selected action from array.
        /// Must be overriden in derived classes.
        /// </summary>
        /// <param name="actionNumber">Index of the action to perform</param>
        protected abstract void DoAction(int actionNumber);

        private bool ErrorRecovery()
        {
            bool discard;

            if (!recovering) // if not recovering from previous error
                ReportError();

            if (!FindErrorRecoveryState())
                return false;
            //
            //  The interim fix for the "looping in error recovery"
            //  artifact involved moving the setting of the recovering 
            //  bool until after invalid tokens have been discarded.
            //
            ShiftErrorToken();
            discard = DiscardInvalidTokens();
            recovering = true;
            tokensSinceLastError = 0;
            return discard;
        }

        private void ReportError()
        {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendFormat("Syntax error, unexpected {0}", TerminalToString(NextToken));

            if (FsaState.ParserTable.Count < 7)
            {
                bool first = true;
                foreach (int terminal in FsaState.ParserTable.Keys)
                {
                    if (first)
                        errorMsg.Append(", expecting ");
                    else
                        errorMsg.Append(", or ");

                    errorMsg.Append(TerminalToString(terminal));
                    first = false;
                }
            }
            scanner.yyerror(errorMsg.ToString());
        }

        private void ShiftErrorToken()
        {
            int old_next = NextToken;
            NextToken = errorToken;

            Shift(FsaState.ParserTable[NextToken]);

#if TRACE_ACTIONS
				Console.Error.WriteLine("Entering state {0} ", FsaState.number);
#endif
            NextToken = old_next;
        }

        private bool FindErrorRecoveryState()
        {
            while (true)    // pop states until one found that accepts error token
            {
                if (FsaState.ParserTable != null &&
                    FsaState.ParserTable.ContainsKey(errorToken) &&
                    FsaState.ParserTable[errorToken] > 0) // shift
                    return true;

#if TRACE_ACTIONS
					Console.Error.WriteLine("Error: popping state {0}", StateStack.TopElement().number);
#endif
                StateStack.Pop();
                valueStack.Pop();
                LocationStack.Pop();

#if TRACE_ACTIONS
			    DisplayStack();
#endif
                if (StateStack.IsEmpty())
                {
#if TRACE_ACTIONS
                        Console.Error.WriteLine("Aborting: didn't find a state that accepts error token");
#endif
                    return false;
                }
                else
                    FsaState = StateStack.TopElement();
            }
        }

        private bool DiscardInvalidTokens()
        {

            int action = FsaState.defaultAction;

            if (FsaState.ParserTable != null)
            {
                // Discard tokens until find one that works ...
                while (true)
                {
                    if (NextToken == 0)
                    {
#if TRACE_ACTIONS
                            Console.Error.Write("Reading a token: ");
#endif                       
                        NextToken = scanner.yylex();
                    }

#if TRACE_ACTIONS
                        Console.Error.WriteLine("Next token is {0}", TerminalToString(NextToken));
#endif
                    if (NextToken == endOfFileToken)
                        return false;

                    if (FsaState.ParserTable.ContainsKey(NextToken))
                        action = FsaState.ParserTable[NextToken];

                    if (action != 0)
                        return true;
                    else
                    {
#if TRACE_ACTIONS
                            Console.Error.WriteLine("Error: Discarding {0}", TerminalToString(NextToken));
#endif
                        NextToken = 0;
                    }
                }
            }
            else if (recovering && tokensSinceLastError == 0)
            {
                // 
                //  Boolean recovering is not set until after the first
                //  error token has been shifted.  Thus if we get back 
                //  here with recovering set and no tokens read we are
                //  looping on the same error recovery action.  This 
                //  happens if current_state.ParserTable is null because
                //  the state has an LR(0) reduction, but not all
                //  lookahead tokens are valid.  This only occurs for
                //  error productions that *end* on "error".
                //
                //  This action discards tokens one at a time until
                //  the looping stops.  Another attack would be to always
                //  use the LALR(1) table if a production ends on "error"
                //
#if TRACE_ACTIONS
                    Console.Error.WriteLine("Error: panic discard of {0}", TerminalToString(NextToken));
#endif
                if (NextToken == endOfFileToken)
                    return false;
                NextToken = 0;
                return true;
            }
            else
                return true;

        }

        /// <summary>
        /// Traditional YACC method.  Discards the next input token.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yyclearin")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yyclearin")]
        // Reason for FxCop message suppression -
        // This is a traditional name for YACC-like functionality
        protected void yyclearin() { NextToken = 0; }

        /// <summary>
        /// Tradional YACC method. Clear the "recovering" flag.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yyerrok")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yyerrok")]
        // Reason for FxCop message suppression -
        // This is a traditional name for YACC-like functionality
        protected void yyerrok()
        {
            recovering = false;
        }

        /// <summary>
        /// OBSOLETE FOR VERSION 1.4.0
        /// Method used by derived types to insert new
        /// state instances in the "states" array.
        /// </summary>
        /// <param name="stateNumber">index of the state</param>
        /// <param name="state">data for the state</param>
        protected void AddState(int stateNumber, State state)
        {
            states[stateNumber] = state;
            state.number = stateNumber;
        }

        private void DisplayStack()
        {
            Console.Error.Write("State stack is now:");
            for (int i = 0; i < StateStack.Depth; i++)
                Console.Error.Write(" {0}", StateStack[i].number);
            Console.Error.WriteLine();
        }

        private void DisplayRule(int ruleNumber)
        {
            Console.Error.Write("Reducing stack by rule {0}, ", ruleNumber);
            DisplayProduction(rules[ruleNumber]);
        }

        private void DisplayProduction(Rule rule)
        {
            if (rule.RightHandSide.Length == 0)
                Console.Error.Write("/* empty */ ");
            else
                foreach (int symbol in rule.RightHandSide)
                    Console.Error.Write("{0} ", SymbolToString(symbol));

            Console.Error.WriteLine("-> {0}", SymbolToString(rule.LeftHandSide));
        }

        /// <summary>
        /// Abstract state class naming terminal symbols.
        /// This is overridden by derived classes with the
        /// name (or alias) to be used in error messages.
        /// </summary>
        /// <param name="terminal">The terminal ordinal</param>
        /// <returns></returns>
        protected abstract string TerminalToString(int terminal);

        private string SymbolToString(int symbol)
        {
            if (symbol < 0)
                return nonTerminals[-symbol-1];
            else
                return TerminalToString(symbol);
        }

        /// <summary>
        /// Return text representation of argument character
        /// </summary>
        /// <param name="input">The character to convert</param>
        /// <returns>String representation of the character</returns>
        protected static string CharToString(char input)
        {
            switch (input)
            {
                case '\a': return @"'\a'";
                case '\b': return @"'\b'";
                case '\f': return @"'\f'";
                case '\n': return @"'\n'";
                case '\r': return @"'\r'";
                case '\t': return @"'\t'";
                case '\v': return @"'\v'";
                case '\0': return @"'\0'";
                default: return string.Format(CultureInfo.InvariantCulture, "'{0}'", input);
            }
        }
    }

    /// <summary>
    /// Classes implementing this interface must supply a
    /// method that merges two location objects to return
    /// a new object of the same type.
    /// GPPG-generated parsers have the default location
    /// action equivalent to "@$ = @1.Merge(@N);" where N
    /// is the right-hand-side length of the production.
    /// </summary>
    /// <typeparam name="TSpan">The Location type</typeparam>
#if EXPORT_GPPG
    public interface IMerge<TSpan>
#else
    public interface IMerge<TSpan>
#endif
    {
        /// <summary>
        /// Interface method that creates a location object from
        /// the current and last object.  Typically used to create
        /// a location object extending from the start of the @1
        /// object to the end of the @N object.
        /// </summary>
        /// <param name="last">The lexically last object to merge</param>
        /// <returns>The merged location object</returns>
        TSpan Merge(TSpan last);
    }

    /// <summary>
    /// This is the default class that carries location
    /// information from the scanner to the parser.
    /// If you don't declare "%YYLTYPE Foo" the parser
    /// will expect to deal with this type.
    /// </summary>
#if EXPORT_GPPG
    public class LexLocation : IMerge<LexLocation>
#else
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public class LexLocation : IMerge<LexLocation>
#endif
    {
        private int startLine;   // start line
        private int startColumn; // start column
        private int endLine;     // end line
        private int endColumn;   // end column

        /// <summary>
        /// The line at which the text span starts.
        /// </summary>
        public int StartLine { get { return startLine; } }

        /// <summary>
        /// The column at which the text span starts.
        /// </summary>
        public int StartColumn { get { return startColumn; } }

        /// <summary>
        /// The line on which the text span ends.
        /// </summary>
        public int EndLine { get { return endLine; } }

        /// <summary>
        /// The column of the first character
        /// beyond the end of the text span.
        /// </summary>
        public int EndColumn { get { return endColumn; } }

        /// <summary>
        /// Default no-arg constructor.
        /// </summary>
        public LexLocation()
        { }

        /// <summary>
        /// Constructor for text-span with given start and end.
        /// </summary>
        /// <param name="sl">start line</param>
        /// <param name="sc">start column</param>
        /// <param name="el">end line </param>
        /// <param name="ec">end column</param>
        public LexLocation(int sl, int sc, int el, int ec)
        { startLine = sl; startColumn = sc; endLine = el; endColumn = ec; }

        /// <summary>
        /// Create a text location which spans from the 
        /// start of "this" to the end of the argument "last"
        /// </summary>
        /// <param name="last">The last location in the result span</param>
        /// <returns>The merged span</returns>
        public LexLocation Merge(LexLocation last)
        { return new LexLocation(this.startLine, this.startColumn, last.endLine, last.endColumn); }
    }

    /// <summary>
    /// Abstract scanner class that GPPG expects its scanners to 
    /// extend.
    /// </summary>
    /// <typeparam name="TValue">Semantic value type YYSTYPE</typeparam>
    /// <typeparam name="TSpan">Source location type YYLTYPE</typeparam>
#if EXPORT_GPPG
    public abstract class AbstractScanner<TValue, TSpan>
#else
    public abstract class AbstractScanner<TValue, TSpan>
#endif
        where TSpan : IMerge<TSpan>
    {
        /// <summary>
        /// Lexical value optionally set by the scanner. The value
        /// is of the %YYSTYPE type declared in the parser spec.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yylval")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yylval")]
        // Reason for FxCop message suppression -
        // This is a traditional name for YACC-like functionality
        // A field must be declared for this value of parametric type,
        // since it may be instantiated by a value struct.  If it were 
        // implemented as a property, machine generated code in derived
        // types would not be able to select on the returned value.
        public TValue yylval;                     // Lexical value: set by scanner

        /// <summary>
        /// Current scanner location property. The value is of the
        /// type declared by %YYLTYPE in the parser specification.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yylloc")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yylloc")]
        // Reason for FxCop message suppression -
        // This is a traditional name for YACC-like functionality
        public virtual TSpan yylloc
        {
            get { return default(TSpan); }       // Empty implementation allowing
            set { /* skip */ }                   // yylloc to be ignored entirely.
        }

        /// <summary>
        /// Main call point for LEX-like scanners.  Returns an int
        /// corresponding to the token recognized by the scanner.
        /// </summary>
        /// <returns>An int corresponding to the token</returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yylex")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yylex")]
        // Reason for FxCop message suppression -
        // This is a traditional name for YACC-like functionality
        public abstract int yylex();

        /// <summary>
        /// Traditional error reporting provided by LEX-like scanners
        /// to their YACC-like clients.
        /// </summary>
        /// <param name="format">Message format string</param>
        /// <param name="args">Optional array of args</param>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yyerror")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yyerror")]
        // Reason for FxCop message suppression -
        // This is a traditional name for YACC-like functionality
        public virtual void yyerror(string format, params object[] args) { }
    }

    /// <summary>
    /// Encapsulated state for the parser.
    /// Opaque to users, visible to the tool-generated code.
    /// </summary>
#if EXPORT_GPPG
    public class State
    {
        /// <summary>
        /// The number of states in the automaton.
        /// </summary>
        public int number;
#else
    public class State
    {
      /// <summary>
      /// The number of states in the automaton.
      /// </summary>
        internal int number;
#endif
        internal Dictionary<int, int> ParserTable;   // Terminal -> ParseAction
        internal Dictionary<int, int> Goto;          // NonTerminal -> State;
        internal int defaultAction; // = 0;		     // ParseAction

        /// <summary>
        /// State transition data for this state. Pairs of elements of the 
        /// goto array associate symbol ordinals with next state indices.
        /// The actions array is passed to another constructor. 
        /// </summary>
        /// <param name="actions">The action list</param>c
        /// <param name="goToList">Next state data</param>
        public State(int[] actions, int[] goToList)
            : this(actions)
        {
            Goto = new Dictionary<int, int>();
            for (int i = 0; i < goToList.Length; i += 2)
                Goto.Add(goToList[i], goToList[i + 1]);
        }

        /// <summary>
        /// Action data for this state. Pairs of elements of the 
        /// action array associate action ordinals with each of
        /// those symbols that have actions in the current state.
        /// </summary>
        /// <param name="actions">The action array</param>
        public State(int[] actions)
        {
            ParserTable = new Dictionary<int, int>();
            for (int i = 0; i < actions.Length; i += 2)
                ParserTable.Add(actions[i], actions[i + 1]);
        }

        /// <summary>
        /// Set the default action for this state.
        /// </summary>
        /// <param name="defaultAction">Ordinal of the default action</param>
        public State(int defaultAction)
        {
            this.defaultAction = defaultAction;
        }

        /// <summary>
        /// Set the default action and the state transition table.
        /// </summary>
        /// <param name="defaultAction">The default action</param>
        /// <param name="goToList">Transitions from this state</param>
        public State(int defaultAction, int[] goToList)
            : this(defaultAction)
        {
            Goto = new Dictionary<int, int>();
            for (int i = 0; i < goToList.Length; i += 2)
                Goto.Add(goToList[i], goToList[i + 1]);
        }
    }

    /// <summary>
    /// Rule representation at runtime.
    /// </summary>
#if EXPORT_GPPG
    public class Rule
#else
    public class Rule
#endif
    {
        internal int LeftHandSide; // symbol
        internal int[] RightHandSide; // symbols

        /// <summary>
        /// Rule constructor.  This holds the ordinal of
        /// the left hand side symbol, and the list of
        /// right hand side symbols, in lexical order.
        /// </summary>
        /// <param name="left">The LHS non-terminal</param>
        /// <param name="right">The RHS symbols, in lexical order</param>
        public Rule(int left, int[] right)
        {
            this.LeftHandSide = left;
            this.RightHandSide = right;
        }
    }

    /// <summary>
    /// Stack utility for the shift-reduce parser.
    /// GPPG parsers have three instances:
    /// (1) The parser state stack, T = QUT.Gppg.State,
    /// (2) The semantic value stack, T = TValue,
    /// (3) The location stack, T = TSpan.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if EXPORT_GPPG
    public class PushdownPrefixState<T>
#else
    public class PushdownPrefixState<T>
#endif
    {
        //  Note that we cannot use the BCL Stack<T> class
        //  here as derived types need to index into stacks.
        //
        private T[] array = new T[8];
        private int tos = 0;

        /// <summary>
        /// Indexer for values of the stack below the top.
        /// </summary>
        /// <param name="index">index of the element, starting from the bottom</param>
        /// <returns>the selected element</returns>
        public T this[int index] { get { return array[index]; } }

        /// <summary>
        /// The current depth of the stack.
        /// </summary>
        public int Depth { get { return tos; } }

        internal void Push(T value)
        {
            if (tos >= array.Length)
            {
                T[] newarray = new T[array.Length * 2];
                System.Array.Copy(array, newarray, tos);
                array = newarray;
            }
            array[tos++] = value;
        }

        internal T Pop()
        {
            T rslt = array[--tos];
            array[tos] = default(T);

            return rslt;
        }

        internal T TopElement() { return array[tos - 1]; }

        internal bool IsEmpty() { return tos == 0; }

        /// <summary>
        /// Display stack values.
        /// </summary>
        /// <returns>A string that represents the stack values.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tos; i++)
                sb.AppendFormat("/{0}", array[i]);

            return sb.ToString();
        }
    }
}
