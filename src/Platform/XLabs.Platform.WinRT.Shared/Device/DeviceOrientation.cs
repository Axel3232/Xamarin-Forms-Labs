using System;
using System.Collections.Generic;
using System.Text;
using XLabs.Enums;
using XLabs.Platform.Device;

namespace XLabs.Platform.WinRT.Shared.Device
{
    public class DeviceOrientation : IDeviceOrientation
    {
        public event EventHandler<EventArgs<Orientation>> ScreenOrientationChanged;

        public Orientation GetOrientation()
        {
            switch (Windows.Graphics.Display.DisplayInformation.GetForCurrentView().CurrentOrientation)
            {

                case Windows.Graphics.Display.DisplayOrientations.Landscape:
                    return Orientation.Landscape & Orientation.LandscapeLeft;
                case Windows.Graphics.Display.DisplayOrientations.Portrait:
                    return Orientation.Portrait & Orientation.PortraitUp;
                case Windows.Graphics.Display.DisplayOrientations.PortraitFlipped:
                    return Orientation.Portrait & Orientation.PortraitDown;
                case Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped:
                    return Orientation.Landscape & Orientation.LandscapeRight;
                default:
                    return Orientation.None;
            }
        }

        public void SetOrientation(Orientation orientation)
        {
            throw new NotImplementedException();
        }
    }
}
