using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace WpfApplication2
{
    public class TextHandler
    {
        private RichTextBox rtb;
        private FlowDocument inputDocument;
        private TextPointer currentPosition;
        private string currentWord;

        public TextHandler(System.Windows.Controls.RichTextBox rtb)
        {

            if (rtb == null)
            {
                throw new ArgumentNullException("documentToFind");
            }
            this.rtb = rtb;
            this.inputDocument = rtb.Document;
            if (rtb.CaretPosition == null)
            {
                this.currentPosition = inputDocument.ContentStart;
            }
            else
            {
                this.currentPosition = rtb.CaretPosition;
            }
        }

        public string CurrentWord
        {
            get
            {
                currentWord = GetWordFromPosition();
                return currentWord;
            }
        }
        public TextPointer CurrentPosition
        {
            get
            {
                return currentPosition;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (value.CompareTo(inputDocument.ContentStart) < 0 || value.CompareTo(inputDocument.ContentEnd) > 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                currentPosition = value;
            }
        }

        public int NoCharsLeftFromPosition { get; set; }
        public int NoCharsRightFromPosition { get; set; }

        public TextRange FindNext(String input, FindOptions findOptions)
        {
            TextRange textRange = FindTextRangeFromPosition(ref currentPosition, input, findOptions);
            return textRange;
        }

        private string GetWordFromPosition()
        {
            TextPointer position = currentPosition;
            string word = "";

            while (position != null)
            {
                if (position.CompareTo(inputDocument.ContentEnd) == 0)
                {
                    break;
                }

                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string[] textForward = position.GetTextInRun(LogicalDirection.Forward).Split(' ');
                    string[] textBack = position.GetTextInRun(LogicalDirection.Backward).Split(' ');
                    NoCharsLeftFromPosition = textBack[textBack.Length - 1].Length;
                    NoCharsRightFromPosition = textForward[0].Length;
                    word = string.Format("{1}{0}", textForward[0], textBack[textBack.Length - 1]);
                    break;
                }
                else
                {
                    //If the current position doesn't represent a text context position, go to the next context position.
                    // This can effectively ignore the formatting or embed element symbols.
                    position = position.GetNextContextPosition(LogicalDirection.Forward);
                }
            }
            return word;
        }

        public TextRange FindTextRangeFromPosition(ref TextPointer position, String input, FindOptions findOptions)
        {
            Boolean matchCase = (findOptions & FindOptions.MatchCase) == FindOptions.MatchCase;
            Boolean matchWholeWord = (findOptions & FindOptions.MatchWholeWord) == FindOptions.MatchWholeWord;

            TextRange textRange = null;

            while (position != null)
            {
                if (position.CompareTo(inputDocument.ContentEnd) == 0)
                {
                    break;
                }

                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    String textRun = position.GetTextInRun(LogicalDirection.Forward);
                    StringComparison stringComparison = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                    Int32 indexInRun = textRun.IndexOf(input, stringComparison);

                    if (indexInRun >= 0)
                    {
                        position = position.GetPositionAtOffset(indexInRun);
                        TextPointer nextPointer = position.GetPositionAtOffset(input.Length);
                        textRange = new TextRange(position, nextPointer);

                        if (matchWholeWord)
                        {
                            if (IsWholeWord(textRange)) // Test if the "textRange" represents a word.
                            {
                                // If a WholeWord match is found, directly terminate the loop.
                                position = position.GetPositionAtOffset(input.Length);
                                break;
                            }
                            else
                            {
                                // If a WholeWord match is not found, go to next recursion to find it.
                                position = position.GetPositionAtOffset(input.Length);
                                return FindTextRangeFromPosition(ref position, input, findOptions);
                            }
                        }
                        else
                        {
                            // If a non-WholeWord match is found, directly terminate the loop.
                            position = position.GetPositionAtOffset(input.Length);
                            break;
                        }
                    }
                    else
                    {
                        // If a match is not found, go over to the next context position after the "textRun".
                        position = position.GetPositionAtOffset(textRun.Length);
                    }
                }
                else
                {
                    //If the current position doesn't represent a text context position, go to the next context position.
                    // This can effectively ignore the formatting or embed element symbols.
                    position = position.GetNextContextPosition(LogicalDirection.Forward);
                }
            }

            return textRange;
        }

        private Boolean IsWordChar(Char character)
        {
            return Char.IsLetterOrDigit(character) || character == '_';
        }

        private Boolean IsWholeWord(TextRange textRange)
        {
            Char[] chars = new Char[1];

            if (textRange.Start.CompareTo(inputDocument.ContentStart) == 0 || textRange.Start.IsAtLineStartPosition)
            {
                textRange.End.GetTextInRun(LogicalDirection.Forward, chars, 0, 1);
                return !IsWordChar(chars[0]);
            }
            else if (textRange.End.CompareTo(inputDocument.ContentEnd) == 0)
            {
                textRange.Start.GetTextInRun(LogicalDirection.Backward, chars, 0, 1);
                return !IsWordChar(chars[0]);
            }
            else
            {
                textRange.End.GetTextInRun(LogicalDirection.Forward, chars, 0, 1);
                if (!IsWordChar(chars[0]))
                {
                    textRange.Start.GetTextInRun(LogicalDirection.Backward, chars, 0, 1);
                    return !IsWordChar(chars[0]);
                }
            }

            return false;
        }

        public void RemoveUnderline()
        {
            rtb.Selection.ApplyPropertyValue(System.Windows.Controls.TextBlock.TextDecorationsProperty, null);
        }

        public void RemoveAllUnderline()
        {
            rtb.SelectAll();
            RemoveUnderline();
        }

        public Int32 UnderlineAll(String input, FindOptions findOptions)
        {
            Int32 count = 0;
            try
            {
                TextDecorationCollection myCollection = new TextDecorationCollection();
                //myCollection.Add(TextDecorator.DashedUnderline());
                myCollection.Add(TextDecorator.WavyUnderline());

                currentPosition = inputDocument.ContentStart;
                while (currentPosition.CompareTo(inputDocument.ContentEnd) < 0)
                {
                    TextRange textRange = FindNext(input, findOptions);
                    if (textRange != null)
                    {
                        count++;

                        rtb.Selection.Select(textRange.Start, textRange.End);
                        rtb.Selection.ApplyPropertyValue(System.Windows.Controls.TextBlock.TextDecorationsProperty, myCollection);
                    }
                }
            }
            catch (Exception ex)
            {
                // For now, we're just benignly ignoring any exceptions that get thrown during underlining
            }
            return count;
        }

        public void ReplaceWordFromPosition(string Replace, RichTextBox rtb)
        {
            TextRange text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            TextPointer current = text.Start.GetInsertionPosition(LogicalDirection.Forward);
            while (current != null)
            {
                string textInRun = current.GetTextInRun(LogicalDirection.Forward);
                if (!string.IsNullOrWhiteSpace(textInRun))
                {
                    int index = textInRun.IndexOf(currentWord);
                    if (index != -1)
                    {
                        TextPointer selectionStart = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                        TextPointer selectionEnd = selectionStart.GetPositionAtOffset(currentWord.Length, LogicalDirection.Forward);
                        TextRange selection = new TextRange(selectionStart, selectionEnd);
                        selection.Text = Replace;
                        rtb.Selection.Select(selection.Start, selection.End);
                        rtb.Focus();
                    }
                }
                current = current.GetNextContextPosition(LogicalDirection.Forward);
            }
        }
    }

    [Flags]
    public enum FindOptions
    {
        None = 0x00000000,              /// Perform case-insensitive non-word search.
        MatchCase = 0x00000001,         /// Perform case-sensitive search.
        MatchWholeWord = 0x00000002,    /// Perform the search against whole word.
    }


    public class Term
    {
        public string Name { get; set; }
    }

}

