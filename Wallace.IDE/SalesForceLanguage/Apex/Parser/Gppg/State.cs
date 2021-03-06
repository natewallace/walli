﻿// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using System.Collections.Generic;

namespace QUT.Gppg
{
    /// <summary>
    /// Encapsulated state for the parser.
    /// Opaque to users, visible to the tool-generated code.
    /// </summary>
    public class State
    {
        #region Fields

        /// <summary>
        /// The number of states in the automaton.
        /// </summary>
        internal int number;

        /// <summary>
        /// Terminal -> ParseAction
        /// </summary>
        internal Dictionary<int, int> ParserTable;

        /// <summary>
        /// NonTerminal -> State;
        /// </summary>
        internal Dictionary<int, int> Goto;

        /// <summary>
        /// ParseAction
        /// </summary>
        internal int defaultAction; // = 0;

        #endregion

        #region Constructors

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

        #endregion
    }
}
