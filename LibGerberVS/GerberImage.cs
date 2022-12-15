﻿/* GerberImage.cs - Holds information of a gerber image. */

/*  Copyright (C) 2015-2021 Milton Neal <milton200954@gmail.com>
    *** Acknowledgments to Gerbv Authors and Contributors. ***

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions
    are met:

    1. Redistributions of source code must retain the above copyright
       notice, this list of conditions and the following disclaimer.
    2. Redistributions in binary form must reproduce the above copyright
       notice, this list of conditions and the following disclaimer in the
       documentation and/or other materials provided with the distribution.
    3. Neither the name of the project nor the names of its contributors
       may be used to endorse or promote products derived from this software
       without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
    ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
    OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
    HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
    LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
    OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GerberVS
{
    /// <summary>
    /// Class defining the information about an image.
    /// </summary>
    public class GerberImageInfo
    {
        /// <summary>
        /// User supplied name for the image.
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// The polarity in wich the image will be rendered.
        /// </summary>
        public GerberPolarity Polarity { get; set; }

        /// <summary>
        /// Minimum X dimension of the image.
        /// </summary>
        public double MinX { get; set; }

        /// <summary>
        /// Minimum Y dimension of the image.
        /// </summary>
        public double MinY { get; set; }

        /// <summary>
        /// Maximum X dimension of the image.
        /// </summary>
        public double MaxX { get; set; }

        /// <summary>
        /// Maximum Y dimension of the image.
        /// </summary>
        public double MaxY { get; set; }

        /// <summary>
        /// Offset on the A axis.
        /// </summary>
        public double OffsetA { get; set; }

        /// <summary>
        /// Offset on the B axis.
        /// </summary>
        public double OffsetB { get; set; }

        /// <summary>
        /// Rotation angle of the image.
        /// </summary>
        public double ImageRotation { get; set; }

        public GerberImageJustifyType ImageJustifyTypeA { get; set; }
        public GerberImageJustifyType ImageJustifyTypeB { get; set; }
        public double ImageJustifyOffsetA { get; set; }
        public double ImageJustifyOffsetB { get; set; }
        public double ImageJustifyOffsetActualA { get; set; }
        public double ImageJustifyOffsetActualB { get; set; }
        public string PlotterFilm { get; set; }
        public string FileTypeName { get; set; }                    // Descriptive string for the type of file (RS274-X, Drill, etc)

        /// <summary>
        /// Creates a new instance of the gerber image information type.
        /// </summary>
        public GerberImageInfo()
        {
            ImageName = String.Empty;
            PlotterFilm = String.Empty;
            FileTypeName = "Unknown";
        }
    }

    /// <summary>
    /// Creates a new gerber image and initializes its members
    /// </summary>
    public class GerberImage
    {
        // Structure for holding aperture translations.
        private struct ApertureTranslate
        {
            public int sourceIndex;
            public int newIndex;
        }

        private readonly Aperture[] apertureArray;

        /// <summary>
        /// A list of all aperture macros used (only used in RS274X types).
        /// </summary>
        public Collection<ApertureMacro> ApertureMacroList { get; }

        /// <summary>
        /// A list of all geometric entities in the layer.
        /// </summary>
        public Collection<GerberNet> GerberNetList { get; }

        /// <summary>
        /// A list of all RS274X levels used (only used in RS274X types).
        /// </summary>
        public Collection<GerberLevel> LevelList { get; }

        /// <summary>
        /// A list all RS274X states used (only used in RS274X types).
        /// </summary>
        public Collection<GerberNetState> NetStateList { get; }

        /// <summary>
        /// Miscellaneous info regarding the layer such as overall size, etc.
        /// </summary>
        public GerberImageInfo ImageInfo { get; }

        /// <summary>
        /// The type of file (RS274X, drill).
        /// </summary>
        public GerberFileType FileType { get; internal set; }

        /// <summary>
        /// The layer formating information.
        /// </summary>
        public GerberFormat Format { get; internal set; }

        /// <summary>
        /// The gerber image unit of messure.
        /// </summary>
        public GerberUnit Unit { get; } 

        /// <summary>
        /// RS274X statistics for the layer.
        /// </summary>
        public GerberFileStats GerberStats { get; internal set; }

        /// <summary>
        /// Excellon drill statistics for the layer.
        /// </summary>
        public DrillFileStats DrillStats { get; internal set; }

        /// <summary>
        /// Creates a new instance of the gerber image type.
        /// </summary>
        public GerberImage()
            : this(String.Empty)
        { }
        /// <summary>
        /// Creates a new instance of the gerber image type.
        /// </summary>
        /// <param name="fileTypeName">type of file, eg rs274-x, drill</param>
        public GerberImage(string fileTypeName)
        {
            ImageInfo = new GerberImageInfo
            {
                MinX = double.MaxValue,
                MinY = double.MaxValue,
                MaxX = double.MinValue,
                MaxY = double.MinValue
            };

            if (!String.IsNullOrEmpty(fileTypeName))
                ImageInfo.FileTypeName = fileTypeName;

            // The individual file parsers will have to set this.
            apertureArray = new Aperture[Gerber.MaximumApertures];
            ApertureMacroList = new Collection<ApertureMacro>();
            LevelList = new Collection<GerberLevel>();
            NetStateList = new Collection<GerberNetState>();
            Format = new GerberFormat();
            GerberNetList = new Collection<GerberNet>();
            GerberStats = new GerberFileStats();
            DrillStats = new DrillFileStats();
        }

        /// <summary>
        /// List of apertures used.
        /// </summary>
        /// <returns>aperture array</returns>
        public Aperture[] ApertureArray()
        {
            return apertureArray;
        }

        /// <summary>
        /// Perform some basic integrity checks on the gerber image.
        /// </summary>
        /// <returns>error status</returns>
        public GerberVerifyErrors ImageVerify()
        {
            GerberVerifyErrors errorStatus = GerberVerifyErrors.None;
            int numberOfNets = 0;

            if (this.GerberNetList == null)
                errorStatus |= GerberVerifyErrors.MissingNetList;

            if (this.Format == null)
                errorStatus |= GerberVerifyErrors.MissingFormat;

            if (this.ImageInfo == null)
                errorStatus |= GerberVerifyErrors.MissingImageInfo;

            if (this.GerberNetList != null)
                numberOfNets = this.GerberNetList.Count;

            // If we have nets but no apertures are defined, then return an error.
            if (numberOfNets > 0)
            {
                if (ApertureArray().Length == 0)
                    errorStatus |= GerberVerifyErrors.MissingApertures;
            }

            return errorStatus;
        }

        /// <summary>
        /// Remove a net from the image.
        /// </summary>
        /// <param name="index">index within the net list</param>
        public void DeleteNet(int index)
        {
            GerberNet currentNet = this.GerberNetList[index];

            if (currentNet.Interpolation != GerberInterpolation.RegionStart)
            {
                currentNet.Aperture = 0;
                currentNet.ApertureState = GerberApertureState.Off;
                currentNet.Interpolation = GerberInterpolation.Deleted;
            }

            // If this is a polygon start, we need to erase all the rest of the nets in this polygon too.
            else
            {
                do
                {
                    currentNet = this.GerberNetList[index];
                    currentNet.Aperture = 0;
                    currentNet.ApertureState = GerberApertureState.Off;
                    currentNet.Interpolation = GerberInterpolation.Deleted;
                    index++;
                }
                while (index < this.GerberNetList.Count && this.GerberNetList[index].Interpolation != GerberInterpolation.RegionEnd);
            }
        }

        /// <summary>
        /// Creates a copy of the gerber image.
        /// </summary>
        /// <param name="sourceImage">image to be copied</param>
        /// <returns>a deep copy of the source image</returns>
        public static GerberImage Copy(GerberImage sourceImage)
        {
            List<ApertureTranslate> apertureTranslateList = new List<ApertureTranslate>(); 
            int apertureIndex = 10;
            int lastAperture = Gerber.MaximumApertures;

            GerberImage newImage = new GerberImage();
            newImage.FileType = sourceImage.FileType;
            newImage.ImageInfo.ImageName = string.Copy(sourceImage.ImageInfo.ImageName);
            newImage.ImageInfo.FileTypeName = string.Copy(sourceImage.ImageInfo.FileTypeName);
            newImage.ImageInfo.PlotterFilm = string.Copy(sourceImage.ImageInfo.PlotterFilm);
            newImage.Format = sourceImage.Format;
            newImage.GerberStats = sourceImage.GerberStats;
            newImage.DrillStats = sourceImage.DrillStats;

            // Copy all the apertures to the new aperture list and remove any vacant positions.
            for (int i = 0; i < lastAperture; i++)
            {
                if (sourceImage.ApertureArray()[i] != null)
                {
                    newImage.ApertureArray()[apertureIndex] = CopyAperture(sourceImage.ApertureArray()[i]);
                    if (i != apertureIndex)
                    {
                        // Add it the translation to the table.
                        ApertureTranslate apertureTranslate = new ApertureTranslate();
                        apertureTranslate.sourceIndex = i;
                        apertureTranslate.newIndex = apertureIndex;
                        apertureTranslateList.Add(apertureTranslate);
                    }

                    apertureIndex++;
                }
            }

            for (int i = 0; i < sourceImage.GerberNetList.Count; i++)
                CopyNet(newImage, sourceImage.GerberNetList[i], apertureTranslateList);

            return newImage;
        }

        // Copy the aperture.
        private static Aperture CopyAperture(Aperture sourceAperture)
        {
            Aperture newAperture = new Aperture();
            if (sourceAperture.SimplifiedMacroList.Count > 0)
            {
                foreach (SimplifiedApertureMacro sam in sourceAperture.SimplifiedMacroList)
                {
                    // Copy the parameters.
                    Collection<double> paramters = new Collection<double>();
                    foreach (double p in sam.Parameters)
                        paramters.Add(p);

                    SimplifiedApertureMacro macro = new SimplifiedApertureMacro(paramters);
                    macro.ApertureType = sam.ApertureType;
                    newAperture.SimplifiedMacroList.Add(macro);
                }
            }

            // Copy aperture parameters.
            Array.Copy(sourceAperture.Parameters(), newAperture.Parameters(), sourceAperture.ParameterCount);
            if (sourceAperture.ApertureMacro != null)
            {
                ApertureMacro am = new ApertureMacro();
                am.Name = sourceAperture.ApertureMacro.Name;
                am.NufPushes = sourceAperture.ApertureMacro.NufPushes;
                newAperture.ApertureMacro = sourceAperture.ApertureMacro;
                newAperture.ParameterCount = 6;
            }

            newAperture.ApertureType = sourceAperture.ApertureType;
            newAperture.ParameterCount = sourceAperture.ParameterCount;
            newAperture.Unit = sourceAperture.Unit;
            return newAperture;
        }

        private static void CopyNet(GerberImage newImage, GerberNet sourceNet, List<ApertureTranslate> translateTable)
        {
            GerberNet newNet = new GerberNet(newImage, sourceNet, sourceNet.Level, sourceNet.NetState);
            newNet.Aperture = GetApertureTranslation(translateTable, sourceNet.Aperture);
            newNet.ApertureState = sourceNet.ApertureState;
            if (sourceNet.BoundingBox != null)
            {
                BoundingBox bBox = sourceNet.BoundingBox;
                newNet.BoundingBox = new BoundingBox(bBox.Left, bBox.Top, bBox.Right, bBox.Bottom);
            }

            if(sourceNet.CircleSegment != null)
            {
                CircleSegment cSeg = sourceNet.CircleSegment;
                newNet.CircleSegment = new CircleSegment(cSeg.CenterX, cSeg.CenterY, cSeg.Width, cSeg.Height, cSeg.StartAngle, cSeg.EndAngle);
            }

            newNet.Interpolation = sourceNet.Interpolation;
            newNet.StartX = sourceNet.StartX;
            newNet.StartY = sourceNet.StartY;
            newNet.EndX = sourceNet.EndX;
            newNet.EndY = sourceNet.EndY;
        }

        // Compact the aperture list.
        private static int GetApertureTranslation(List<ApertureTranslate> translateTable, int sourceAperture)
        {
            for(int i = 0; i < translateTable.Count; i++)
            {
                if (translateTable[i].sourceIndex == sourceAperture)
                    return translateTable[i].newIndex;
            }

            // Not in the table, return the original aperture number.
            return sourceAperture;
        }
    }
}
