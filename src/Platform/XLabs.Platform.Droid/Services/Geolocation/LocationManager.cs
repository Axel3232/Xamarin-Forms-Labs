using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Locations;
using Android.OS;
using XLabs.Platform.Services.GeoLocation;
using Android;

namespace XLabs.Platform.Services.Geolocation
{
    public sealed class LocationManager : ILocationManager
    {
       private static readonly Accuracy[] Accuracies = {
                Android.Locations.Accuracy.NoRequirement,
                Android.Locations.Accuracy.Fine,
                Android.Locations.Accuracy.Low,
                Android.Locations.Accuracy.Coarse,
                Android.Locations.Accuracy.Medium,
                Android.Locations.Accuracy.High };
        private Android.Locations.LocationManager locationManager;
        private LocationListener locationListener;
        private bool isDisposed;

        public AuthorizationStatus AuthorizationStatus { get { return AuthorizationStatus.AuthorizedAlways; } }

        public bool AskAuthorization(bool alsoWhenInBackground = false)
        {
            return true;
        }

        public double Accuracy
        {
            get { throw new NotImplementedException(); }
            set
            {
                throw new NotImplementedException();

            }
        }

        public string[] PermissionSet
        {
            get
            {
                return new string[] {
                    Manifest.Permission.AccessCoarseLocation,
                  Manifest.Permission.AccessFineLocation
                };
            }

        }

        public bool IsStarted { get { return locationManager != null; } }

        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                Stop();
                GC.SuppressFinalize(this);
            }
        }

        ~LocationManager()
        {
            if (!isDisposed)
                Dispose();
        }

        public LocationManager()
        {


        }

        //private void FireError(string error)
        //{
        //    var handler = Error;
        //    if(handler != null)
        //        handler(this, new ErrorEventArgs(error));
        //}

        private void FireLocationUpdated(Android.Locations.Location location)
        {
            var handler = LocationUpdated;
            if (handler != null)
                handler(this, new LocationUpdatedEventArgs(Convert(location)));
        }

        public Task<XLabs.Platform.Services.GeoLocation.Location> GetCurrentPosition()
        {
            throw new NotImplementedException();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity">Specify the goal of this monitoring, in order to decrease the battery usage</param>
        /// <param name="alsoWhenInBackground">If true, location updates will also arrive when the app is in background</param>
        /// <param name="maxDelayBetweenUpdates">Resolution: one second</param>
        /// <param name="accuracy"></param>
        /// <param name="minDistanceBetweenUpdatesInMeters"></param>
        public bool Start(Android.Locations.Accuracy accuracy, bool alsoWhenInBackground = false,
                          double minDistanceBetweenUpdatesInMeters = 0, TimeSpan? maxDelayBetweenUpdates = null)
        {

            System.Diagnostics.Debug.WriteLine("DeviceLocation Starting");
            try
            {
                if (locationManager != null)
                    return true;

                locationManager = Android.App.Application.Context.GetSystemService(Context.LocationService) as Android.Locations.LocationManager;
                if (locationManager == null)
                    return false;

                var locProvider = locationManager.GetBestProvider(new Criteria() { Accuracy = accuracy, AltitudeRequired = true, HorizontalAccuracy = accuracy, VerticalAccuracy = accuracy, BearingRequired = true }, true);

                if (locProvider == null)
                {
                    locProvider = Android.Locations.LocationManager.GpsProvider;
                    if (!locationManager.IsProviderEnabled(locProvider))
                        return false;
                }


                locationListener = new LocationListener { LocationChanged = loc => FireLocationUpdated(loc) };

                locationManager.RequestLocationUpdates(locProvider, maxDelayBetweenUpdates == null ? 8000 : (long)maxDelayBetweenUpdates.Value.TotalMilliseconds, (float)minDistanceBetweenUpdatesInMeters, locationListener);
                System.Diagnostics.Debug.WriteLine("DeviceLocation started ok");


                return true;

            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DeviceLocation.Start failed: {0}", e.ToString());

                return false;
            }


        }




        public void Stop()
        {
            System.Diagnostics.Debug.WriteLine("DeviceLocation Stopping");


            if (locationManager != null)
            {
                if (locationListener != null)
                {
                    locationManager.RemoveUpdates(locationListener);
                    locationListener.Dispose();
                    locationListener = null;
                }
                locationManager.Dispose();
                locationManager = null;
                System.Diagnostics.Debug.WriteLine("DeviceLocation stopped ok");

            }
        }

        static XLabs.Platform.Services.GeoLocation.Location Convert(Android.Locations.Location location)
        {

            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds((double)location.Time);
            return new XLabs.Platform.Services.GeoLocation.Location
            {
                LocalTimeStamp = dt,
                Altitude = location.Altitude,
                Longitude = location.Longitude,
                Latitude = location.Latitude,
                Speed = location.Speed,
                Direction = location.Bearing,
                HorizontalAccuracy = location.Accuracy,
                VerticalAccuracy = location.Accuracy,
            };
        }



        public bool StartPreciseLocationRecording()
        {
            try
            {
                if (locationManager != null)
                {
                    Stop();
                    return Start(Android.Locations.Accuracy.High, true, 10);


                }
                return Start(Android.Locations.Accuracy.High, true, 10);
            }
            catch (System.Exception)
            {

                return false;
            }
        }



        public Task<XLabs.Platform.Services.GeoLocation.Location> GetCurrentPosition(int timeout, CancellationToken token, bool includeHeading = false)
        {
            throw new NotImplementedException();
        }

        public bool StartPreciseLocationRecording(bool clearCachedLoc = true)
        {
            return Start(Android.Locations.Accuracy.Fine, true,
                          0, null);
        }

        void ILocationManager.AskAuthorization(bool alsoWhenInBackground)
        {
            throw new NotImplementedException();
        }
    }


    internal class LocationListener : Java.Lang.Object, ILocationListener
    {
        public Action<Android.Locations.Location> LocationChanged;
        public Action<string> Disabled, Enabled;
        public Action<string, Availability, Bundle> StatusChanged;

        public void OnLocationChanged(Android.Locations.Location location)
        {
            System.Diagnostics.Debug.WriteLine("LocationListener: location changed");

            if (LocationChanged != null)
                LocationChanged(location);
        }

        public void OnProviderDisabled(string provider)
        {
            System.Diagnostics.Debug.WriteLine("LocationListener: provider {0} disabled", provider);

            if (Disabled != null)
                Disabled(provider);
        }

        public void OnProviderEnabled(string provider)
        {
            System.Diagnostics.Debug.WriteLine("LocationListener: provider {0} enabled", provider);

            if (Enabled != null)
                Enabled(provider);
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            System.Diagnostics.Debug.WriteLine("LocationListener: provider {0} status changed: {1}", provider, status);

            if (StatusChanged != null)
                StatusChanged(provider, status, extras);
        }
    }
}