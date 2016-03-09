using CoreLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using XLabs.Platform.Services.GeoLocation;
using XLabs.Platform.Extensions;
using System.Threading;

namespace XLabs.Platform.Services.Geolocation
{
    public sealed class LocationManager : ILocationManager
    {
        private CLLocationManager manager;
        private static readonly double[] Accuracies = { CLLocation.AccuracyBest, CLLocation.AccuracyNearestTenMeters, CLLocation.AccuracyHundredMeters, CLLocation.AccuracyKilometer, CLLocation.AccuracyThreeKilometers, CLLocation.AccurracyBestForNavigation };

        public AuthorizationStatus AuthorizationStatus { get { return GetAuthStatus(); } }

        public bool IsStarted { get { return manager != null; } }

        public double Accuracy
        {
            get { return manager.DesiredAccuracy; }

        }

        private DateTime locationUpdateStartTime;
        private bool clearCachedLocations;



        private AuthorizationStatus GetAuthStatus()
        {

            return (AuthorizationStatus)CLLocationManager.Status;
        }

        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

        public LocationManager()
        {
        }


        public void Dispose()
        {
            if (manager != null)
            {
                manager.StopUpdatingLocation();
                manager.Dispose();
                manager = null;
            }

            GC.SuppressFinalize(this);
        }

        ~LocationManager()
        {
            if (manager != null)
            {
                manager.StopUpdatingLocation();
                manager.Dispose();
                manager = null;
            }
        }

        private void FireError(string error)
        {
            var handler = Error;
            if (handler != null)
                handler(this, new ErrorEventArgs(error));
        }
        private void FireLocationUpdated(IEnumerable<CLLocation> locations)
        {
            if (LocationUpdated != null)
            {

                if (clearCachedLocations)
                    locations = ClearCachedLocation(locations);
                List<Location> loc = Convert(locations);
                if (loc.Count == 0)
                    return;
                foreach (var l in loc)
                    LocationUpdated(this, new LocationUpdatedEventArgs(l));
            }
        }

        public bool AskAuthorization(bool alsoWhenInBackground = false)
        {
            if (AuthorizationStatus != AuthorizationStatus.NotDetermined)
                return false;

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                if (alsoWhenInBackground)
                    manager.RequestAlwaysAuthorization();
                else
                    manager.RequestWhenInUseAuthorization();
            }

            return true;
        }


        public bool StartPreciseLocationRecording(bool clearCachedLoc = true)
        {
            this.clearCachedLocations = clearCachedLoc;
            if (manager != null)
            {
                manager.StopUpdatingLocation();
            }

            return Start(ActivityType.OtherNavigation, true, LocationAccuracy.AccurracyBestForNavigation, 10);


        }

        private bool StartForegroundPreciseLocationRecording(bool clearCachedLoc = true)
        {
            this.clearCachedLocations = clearCachedLoc;
            if (manager != null)
            {
                manager.StopUpdatingLocation();
            }

            return Start(ActivityType.OtherNavigation, false, LocationAccuracy.AccurracyBestForNavigation, 10);


        }

        //public bool StartAsBackgroundTask()
        //{
        //    if (manager != null)
        //    {
        //        //if (Resolver.Resolve<IAircraftMotionDetector>().IsStarted)
        //        //{
        //        //    manager.StopUpdatingLocation();
        //        //    manager.DistanceFilter = 5000;
        //        //    manager.DesiredAccuracy = CLLocation.AccuracyThreeKilometers;
        //        //    manager.StartUpdatingLocation();
        //        //}
        //        //else
        //        //{
        //            this.Stop();
        //        //}
        //        return true;
        //    }
        //    return Start(ActivityType.OtherNavigation, true, LocationAccuracy.AccuracyThreeKilometers, 5000);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity">Specify the goal of this monitoring, in order to decrease the battery usage</param>
        /// <param name="alsoWhenInBackground">If true, location updates will also arrive when the app is in background</param>
        /// <param name="maxDelayBetweenUpdates">Resolution: one second</param>
        /// <param name="minDistanceBetweenUpdatesInMeters"></param>
        private bool Start(ActivityType activity = ActivityType.OtherNavigation, bool alsoWhenInBackground = false, LocationAccuracy accuracy = LocationAccuracy.AccurracyBestForNavigation,
                          double minDistanceBetweenUpdatesInMeters = 0, TimeSpan? maxDelayBetweenUpdates = null)
        {

            if (minDistanceBetweenUpdatesInMeters == 0)
                minDistanceBetweenUpdatesInMeters = CLLocationDistance.MaxDistance;
            if (maxDelayBetweenUpdates == null)
                maxDelayBetweenUpdates = TimeSpan.FromMinutes(5);
            manager = new CLLocationManager();
            manager.Failed += (sender, args) => FireError(args.Error.ToString());
            manager.LocationsUpdated += (sender, args) => FireLocationUpdated(args.Locations);

            //manager.AuthorizationChanged

            manager.ActivityType = (CLActivityType)activity;
            manager.DesiredAccuracy = Accuracies[(int)accuracy];

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                manager.AllowsBackgroundLocationUpdates = alsoWhenInBackground;
            if ((maxDelayBetweenUpdates != null || minDistanceBetweenUpdatesInMeters > double.Epsilon) && CLLocationManager.DeferredLocationUpdatesAvailable && alsoWhenInBackground)
                manager.AllowDeferredLocationUpdatesUntil(minDistanceBetweenUpdatesInMeters, maxDelayBetweenUpdates.Value.TotalSeconds);
            //Sugggested by https://developer.apple.com/library/ios/documentation/UserExperience/Conceptual/LocationAwarenessPG/CoreLocation/CoreLocation.html
            if (alsoWhenInBackground)
                manager.PausesLocationUpdatesAutomatically = true;

            //Required: ask for authorization
            if (AuthorizationStatus == AuthorizationStatus.NotDetermined)
                AskAuthorization(alsoWhenInBackground);

            var authStatus = AuthorizationStatus;
            if (authStatus < AuthorizationStatus.AuthorizedAlways && authStatus != AuthorizationStatus.NotDetermined)
            {
                System.Diagnostics.Debug.WriteLine("user denied access to location");

                return false;
            }
            if (authStatus == AuthorizationStatus.AuthorizedWhenInUse && alsoWhenInBackground)
            {
                System.Diagnostics.Debug.WriteLine("alsoWhenInBackground is true, but user denied access to location in background");

                return false;
            }
            locationUpdateStartTime = DateTime.Now;
            manager.StartUpdatingLocation();
            return true;
        }

