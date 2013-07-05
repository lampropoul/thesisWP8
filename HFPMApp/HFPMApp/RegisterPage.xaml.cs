using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Notification;
using System.Text;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace HFPMApp
{
    public partial class RegisterPage : PhoneApplicationPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }



        private void createNewUser(object sender, RoutedEventArgs e)
        {

            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {
                db.CreateIfNotExists();
                db.LogDebug = true;

                // new user


                if (password1.Password == password2.Password)
                {

                    try
                    {
                        Users newUser = new Users()
                        {
                            Username = username.Text,
                            Password = password1.Password,
                            Userid = Convert.ToInt32(id.Text, 2),
                            Nameuser = name.Text,
                            Surnameuser = surname.Text,
                            Userteam = Convert.ToInt32(userteam.Text, 2),
                            Amka = amka.Text,
                            Department = department.Text
                        };

                        db.Users.InsertOnSubmit(newUser);
                        // Data is not saved to the phone database until the SubmitChanges method is called.
                        db.SubmitChanges();

                        //ShellToast toast = new ShellToast();

                    }
                    catch (FormatException exc)
                    {
                        //Console.WriteLine(exc.Message);
                        errorbox.Text = "FormatException: " + exc.Message;
                        ShellToast toast = new ShellToast();
                        toast.Title = "Warning";
                        toast.Content = "FormatException: " + exc.Message;
                        toast.Show();
                    }
                    catch (Exception exc)
                    {
                        //Console.WriteLine(exc.Message);
                        errorbox.Text = "Exception: " + exc.Message;
                    }
                    finally
                    {
                        // perform any cleanup
                    }
                }
                else
                {
                    errorbox.Text = "Passwords not matched!";
                    //password1.Focus();

                    ShellToast toast = new ShellToast();
                    toast.Title = "Warning";
                    toast.Content = "Passwords not matched!";
                    toast.Show();
                }
                
            }





        }
    }
}