using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Diagnostics;
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

namespace ScheduledTaskAgent1
{
    public class ScheduledAgent : ScheduledTaskAgent
    {

        WebClient client;
        string url = String.Empty;
        string server_ip = String.Empty;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });

            
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background

            try
            {
                using (StreamReader sr = new StreamReader("server_ip.txt"))
                {
                    server_ip = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("The file could not be read:");
                MessageBox.Show(e.Message);
            }


            // CLIENT
            //client = new WebClient();
            //client.DownloadStringCompleted += client_DownloadStringCompleted;

            //Random rnd = new Random();
            //int rand = rnd.Next(1, 10000);
            //string user_id = PhoneApplicationService.Current.State["UserId"].ToString();

            //// REST Call
            //url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/notifications/id/" + user_id + "/randomnum/" + rand;
            //client.DownloadStringAsync(new Uri(url));



            string toastMessage = "";

            // If your application uses both PeriodicTask and ResourceIntensiveTask
            // you can branch your application code here. Otherwise, you don't need to.
            if (task is PeriodicTask)
            {
                // Execute periodic task actions here.
                toastMessage = "Periodic task running.";
            }
            else
            {
                // Execute resource-intensive task actions here.
                toastMessage = "Resource-intensive task running.";
            }

            // Launch a toast to show that the agent is running.
            // The toast will not be shown if the foreground application is running.
            ShellToast toast = new ShellToast();
            toast.Title = "Background Agent Sample";
            toast.Content = toastMessage;
            toast.Show();

            IconicTileData oIcontile = new IconicTileData();
            oIcontile.Title = "HFPM Notifications";
            oIcontile.Count = 2;

            oIcontile.IconImage = new Uri("Assets/Tiles/110x110.png", UriKind.Relative);
            oIcontile.SmallIconImage = new Uri("Assets/Tiles/220x220.png", UriKind.Relative);

            oIcontile.WideContent1 = "windows phone 8 Live tile";
            oIcontile.WideContent2 = "Icon tile";
            oIcontile.WideContent3 = "All about Live tiles By WmDev";

            oIcontile.BackgroundColor = System.Windows.Media.Colors.Orange;

            // find the tile object for the application tile that using "Iconic" contains string in it.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("Iconic".ToString()));

            if (TileToFind != null && TileToFind.NavigationUri.ToString().Contains("Iconic"))
            {
                TileToFind.Delete();
                ShellTile.Create(new Uri("/MainPage.xaml?id=Iconic", UriKind.Relative), oIcontile, true);
            }
            else
            {
                ShellTile.Create(new Uri("/MainPage.xaml?id=Iconic", UriKind.Relative), oIcontile, true);
            }

            // If debugging is enabled, launch the agent again in one minute.
            #if DEBUG_AGENT
              ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(20));
            #endif


            NotifyComplete();
        }



        static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }


    }
}