        public Task<Location> GetCurrentPosition(int timeout)
        {
            TaskCompletionSource<Location> tcs = new TaskCompletionSource<Location>();
            Location result = null;
            bool shouldStopAfterFix = !IsStarted;
           
            double currentAccuracy = 0;
            locationUpdateStartTime = DateTime.Now;
            if (manager == null)
                StartForegroundPreciseLocationRecording();
            else
            {
                currentAccuracy = manager.DesiredAccuracy;
            }
            manager.StartUpdatingHeading();
            double trueHeading = -1;
            manager.UpdatedHeading += (s, arg) =>
            {
                trueHeading = arg.NewHeading.TrueHeading;
            };


            manager.LocationsUpdated += (s, e) =>
            {
                if (!tcs.Task.IsFaulted && !tcs.Task.IsCompleted && !tcs.Task.IsCanceled)
                {

                    if (clearCachedLocations)
                        e.Locations = ClearCachedLocation(e.Locations.ToList()).ToArray();
                    List<Location> l = Convert(e.Locations);
                    if (l.Count == 0)
                        return;
                    result = l.OrderBy(t => t.LocalTimeStamp).Last();


                    result.Direction = trueHeading;
                    if (trueHeading != -1)
                    {
                        tcs.SetResult(result);
                        manager.StopUpdatingHeading();
                        //Si on a demarrer le gps uniquement pour ce fix, on l'arrete apres, 
                        //Si il etait deja en marche on remet l'accuracy a la valeur ou elle etait
                        if (shouldStopAfterFix)
                            this.Stop();
                        else
                            manager.DesiredAccuracy = currentAccuracy;
                    }
                }
            };
            int currentTimerCount = 0;
            
            Timer timer = new Timer((obj) =>
            {

                if (tcs.Task.Status == TaskStatus.Canceled || tcs.Task.Status == TaskStatus.Faulted || tcs.Task.Status == TaskStatus.RanToCompletion)
                    return;
               
                if (result != null)
                {
                  
                    tcs.TrySetResult(result);
                    if (shouldStopAfterFix)
                        this.Stop();
                    else
                        manager.DesiredAccuracy = currentAccuracy;
                }
                else
                {
                    tcs.SetCanceled();
                    if (shouldStopAfterFix)
                        this.Stop();
                    else
                        manager.DesiredAccuracy = currentAccuracy;


                        
                }


                
               

            }, null, timeout, 0);
           

            return tcs.Task;

        }

        public void Stop()
        {
            if (manager != null)
            {
                manager.StopUpdatingLocation();
                manager.Dispose();
                manager = null;
            }
        }


        private IEnumerable<CLLocation> ClearCachedLocation(IEnumerable<CLLocation> loc)
        {
            if (clearCachedLocations)
            {
                List<CLLocation> clearedLocation = new List<CLLocation>();
                foreach (var location in loc)
                {
                    var locTimeStamp = location.Timestamp.NSDateToDateTime();
                    //On enlève les points dont la date est < à la date de depart de l'enregistrement + 5ms
                    if ((locTimeStamp - locationUpdateStartTime).TotalMilliseconds > 5)
                    {
                        clearedLocation.Add(location);
                    }
                }
                return clearedLocation;
            }
            return loc;
        }

        private List<Location> Convert(IEnumerable<CLLocation> locations)
        {
            if (locations.Count() > 0)
            {
                DateTime timestamp = new DateTime(2001, 1, 1, 0, 0, 0);
                return locations.Select(location => new Location
                {
                    UtcTimeStamp = timestamp.AddSeconds(location.Timestamp.SecondsSinceReferenceDate),
                    LocalTimeStamp = location.Timestamp.NSDateToDateTime(),
                    Altitude = location.Altitude,
                    Longitude = location.Coordinate.Longitude,
                    Latitude = location.Coordinate.Latitude,
                    Speed = (location.Speed < 0 ? null : new Nullable<double>(location.Speed)),
                    Direction = (location.Course < 0 ? null : new Nullable<double>(location.Course)),
                    HorizontalAccuracy = location.HorizontalAccuracy,
                    VerticalAccuracy = location.VerticalAccuracy
                })
                .ToList();
            }
            return new List<Location>();
        }
    }
}
