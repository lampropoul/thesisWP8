﻿#pragma checksum "C:\Users\Basilis\documents\visual studio 2012\Projects\HFPMApp\HFPMApp\MainPage_GR.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "00B485F7CBCCB41A4552CE2F2F51F59A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace HFPMApp {
    
    
    public partial class MainPage_GR : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock app_title;
        
        internal System.Windows.Controls.TextBlock page_title;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBox username;
        
        internal System.Windows.Controls.PasswordBox password;
        
        internal System.Windows.Controls.TextBlock uname;
        
        internal System.Windows.Controls.TextBlock pwd;
        
        internal System.Windows.Controls.Button login_btn;
        
        internal System.Windows.Controls.Button gr;
        
        internal System.Windows.Controls.Button en;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/HFPMApp;component/MainPage_GR.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.app_title = ((System.Windows.Controls.TextBlock)(this.FindName("app_title")));
            this.page_title = ((System.Windows.Controls.TextBlock)(this.FindName("page_title")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.username = ((System.Windows.Controls.TextBox)(this.FindName("username")));
            this.password = ((System.Windows.Controls.PasswordBox)(this.FindName("password")));
            this.uname = ((System.Windows.Controls.TextBlock)(this.FindName("uname")));
            this.pwd = ((System.Windows.Controls.TextBlock)(this.FindName("pwd")));
            this.login_btn = ((System.Windows.Controls.Button)(this.FindName("login_btn")));
            this.gr = ((System.Windows.Controls.Button)(this.FindName("gr")));
            this.en = ((System.Windows.Controls.Button)(this.FindName("en")));
        }
    }
}

