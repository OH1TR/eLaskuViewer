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
using System.Windows.Shapes;

namespace eLaskuViewer
{
    /// <summary>
    /// Interaction logic for Rename.xaml
    /// </summary>
    public partial class Rename : Window
    {
        public Rename()
        {
            Loaded += Rename_Loaded;
            InitializeComponent();
        }

        void Rename_Loaded(object sender, RoutedEventArgs e)
        {
            nimi.Focus();
            nimi.Select(0, nimi.Text.Length - 4);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        string path=null;

        public string FileName 
        {
            get
            {
                return (path + "\\" + nimi.Text);
            }
            set
            {
                path = System.IO.Path.GetDirectoryName(value);
                nimi.Text = System.IO.Path.GetFileName(value);
            }
        }
    }
}
