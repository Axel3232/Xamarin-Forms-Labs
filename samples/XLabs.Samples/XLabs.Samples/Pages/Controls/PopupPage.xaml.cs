// ***********************************************************************
// Assembly         : XLabs.Sample
// Author           : XLabs Team
// Created          : 12-27-2015
// 
// Last Modified By : XLabs Team
// Last Modified On : 01-04-2016
// ***********************************************************************
// <copyright file="PopupPage.xaml.cs" company="XLabs Team">
//     Copyright (c) XLabs Team. All rights reserved.
// </copyright>
// <summary>
//       This project is licensed under the Apache 2.0 license
//       https://github.com/XLabs/Xamarin-Forms-Labs/blob/master/LICENSE
//       
//       XLabs is a open source project that aims to provide a powerfull and cross 
//       platform set of controls tailored to work with Xamarin Forms.
// </summary>
// ***********************************************************************
// 

using System;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace XLabs.Samples.Pages.Controls
{
    public partial class PopupPage : ContentPage
    {
        public PopupPage()
        {
            InitializeComponent();

            this.OpenButton.Clicked += OpenButtonClicked;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Content.BackgroundColor = Color.Blue;
            
        }

        void OpenButtonClicked(object sender, EventArgs e)
        {
            var popupLayout = this.Content as PopupLayout;

            if (popupLayout.IsPopupActive)
            {
                popupLayout.DismissPopup();
            }
            else
            {
                var list = new WrapLayout()
                {
                    BackgroundColor = Color.White,
                    HeightRequest = this.Height * .5,
                    WidthRequest = this.Width * .8,
                    Orientation = StackOrientation.Horizontal
                };
                Random random = new Random(200);
                for (int i = 0; i < 10; i++)
                {
                    list.Children.Add(new Label() { Text = "nb : " + i, BackgroundColor = new Color(random.NextDouble(), random.NextDouble(), random.NextDouble()).WithLuminosity(.8).WithSaturation(0.8)  });
                }
               
                Button close = new Button() { Text = "Close" };
                close .Clicked+= (s, args) => 
                    popupLayout.DismissPopup();
                list.Children.Add(close);
                popupLayout.ShowPopup(list);
            }
        }
    }
}
