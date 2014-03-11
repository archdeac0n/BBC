using NHunspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace WpfApplication2
{
    class SpellHandler
    {
        private Hunspell speller;
        public SpellHandler(string aff, string dic)
        {
            speller = new Hunspell(aff, dic);
        }
        
        public SpellHandler()
        {
            speller = new Hunspell("en_US.aff", "en_US.dic");
        }

        public bool Spell(string Word)
        {
            return !speller.Spell(Word);
        }

        public void SpellRange(string textRange, RichTextBox richTextBox1)
        {
            int wordCount = 0;
            var incorrectWords = new List<string>();
            textRange = textRange.Replace("\r", "");
            textRange = textRange.Replace("\n", "");
            string[] wordBag = textRange.Split(' ');

            foreach (var word in wordBag)
            {
                wordCount += 1;
                
                if (Spell(word))
                {
                    TextHandler text = new TextHandler(richTextBox1);
                    text.UnderlineAll(word, FindOptions.MatchWholeWord);
                }
            }
        }

        public List<string> Corrections(string word)
        {
            var result = new List<string>();
            result = speller.Suggest(word);
            
            return result;
        }
    }
}
