﻿#pragma checksum "C:\Users\Basilis\documents\visual studio 2012\Projects\HFPMApp\HFPMApp\Camera.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A3CB19BCB5562AA283924C1B47B50EB8"
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
    
    
    public partial class Camera : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Canvas viewfinderCanvas;
        
        internal System.Windows.Media.VideoBrush viewfinderBrush;
        
        internal System.Windows.Controls.TextBlock focusBrackets;
        
        internal System.Windows.Controls.Button ShutterButton;
        
        internal System.Windows.Controls.Button FlashButton;
        
        internal System.Windows.Controls.Button AFButton;
        
        internal System.Windows.Controls.Button ResButton;
        
        internal System.Windows.Controls.TextBlock txtDebug;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/HFPMApp;component/Camera.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.viewfinderCanvas = ((System.Windows.Controls.Canvas)(this.FindName("viewfinderCanvas")));
            this.viewfinderBrush = ((System.Windows.Media.VideoBrush)(this.FindName("viewfinderBrush")));
            this.focusBrackets = ((System.Windows.Controls.TextBlock)(this.FindName("focusBrackets")));
            this.ShutterButton = ((System.Windows.Controls.Button)(this.FindName("ShutterButton")));
            this.FlashButton = ((System.Windows.Controls.Button)(this.FindName("FlashButton")));
            this.AFButton = ((System.Windows.Controls.Button)(this.FindName("AFButton")));
            this.ResButton = ((System.Windows.Controls.Button)(this.FindName("ResButton")));
            this.txtDebug = ((System.Windows.Controls.TextBlock)(this.FindName("txtDebug")));
        }
    }
}

