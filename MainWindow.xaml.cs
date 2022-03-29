using GraphenCalc.Classes;
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

namespace GraphCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Graph g0;
        
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void BtnExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();            
        }

        private void BtnClearClick(object sender, RoutedEventArgs e)
        {
            AppConsole.Clear();
        }
        private void BtnCSVClick(object sender, RoutedEventArgs e)
        {
            try
            {
                g0 = new Graph(FilePathBox.Text);
                AppConsole.Text = g0.ToString();
            }
            catch(Exception ex)
            {
                AppConsole.Text = ex.Message;
            }
            
        }
        private void BtnRandomGraphClick(object sender, RoutedEventArgs e)
        {
            try
            {
                g0 = new Graph(Int32.Parse(RandomNodes.Text), Int32.Parse(RandomChance.Text));
                AppConsole.Text = g0.ToString();                
            }
            catch(Exception ex)
            {
                AppConsole.Text = ex.Message;
            }            
        }

        private void BtnAddEdge(object sender, RoutedEventArgs e)
        {
            g0.GetMatrix().AddEdge(Int32.Parse(EdgeNodeA.Text), Int32.Parse(EdgeNodeB.Text));
            g0.CalcAll();
            AppConsole.Text = g0.ToString();
        }

        private void BtnRemoveEdge(object sender, RoutedEventArgs e)
        {
            g0.GetMatrix().RemoveEdge(Int32.Parse(EdgeNodeA.Text), Int32.Parse(EdgeNodeB.Text));
            g0.CalcAll();
            AppConsole.Text = g0.ToString();
        }

        private void BtnRemoveEdgesOfNode(object sender, RoutedEventArgs e)
        {
            g0.GetMatrix().RemoveEdgesOfNode(Int32.Parse(Node.Text));
            g0.CalcAll();
            AppConsole.Text = g0.ToString();
        }


    }
}
