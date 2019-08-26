using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace NameGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*model.FeedList(testList);
            model.CreateMarkovSpace();
            parser.ChangeParameters(0, 3, 0);
            parser.ParseFile(@"F:\Documents\Visual Studio 2017\Projects\NameGenerator\NameGenerator\lists\Vantesh.txt");
            for(int i = 0; i < 30; i++)
            {
                Console.Out.WriteLine(textInfo.ToTitleCase(parser.GenerateWord(true)));
            }*/

            sca.AddCategory('C', "xmpl");
            sca.AddSoundChange("C", "C", "C_C");
            //sca.AddSoundChange("pl", "H", "_e");

            foreach(string l in testList)
            {
                Console.Out.Write(l + "\t->\t");
                Console.Out.WriteLine(sca.ApplyChanges(l));
            }
        }

        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        List<string> testList = new List<string> { "example", "eaample", "eample", "haiduke", "giant", "otter" };
        MarkovSpace model = new MarkovSpace(0,3,0);
        LanguageFileParser parser = new LanguageFileParser();

        SoundChanger sca = new SoundChanger();
    }
}
