﻿using System;

namespace XLabs.Platform.Services.Media
{
    /// <summary>
    /// Enable a class to retrieve exif tags
    /// </summary>
    public interface IExifReader : IDisposable
    {

        int PixelWidth { get;}

        int PixelHeight { get; }

        /// <summary>
        /// Try to retreive an Exif tag value by its ushort code, you must supply Type of the result , for a int of the result's type see
        /// http://www.exiv2.org/tags.html
        /// </summary>
        /// <typeparam name="T">.NET Type of the result</typeparam>
        /// <param name="tagID">Ushort code of the Exif tag</param>
        /// <param name="result">The result</param>
        /// <returns>True is successfull false otherwise</returns>
        bool TryGetTagValue<T>(ushort tagID, out T result);
        /// <summary>
        /// Try to retreive an Exif tag value using the <see cref="ExifTag"/> enum, you must supply Type of the result, for a int of the result's type see
        /// http://www.exiv2.org/tags.html
        /// </summary>
        /// <typeparam name="T">.NET Type of the result</typeparam>
        /// <param name="tagID">The Exif tag  name</param>
        /// <param name="result">The result</param>
        /// <returns>True is successfull false otherwise</returns>
        bool TryGetTagValue<T>(ExifTags tag, out T result);
        /// <summary>
        /// Add an exif tag in the cached dictionnary but dont write to the underlying file
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="obj"></param>
        void SetTagValueInCache(ExifTags tag, object obj);



    }
}
