using System;
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
            model.FeedList(testList);
            model.CreateMarkovSpace();
            parser.ChangeParameters(0, 3, 0);
            parser.ParseFile("F:\\Documents\\Visual Studio 2017\\Projects\\NameGenerator\\NameGenerator\\lists\\Vantesh.txt");
            for(int i = 0; i < 30; i++)
            {
                Console.Out.WriteLine(parser.GenerateWord(true));
            }
        }

        List<string> testList = new List<string> { "example", "eaample", "eample", "haiduke", "giant", "otter" };
        MarkovSpace model = new MarkovSpace(0,3,0);
        LanguageFileParser parser = new LanguageFileParser();
    }
}
