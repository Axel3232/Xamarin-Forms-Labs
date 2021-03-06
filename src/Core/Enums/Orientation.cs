﻿// ***********************************************************************
// Assembly         : XLabs.Core
// Author           : XLabs Team
// Created          : 12-27-2015
// 
// Last Modified By : XLabs Team
// Last Modified On : 01-04-2016
// ***********************************************************************
// <copyright file="Orientation.cs" company="XLabs Team">
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

namespace XLabs.Enums
{
    /// <summary>
    /// Enum Orientation
    /// </summary>
    [Flags]
    public enum Orientation
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The portrait
        /// </summary>
        Portrait = 1,
        /// <summary>
        /// The landscape
        /// </summary>
        Landscape = 2,
        /// <summary>
        /// The portrait up
        /// </summary>
        PortraitUp = 5,
        /// <summary>
        /// The portrait down
        /// </summary>
        PortraitDown = 9,
        /// <summary>
        /// The landscape left
        /// </summary>
        LandscapeLeft = 18,
        /// <summary>
        /// The landscape right
        /// </summary>
        LandscapeRight = 34,
        FaceUp = 40,
        FaceDown = 50,
        /// <summary>
        /// Sur android, demander un orientation explicitement desactive la gestion automatique de l'orientation, 
        /// Pour la réactiver il faut repasser cette valeur
        /// </summary>
        SensorAuto = 60
    }

    public struct CurrentOrientation
    {
        
        public Orientation Orientation { get; private set; }
        public bool IsPortrait { get {
                return Orientation == Orientation.Portrait || Orientation == Orientation.PortraitDown || Orientation == Orientation.PortraitUp;
            } }

        public bool IsLandscape
        {
            get
            {
                return Orientation == Orientation.Landscape || Orientation == Orientation.LandscapeLeft || Orientation == Orientation.LandscapeRight;
            }
        }

        public CurrentOrientation(Orientation orientation)
        {
            Orientation = orientation;
        }
    }
}
