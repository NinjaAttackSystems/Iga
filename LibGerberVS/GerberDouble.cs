﻿/* GerberDouble.cs - PointD and SizeD structures. */

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
using System.ComponentModel;
using System.Globalization;

namespace GerberVS
{
    /// <summary>
    /// Represents an ordered pair of double precision coordinates.
    /// </summary>
    public struct PointD 
    {
        /// <summary>
        /// Gets or sets the value of the X coordinate.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the value of the Y coordinate.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Initialises a new instance of the PointD class with the specified values.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinal</param>
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets a value indicating whether this PointD is empty.
        /// </summary>
        public bool IsEmpty
        {
            get {return X == 0.0 && Y == 0.0; }
        }
        
        /// <summary>
        /// Returns an empty PointD structure.
        /// </summary>
        /// <returns>PointD where X = 0 and Y = 0</returns>
        public static PointD Empty
        {
            get { return new PointD(0.0, 0.0); }
        }

        /// <summary>
        /// Determines whether the coordinates of two points are equal.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns>true if equal</returns>
        public static bool operator == (PointD point1, PointD point2)
        {
            return point1.Equals(point2);
        }

        /// <summary>
        /// Determines whether the coordinates of two points are not equal.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns>true if not equal</returns>
        public static bool operator != (PointD point1, PointD point2)
        {
            return !point1.Equals(point2);
        }

        /// <summary>
        /// Get a hash code for this PointD structure.
        /// </summary>
        /// <returns>has code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Test to see if object PointD is the same type and dimension as this.
        /// </summary>
        /// <param name="obj">obj to test</param>
        /// <returns>true if equal</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is PointD))
                return false;

            PointD comp = (PointD)obj;
            return comp.X == this.X && comp.Y == this.Y && comp.GetType().Equals(this.GetType());
        }

        /// <summary>
        /// Gets a string representation of the coordinates.
        /// </summary>
        /// <returns>coordinates as a string value</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X = {0}, Y = {1}", X, Y);
        } 
    }

    /// <summary>
    /// Represents an order pair of double precission numbers typically for height and width of a rectangle.
    /// </summary>
    public struct SizeD
    {
        /// <summary>
        /// Gets or sets the value of the width coordinate.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the value of the height coordinate.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Initialises a new instance of the SizeD class with the specified dimensions.
        /// </summary>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        public SizeD(double width, double height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Test if the SizeD has a width and height value of 0.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty
        {
            get { return Width == 0.0 && Height == 0.0; }
        }

        /// <summary>
        /// Returns an empty SizeD structure.
        /// </summary>
        /// <returns>SizeD where Width = 0 and Height = 0</returns>
        public static SizeD Empty()
        {
            return new SizeD(0.0, 0.0);
        }

        /// <summary>
        /// Determines whether the coordinates of two sizes are equal.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns>true if equal</returns>
        public static bool operator == (SizeD value1, SizeD value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Determines whether the coordinates of two sizes are not equal.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns>true if equal</returns>
        public static bool operator != (SizeD value1, SizeD value2)
        {
            return !value1.Equals(value2);
        }

        /*public static SizeD Divide(SizeD size, double value)
        {
            return size / value;
        }*/
        /// <summary>
        /// Divide operator.
        /// </summary>
        /// <param name="size">size</param>
        /// <param name="value">divisor</param>
        /// <returns></returns>
        public static SizeD operator / (SizeD size, double value)
        {
            if (value == 0) // Catch divide by zero.
                return new SizeD(0.0, 0.0);

            return new SizeD(size.Width / value, size.Height / value);
        }

        /// <summary>
        /// Get a hash code for this SizeD structure.
        /// </summary>
        /// <returns>has code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Test to see if object SizeD is the same type and dimension as this.
        /// </summary>
        /// <param name="obj">obj to test</param>
        /// <returns>true if equal</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is SizeD))
                return false;

            SizeD comp = (SizeD)obj;
            return comp.Width == this.Width && comp.Height == this.Height && comp.GetType().Equals(this.GetType());
        }

        /// <summary>
        /// Gets a string representation of the coordinates.
        /// </summary>
        /// <returns>coordinates as a string value</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Width = {0}, Height = {1}", Width, Height);
        } 
    }
}
