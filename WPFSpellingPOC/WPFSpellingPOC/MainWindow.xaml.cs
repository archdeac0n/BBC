using NHunspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextHandler textHandler;
        private SpellHandler spellHandler = new SpellHandler();
        public MainWindow()
        {
            InitializeComponent();
            textHandler = new TextHandler(richTextBox1);
            spellHandler.SpellRange(new TextRange(flowDocument1.ContentStart, flowDocument1.ContentEnd).Text, richTextBox1);
            cbxLanguage.Items.Add("en_AU");
            cbxLanguage.Items.Add("en_GB");
            cbxLanguage.Items.Add("en_US");
            cbxLanguage.SelectedIndex = 1;
        }

        private void btnSpelling_Click(object sender, RoutedEventArgs e)
        {
            string myText = new TextRange(flowDocument1.ContentStart, flowDocument1.ContentEnd).Text;
            spellHandler.SpellRange(myText, richTextBox1);
        }

        private void Paragraph_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            SuggestionMenu();
            e.Handled = true;
        }

        private void SuggestionMenu()
        {
            textHandler.CurrentPosition = richTextBox1.CaretPosition;
            string word = textHandler.CurrentWord;
            int maxSuggestions = 4;
            int countSuggestions = 0;
            ContextMenu contextMenu = new ContextMenu();

            bool missSpelt = spellHandler.Spell(word);
            if (missSpelt)
            {

                List<string> suggestions = spellHandler.Corrections(word);

                foreach (var item in suggestions)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = item;
                    menuItem.Click += new RoutedEventHandler(Menu_Click);
                    contextMenu.Items.Add(menuItem);
                    countSuggestions += 1;
                    if (countSuggestions > maxSuggestions)
                    {
                        break;
                    }
                }
                contextMenu.IsOpen = true;
            }
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi == null)
            {
                return;
            }
            textHandler.CurrentPosition = richTextBox1.CaretPosition;
            textHandler.ReplaceWordFromPosition(mi.Header.ToString(), richTextBox1);
        }

        private void cbxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (string lang in e.AddedItems)
            {
                string aff = string.Format("{0}.{1}", lang, "aff");
                string dic = string.Format("{0}.{1}", lang, "dic");
                spellHandler = null;
                spellHandler = new SpellHandler(aff, dic);
               
                break;
            }
            textHandler = new TextHandler(richTextBox1);
            textHandler.RemoveAllUnderline();
            spellHandler.SpellRange(new TextRange(flowDocument1.ContentStart, flowDocument1.ContentEnd).Text, richTextBox1);
        }

        private void Paragraph_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton==MouseButtonState.Pressed)
            {
                SuggestionMenu();
                e.Handled = true;
            }
        }
    }
}
