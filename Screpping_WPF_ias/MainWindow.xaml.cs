using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

namespace Screpping_WPF_ias
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
        string mt = "";
        string yr = "";
        List<string> links = new List<string>();
        List<BitmapImage> img = new List<BitmapImage>();
        private async void save_Click(object sender, RoutedEventArgs e)
        {
            int i = 1;
            yr = year.Text;
            mt = month.Text;
            float xcord = float.Parse(x.Text);
            float ycord = float.Parse(y.Text);

            string pt = path.Text;
            try
            {

                HttpClient client = new HttpClient();
                var response = await client.GetAsync("http://www.insightsonindia.com/insights-mindmaps-on-important-current-issues-for-upsc-civil-services-exam");
                string result = response.Content.ReadAsStringAsync().Result;
                splt(result);

                foreach (string str in links)
                {
                    using (WebClient client1 = new WebClient())
                    {
                        client1.DownloadFile(new Uri(str), @pt  + i.ToString() + str.Remove(0, str.Length - 4));
                        i++;
                    }
                }
                i = 1;
                Document document = new Document();
                using (var stream = new FileStream(@pt + mt + "_" +  yr +  ".pdf", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfWriter.GetInstance(document, stream);
                    iTextSharp.text.Rectangle one = new iTextSharp.text.Rectangle(xcord,ycord);
                    document.SetPageSize(one);
                    document.Open();
                    foreach (string str in links)
                    {
                        using (var imageStream = new FileStream(@pt + i.ToString() + str.Remove(0, str.Length - 4), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            var image = iTextSharp.text.Image.GetInstance(imageStream);
                            document.Add(image);
                            i++;
                        }
                    }
                    document.Close();
                }
            }
            catch (Exception e1)
            {
                MessageBoxResult result = MessageBox.Show(e1.Message);
            }

            MessageBoxResult result1 = MessageBox.Show("lol ho gya");
        }

        private void splt(string result)
        {
            string find = "http://www.insightsonindia.com/wp-content/uploads/" + yr + "/" + mt + "/";
            int length = result.Length;
            int pos = result.IndexOf(find);
            if (pos != -1)
            {
                string n = result.Substring(pos);
                find = ".jpg";
                length = n.Length;
                pos = n.IndexOf(find);
                string rr = n.Substring(0, pos + 4);
                rr = rr.Replace('"', ' ');
                links.Add(rr);
                splt(n.Substring(pos));
            }
            else
                return;
        }
    }
}
