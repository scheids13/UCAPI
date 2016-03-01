using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UCAPI_directory_scraper
{
    public partial class Form1 : Form
    {
        string current_link = "";
        string status = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
            var task = DoNavigationAsync();
            task.ContinueWith((t) =>
            {
                MessageBox.Show("Navigation done!");
            }, TaskScheduler.FromCurrentSynchronizationContext());
             */

            // http://www.uc.edu/smart.html?gclid=Cj0KEQiAr8W2BRD2qbCOv8_H7qEBEiQA1ErTBoOs3y7R0ng4y3uq5ozUzdXh60_z_OXWyJJUPtO3AXQaAgem8P8HAQ

            browser.Navigate("https://webapps2.uc.edu/scheduleofclasses/");
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);

            //browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser1_DocumentCompleted);
        }

        private void visit_each_link(List<string> links_to_visit)
        {
            var i = 1;
            /*
            AutoResetEvent evt = new AutoResetEvent(false);
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(
                        delegate(object sender2,
                            WebBrowserDocumentCompletedEventArgs args)
                        {
                            evt.Set();
                        });
             
            foreach (string link in links_to_visit)
            {
                //browser.DocumentCompleted -= WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
                current_link = "Directing to: " + link;
                current_link += Environment.NewLine;
                current_link += "Website " + i + " of " + links_to_visit.ToArray().Length;
                current_link += Environment.NewLine;
                txt.Text = current_link;
                //browser.Navigate(link);

                var task = DoNavigationAsync(link);
                task.ContinueWith((t) =>
                {
                    txt.Text += Environment.NewLine;
                    txt.Text += "NAVIGATION COMPLETED";
                    scrape_dir_data();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            
                //while (browser.ReadyState != WebBrowserReadyState.Complete) { }
                //scrape_dir_data();
                //evt.WaitOne();
                //browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
                //browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser1_DocumentCompleted);
                //browser.Navigate(link);

                i++;
            }
             */
            var task = DoNavigationAsync(links_to_visit);
            task.ContinueWith((t) =>
            {
                //txt.Text += Environment.NewLine;
                txt.Text = "NAVIGATION COMPLETED";
                scrape_dir_data();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void scrape_dir_data()
        {
            var links = new List<string>();
            var table_rows = browser.Document.GetElementsByTagName("tr");
            var ii = 1;
            foreach (HtmlElement row in table_rows)
            {
                txt.Text = current_link;
                txt.Text += "Searching " + ii + " of " + table_rows.Count.ToString();
                ii++;
                var elements = row.All;
                foreach (HtmlElement we in elements)
                {
                    if (we.TagName.ToLower().Equals("td"))
                    {
                        var tdc = we.All;
                        foreach (HtmlElement nwe in tdc)
                        {
                            if (nwe.TagName.ToLower().Equals("a")
                                && nwe.GetAttribute("href").ToLower().Contains("ucdirectory"))
                            {
                                links.Add(nwe.GetAttribute("href"));
                            }
                        }
                    }
                }
            }
            txt.Text = current_link;
            txt.Text += "Acquiring saved links";
            using (var sr = new StreamReader(@"C:\Users\Scheidler\Desktop\individual_dir_links.txt"))
            {
                var line = sr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    txt.Text = current_link;
                    txt.Text += "Acquiring saved link: " + line;
                    links.Add(line);
                    line = sr.ReadLine();
                }
            }
            txt.Text = current_link;
            txt.Text += "Press button to begin scraping directory";
            using (var sw = new StreamWriter(@"C:\Users\Scheidler\Desktop\individual_dir_links.txt"))
            {
                var iii = 1;
                foreach (string l in links)
                {
                    txt.Text = current_link;
                    txt.Text += "Saving link " + iii + " of " + links.ToArray().Length;
                    sw.WriteLine(l);
                    iii++;
                }
            }
        }

        public void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            /*
            webBrowser.Document.GetElementById("product").SetAttribute("value", product);
            webBrowser.Document.GetElementById("version").SetAttribute("value", version);
            webBrowser.Document.GetElementById("commit").InvokeMember("click");
             */
            
            //scrape_dir_data();
        }

        async Task DoNavigationAsync(List<string> links)
        {
            TaskCompletionSource<bool> tcsNavigation = null;
            TaskCompletionSource<bool> tcsDocument = null;

            this.browser.Navigated += (s, e) =>
            {
                if (tcsNavigation.Task.IsCompleted)
                    return;
                tcsNavigation.SetResult(true);
            };

            this.browser.DocumentCompleted += (s, e) =>
            {
                if (this.browser.ReadyState != WebBrowserReadyState.Complete)
                    return;
                if (tcsDocument.Task.IsCompleted)
                    return;
                tcsDocument.SetResult(true);
            };

            //for (var i = 0; i <= 21; i++)
            foreach(string link in links)
            {
                tcsNavigation = new TaskCompletionSource<bool>();
                tcsDocument = new TaskCompletionSource<bool>();

                //this.browser.Navigate("http://www.example.com?i=" + i.ToString());
                this.browser.Navigate(link);
                await tcsNavigation.Task;
                //Debug.Print("Navigated: {0}", this.browser.Document.Url);
                txt.Text += Environment.NewLine;
                txt.Text+=("Navigated: "+ this.browser.Document.Url);
                // navigation completed, but the document may still be loading

                await tcsDocument.Task;
                //Debug.Print("Loaded: {0}", this.browser.DocumentText);
                txt.Text += Environment.NewLine;
                txt.Text += ("Loaded: " + link);
                // the document has been fully loaded, you can access DOM here
            }
        }

        /*
        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var webBrowser = sender as WebBrowser;
            webBrowser.DocumentCompleted -= WebBrowser1_DocumentCompleted;
            //MessageBox.Show(webBrowser.Url.ToString());
            if (e.Url.AbsolutePath.ToLower().Contains("directory")) { scrape_dir_data(); }
        }
        */

        public string scrape_classes_from_registration_semester(string semester)
        {
            var sem = semester.Split('\r')[0].Split(' ')[0] + " " +semester.Split('\r')[0].Split(' ')[1];
            
            var session = "";
            var i = 2;
            while (i < semester.Split('\r')[0].Split(' ').Length)
            {
                session += semester.Split('\r')[0].Split(' ')[i] + " ";
                i++;
            }
            session = session.Trim();

            var dates = semester.Split('\r')[1];
            try
            {
                dates = dates.Replace('(', ' ');
                dates = dates.Replace(')', ' ');
                dates = dates.Replace('\n', ' ');
            }
            catch { }
            dates = dates.Trim();
            var start_date = dates.Split('-')[0].Trim();
            var end_date = dates.Split('-')[1].Trim();
            if (start_date.ToCharArray().Length == 5)
            {
                start_date = start_date + "/2016";
            }

            var s = Environment.NewLine;
            s += "SEMESTER:" + sem;
            s += Environment.NewLine;
            s += "SESSION:" + session;
            s += Environment.NewLine;
            s += "DATES:";
            s += Environment.NewLine;
            s += "     START:" + start_date;
            s += Environment.NewLine;
            s += "     END:" + end_date;

            /*
            var split = semester.Split('\r');
            var revised_semester = "SEMESTER:";
            revised_semester += Environment.NewLine;
            revised_semester += "\t" + "semester: " + split[0].Split(' ')[0] + " " + split[0].Split(' ')[1];
            revised_semester += Environment.NewLine;
            revised_semester += "\t" + "session: " + split[0].Remove(0, split[0].Split(' ')[0].ToCharArray().Length + split[0].Remove(0, split[0].Split(' ')[1].ToCharArray().Length));
            */

            //split by line break [0] --> Semester and Session
            //                    [1] --> dates of semester



            return s;
        }
        public string scrape_classes_from_registration_class_header(string header)
        {
            var revised_header = header;

            //AIS4052 (3)APPL MEDIA METHODS
            //COURSE NUMBER, CREDIT HOURS, CLASS NAME

            var course_number = header.Split(' ')[0];
            var credit_hours = header.Split(' ')[1].Replace('(', ' ').Trim().ToCharArray()[0];
            var class_name = header.Split(')')[1].Trim();
            var s = "";
            s += "COURSE NUMBER: " + course_number;
            s += Environment.NewLine;
            s += "CREDIT HOURS:  " + credit_hours;
            s += Environment.NewLine;
            s += "NAME:          " + class_name;

            return s;
        }
        public string scrape_classes_from_registration_class_info(string class_info)
        {
            var revised_info = class_info;

            //001700681UCBALEM 6:00p - 9:00pCI Hubble, D.

            return revised_info;
        }

        public void scrape_classes_from_registration()
        {
            var table = browser.Document.GetElementsByTagName("tr");
            var row_string = "";
            var semester_session="";
            var class_header = "";
            foreach (HtmlElement row in table)
            {
                var r = row.InnerText;
                try
                {
                    if (!r.Contains("Course #  CreditsCourse Title")
                    || !r.Contains("SectionCall"))
                    {
                        //class header
                        if (r.Contains('(') && r.Contains(')')) { class_header = scrape_classes_from_registration_class_header(r.Trim()); }
                        //class info
                        if (r.ToCharArray()[0].Equals(' '))
                        {
                            row_string += Environment.NewLine;
                            row_string += "***********************************************************************************************************";
                            row_string += Environment.NewLine;
                            row_string += semester_session;
                            row_string += Environment.NewLine;
                            row_string += class_header;
                            row_string += Environment.NewLine;
                            row_string += "CLASS INFO: " + scrape_classes_from_registration_class_info(r.Trim());
                        }
                        //semester
                        if (r.Contains("Semester")) { semester_session = scrape_classes_from_registration_semester(r.Trim()); }

                        //day time frame of class
                        if (r.Contains("May ") || r.Contains("Aug ") || r.Contains("Jul ")) { row_string += r; }
                    }
                }
                catch { }
            }
            txt.Text = row_string;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Single page
            scrape_classes_from_registration();
            
            /*
            //Link Series
            current_link = "Reading links to visit:";
            txt.Text = current_link;
            List<string> links_to_visit = new List<string>();
            using (var sr = new StreamReader(@"C:\Users\Scheidler\Desktop\main_dir_links.txt"))
            {
                var line = sr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    current_link = "Reading links to visit:";
                    current_link += Environment.NewLine;
                    current_link += links_to_visit;
                    txt.Text = current_link;
                    links_to_visit.Add(line);
                    line = sr.ReadLine();
                }
            }
            visit_each_link(links_to_visit);
            */
        }

    }
}
