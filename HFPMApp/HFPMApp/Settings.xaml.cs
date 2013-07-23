﻿using HFPMApp.Resources;
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
    public partial class Settings : PhoneApplicationPage
    {

        string uri;


        public Settings()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                settings.Text = "Ρυθμίσεις";

                list_all_departments.Content = "Όλα";
                list_call.Content = "Εφημερία";
                list_shift.Content = "Βάρδια";
                list_everyday.Content = "Καθημερινά Ιατρεία";

                list_all_types.Content = "Όλα";
                list_swtiria.Content = "ΣΩΤΗΡΙΑ";
                list_ges.Content = "ΓΕΣ";

                submit_button.Content = "Ολοκλήρωση";

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

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
                menuItem1.Text = "Έξοδος (" + PhoneApplicationService.Current.State["Username"] + ")";
            else
                menuItem1.Text = "Logout (" + PhoneApplicationService.Current.State["Username"] + ")";

            ApplicationBar.MenuItems.Add(menuItem1);
            menuItem1.Click += new EventHandler(logout_Click);

            ApplicationBarMenuItem menuItem2 = new ApplicationBarMenuItem();

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
                menuItem2.Text = "Εκκαθάριση καθηκόντων";
            else
                menuItem2.Text = "Clear old entries";

            ApplicationBar.MenuItems.Add(menuItem2);


        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {

                // -------------------------------------------------------------------//
                // -------------------------- LOCAL DATABASE -------------------------//
                // -------------------------------------------------------------------//

                db.CreateIfNotExists();
                db.LogDebug = true;


                
                IEnumerable<Declared_types> query1 =
                            from types in db.Declared_types
                            //where student.Scores[0] > 90
                            select types;


                //MessageBox.Show("Query 1: " + query);
                
                foreach (Declared_types dectyp in query1)
                {
                    types.SelectedIndex = Int32.Parse(dectyp.Type);
                }


                IEnumerable<Declared_locations> query2 =
                            from locations in db.Declared_locations
                            //where student.Scores[0] > 90
                            select locations;

                foreach (Declared_locations decloc in query2)
                {
                    departments.SelectedIndex = Int32.Parse(decloc.Location);
                }



            }

        }




        private void selectionSubmitted(object sender, RoutedEventArgs e)
        {
            

            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {

                // ------------------------------------------------------------------- //
                // -------------------------- LOCAL DATABASE ------------------------- //
                // ------------------------------------------------------------------- //

                db.CreateIfNotExists();
                db.LogDebug = true;


                // retreive user id from table users

                IEnumerable<Users> query =
                            from user in db.Users
                            where user.Username == PhoneApplicationService.Current.State["Username"].ToString()
                            select user;

                int id=0;
                foreach (Users us in query)
                {
                    id = us.Userid;
                }



                IEnumerable<Declared_types> query2 =
                            from types in db.Declared_types
                            where types.Userid == id
                            select types;


                foreach (Declared_types dectyp in query2)
                {
                    db.Declared_types.DeleteOnSubmit(dectyp);
                }


                IEnumerable<Declared_locations> query3 =
                            from locations in db.Declared_locations
                            where locations.Userid == id
                            select locations;


                foreach (Declared_locations decloc in query3)
                {
                    db.Declared_locations.DeleteOnSubmit(decloc);
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




                if (id!=0)
                {
                    // insert entry in DB
                    db.Declared_types.InsertOnSubmit(new Declared_types
                    {
                        Userid = id,
                        Type = types.SelectedIndex.ToString()
                    });

                    db.Declared_locations.InsertOnSubmit(new Declared_locations
                    {
                        Userid = id,
                        Location = departments.SelectedIndex.ToString()
                    });
                }


                // changes do not take place until SubmitChanges method is called
                try
                {
                    db.SubmitChanges();
                    MessageBox.Show("OK.");
                    string uri = "/MainMenuPage.xaml";
                    NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                


            }


        }// end selectionSubmitted





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

        private void main_menu_Click(object sender, EventArgs e)
        {

            uri = "/MainMenuPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }


        private void back_Click(object sender, EventArgs e)
        {

            NavigationService.GoBack();
        }

        

    }
}