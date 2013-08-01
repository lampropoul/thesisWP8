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
    public partial class Search : PhoneApplicationPage
    {

        string uri;
        public string downloadedText;
        WebClient client;
        string url;
        String server_ip;

        
        public Search()
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
                page_title.Text = "Αναζήτηση";
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


            // CLIENTS
            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;

            


            any_date.Checked += any_date_Checked;
            any_date.Unchecked += any_date_Unchecked;

        }

        void any_date_Unchecked(object sender, RoutedEventArgs e)
        {

            pick_date.Visibility = Visibility.Visible;

        }

        void any_date_Checked(object sender, RoutedEventArgs e)
        {

            pick_date.Visibility = Visibility.Collapsed;

        }





        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            loadingProgressBar.IsVisible = true;

            string user_id = PhoneApplicationService.Current.State["UserId"].ToString();
            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);
            
            
            
            // kalw gia na gemisw tis listes



            ListPickerItem new_item1 = new ListPickerItem();
            list_duties.Items.Add(new_item1);
            new_item1.Content = "All";

            ListPickerItem new_item2 = new ListPickerItem();
            list_locations.Items.Add(new_item2);
            new_item2.Content = "All";

            ListPickerItem new_item3 = new ListPickerItem();
            list_program_names.Items.Add(new_item3);
            new_item3.Content = "All";


            if (Convert.ToBoolean(PhoneApplicationService.Current.State["hasInternet"]))
            {



                // REST CALL to populate lists
                url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/populatesearch/id/" + user_id + "/randomnum/" + rand;
                client.DownloadStringAsync(new Uri(url));



            }
            // an den exw intenet, apo to declared_duties kai declared_locations ths topikis
            else
            {


                using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                {

                    // -------------------------------------------------------------------//
                    // -------------------------- LOCAL DATABASE -------------------------//
                    // -------------------------------------------------------------------//

                    db.CreateIfNotExists();
                    db.LogDebug = true;




                    IEnumerable<Declared_types> query1 =
                                    from types in db.Declared_types
                                    select types;


                    foreach (Declared_types dt in query1)
                    {
                        // populate list
                        ListPickerItem new_item = new ListPickerItem();
                        list_duties.Items.Add(new_item);
                        new_item.Content = dt.Type;
                    }


                    IEnumerable<Declared_locations> query2 =
                                    from locations in db.Declared_locations
                                    select locations;


                    foreach (Declared_locations dl in query2)
                    {
                        // populate list
                        ListPickerItem new_item = new ListPickerItem();
                        list_locations.Items.Add(new_item);
                        new_item.Content = dl.Location;
                    }


                    IEnumerable<Program_names> query3 =
                                    from names in db.Program_names
                                    select names;


                    foreach (Program_names pn in query3)
                    {
                        // populate list
                        ListPickerItem new_item = new ListPickerItem();
                        list_program_names.Items.Add(new_item);
                        new_item.Content = pn.Name;
                    }


                }

                loadingProgressBar.IsVisible = false;

            }



        }





        // function that retreives json data from web server
        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            
            try
            {

                this.downloadedText = e.Result;
                
                // decode JSON
                RootObjectPop jsonObject = JsonConvert.DeserializeObject<RootObjectPop>(this.downloadedText);


                int duty_types_count = jsonObject.duty_types.Count;
                int locations_count = jsonObject.locations.Count;
                int program_names_count = jsonObject.program_names.Count;


                // logged via internet. now delete and insert to local db
                using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                {

                    // -------------------------------------------------------------------//
                    // -------------------------- LOCAL DATABASE -------------------------//
                    // -------------------------------------------------------------------//

                    db.CreateIfNotExists();
                    db.LogDebug = true;



                    // delete all programs just to fetch the (possibly) updated ones
                    IEnumerable<Declared_types> query1 =
                                    from types in db.Declared_types
                                    select types;

                    // delete
                    foreach (Declared_types dt in query1)
                    {
                        db.Declared_types.DeleteOnSubmit(dt);
                    }

                    // changes do not take place until SubmitChanges method is called
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                    
                    

                    for (int i = 0; i < duty_types_count; i++)
                    {


                        int user_id = Convert.ToInt32(jsonObject.duty_types[i].user_id);
                        string duty_type = jsonObject.duty_types[i].duty_type;


                        // populate list
                        ListPickerItem new_item = new ListPickerItem();
                        list_duties.Items.Add(new_item);
                        new_item.Content = duty_type;

                        db.Declared_types.InsertOnSubmit(new Declared_types
                        {
                            Userid = i+1,
                            Type = duty_type
                        });



                        // changes do not take place until SubmitChanges method is called
                        try
                        {
                            db.SubmitChanges();
                            //MessageBox.Show("Program with id=" + program_id + " inserted.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }




                    }// for

                    //MessageBox.Show("Loaded.");



                    

                    // delete all programs just to fetch the (possibly) updated ones
                    IEnumerable<Declared_locations> query2 =
                                    from locations in db.Declared_locations
                                    select locations;

                    // delete
                    foreach (Declared_locations dl in query2)
                    {
                        db.Declared_locations.DeleteOnSubmit(dl);
                    }

                    // changes do not take place until SubmitChanges method is called
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                    
                    

                    for (int i = 0; i < locations_count; i++)
                    {


                        int user_id = Convert.ToInt32(jsonObject.locations[i].user_id);
                        string location = jsonObject.locations[i].location;


                        // populate list
                        ListPickerItem new_item = new ListPickerItem();
                        list_locations.Items.Add(new_item);
                        new_item.Content = location;

                        db.Declared_locations.InsertOnSubmit(new Declared_locations
                        {
                            Userid = i+1,
                            Location = location
                        });



                        // changes do not take place until SubmitChanges method is called
                        try
                        {
                            db.SubmitChanges();
                            //MessageBox.Show("Program with id=" + program_id + " inserted.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }




                    }// for



                    

                    // delete all programs just to fetch the (possibly) updated ones
                    IEnumerable<Program_names> query3 =
                                    from names in db.Program_names
                                    select names;

                    // delete
                    foreach (Program_names pn in query3)
                    {
                        db.Program_names.DeleteOnSubmit(pn);
                    }

                    // changes do not take place until SubmitChanges method is called
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }



                    

                    for (int i = 0; i < program_names_count; i++)
                    {


                        string program_name = jsonObject.program_names[i];


                        // populate list
                        ListPickerItem new_item = new ListPickerItem();
                        list_program_names.Items.Add(new_item);
                        new_item.Content = program_name;

                        db.Program_names.InsertOnSubmit(new Program_names
                        {
                            Name = program_name
                        });



                        // changes do not take place until SubmitChanges method is called
                        try
                        {
                            db.SubmitChanges();
                            //MessageBox.Show("Program with id=" + program_id + " inserted.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }


                        

                    }// for


                }// using

                loadingProgressBar.IsVisible = false;


            }
            catch (TargetInvocationException ex)
            {

                loadingProgressBar.IsVisible = false;

                MessageBox.Show("TargetInvocationException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {

                loadingProgressBar.IsVisible = false;

                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }




        }







        private void search_Click(object sender, RoutedEventArgs e)
        {

            loadingProgressBar.IsVisible = true;

            string user_id = PhoneApplicationService.Current.State["UserId"].ToString();
            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);


            

            pick_date.ValueStringFormat = "{0:yyyy-MM-dd}";

            string date = pick_date.ValueString;
            if (Convert.ToBoolean(any_date.IsChecked))
            {
                date = "Any";
            }
            string type = "All";
            string location = "All";
            string progname = "All";




            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {

                // -------------------------------------------------------------------//
                // -------------------------- LOCAL DATABASE -------------------------//
                // -------------------------------------------------------------------//

                db.CreateIfNotExists();
                db.LogDebug = true;




                IEnumerable<Declared_types> query =
                                from types in db.Declared_types
                                where types.Userid == Convert.ToInt32(list_duties.SelectedIndex)
                                select types;


                foreach (Declared_types dt in query)
                {

                    type = dt.Type;
                }




                IEnumerable<Declared_locations> query2 =
                                from locations in db.Declared_locations
                                where locations.Userid == Convert.ToInt32(list_locations.SelectedIndex)
                                select locations;


                foreach (Declared_locations dl in query2)
                {

                    location = dl.Location;
                }




                IEnumerable<Program_names> query3 =
                                from progs in db.Program_names
                                select progs;


                int i = 1;
                foreach (Program_names pr in query3)
                {
                    if (list_program_names.SelectedIndex == i)
                    {
                        progname = pr.Name;
                    }
                    i++;
                }



            }// using


            loadingProgressBar.IsVisible = false;

            // TODO
            // navigate to search results

            uri = "/Search_Results.xaml?date=" + date + "&type=" + type + "&location=" + location + "&progname=" + progname;
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));



        }







        private void main_menu_Click(object sender, EventArgs e)
        {

            uri = "/MainMenuPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }


        private void back_Click(object sender, EventArgs e)
        {

            uri = "/MainMenuPage.xaml";
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
        public class RootObjectPop
        {
            public List<Types> duty_types { get; set; }
            public List<Locations> locations { get; set; }
            public List<String> program_names { get; set; }
        }



        public class Types
        {
            public string user_id;
            public string duty_type;
        }

        public class Locations
        {
            public string user_id;
            public string location;
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