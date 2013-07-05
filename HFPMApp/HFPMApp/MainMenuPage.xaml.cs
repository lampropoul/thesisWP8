using System;
using System.Collections.Generic;
//using System.Data.SqlServerCe;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;


namespace HFPMApp
{
    public partial class MainMenuPage : PhoneApplicationPage
    {
        public MainMenuPage()
        {
            InitializeComponent();

        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //IDictionary<string, string> parameters = this.NavigationContext.QueryString;

            // pairnw ta parameters (username kai password) apo tin MainPage.xaml
            string given_username = this.NavigationContext.QueryString["username"];
            string given_password = this.NavigationContext.QueryString["password"];


            // twra kanw o,ti 8elw ta username kai password

            uname.Text = given_username;
            pwd.Text = given_password;
            

            

        }
    }
}