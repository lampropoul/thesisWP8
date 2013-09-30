using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Phone;

namespace HFPMApp
{
    public partial class Library : PhoneApplicationPage
    {
        public Library()
        {
            InitializeComponent();

            MediaLibrary library = new MediaLibrary();
            List<Image> PicList = new List<Image> { };

            foreach (var r in library.Pictures)
            {
                Stream imageStream = r.GetImage();

                var imageToShow = new Image()
                {
                    Source = PictureDecoder.DecodeJpeg(r.GetImage())
                };

                PicList.Add(imageToShow);
            }

            pictures_list.ItemsSource = PicList;


        }
    }
}