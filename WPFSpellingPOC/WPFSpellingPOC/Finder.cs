using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WpfApplication2
{

    public class Finder
    {
        private void UnderlineTerms(TextRange range, RichTextBox rtb)
        {
            try
            {
                TextHandler findAndReplaceManager = new TextHandler(rtb);
                findAndReplaceManager.RemoveAllUnderline();

                List<Term> terms = GetTerms();
                List<Term> sortedTerms = (from t in terms orderby t.Name descending select t).ToList();

                foreach (Term term in sortedTerms)
                {
                    string regexWord = string.Format("{0}{1}{2}", "\\b", term.Name.Trim(), "\\b");
                    MatchCollection matches = Regex.Matches(range.Text, regexWord, RegexOptions.IgnoreCase);
                    foreach (Match m in matches)
                    {
                        findAndReplaceManager.UnderlineAll(m.ToString(), FindOptions.MatchWholeWord);
                    }
                }
            }
            finally
            {
                //narrativeWindow.IsDecorating = false;
            }
        }

        private List<Term> GetTerms()
        {
            List<Term> myTerms = new List<Term>();
            myTerms.Add(new Term { Name = "Actual Customer" });
            myTerms.Add(new Term { Name = "Campaign" });
            myTerms.Add(new Term { Name = "Campaign Offset" });
            myTerms.Add(new Term { Name = "Campaign Site" });
            myTerms.Add(new Term { Name = "Customer" });
            myTerms.Add(new Term { Name = "Site" });
            return myTerms;
        }
    }


}
