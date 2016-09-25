namespace XLabs.Platform.Services.Geolocation
{
    using GeoLocation;
    using Windows.Devices.Geolocation;

    /// <summary>
    /// The coordinate extensions for Windows Phone.
    /// </summary>
    public static class CoordinateExtensions
	{
        /// <summary>
        /// Converts <see cref="Geocoordinate" /> class into <see cref="Position" />.
        /// </summary>
        /// <param name="geocoordinate">The Geocoordinate.</param>
        /// <returns>The <see cref="Location" />.</returns>
        public static Location GetPosition(this Geocoordinate geocoordinate)
		{
			return new Location
            {
					       HorizontalAccuracy = geocoordinate.Accuracy,
					       Altitude = geocoordinate.Point.Position.Altitude,
					       Direction = geocoordinate.Heading,
					       Latitude = geocoordinate.Point.Position.Latitude,
					       Longitude = geocoordinate.Point.Position.Longitude,
					       Speed = geocoordinate.Speed,
					        LocalTimeStamp = geocoordinate.Timestamp.DateTime
				       };
		}
	}
}