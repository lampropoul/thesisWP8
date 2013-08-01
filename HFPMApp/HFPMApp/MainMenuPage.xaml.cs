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

    


    public partial class MainMenuPage : PhoneApplicationPage
    {

        public string downloadedText;
        WebClient client;
        WebClient client_up;
        string url;
        string url_post;
        string uri;
        string json_to_send = "";
        string clear_old = "";


        public MainMenuPage()
        {
            InitializeComponent();

            // APP BAR
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;



            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem1.Text = "Έξοδος (" + PhoneApplicationService.Current.State["Username"] + ")";
            else menuItem1.Text = "Logout (" + PhoneApplicationService.Current.State["Username"] + ")";
            ApplicationBar.MenuItems.Add(menuItem1);
            menuItem1.Click += new EventHandler(logout_Click);

            ApplicationBarMenuItem menuItem2 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem2.Text = "Εκκαθάριση καθηκόντων";
            else menuItem2.Text = "Clear old entries";
            ApplicationBar.MenuItems.Add(menuItem2);
            menuItem2.Click += new EventHandler(clear_old_entries_Click);

            ApplicationBarMenuItem menuItem3 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem3.Text = "Ρυθμίσεις για τα αιτήματα.";
            else menuItem3.Text = "Settings for declared";
            ApplicationBar.MenuItems.Add(menuItem3);
            menuItem3.Click += new EventHandler(settings_Click);


            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                app_title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                page_title.Text = "Κεντρικό Μενού";

                pers_prog_button.Content = "Πρόγραμμα";
                search_button.Content = "Αναζήτηση";
                declared_button.Content = "Αιτήματα";
                edit_button.Content = "Επεξεργασία";
            }
            
        }




        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            
            // TODO

            // edw erxomai otan kanw logout kai pataw to hardware back
            // 8elei dior8wsi

            // pairnw ta parameters (username kai password) apo tin MainPage.xaml
            string given_username = PhoneApplicationService.Current.State["Username"].ToString();


            

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




        private void settings_Click(object sender, EventArgs e)
        {
            uri = "/Settings.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }




        /// <summary>
        /// ka8arizw ton program kai ton change_list tis topikis basis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clear_old_entries_Click(object sender, EventArgs e)
        {


            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {

                // -------------------------------------------------------------------//
                // -------------------------- LOCAL DATABASE -------------------------//
                // -------------------------------------------------------------------//

                db.CreateIfNotExists();
                db.LogDebug = true;



                // === PROGRAM ===

                IEnumerable<Program> query =
                        from prog in db.Program
                        where prog.Userid == PhoneApplicationService.Current.State["UserId"].ToString() && Convert.ToDateTime(prog.Date) < DateTime.Now
                        select prog;

                // delete
                foreach (Program pr in query)
                {
                    db.Program.DeleteOnSubmit(pr);
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




                // === CHANGE_LIST ===
                
                IEnumerable<Change_list> query2 =
                        from change in db.Change_list
                        where change.Userid == Convert.ToInt32(PhoneApplicationService.Current.State["UserId"]) && Convert.ToDateTime(change.RequestDate) < DateTime.Now
                        select change;

                // delete
                foreach (Change_list ch in query2)
                {
                    db.Change_list.DeleteOnSubmit(ch);
                }

                // changes do not take place until SubmitChanges method is called
                try
                {
                    db.SubmitChanges();
                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Διαγράφηκαν.");
                    else MessageBox.Show("Cleared.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }


        }



       

        private void pers_prog_button_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Personal_Program.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void search_button_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Search.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void declared_button_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Declared.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void edit_button_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Edit.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        


    }
}