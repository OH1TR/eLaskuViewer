using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;

namespace eLaskuViewer
{
    public class ELasku : INotifyPropertyChanged
    {
        public static int ConpareByDate(ELasku a,ELasku b)
        {
            return((int)a.InvoiceDate.Subtract(b.InvoiceDate).TotalSeconds);
        }

        public static List<ELasku> ReadFolder(string pathAndMask)
        {
            List<ELasku> retval= new List<ELasku>();
            string dir=Path.GetDirectoryName(pathAndMask);
            string mask=Path.GetFileName(pathAndMask);
            string[] files = Directory.GetFiles(dir,mask,SearchOption.TopDirectoryOnly);

            foreach(string file in files)
            {
                ELasku e=new ELasku(file);

                if (retval.Count(i => i.EpiReference == e.EpiReference && i.InvoiceDueDate == e.InvoiceDueDate && i.SellerOrganisationName == e.SellerOrganisationName) > 0)
                {
                    File.Delete(file);
                    continue;
                }

                retval.Add(e);
            }

            retval.Sort(ConpareByDate);
            return(retval);
        }

        public ELasku()
        { 
        }

        public ELasku(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            SellerOrganisationName="";
            foreach(XmlNode i in doc.SelectNodes("/Finvoice/SellerPartyDetails/SellerOrganisationName"))            
                SellerOrganisationName+=i.InnerText+" ";
            SellerOrganisationName=SellerOrganisationName.Trim(' ');

            InvoiceDueDate = ReadDate(doc.SelectSingleNode("/Finvoice/InvoiceDetails/PaymentTermsDetails/InvoiceDueDate"));
            InvoiceDate = ReadDate(doc.SelectSingleNode("/Finvoice/InvoiceDetails/InvoiceDate"));
            InvoiceTotalVatIncludedAmount = doc.SelectSingleNode("/Finvoice/InvoiceDetails/InvoiceTotalVatIncludedAmount").InnerText;
            InvoiceNumber = doc.SelectSingleNode("/Finvoice/InvoiceDetails/InvoiceNumber").InnerText;
            EpiReference = doc.SelectSingleNode("/Finvoice/EpiDetails/EpiIdentificationDetails/EpiReference").InnerText;
            EpiDate = ReadDate(doc.SelectSingleNode("/Finvoice/EpiDetails/EpiIdentificationDetails/EpiDate"));
            EpiNameAddressDetails = doc.SelectSingleNode("/Finvoice/EpiDetails/EpiPartyDetails/EpiBeneficiaryPartyDetails/EpiNameAddressDetails").InnerText;
            
            FileName=fileName;

            if (SellerOrganisationName.Contains("VEHMAA"))
                System.Threading.Thread.Sleep(1);
        }

        protected XmlNamespaceManager ConstructNamespaceManager(XmlNode doc)
        {
            XPathNavigator nav = doc.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(nav.NameTable);
            manager.AddNamespace("xsi","http://www.w3.org/2001/XMLSchema-instance");
            return (manager);
        }

        public DateTime ReadDate(XmlNode date)
        {
            DateTime retval;
            string format = date.Attributes["Format"].Value.ToString();
            format = format.ToLower().Replace("cc", "yy").Replace("mm", "MM");
            if (DateTime.TryParseExact(date.InnerText, format, new System.Globalization.DateTimeFormatInfo(), System.Globalization.DateTimeStyles.None, out retval))
                return (retval);
            return (DateTime.MinValue);
        }

        string _FileName;
        public string FileName
        {
            get
            {
                return _FileName;
            }

            set
            {
                if (value != _FileName)
                {
                    _FileName = value;
                    NotifyPropertyChanged();
                }
            }
        }



        DateTime _InvoiceDueDate;
        public DateTime InvoiceDueDate
        {
            get
            {
                return _InvoiceDueDate;
            }

            set
            {
                if (value != _InvoiceDueDate)
                {
                    _InvoiceDueDate = value;
                    NotifyPropertyChanged();
                }
            }
        }


        string _InvoiceTotalVatIncludedAmount;
        public string InvoiceTotalVatIncludedAmount
        {
            get
            {
                return _InvoiceTotalVatIncludedAmount;
            }

            set
            {
                if (value != _InvoiceTotalVatIncludedAmount)
                {
                    _InvoiceTotalVatIncludedAmount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        DateTime _InvoiceDate;
        public DateTime InvoiceDate
        {
            get
            {
                return _InvoiceDate;
            }

            set
            {
                if (value != _InvoiceDate)
                {
                    _InvoiceDate = value;
                    NotifyPropertyChanged();
                }
            }
        }


        string _InvoiceNumber;
        public string InvoiceNumber
        {
            get
            {
                return _InvoiceNumber;
            }

            set
            {
                if (value != _InvoiceNumber)
                {
                    _InvoiceNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }


        string _SellerOrganisationName;
        public string SellerOrganisationName
        {
            get
            {
                return _SellerOrganisationName;
            }

            set
            {
                if (value != _SellerOrganisationName)
                {
                    _SellerOrganisationName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _EpiReference;
        public string EpiReference
        {
            get
            {
                return _EpiReference;
            }

            set
            {
                if (value != _EpiReference)
                {
                    _EpiReference = value;
                    NotifyPropertyChanged();
                }
            }
        }

        DateTime _EpiDate;
        public DateTime EpiDate
        {
            get
            {
                return _EpiDate;
            }

            set
            {
                if (value != _EpiDate)
                {
                    _EpiDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _EpiNameAddressDetails;
        public string EpiNameAddressDetails
        {
            get
            {
                return _EpiNameAddressDetails;
            }

            set
            {
                if (value != _EpiNameAddressDetails)
                {
                    _EpiNameAddressDetails = value;
                    NotifyPropertyChanged();
                }
            }
        }


        bool _Tiliotteella = false;
        public bool Tiliotteella
        {
            get
            {
                return _Tiliotteella;
            }

            set
            {
                if (value != _Tiliotteella)
                {
                    _Tiliotteella = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
