using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Util;
using Android.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using XLabs.Enums;
using XLabs.Ioc;
using XLabs.Platform.Mvvm;

namespace XLabs.Platform.Device
{
    public class DeviceOrientation : IDeviceOrientation
    {
        public event EventHandler<EventArgs<CurrentOrientation>> ScreenOrientationChanged;

       

        public CurrentOrientation GetOrientation()
        {
           

            using (var wm = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>())
            using (var dm = new DisplayMetrics())
            {
                
                var rotation = wm.DefaultDisplay.Rotation;
                wm.DefaultDisplay.GetMetrics(dm);

                var width = dm.WidthPixels;
                var height = dm.HeightPixels;

                if (height > width && (rotation == SurfaceOrientation.Rotation0 || rotation == SurfaceOrientation.Rotation180) ||
                    width > height && (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270))
                {
                    switch (rotation)
                    {
                        case SurfaceOrientation.Rotation0:
                            return new CurrentOrientation(Orientation.Portrait);
                        case SurfaceOrientation.Rotation90:
                            return new CurrentOrientation(Orientation.LandscapeLeft);
                        case SurfaceOrientation.Rotation180:
                            return new CurrentOrientation(Orientation.PortraitDown);
                        case SurfaceOrientation.Rotation270:
                            return new CurrentOrientation(Orientation.LandscapeRight);
                        default:
                            return new CurrentOrientation(Orientation.None);
                    }
                }

                switch (rotation)
                {
                    case SurfaceOrientation.Rotation0:
                        return new CurrentOrientation(Orientation.LandscapeLeft);
                    case SurfaceOrientation.Rotation90:
                        return new CurrentOrientation(Orientation.Portrait);
                    case SurfaceOrientation.Rotation180:
                        return new CurrentOrientation(Orientation.LandscapeRight);
                    case SurfaceOrientation.Rotation270:
                        return new CurrentOrientation(Orientation.PortraitDown);
                    default:
                        return new CurrentOrientation(Orientation.None);
                }
            }
        }

        public void NotifyOrientationChange(Android.Content.Res.Orientation orientation)
        {
            switch (orientation)
            {
                case Android.Content.Res.Orientation.Landscape:
                    OnDeviceOrientationChanged(new CurrentOrientation(Orientation.Landscape)); break;
                case Android.Content.Res.Orientation.Portrait:
                    OnDeviceOrientationChanged(new CurrentOrientation(Orientation.Portrait)); break;
                default:
                    OnDeviceOrientationChanged(new CurrentOrientation(Orientation.None)); break;
            }
        }

        private void OnDeviceOrientationChanged(CurrentOrientation orientation)
        {
            ScreenOrientationChanged?.Invoke(this, orientation);
        }

        public async void SetOrientation(Orientation orientation)
        {
            ScreenOrientation request = ScreenOrientation.Unspecified;
            switch(orientation)
            {
                case Orientation.PortraitDown: request = ScreenOrientation.ReversePortrait;break;              
                case Orientation.Landscape: request = ScreenOrientation.Landscape; break;
                case Orientation.LandscapeLeft: request = ScreenOrientation.Landscape; break;
                case Orientation.LandscapeRight: request = ScreenOrientation.ReverseLandscape ; break;
                default: request = ScreenOrientation.Sensor;break;
            }
            //Necessite Register<Activity>(t=>app.AppContext)
            Activity act = Resolver.Resolve<Activity>();
            if (act != null)
            {
                act.RequestedOrientation = request;
                //Quand on demande une orientation explicitement l'orientation automatique est désactivée
                //Il faut remettre RequestedOrientation = ScreenOrientation.Sensor afin de retablire l'orientation auto
                //On attend que l'utilisteur bouge effectivement le téléphone pour remettre la valeur
                //Sensor
                await Task.Delay(2000);
                act.RequestedOrientation = ScreenOrientation.Sensor;
            }
            else
                Debug.WriteLine("WARN Resolver.Resolve<Activity>() returned null did you register Register<Activity>(t=>app.AppContext)");
            
        }

        private async Task WaitFor(Func<bool> predicate, TimeSpan timeout)
        {

            TimeSpan elapsed = TimeSpan.Zero;
            while (predicate())
            {
                if (elapsed >= timeout)
                {
                    Debug.WriteLine("WARNING TIMEOUT WaitFor");
                    break;
                }
                await Task.Delay(250);
                elapsed = elapsed.Add(TimeSpan.FromMilliseconds(250));
            }
        }
    }
}