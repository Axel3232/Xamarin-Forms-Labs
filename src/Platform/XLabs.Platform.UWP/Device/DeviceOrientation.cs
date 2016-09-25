using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLabs.Enums;

namespace XLabs.Platform.Device
{
    public class DeviceOrientation : IDeviceOrientation
    {
        public event EventHandler<EventArgs<Orientation>> ScreenOrientationChanged;

        public Orientation GetOrientation()
        {
            Orientation orientation = Orientation.None;
            switch (DeviceInfo.DeviceProperties.GetInstance().DisplayInfo.CurrentOrientation)
            {

                case Windows.Graphics.Display.DisplayOrientations.Landscape:
                    orientation = Orientation.Landscape & Orientation.LandscapeLeft;
                    break;
                case Windows.Graphics.Display.DisplayOrientations.Portrait:
                    orientation = Orientation.Portrait & Orientation.PortraitUp;
                    break;
                case Windows.Graphics.Display.DisplayOrientations.PortraitFlipped:
                    orientation = Orientation.Portrait & Orientation.PortraitDown;
                    break;
                case Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped:
                    orientation = Orientation.Landscape & Orientation.LandscapeRight;
                    break;
                default:
                    orientation = Orientation.None;
                    break;
            }
            return orientation;
        }

        public void SetOrientation(Orientation orientation)
        {
            throw new NotImplementedException();
        }
    }
}
