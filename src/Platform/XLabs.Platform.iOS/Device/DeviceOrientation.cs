using System;
using Foundation;
using UIKit;
using XLabs.Enums;
using XLabs.Platform.Extensions;

namespace XLabs.Platform.Device
{
    public class DeviceOrientation : IDeviceOrientation
    {
       
        public DeviceOrientation()
        {
            var notificationCenter = NSNotificationCenter.DefaultCenter;
            notificationCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification, DeviceOrientationDidChange);

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

       

        #region IDeviceOrientation implementation

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <returns>The orientation.</returns>
        public CurrentOrientation GetOrientation()
        {
            switch (UIApplication.SharedApplication.StatusBarOrientation)
            {
                case UIInterfaceOrientation.LandscapeLeft:
                    return new CurrentOrientation(Orientation.LandscapeLeft);
                case UIInterfaceOrientation.Portrait:
                    return new CurrentOrientation(Orientation.Portrait);
                case UIInterfaceOrientation.PortraitUpsideDown:
                    return new CurrentOrientation(Orientation.PortraitDown);
                case UIInterfaceOrientation.LandscapeRight:
                    return new CurrentOrientation(Orientation.LandscapeRight);
                default:
                    return new CurrentOrientation(Orientation.None);
            }
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