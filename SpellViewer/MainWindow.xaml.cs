using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpellViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpellsManager db;

        private Dictionary<string, string> Schools = new Dictionary<string, string>();

        private string[] DamageTypes =
        {
            "slashing ",
            "bludgeoning ",
            "piecing ",
            "acid ",
            "fire ",
            "cold ",
            "poison ",
            "necrotic ",
            "radiant ",
            "lightning ",
            "psychic ",
            "thunder ",
            "force ",
        };

        public MainWindow()
        {
            InitializeComponent();
            Assembly assembly = Assembly.GetExecutingAssembly();

            db = SpellsManager.Load(assembly.GetManifestResourceStream("SpellViewer.PHB Spells 3.4.6.xml"));
            
            this.Title = "Spell Viewer (Using: PHB Spells 3.4.6.xml)";

            /*
            Abjuration: spells of protection, blocking, and banishing. Specialists are called abjurers.
            Conjuration: spells that bring creatures or materials. Specialists are called conjurers.
            Divination: spells that reveal information. Specialists are called diviners.
            Enchantment: spells that magically imbue the target or give the caster power over the target. Specialists are called enchanters.
            Evocation: spells that manipulate energy or create something from nothing. Specialists are called evokers.
            Illusion: spells that alter perception or create false images. Specialists are called illusionists.
            Necromancy: spells that manipulate life or life force. Specialists are called necromancers.
            Transmutation: spells that transform the target. Specialists are called transmuters.
            */

            Schools.Add("A", "Abjuration");
            Schools.Add("C", "Conjuration");
            Schools.Add("D", "Divination");
            Schools.Add("EN", "Enchantment");
            Schools.Add("EV", "Evocation");
            Schools.Add("I", "Illusion");
            Schools.Add("N", "Necromancy");
            Schools.Add("T", "Transmutation");

            // Debug
            //DisplaySpell(db.spells[0]);

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Show all the spells
            textSearch_TextChanged(null, null);

            // Select the first spell
            listItems.SelectedIndex = 0;
        }

        private void textSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool displayAll = textSearch.Text == "";
            string search = textSearch.Text.ToLower();
            listItems.Items.Clear();

            foreach (spell s in db.spells)
                if (displayAll || s.name.ToLower().Contains(search))
                    listItems.Items.Add(new ListBoxItem() { Content = s.name });
        }

        private void DisplaySpell(spell s)
        {
            FlowDocument doc = new FlowDocument();

            StringBuilder header = new StringBuilder();

            header.Append("Name: " + (s.name is string ? s.name : "") + "\r\n");
            header.Append("Level: " + (s.level is string ? s.level : "") + "\r\n");
            header.Append("School: " + (s.school is string ? (s.school + " (" + (Schools[s.school] is String ? Schools[s.school] : "Unknown") + ")") : "") + "\r\n");
            header.Append("Ritual: " + (s.ritual is string ? s.ritual : "") + "\r\n");
            header.Append("Time: " + (s.time is string ? s.time : "") + "\r\n");
            header.Append("Range: " + (s.range is string ? s.range : "") + "\r\n");
            header.Append("Components: " + (s.components is string ? s.components : "") + "\r\n");
            header.Append("Duration: " + (s.duration is string ? s.duration : "") + "\r\n");
            header.Append("Classes: " + (s.classes is string ? s.classes : "") + "\r\n");
            header.Append("\r\nDescription:");

            foreach (string text in s.text)
            {
                header.Append("\r\n" + text);
            }

            Paragraph headerParagraph = new Paragraph(new Run(header.ToString()));
            doc.Blocks.Add(headerParagraph);



            /*foreach (string damageType in DamageTypes)
            {
                int i = -1;
                do
                {
                    TextRange totalRange = new TextRange(doc.ContentStart, doc.ContentEnd);
                    i = totalRange.Text.IndexOf(damageType, i + 1);


                    if (i >= 0)
                    {
                        TextPointer start = doc.ContentStart.GetPositionAtOffset(i, LogicalDirection.Forward);
                        TextPointer end = doc.ContentStart.GetPositionAtOffset(i + damageType.Length, LogicalDirection.Forward);
                        TextRange range = new TextRange(start, end);

                        range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);

                        //descriptionRun.TextEffects.Add(new TextEffect { PositionStart = i, PositionCount = damageType.Length, Foreground = Brushes.Red });
                    }
                }
                while (i >= 0);
            }*/

            
            
            Regex reg = new Regex("\\d+d\\d+\\s*\\+?\\s*\\d*");
            TextPointer pointerOut = doc.ContentStart;
            bool bdone = false;
            do
            {
                string allText = GetAllTextFromPointer(pointerOut, out pointerOut);
                Match match = reg.Match(allText);
                if (match != null && match.Success)
                {
                    TextPointer start = pointerOut.GetPositionAtOffset(match.Index);
                    TextPointer end = start.GetPositionAtOffset(match.Value.Length);
                    TextRange range = new TextRange(start, end);

                    range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Blue);
                    range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    range.ApplyPropertyValue(TextElement.FontSizeProperty, 17.0);
                }
                else
                {
                    bdone = true;
                }
                
            }
            while (!bdone);


            foreach (string damageType in DamageTypes)
            {
                TextRange range = null;
                TextPointer current = doc.ContentStart;
                do
                {
                    range = FindWordFromPosition(current, damageType);

                    if (range != null)
                    {
                        //range.Text = "<" + damageType.TrimEnd() + "> ";
                        range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                        range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        range.ApplyPropertyValue(TextElement.FontSizeProperty, 17.0);
                        current = range.End;
                    }
                }
                while (range != null);
            }



            /*int i = -1;
            do
            {
                string nextType = null;
                int nextTypeAt = -1;
                foreach (string damageType in DamageTypes)
                {
                    int index = description.IndexOf(damageType, i + 1);
                    if (index != -1 && nextTypeAt < index)
                    {
                        nextType = damageType;
                        nextTypeAt = index;
                    }
                }

                string sub = 
            }
            while (i >= 0);*/




            textDescription.Document = doc;

            /*
            textDescription.AppendText("\r\n");

            foreach (string roll in s.roll)
            {
                textDescription.AppendText("\r\n" + roll);
            }*/
        }

        private string GetAllTextFromPointer(TextPointer pointer, out TextPointer pointerOut)
        {
            string allText = null;
            pointerOut = null;
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    allText = pointer.GetTextInRun(LogicalDirection.Forward);
                    pointerOut = pointer;
                }
                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
            return allText;
        }


        private TextRange FindWordFromPosition(TextPointer position, string word)
        {
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    // Find the starting index of any substring that matches "word".
                    int indexInRun = textRun.IndexOf(word);
                    if (indexInRun >= 0)
                    {
                        TextPointer start = position.GetPositionAtOffset(indexInRun);
                        TextPointer end = start.GetPositionAtOffset(word.Length);
                        return new TextRange(start, end);
                    }
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            // position will be null if "word" is not found.
            return null;
        }

        private void listItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listItems.SelectedItem is ListBoxItem)
            {
                string name = (listItems.SelectedItem as ListBoxItem).Content.ToString();
                foreach(spell s in db.spells)
                    if (name == s.name)
                    {
                        DisplaySpell(s);
                        break;
                    }
            }
        }
    }
}
