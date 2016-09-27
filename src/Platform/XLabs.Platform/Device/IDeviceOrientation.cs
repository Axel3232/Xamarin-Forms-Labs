using System;
using XLabs.Enums;

namespace XLabs.Platform.Device
{
    /// <summary>
    /// DeviceOrientation Interface
    /// Inspired by https://github.com/aliozgur/Xamarin.Plugins/tree/master/DeviceOrientation
    /// </summary>
    public interface IDeviceOrientation
    {
        /// <summary>
        /// Triggered only when orienationation change by moving device.
        /// Not trigered when calling <see cref="SetOrientation(Orientation)"/>
        /// </summary>
        event EventHandler<EventArgs<CurrentOrientation>> ScreenOrientationChanged;
        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <returns>The orientation.</returns>
        CurrentOrientation GetOrientation();
        /// <summary>
        /// Force the orientation of the screen 
        /// </summary>
        /// <param name="orientation"></param>
        void SetOrientation(Orientation orientation);
    }
}
