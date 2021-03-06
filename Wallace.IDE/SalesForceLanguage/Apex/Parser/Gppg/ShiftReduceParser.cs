// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using System;
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
    public abstract class ShiftReduceParser<TValue, TSpan> where TSpan : IMerge<TSpan>, new()
    {
        #region Fields

        /// <summary>
        /// Holds values that will be passed to the ProcessReduce method. - Nate Wallace
        /// </summary>
        TValue[] _valueParameterList;

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

        /// <summary>
        /// The next token.
        /// </summary>
        protected int NextToken;

        /// <summary>
        /// The last span.
        /// </summary>
        private TSpan LastSpan;

        /// <summary>
        /// The fsa state.
        /// </summary>
        private State FsaState;

        /// <summary>
        /// recovering flag.
        /// </summary>
        private bool recovering;

        /// <summary>
        /// tokens since last error.
        /// </summary>
        private int tokensSinceLastError;

        /// <summary>
        /// error token.
        /// </summary>
        private int errorToken;

        /// <summary>
        /// end of file token.
        /// </summary>
        private int endOfFileToken;

        /// <summary>
        /// non terminals.
        /// </summary>
        private string[] nonTerminals;

        /// <summary>
        /// states.
        /// </summary>
        private State[] states;

        /// <summary>
        /// rules.
        /// </summary>
        private Rule[] rules;

        /// <summary>
        /// Set to true after an error has occured and is reset after the next reduce.
        /// </summary>
        protected bool _errorOccured;

        /// <summary>
        /// When an error occurs the token that caused the error is held here.
        /// </summary>
        protected int _tokenOnError;

        /// <summary>
        /// When an error occurs the location that caused the error is held here.
        /// </summary>
        protected TSpan _locationOnError;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for base class
        /// </summary>
        /// <param name="scanner">Scanner instance for this parser</param>
        protected ShiftReduceParser(AbstractScanner<TValue, TSpan> scanner)
        {
            this.Scanner = scanner;

            StateStack = new PushdownPrefixState<State>();
            ValueStack = new PushdownPrefixState<TValue>();
            LocationStack = new PushdownPrefixState<TSpan>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The abstract scanner for this parser.
        /// </summary>
        protected AbstractScanner<TValue, TSpan> Scanner { get; set; }

        /// <summary>
        /// State stack.
        /// </summary>
        private PushdownPrefixState<State> StateStack { get; set; }

        /// <summary>
        /// The stack of semantic value (YYSTYPE) values.
        /// </summary>
        protected PushdownPrefixState<TValue> ValueStack { get; private set; }

        /// <summary>
        /// The stack of location value (YYLTYPE) varlues.
        /// </summary>
        protected PushdownPrefixState<TSpan> LocationStack { get; private set; }

        #endregion

        #region Methods

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
        protected void InitStates(State[] states) 
        { 
            this.states = states; 
        }

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
        protected virtual void InitNonTerminals(string[] names) 
        { 
            nonTerminals = names; 
        }

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
            _valueParameterList = new TValue[15]; // added Nate Wallace
            _errorOccured = false;

            Initialize();	// allow derived classes to instantiate rules, states and nonTerminals

            NextToken = 0;
            FsaState = states[0];

            StateStack.Push(FsaState);
            ValueStack.Push(CurrentSemanticValue);
            LocationStack.Push(CurrentLocationSpan);

            while (true)
            {
                int action = FsaState.defaultAction;

                if (FsaState.ParserTable != null)
                {
                    if (NextToken == 0)
                    {
                        // We save the last token span, so that the location span
                        // of production right hand sides that begin or end with a
                        // nullable production will be correct.
                        LastSpan = Scanner.yylloc;
                        NextToken = Scanner.yylex();
                    }

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
                {
                    _tokenOnError = NextToken;
                    _locationOnError = Scanner.yylloc;
                    if (!ErrorRecovery())
                        return false;
                }
            }
        }

        /// <summary>
        /// Shift.
        /// </summary>
        /// <param name="stateIndex">state index.</param>
        private void Shift(int stateIndex)
        {
            FsaState = states[stateIndex];

            ValueStack.Push(Scanner.yylval);
            StateStack.Push(FsaState);
            LocationStack.Push(Scanner.yylloc);

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

        /// <summary>
        /// Reduce.
        /// </summary>
        /// <param name="ruleNumber">The rule to reduce by.</param>
        private void Reduce(int ruleNumber)
        {
            Rule rule = rules[ruleNumber];
            //
            //  Default actions for unit productions.
            //
            if (rule.RightHandSide.Length == 1)
            {
                CurrentSemanticValue = ValueStack.TopElement();    // Default action: $$ = $1;
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
                    CurrentLocationSpan = (Scanner.yylloc != null && LastSpan != null ?
                        Scanner.yylloc.Merge(LastSpan) :
                        default(TSpan));
                }
                else
                {
                    // Default action: $$ = $1;
                    CurrentSemanticValue = ValueStack.TopElement();
                    //  Default action "@$ = @1.Merge(@N)" for location info.
                    TSpan at1 = LocationStack[LocationStack.Depth - rule.RightHandSide.Length];
                    TSpan atN = LocationStack[LocationStack.Depth - 1];
                    CurrentLocationSpan = 
                        ((at1 != null && atN != null) ? at1.Merge(atN) : default(TSpan));
                }
            }

            DoAction(ruleNumber);  

            for (int i = rule.RightHandSide.Length - 1; i >=0; i--)
            {
                _valueParameterList[i] = ValueStack.Pop(); // capture the values - added by Nate Wallace
                StateStack.Pop();
                LocationStack.Pop();
            }

            if (!_errorOccured)
                ProcessReduce(rule.LeftHandSide, _valueParameterList, rule.RightHandSide.Length); // process the reduce action - added by Nate Wallace
            else
                _errorOccured = false;

            FsaState = StateStack.TopElement();

            if (FsaState.Goto.ContainsKey(rule.LeftHandSide))
                FsaState = states[FsaState.Goto[rule.LeftHandSide]];

            StateStack.Push(FsaState);
            ValueStack.Push(CurrentSemanticValue);
            LocationStack.Push(CurrentLocationSpan);
        }

        /// <summary>
        /// Execute the selected action from array.
        /// Must be overriden in derived classes.
        /// </summary>
        /// <param name="actionNumber">Index of the action to perform</param>
        protected abstract void DoAction(int actionNumber);

        /// <summary>
        /// Abstract state class naming terminal symbols.
        /// This is overridden by derived classes with the
        /// name (or alias) to be used in error messages.
        /// </summary>
        /// <param name="terminal">The terminal ordinal</param>
        /// <returns></returns>
        protected abstract string TerminalToString(int terminal);

        /// <summary>
        /// Process the reduce.
        /// </summary>
        /// <param name="token">The token that was generated from the reduce.</param>
        /// <param name="values">The values that are being reduced.</param>
        /// <param name="valuesLength">The number of values in the values parameter.</param>
        protected virtual void ProcessReduce(int token, TValue[] values, int valuesLength)
        {
        }

        /// <summary>
        /// Error recovery.
        /// </summary>
        /// <returns>result.</returns>
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

        /// <summary>
        /// Report error.
        /// </summary>
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
            Scanner.yyerror(errorMsg.ToString());
        }

        /// <summary>
        /// Shift error token.
        /// </summary>
        private void ShiftErrorToken()
        {
            int old_next = NextToken;
            NextToken = errorToken;

            Shift(FsaState.ParserTable[NextToken]);

            NextToken = old_next;
        }

        /// <summary>
        /// Find error recovery state.
        /// </summary>
        /// <returns>result.</returns>
        private bool FindErrorRecoveryState()
        {
            while (true)    // pop states until one found that accepts error token
            {
                if (FsaState.ParserTable != null &&
                    FsaState.ParserTable.ContainsKey(errorToken) &&
                    FsaState.ParserTable[errorToken] > 0) // shift
                    return true;

                StateStack.Pop();
                ValueStack.Pop();
                LocationStack.Pop();

                if (StateStack.IsEmpty())
                {
                    return false;
                }
                else
                    FsaState = StateStack.TopElement();
            }
        }

        /// <summary>
        /// Discard invalid tokens.
        /// </summary>
        /// <returns>result.</returns>
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
                        NextToken = Scanner.yylex();
                    }

                    if (NextToken == endOfFileToken)
                        return false;

                    if (FsaState.ParserTable.ContainsKey(NextToken))
                        action = FsaState.ParserTable[NextToken];

                    if (action != 0)
                        return true;
                    else
                    {
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
        protected void yyclearin() 
        { 
            NextToken = 0; 
        }

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

        /// <summary>
        /// Display stack.
        /// </summary>
        private void DisplayStack()
        {
            Console.Error.Write("State stack is now:");
            for (int i = 0; i < StateStack.Depth; i++)
                Console.Error.Write(" {0}", StateStack[i].number);
            Console.Error.WriteLine();
        }

        /// <summary>
        /// Display rule.
        /// </summary>
        /// <param name="ruleNumber">The rule to display.</param>
        private void DisplayRule(int ruleNumber)
        {
            Console.Error.Write("Reducing stack by rule {0}, ", ruleNumber);
            DisplayProduction(rules[ruleNumber]);
        }

        /// <summary>
        /// Display production.
        /// </summary>
        /// <param name="rule">The production to display.</param>
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
        /// Turn symbol to string.
        /// </summary>
        /// <param name="symbol">The symbol to convert.</param>
        /// <returns>The string for the symbol.</returns>
        protected string SymbolToString(int symbol)
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

        #endregion
    }
}
