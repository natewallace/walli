using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceLanguage
{
    /// <summary>
    /// A text span.
    /// </summary>
    public class TextSpan : IComparable
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="startPosition">StartPosition.</param>
        /// <param name="endPosition">EndPosition.</param>
        public TextSpan(TextPosition startPosition, TextPosition endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="textSpan">The text span to convert.</param>
        internal TextSpan(SalesForceLanguage.Apex.Parser.ApexTextSpan textSpan)
        {
            if (textSpan.StartLine == textSpan.EndLine)
            {
                StartPosition = new TextPosition(textSpan.StartLine, textSpan.StartColumn);
                EndPosition = new TextPosition(textSpan.EndLine, textSpan.EndColumn);
            }
            else if (textSpan.StartLine == textSpan.EndLine - 1)
            {
                StartPosition = new TextPosition(textSpan.StartLine, textSpan.StartColumn);
                EndPosition = new TextPosition(
                    textSpan.StartLine,
                    textSpan.StartColumn + textSpan.EndPosition - textSpan.StartPosition - textSpan.EndColumn - 1);
            }
            else
            {
                StartPosition = new TextPosition(textSpan.StartLine, textSpan.StartColumn);
                EndPosition = new TextPosition(textSpan.StartLine, textSpan.StartColumn + 1);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The start position of the span.
        /// </summary>
        public TextPosition StartPosition { get; private set; }

        /// <summary>
        /// The end position of the span.
        /// </summary>
        public TextPosition EndPosition { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Check to see if the given position is contained within this span.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>true if the given position is contained within this span.</returns>
        public bool Contains(TextPosition position)
        {
            if (StartPosition.Line == position.Line)
            {
                if (StartPosition.Line == EndPosition.Line)
                    return StartPosition.Column <= position.Column && EndPosition.Column >= position.Column;
                else
                    return StartPosition.Column <= position.Column;
            }

            if (EndPosition.Line == position.Line)
            {
                if (StartPosition.Line == EndPosition.Line)
                    return StartPosition.Column <= position.Column && EndPosition.Column >= position.Column;
                else
                    return EndPosition.Column >= position.Column;
            }

            return (StartPosition.Line < position.Line && EndPosition.Line > position.Line);
        }

        /// <summary>
        /// Returns a human readible string for this object.
        /// </summary>
        /// <returns>A human readible string for this object.</returns>
        public override string ToString()
        {
            return String.Format("{0} -> {1}", StartPosition, EndPosition);
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows,
        /// or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: 
        /// Less than zero - This instance precedes obj in the sort order. 
        /// Zero - This instance occurs in the same position in the sort order as obj. 
        /// Greater than zero - This instance follows obj in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            TextSpan other = obj as TextSpan;
            if (other == null)
                return -1;

            return this.StartPosition.CompareTo(other.StartPosition);
        }

        #endregion
    }
}
