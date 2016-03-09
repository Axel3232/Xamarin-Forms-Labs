using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace XLabs.Platform.Services.Media
{
    public class ExifNSDictionnaryReader : ExifReaderBase, IExifReader
    {
        public int PixelWidth
        { get; private set; }

        public int PixelHeight
        { get; private set; }

        public void Dispose()
        {
            
        }



        public ExifNSDictionnaryReader(NSDictionary dico)
        {
            var meta = new NSMutableDictionary(dico.ValueForKey(new NSString("UIImagePickerControllerMediaMetadata")) as NSDictionary);
            var exifDic = new NSMutableDictionary(((NSDictionary)meta.ValueForKey(ImageIO.CGImageProperties.ExifDictionary))  ?? new NSDictionary());
            var tiffDic = new NSMutableDictionary(((NSDictionary)meta.ValueForKey(ImageIO.CGImageProperties.TIFFDictionary)) ?? new NSDictionary());
            var gpsDic = new NSMutableDictionary(((NSDictionary)meta.ValueForKey(ImageIO.CGImageProperties.GPSDictionary)) ?? new NSDictionary());
            if (gpsDic != null)
                SetGpsData(gpsDic);
            if (exifDic != null)
                SetExifData(exifDic);
            if (tiffDic != null)
                SetTiffData(tiffDic);
            var photo = dico.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
            PixelHeight = (int)photo.Size.Height;
            PixelWidth = (int)photo.Size.Width;
        }
    }
}
