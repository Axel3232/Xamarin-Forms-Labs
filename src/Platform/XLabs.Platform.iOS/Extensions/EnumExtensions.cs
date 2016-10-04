// ***********************************************************************
// Assembly         : XLabs.Platform.iOS
// Author           : XLabs Team
// Created          : 12-27-2015
// 
// Last Modified By : XLabs Team
// Last Modified On : 01-04-2016
// ***********************************************************************
// <copyright file="EnumExtensions.cs" company="XLabs Team">
//     Copyright (c) XLabs Team. All rights reserved.
// </copyright>
// <summary>
//       This project is licensed under the Apache 2.0 license
//       https://github.com/XLabs/Xamarin-Forms-Labs/blob/master/LICENSE
//       
//       XLabs is a open source project that aims to provide a powerfull and cross 
//       platform set of controls tailored to work with Xamarin Forms.
// </summary>
// ***********************************************************************
// 

using System;
using System.ComponentModel;
using UIKit;
using XLabs.Enums;

namespace XLabs.Platform.Extensions
{
	/// <summary>
	/// Class EnumExtensions.
	/// </summary>
	public static class EnumExtensions
	{
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>System.String.</returns>
		public static string GetDescription(this Enum value)
		{
			var field = value.GetType().GetField(value.ToString());

			var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

			return attribute == null ? value.ToString() : attribute.Description;
		}

        /// <summary>
        /// Transform an <see cref="UIDeviceOrientation"/> into an <see cref="Orientation"/>
        /// </summary>
        /// <remarks>
        /// The UIDeviceOrientation
        /// </remarks>
        /// <param name="self"></param>
        /// <returns>Orientation</returns>
        public static Orientation ToOrientation(this UIDeviceOrientation self)
        {
            switch (self)
            {
                case UIDeviceOrientation.Unknown: return Orientation.None;
                case UIDeviceOrientation.Portrait: return Orientation.Portrait;
                case UIDeviceOrientation.PortraitUpsideDown: return Orientation.PortraitDown;
                case UIDeviceOrientation.LandscapeLeft: return Orientation.LandscapeLeft;
                case UIDeviceOrientation.LandscapeRight: return Orientation.LandscapeRight;
                case UIDeviceOrientation.FaceUp: return Orientation.FaceUp;
                case UIDeviceOrientation.FaceDown: return Orientation.FaceDown;
                default: return Orientation.None;
            }
        }
    }
}