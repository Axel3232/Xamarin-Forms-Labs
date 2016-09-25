using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using System;
using XLabs.Enums;

namespace XLabs.Platform.Device
{
    public class DeviceOrientation : IDeviceOrientation
    {
        public event EventHandler<EventArgs<Orientation>> ScreenOrientationChanged;

        public Orientation GetOrientation()
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
                            return Orientation.Portrait & Orientation.PortraitUp;
                        case SurfaceOrientation.Rotation90:
                            return Orientation.Landscape & Orientation.LandscapeLeft;
                        case SurfaceOrientation.Rotation180:
                            return Orientation.Portrait & Orientation.PortraitDown;
                        case SurfaceOrientation.Rotation270:
                            return Orientation.Landscape & Orientation.LandscapeRight;
                        default:
                            return Orientation.None;
                    }
                }

                switch (rotation)
                {
                    case SurfaceOrientation.Rotation0:
                        return Orientation.Landscape & Orientation.LandscapeLeft;
                    case SurfaceOrientation.Rotation90:
                        return Orientation.Portrait & Orientation.PortraitUp;
                    case SurfaceOrientation.Rotation180:
                        return Orientation.Landscape & Orientation.LandscapeRight;
                    case SurfaceOrientation.Rotation270:
                        return Orientation.Portrait & Orientation.PortraitDown;
                    default:
                        return Orientation.None;
                }
            }
        }

        public void SetOrientation(Orientation orientation)
        {
            throw new NotImplementedException();
        }
    }
}