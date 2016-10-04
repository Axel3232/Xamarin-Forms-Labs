using System;
using Foundation;
using UIKit;
using XLabs.Enums;
using XLabs.Platform.Extensions;

namespace XLabs.Platform.Device
{
    public class DeviceOrientation : IDeviceOrientation, IDisposable
    {
        NSObject orientationObserver;
        public DeviceOrientation()
        {
            var notificationCenter = NSNotificationCenter.DefaultCenter;
            orientationObserver =notificationCenter.AddObserver(UIDevice.OrientationDidChangeNotification, DeviceOrientationDidChange);
            UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
        }

        public event EventHandler<EventArgs<CurrentOrientation>> ScreenOrientationChanged;



        /// <summary>
        /// Devices the orientation did change.
        /// </summary>
        /// <param name="notification">Notification.</param>
        public  void DeviceOrientationDidChange(NSNotification notification)
        {
            UIDeviceOrientation orientation = UIDevice.CurrentDevice.Orientation;
            this.OnDeviceOrientationChanged(new CurrentOrientation(orientation.ToOrientation()));
        }

       

        public void Dispose()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(orientationObserver);
        }



        #region IDeviceOrientation implementation

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <returns>The orientation.</returns>
        public CurrentOrientation GetOrientation()
        {
            return new CurrentOrientation(UIDevice.CurrentDevice.Orientation.ToOrientation());
        }

        public void SetOrientation(Orientation orientation)
        {
           
            if (orientation == Orientation.LandscapeLeft)
                UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeLeft), new NSString("orientation"));
            else if (orientation == Orientation.LandscapeRight)
                UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeRight), new NSString("orientation"));
            else if (orientation == Orientation.Portrait || orientation == Orientation.PortraitUp)
                UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Portrait), new NSString("orientation"));
            else if (orientation == Orientation.PortraitDown)
                UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.PortraitUpsideDown), new NSString("orientation"));
        }


        private void OnDeviceOrientationChanged(CurrentOrientation orientation)
        {
            ScreenOrientationChanged?.Invoke(this, orientation);
        }

       



        #endregion
    }
}