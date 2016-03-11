using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XLabs.Platform.Services.GeoLocation
{
    public interface ILocationManager : IDisposable
    {
        AuthorizationStatus AuthorizationStatus { get; }

        event EventHandler<ErrorEventArgs> Error;
        event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

        bool AskAuthorization(bool alsoWhenInBackground = false);

        double Accuracy { get; }



        bool IsStarted { get; }

        Task<Location> GetCurrentPosition(int timeout, CancellationToken token,bool includeHeading=false);



        bool StartPreciseLocationRecording(bool clearCachedLoc = true);
        /// <summary>
        /// Mets le Gps en mode economie d'energie (Precision = 3 km) si le detecteur de mouvement est activé ou l'arrete si il n'est pas activé
        /// </summary>
        /// <returns></returns>
        //bool StartAsBackgroundTask();

        void Stop();
    }
   
  
    public class ErrorEventArgs : EventArgs
    {
        public string Error { get; private set; }

        public ErrorEventArgs(string error)
        {
            Error = error;
        }
    }
 
    public class LocationUpdatedEventArgs : EventArgs
    {
        public Location Location { get; private set; }

        public LocationUpdatedEventArgs(Location loc)
        {
            Location = loc;
        }
    }
  
    public enum ActivityType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Other = 1,
        /// <summary>
        /// Car
        /// </summary>
        AutomotiveNavigation = 2,
        Fitness = 3,
        /// <summary>
        /// Planes, boat, ...
        /// </summary>
        OtherNavigation = 4,
    }

 
    public class Location
    {
        public DateTime UtcTimeStamp { get; set; }

        public DateTime LocalTimeStamp { get; set; }
        /// <summary>
        /// In degrees
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// In degrees
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// In degrees from north
        /// </summary>
        public double? Direction { get; set; }

        /// <summary>
        /// Radius in meters
        /// </summary>
        public double HorizontalAccuracy { get; set; }

        /// <summary>
        /// Radius in meters
        /// </summary>
        public double VerticalAccuracy { get; set; }

        /// <summary>
        /// In meters
        /// </summary>
        public double Altitude { get; set; }


        /// <summary>
        /// In meters/second
        /// </summary>
        public double? Speed { get; set; }
    }
 
    public enum AuthorizationStatus
    {
        NotDetermined = 0,
        Restricted = 1,
        Denied = 2,
        AuthorizedAlways = 3,
        AuthorizedWhenInUse = 4,
    }
  
    public enum LocationAccuracy
    {
        AccuracyBest,

        /// <summary>
        /// Constant representing an accuracy within 10m of the target.
        /// </summary>
        AccuracyNearestTenMeters,

        /// <summary>
        /// Constant representing an accuracy within 100m of the target.
        /// </summary>
        AccuracyHundredMeters,

        /// <summary>
        /// Constant representing an accuracy within 1km of the target.
        /// </summary>
        AccuracyKilometer,

        /// <summary>
        /// Constant representing an accuracy within 3km of the target.
        /// </summary>
        AccuracyThreeKilometers,

        /// <summary>
        /// Constant representing the highest-level of location accuracy and indicates that the calculation should incorporate additional sensor input.
        /// </summary>
        /// 
        /// <remarks>
        /// This accuracy is intended for navigation applications and incorporates additional sensor information such as geomagnetic and acceleration data. It consumes more power than the other accuracy-levels and is intended for use when the device is plugged-in.
        /// </remarks>
        AccurracyBestForNavigation,
    }
}
