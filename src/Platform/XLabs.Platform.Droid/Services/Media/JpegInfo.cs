using System;
using System.Threading.Tasks;
using XLabs.Platform.Services.GeoLocation;

namespace XLabs.Platform.Services.Media
{
    /// <summary>
    /// This class is a shortcut to access the most used exif tag, it also take care of formating the Raw exif data like transforming latitude/longitude from int[3] to double
    /// if a tag is missing use MediaFile TryGetRawTagValue to get your particular exif data.
    /// </summary>
    /// <remarks>
    /// For a reference of ExifTags see : http://www.exiv2.org/tags.html
    /// </remarks>
    public class JpegInfo : JpegInfoBase, IJpegInfo
    {
      

        public JpegInfo(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");
            _reader = new ExifBinaryReader(filePath);
        }

        /// <summary>
        /// Retreive one exif tag value according to the tag name, this value is not formated, so you have to know the return type on each platform.
        /// For instance latitude is an array of 3 double on Android and  a single double on iOS
        /// </summary>
        /// <typeparam name="T">Type of the result value</typeparam>
        /// <param name="tag">Tag name</param>
        /// <param name="result">Result</param>
        /// <returns>Did succed</returns>
        public bool TryGetRawTagValue<T>(ExifTags tag, out T result)
        {
            try
            {
                return _reader.TryGetTagValue(tag, out result);
            }
            catch (Exception ex)
            {
                result = default(T);
                return false;
            }
        }

        public Task TrySetGpsData(Location loc, string ressourcePath)
        {
            throw new NotImplementedException();
        }

        private double ExifLatCoordinateToDouble(double deg, double min, double sec, ExifTagGpsLatitudeRef coordRef)
        {
            if (coordRef == ExifTagGpsLatitudeRef.North)
                return deg + (min / 60) + (sec / 3600);
            else
                return (deg + (min / 60) + (sec / 3600)) * -1;

        }

        private double ExifLngCoordinateToDouble(double deg, double min, double sec, ExifTagGpsLongitudeRef coordRef)
        {
            if (coordRef == ExifTagGpsLongitudeRef.East)
                return deg + (min / 60) + (sec / 3600);
            else
                return (deg + (min / 60) + (sec / 3600)) * -1;
        }

    }
}