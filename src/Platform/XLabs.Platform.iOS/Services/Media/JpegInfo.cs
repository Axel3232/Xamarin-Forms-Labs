using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using System.Threading.Tasks;
using XLabs.Platform.Services.GeoLocation;
using Photos;
using CoreLocation;

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


        public JpegInfo(NSUrl assetURL)
        {
            if (assetURL == null) throw new ArgumentNullException("assetUrl");
            
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {

                _reader = new ExifPhotoKitReader(assetURL);
            }
            else
            {
                //If iOS<8 use Assets Catalog
                _reader = new ExifAssetsCatalogReader(assetURL);
            }
        }

        public JpegInfo(NSDictionary info)
        {
            if (info == null) throw new ArgumentNullException("NSDictionary");
            _reader = new ExifNSDictionnaryReader(info);
        }

        public  Task TrySetGpsData(Location loc, string ressourcePath)
        {
            this.GpsAltitude = loc.Altitude;
            this.GpsLatitude = loc.Latitude;
            this.GpsLongitude = loc.Longitude;
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                return UpdateGpsMetaDataWithPhotoKit(NSUrl.FromString(ressourcePath), loc);
            }
            else
            {
                return new Task(() => { });
            }
        }

        private Task UpdateGpsMetaDataWithPhotoKit(NSUrl image, Location loc)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            var assetResult = PHAsset.FetchAssetsUsingLocalIdentifiers(new string[] { image.AbsoluteString },null);
            PHAsset a = (PHAsset)assetResult.firstObject;
            if (a == null) throw new ArgumentException("NSURL does not point to an image in photokit");
         
            PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() => {
                //build temp image
                PHAssetChangeRequest req = PHAssetChangeRequest.ChangeRequest(a);
               
                //This will update the photokit metadata database but not the file itself
                var iosLoc = new CLLocation(new CLLocationCoordinate2D(loc.Latitude, loc.Longitude), loc.Altitude, 1, 1, NSDate.Now);
                req.Location = iosLoc;
                
            }, (success, error) =>
            {
                if (success)
                {

                    tcs.SetResult(0);
                }
                else
                {
                    tcs.SetException(new Exception(error.LocalizedDescription));
                }
            });
            return tcs.Task;
        }

    }
}
