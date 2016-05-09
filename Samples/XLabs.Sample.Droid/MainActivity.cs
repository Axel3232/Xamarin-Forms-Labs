// ***********************************************************************
// Assembly         : XLabs.Sample.Droid
// Author           : XLabs Team
// Created          : 12-27-2015
// 
// Last Modified By : XLabs Team
// Last Modified On : 01-04-2016
// ***********************************************************************
// <copyright file="MainActivity.cs" company="XLabs Team">
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

using System.IO;
using Android.App;
using Android.Content.PM;
using Android.OS;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;
using XLabs.Caching;
using XLabs.Caching.SQLite;
using XLabs.Forms;
using XLabs.Forms.Services;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Mvvm;
using XLabs.Platform.Services;
using XLabs.Platform.Services.Email;
using XLabs.Platform.Services.Media;
using XLabs.Serialization;
using XLabs.Serialization.ServiceStack;
using Xamarin.Forms.Platform.Android;
using System;
using System.Threading.Tasks;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android.Runtime;

namespace XLabs.Sample.Droid
{
    /// <summary>
    /// Class MainActivity.
    /// </summary>
    [Activity(Label = "XLabs.Sample.Droid", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : XFormsApplicationDroid, IPermissionManager
    {

        public TaskCompletionSource<bool> PermissionGranted;
        public Task<bool> CheckPermission(string[] Permissions)
        {

            PermissionGranted = new TaskCompletionSource<bool>();

            if (((int)Android.OS.Build.VERSION.SdkInt) >= 23)
            {
                const int RequestLocationId = 0;
                bool shouldAskPermission = false;
                foreach (string perm in Permissions)
                {
                    if (ContextCompat.CheckSelfPermission(this.ApplicationContext, perm) != (int)Permission.Granted)
                    {
                        shouldAskPermission = true;
                        break;
                    }

                }

                if (shouldAskPermission)
                {
                    RunOnUiThread(() =>
                    {
                        ActivityCompat.RequestPermissions(this, Permissions, RequestLocationId);
                    });

                    return PermissionGranted.Task;
                }
                else
                {
                    return Task.FromResult<bool>(true);
                }



            }
            return Task.FromResult<bool>(true);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            foreach (var perm in grantResults)
            {
                if (perm == Permission.Denied)
                {
                    PermissionGranted.SetResult(false);
                    return;
                }
            }
            PermissionGranted.SetResult(true);
        }

        /// <summary>
        /// Called when [create].
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            //FormsAppCompatActivity.ToolbarResource = Resource.Layout.toolbar;
            //FormsAppCompatActivity.TabLayoutResource = Resource.Layout.tabs;

            base.OnCreate(bundle);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat) 
            {
                Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true); 
            }

            XLabs.Forms.Controls.HybridWebViewRenderer.GetWebViewClientDelegate = r => new CustomClient(r);
            XLabs.Forms.Controls.HybridWebViewRenderer.GetWebChromeClientDelegate = r => new CustomChromeClient();

            if (!Resolver.IsSet)
            {
                this.SetIoc();
            }
            else
            {
                var app = Resolver.Resolve<IXFormsApp>() as IXFormsApp<XFormsApplicationDroid>;
                if (app != null) app.AppContext = this;
            }

            Xamarin.Forms.Forms.Init(this, bundle);

            Xamarin.Forms.Forms.ViewInitialized += (sender, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.View.StyleId))
                {
                    e.NativeView.ContentDescription = e.View.StyleId;
                }
            };

            this.LoadApplication(new App());
        }

        /// <summary>
        /// Sets the IoC.
        /// </summary>
        private void SetIoc()
        {
            var resolverContainer = new SimpleContainer();

            var app = new XFormsAppDroid();

            app.Init(this);

            var documents = app.AppDataDirectory;
            var pathToDatabase = Path.Combine(documents, "xforms.db");

            resolverContainer.Register<IDevice>(t => AndroidDevice.CurrentDevice)
                .Register<IDisplay>(t => t.Resolve<IDevice>().Display)
                .Register<IFontManager>(t => new FontManager(t.Resolve<IDisplay>()))
                //.Register<IJsonSerializer, Services.Serialization.JsonNET.JsonSerializer>()
                .Register<IJsonSerializer, JsonSerializer>()
                .Register<IEmailService, EmailService>()
                .Register<IMediaPicker, MediaPicker>()
                .Register<ITextToSpeechService, TextToSpeechService>()
                .Register<IDependencyContainer>(resolverContainer)
                .Register<IXFormsApp>(app)
                .Register<ISecureStorage>(t => new KeyVaultStorage(t.Resolve<IDevice>().Id.ToCharArray()))
                .Register<IPermissionManager>(this)
                .Register<ICacheProvider>(
                    t => new SQLiteSimpleCache(new SQLitePlatformAndroid(),
                        new SQLiteConnectionString(pathToDatabase, true), t.Resolve<IJsonSerializer>()));


            Resolver.SetResolver(resolverContainer.GetResolver());
        }
    }
}


