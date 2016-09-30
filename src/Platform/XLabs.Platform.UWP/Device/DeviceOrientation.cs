using System;
using XLabs.Enums;

namespace XLabs.Platform.Device
{
    public class DeviceOrientation : IDeviceOrientation
    {
        public event EventHandler<EventArgs<CurrentOrientation>> ScreenOrientationChanged;

        public CurrentOrientation GetOrientation()
        {
            
            switch (DeviceInfo.DeviceProperties.GetInstance().DisplayInfo.CurrentOrientation)
            {

                case Windows.Graphics.Display.DisplayOrientations.Landscape:
                    return new CurrentOrientation(Orientation.Landscape);
                case Windows.Graphics.Display.DisplayOrientations.Portrait:
                    return new CurrentOrientation(Orientation.Portrait);
                case Windows.Graphics.Display.DisplayOrientations.PortraitFlipped:
                    return new CurrentOrientation(Orientation.PortraitDown);
                case Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped:
                    return new CurrentOrientation(Orientation.LandscapeRight);
                  
                default:
                    return new CurrentOrientation(Orientation.None);
            }
    
        }

        public void SetOrientation(Orientation orientation)
        {
            throw new NotImplementedException();
        }
    }
}
