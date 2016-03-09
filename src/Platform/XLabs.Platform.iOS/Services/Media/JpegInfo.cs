using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

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


    }
}
