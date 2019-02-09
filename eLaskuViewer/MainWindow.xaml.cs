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
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections.ObjectModel;

namespace eLaskuViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Folder;

        // !!!!!!!!!!!! Unlock unsafe files !!!!!!!!!!!!!!!!!!!!!
        //  dir -Path "C:\xxx" -Recurse | Unblock-File

        public MainWindow()
        {
            Laskut = new ObservableCollection<ELasku>();

            Loaded += MainWindow_Loaded;
            DataContext = this;

            InitializeComponent();

            Folder = System.Configuration.ConfigurationManager.AppSettings["Folder"];

            if (string.IsNullOrEmpty(Folder))
            {
                MessageBox.Show("Set Folder on app.config");
                Application.Current.Shutdown();
                return;
            }

            if (!Folder.EndsWith("\\"))
                Folder += "\\";
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadLaskut();
        }


        public ObservableCollection<ELasku> Laskut { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        private void lvLaskut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ELasku lasku = lvLaskut.SelectedItem as ELasku;

            if (lasku != null)
                browser.Navigate(new Uri(lasku.FileName));
            else
                browser.Navigate("about:blank");
        }


        void ReloadLaskut()
        {
            ELasku lasku = lvLaskut.SelectedItem as ELasku;
            ELasku select = null;

            Laskut.Clear();
            List<ELasku> laskut = ELasku.ReadFolder(Folder + "*.xml");

            foreach (var i in laskut)
            {
                Laskut.Add(i);
                if (lasku != null && i.SellerOrganisationName == lasku.SellerOrganisationName && i.InvoiceNumber == lasku.InvoiceNumber)
                    select = i;
            }

            if (select != null)
                lvLaskut.SelectedItem = select;


            var xx = laskut.GroupBy(l => l.EpiReference + l.EpiReference.ToString()).Where(g => g.Count() > 1).Select(i => new {EpiReference = i.Key, File = i.First().FileName}).ToArray();

            var files = Directory.GetFiles(Folder, "tiliote*.csv");

            if (files.Length > 0)
            {
                List<Tilirivi> rivit = Tilirivi.ReadFile(files[0]);

                rivit = rivit.Where(i => i.MaksupalveluID != null).ToList();

                foreach (var l in laskut)
                {
                    if (l.InvoiceDueDate == DateTime.Parse("9.12.2015"))
                        System.Threading.Thread.Sleep(1);

                    bool found = false;
                    for (int i = 0; i < rivit.Count; i++)
                    {
                        if (l.Tiliotteella == false &&
                            (l.EpiNameAddressDetails.Truncate(30) == rivit[i].Saaja.Replace("Ö", "O").Truncate(30) || rivit[i].Saaja.ToUpper().Contains(l.EpiNameAddressDetails.ToUpper()))
                            && Math.Abs(l.InvoiceDueDate.Subtract(rivit[i].SuoritePaiva).TotalDays) < 2)
                        {
                            rivit.RemoveAt(i);
                            l.Tiliotteella = true;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        System.Threading.Thread.Sleep(1);
                }

                string notFound = "";

                rivit.ForEach(i => notFound += i.SuoritePaiva + " " + i.Saaja + " " + i.Summa.ToString() + Environment.NewLine);

                if (notFound.Length > 0)
                    MessageBox.Show(notFound);

            }
            else
            {
                foreach (var l in laskut)
                    l.Tiliotteella = true;
            }
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Poista?", "Poista tiedosto", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ELasku lasku = lvLaskut.SelectedItem as ELasku;

                if (lasku != null)
                {
                    File.Delete(lasku.FileName);
                    ReloadLaskut();
                }
            }
        }

        private void MenuItemRename_Click(object sender, RoutedEventArgs e)
        {
            ELasku lasku = lvLaskut.SelectedItem as ELasku;

            if (lasku != null)
            {
                Rename ren = new Rename();
                ren.FileName = lasku.FileName;
                if(ren.ShowDialog()==true)
                {
                    File.Move(lasku.FileName, ren.FileName);
                    ReloadLaskut();
                }
            }
        }
    }
}
