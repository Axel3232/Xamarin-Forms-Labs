using CoreImage;
using Foundation;
using Photos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLabs.Platform.Services.Media
{
    /// <summary>
    /// Retreive Exif tag using PhotoKit on iOS for iOS version >= 8
    /// </summary>
    public sealed class ExifPhotoKitReader : ExifReaderBase, IExifReader
    {
        private NSUrl _assetURL;

        public int PixelWidth
        { get; private set; }

        public int PixelHeight
        { get; private set; }

        public ExifPhotoKitReader(NSUrl assetURL) : base()
        {
            if (assetURL == null) throw new ArgumentException("assetURL");
            _assetURL = assetURL;
            ReadExifTags().ContinueWith((task) =>
            {
                if (!task.IsCompleted)
                {

                    throw new MediaExifException("Could not read exif data from photokit");
                }
            });
        }

        /// <summary>
        /// Get meta data dictionaries from PhotoKit
        /// </summary>
        /// <returns></returns>
        private Task ReadExifTags()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            try
            {

                var res = PHAsset.FetchAssets(new NSUrl[] { _assetURL }, new PHFetchOptions());
               
                //Since we selected only one pic there should be only one item in the list
                PHAsset photo = (PHAsset)res.FirstOrDefault();
                PixelWidth =(int) photo.PixelWidth;
                PixelHeight = (int)photo.PixelHeight;
                if (photo == null)
                    tcs.SetException(new MediaFileNotFoundException(_assetURL.ToString()));
                if (PHPhotoLibrary.AuthorizationStatus != PHAuthorizationStatus.Authorized)
                {
                    PHPhotoLibrary.RequestAuthorization((status) =>
                    {
                        if (status != PHAuthorizationStatus.Authorized)
                        {
                            tcs.SetCanceled();
                            return;
                        }
                    });
                }
                photo.RequestContentEditingInput(new PHContentEditingInputRequestOptions() { NetworkAccessAllowed = false }, (input, options) =>
                {
                    try
                    {
                        //Get the orginial image (FullSize)
                        CIImage img = CIImage.FromUrl(input.FullSizeImageUrl);
                        var prop = img.Properties;
                        
                        SetGlobalData(prop.Dictionary);
                        //si il y a des tags GPS au sein du fichier
                        if (prop.Gps != null)
                            SetGpsData(prop.Gps.Dictionary);
                        else
                        {
                            //Sinon on essaye de recuperer ces valeurs GPS au sein de la BD photoKit
                            if(photo.Location != null)
                            {
                                SetGpsData(photo.Location);
                            }
                        }
                        if (prop.Exif != null)
                            SetExifData(prop.Exif.Dictionary);
                        if (prop.Tiff != null)
                            SetTiffData(prop.Tiff.Dictionary);

                        tcs.SetResult(0);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exif parsing failed : " + ex.ToString());

                        tcs.SetException(ex);
                        //fail silently
                    }


                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                tcs.TrySetException(ex);

            }
            return tcs.Task;
        }







        public void Dispose()
        {

        }
    }
}
