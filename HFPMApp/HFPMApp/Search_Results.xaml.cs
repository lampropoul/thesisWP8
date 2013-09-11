using HFPMApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace HFPMApp
{
    public partial class Search_Results : PhoneApplicationPage
    {


        string uri;
        public string downloadedText;
        WebClient client;
        string url;
        String server_ip;
        string date;
        string type;
        string location;
        string progname;




        public Search_Results()
        {
            InitializeComponent();



            try
            {
                using (StreamReader sr = new StreamReader("server_ip.txt"))
                {
                    server_ip = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("The file could not be read:");
                MessageBox.Show(e.Message);
            }

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                app_title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                page_title.Text = "Αποτελέσματα";
            }



            // APP BAR
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/appbar.back.png", UriKind.Relative);
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") button2.Text = "πίσω";
            else button2.Text = "go back";
            ApplicationBar.Buttons.Add(button2);
            button2.Click += new EventHandler(back_Click);

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.Relative);
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") button1.Text = "κυρίως μενού";
            else button1.Text = "main menu";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(main_menu_Click);


            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem1.Text = "Έξοδος (" + PhoneApplicationService.Current.State["Username"] + ")";
            else menuItem1.Text = "Logout (" + PhoneApplicationService.Current.State["Username"] + ")";
            ApplicationBar.MenuItems.Add(menuItem1);
            menuItem1.Click += new EventHandler(logout_Click);


            client = new WebClient();
            client.UploadStringCompleted += client_UploadStringCompleted;


        }





        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            loadingProgressBar.IsVisible = true;


            string user_id = PhoneApplicationService.Current.State["UserId"].ToString();
            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);



            // retrieve parameters from Search.xaml

            NavigationContext.QueryString.TryGetValue("date", out date);
            NavigationContext.QueryString.TryGetValue("type", out type);
            NavigationContext.QueryString.TryGetValue("location", out location);
            NavigationContext.QueryString.TryGetValue("progname", out progname);


            if (type == "Όλα") date = "All";
            if (location == "Όλα") location = "All";
            if (progname == "Όλα") progname = "All";

            if (Convert.ToBoolean(PhoneApplicationService.Current.State["hasInternet"]))
            {



                RootObjectSearch jsonObject = new RootObjectSearch();
                jsonObject.date = date;
                jsonObject.location = location;
                jsonObject.type = type;
                jsonObject.progname = progname;

                // REST CALL

                string json_to_send = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, new JsonSerializerSettings { });

                client.Headers["Method"] = "POST";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/search/id/" + user_id + "/randomnum/" + rand;
                client.UploadStringAsync(new Uri(url), json_to_send);



            }
            // an den exw intenet, apo ton Program ths topikis
            else
            {


                using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                {

                    // -------------------------------------------------------------------//
                    // -------------------------- LOCAL DATABASE -------------------------//
                    // -------------------------------------------------------------------//

                    db.CreateIfNotExists();
                    db.LogDebug = true;




                    IEnumerable<Program> query = from prog in db.Program
                                                 where prog.Userid == user_id
                                                 select prog;


                    if (date != "Any")
                    {
                        if (type != "All" && location != "All" && progname != "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Dutytype == type
                                       && prog.Location == location
                                       && prog.Progname == progname
                                       && prog.Date == date
                                    select prog;
                        }
                        if (type != "All" && location != "All" && progname == "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Dutytype == type
                                       && prog.Location == location
                                       && prog.Date == date
                                    select prog;
                        }
                        if (type != "All" && location == "All" && progname != "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Dutytype == type
                                       && prog.Progname == progname
                                       && prog.Date == date
                                    select prog;
                        }
                        if (type != "All" && location == "All" && progname == "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Dutytype == type
                                       && prog.Date == date
                                    select prog;
                        }
                        if (type == "All" && location != "All" && progname != "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Location == location
                                       && prog.Progname == progname
                                       && prog.Date == date
                                    select prog;
                        }
                        if (type == "All" && location != "All" && progname == "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Location == location
                                       && prog.Date == date
                                    select prog;
                        }
                        if (type == "All" && location == "All" && progname != "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Progname == progname
                                       && prog.Date == date
                                    select prog;
                        }
                        if (type == "All" && location == "All" && progname == "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Date == date
                                    select prog;
                        }

                    }
                    else
                    {
                        if (type != "All" && location != "All" && progname != "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Dutytype == type
                                       && prog.Location == location
                                       && prog.Progname == progname
                                    select prog;
                        }
                        if (type != "All" && location != "All" && progname == "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Dutytype == type
                                       && prog.Location == location
                                    select prog;
                        }
                        if (type != "All" && location == "All" && progname != "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Dutytype == type
                                       && prog.Progname == progname
                                    select prog;
                        }
                        if (type != "All" && location == "All" && progname == "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Dutytype == type
                                    select prog;
                        }
                        if (type == "All" && location != "All" && progname != "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Location == location
                                       && prog.Progname == progname
                                    select prog;
                        }
                        if (type == "All" && location != "All" && progname == "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Location == location
                                    select prog;
                        }
                        if (type == "All" && location == "All" && progname != "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                       && prog.Progname == progname
                                    select prog;
                        }
                        if (type == "All" && location == "All" && progname == "All")
                        {
                            query = from prog in db.Program
                                    where prog.Userid == user_id
                                    select prog;
                        }
                    }



                    List<string> StringsList = new List<string> { };

                    int r = 0;
                    foreach (Program pr in query)
                    {
                        r++;


                        string[] words = pr.Date.Split('-');
                        DateTime bt = new DateTime(Convert.ToInt32(words[0]), Convert.ToInt32(words[1]), Convert.ToInt32(words[2]));
                        

                        if (bt >= DateTime.Now)
                        {

                            string item = r.ToString() + ". " + pr.Progname + "\n    " + pr.Date + "\n    " + pr.Start + " - " + pr.End + "\n    " + pr.Dutytype + "\n    " + pr.Location;

                            StringsList.Add(item);
                        }
                    }



                    //MessageBox.Show(r + " results.");
                    
                    
                    // populate list
                    search_results.ItemsSource = StringsList;


                }// using


                loadingProgressBar.IsVisible = false;


            }




        }





        private void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {

                this.downloadedText = e.Result;


                //LongListSelector search_results = new LongListSelector();
                List<string> StringsList = new List<string> {};
                

                RootObject jsonObject = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);
                int length = jsonObject.programs.Count;


                //MessageBox.Show(length.ToString() + " results.");


                for (int i = 0; i < length; i++)
                {

                    
                    string date = jsonObject.programs[i].date;
                    string duty_type = jsonObject.programs[i].duty_type;
                    string duty_start_time = jsonObject.programs[i].duty_start_time;
                    string duty_end_time = jsonObject.programs[i].duty_end_time;
                    string location = jsonObject.programs[i].location;
                    string program_name = jsonObject.programs[i].program_name;

                    int num = i+1;
                    string item = num.ToString() + ". " + program_name + "\n    " + date + "\n    " + duty_start_time + " - " + duty_end_time + "\n    " + duty_type + "\n    " + location;
                    if (i != length - 1) item += "\n    -----------------------------------";

                    StringsList.Add(item);

                }


                // populate list
                search_results.ItemsSource = StringsList;

                loadingProgressBar.IsVisible = false;


                if (length == 0)
                {
                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Δεν βρέθηκε κάτι.");
                    else MessageBox.Show("No results.");
                    
                    uri = "/Search.xaml";
                    NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                }

            }
            catch (TargetInvocationException ex)
            {

                loadingProgressBar.IsVisible = false;

                if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Δεν βρέθηκε κάτι.");
                else MessageBox.Show("No results.");
                //MessageBox.Show("TargetInvocationException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);

                uri = "/Search.xaml";
                NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
            }
            catch (WebException ex)
            {

                loadingProgressBar.IsVisible = false;

                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }
        }












        private void main_menu_Click(object sender, EventArgs e)
        {

            uri = "/MainMenuPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }


        private void back_Click(object sender, EventArgs e)
        {

            uri = "/Search.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }


        private void logout_Click(object sender, EventArgs e)
        {

            PhoneApplicationService.Current.State["Username"] = null;


            using (StreamWriter writer = new StreamWriter("already_logged.txt"))
            {
                writer.Write(PhoneApplicationService.Current.State["Username"]);
                writer.Close();
            }

            uri = "/MainPage.xaml?logout=true";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }






        // the classes which contain the properties of the specific JSON response
        public class RootObject
        {
            public List<Programs> programs { get; set; }
        }




        public class Programs
        {
            public int user_id { get; set; }
            public int id { get; set; }
            public string date { get; set; }
            public string duty_type { get; set; }
            public string duty_start_time { get; set; }
            public string duty_end_time { get; set; }
            public string location { get; set; }
            public string program_name { get; set; }
            public string error { get; set; }
            public string message { get; set; }
        }




        public class RootObjectSearch
        {
            public string date { get; set; }
            public string type { get; set; }
            public string location { get; set; }
            public string progname { get; set; }
        }






    }
}