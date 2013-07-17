using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

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

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("menu_button.gif", UriKind.Relative);
            button1.Text = "Main Menu";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(main_menu_Click);

            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
                menuItem1.Text = "Έξοδος";
            else
                menuItem1.Text = "Logout";

            ApplicationBar.MenuItems.Add(menuItem1);
            menuItem1.Click += new EventHandler(logout_Click);

            ApplicationBarMenuItem menuItem2 = new ApplicationBarMenuItem();

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
                menuItem2.Text = "Εκκαθάριση περασμένων καθηκόντων";
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

                // -------------------------------------------------------------------//
                // -------------------------- LOCAL DATABASE -------------------------//
                // -------------------------------------------------------------------//

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
                    MessageBox.Show("Deleted.");
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
            string uri = "/MainPage.xaml?logout=true";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }

        private void main_menu_Click(object sender, EventArgs e)
        {

            uri = "/MainMenuPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }
        

    }
}