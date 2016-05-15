using CreativeXamlToolkit.Wpf;
using Demo.Wpf.Properties;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void image1_MouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard story = (Storyboard)FindResource("expandStoryboard");
            Image image = sender as Image;
            image.BeginStoryboard(story);

        }

        private void image1_MouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard story = (Storyboard)FindResource("shrinkStoryboard");
            Image image = sender as Image;
            image.BeginStoryboard(story);

        }

        private void HintLabel_IsExpandedChanged(object sender, RoutedEventArgs e)
        {
            HintLabel label = (HintLabel)sender;
            Console.WriteLine(label.IsExpanded.ToString());
        }
    }
}
