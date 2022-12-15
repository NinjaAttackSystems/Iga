﻿/* DrillStats.cs - Classes for handling drill file statistics and error information. */

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
using System.Collections.ObjectModel;

namespace GerberVS
{
    /// <summary>
    /// List of drills found in active levels.
    /// </summary>
    /// <remarks>Used in reporting statistics.</remarks>
    public class DrillInfo
    {
        /// <summary>
        /// Number of drill holes of this size (tool number)
        /// </summary>
        public int DrillCount { get; set; }

        /// <summary>
        /// Drill number (tool number)
        /// </summary>
        public int DrillNumber { get; set; }

        /// <summary>
        /// Drill hole size.
        /// </summary>
        public double DrillSize { get; set; }

        /// <summary>
        /// Drill size units.
        /// </summary>
        public string DrillUnit { get; set; }

        /// <summary>
        /// Information pertaining to a tool (drill) number.
        /// </summary>
        public DrillInfo()
        {
            DrillCount = 0;
            DrillNumber = -1;
            DrillSize = 0.0;
            DrillUnit = String.Empty;
        }
    }

    /// <summary>
    /// Maintains statistics on the various codes used in a Drill file.
    /// </summary>
    public class DrillFileStats
    {
        private Collection<GerberError> errorList;
        private Collection<DrillInfo> drillInfoList;

        public int LevelCount { get; set; }
        public int Comment { get; set; }
        public int F { get; set; }
        public int G00 { get; set; }
        public int G01 { get; set; }
        public int G02 { get; set; }
        public int G03 { get; set; }
        public int G04 { get; set; }
        public int G05 { get; set; }
        public int G85 { get; set; }
        public int G90 { get; set; }
        public int G91 { get; set; }
        public int G93 { get; set; }
        public int GUnknown { get; set; }
        public int M00 { get; set; }
        public int M01 { get; set; }
        public int M18 { get; set; }
        public int M25 { get; set; }
        public int M30 { get; set; }
        public int M31 { get; set; }
        public int M45 { get; set; }
        public int M47 { get; set; }
        public int M48 { get; set; }
        public int M71 { get; set; }
        public int M72 { get; set; }
        public int M95 { get; set; }
        public int M97 { get; set; }
        public int M98 { get; set; }
        public int MUnknown { get; set; }
        public int R { get; set; }
        public int Unknown { get; set; }
        public int TotalCount { get; set; }  // Used to total up the drill count across all levels/sizes.
        public string Detect { get; set; }

        internal DrillFileStats()
        {
            errorList = new Collection<GerberError>();
            drillInfoList = new Collection<DrillInfo>();
        }

        /// <summary>
        /// Errors found during parsing of a drill file.
        /// </summary>
        public Collection<GerberError> ErrorList
        {
            get { return errorList; }
        }

        /// <summary>
        /// Drill information for each tool in the drill file.
        /// </summary>
        public Collection<DrillInfo> DrillInfoList
        {
            get { return drillInfoList; }
        }

        /// <summary>
        /// Adds a new error to the error list;
        /// </summary>
        /// <param name="level">level</param>
        /// <param name="errorMessage">error message</param>
        /// <param name="errorType">type of error</param>
        /// <param name="lineNumber">line number in the file where the error occurred</param>
        /// <param name="fileName"> file in which the error occurred</param>
        /// <remarks>
        /// Only unique errors are added to the list.
        /// </remarks>
        internal void AddNewError(int level, string errorMessage, GerberErrorType errorType, string fileName, int lineNumber)
        {
            bool exists = false;

            // Check that the new error is unique.
            foreach (GerberError error in errorList)
            {
                if (error.ErrorMessage == errorMessage && error.Level == level && error.FileName == fileName)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
                errorList.Add(new GerberError(level, errorMessage, errorType, fileName, lineNumber));
        }

        /// <summary>
        /// Adds a new error to the error list;
        /// </summary>
        /// <param name="level">level</param>
        /// <param name="errorMessage">error message</param>
        /// <param name="errorType">type of error</param>
        internal void AddNewError(int level, string errorMessage, GerberErrorType errorType)
        {
            AddNewError(level, errorMessage, errorType, String.Empty, 0);
        }

        /// <summary>
        /// Update the count of an existing drill.
        /// </summary>
        /// <param name="drillNumber"></param>
        internal void IncrementDrillCounter(int drillNumber)
        {
            // First check to see if this drill is already in the list.
            foreach (DrillInfo drillInfo in drillInfoList)
            {
                if (drillInfo.DrillNumber == drillNumber)
                {
                    drillInfo.DrillCount++;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the drill information of an existing drill entry.
        /// </summary>
        /// <param name="drillNumber"></param>
        /// <param name="drillSize"></param>
        /// <param name="drillUnit"></param>
        internal void ModifyDrillList(int drillNumber, double drillSize, string drillUnit)
        {

            // Look for this drill number in drill list.
            foreach (DrillInfo drillInfo in drillInfoList)
            {
                // And update it.
                if (drillInfo.DrillNumber == drillNumber)
                {
                    drillInfo.DrillSize = drillSize;
                    drillInfo.DrillUnit = drillUnit;
                }
            }

            return;
        }

        /// <summary>
        /// Adds a new drill to the drill list.
        /// </summary>
        /// <param name="drillNumber"></param>
        /// <param name="drillSize"></param>
        /// <param name="drillUnit"></param>
        internal void AddToDrillList(int drillNumber, double drillSize, string drillUnit)
        {
            bool exists = false;
            // First check to see if this drill is already in the list.
            if (drillInfoList.Count > 0)
            {
                foreach (DrillInfo drillInfo in drillInfoList)
                {
                    if (drillNumber == drillInfo.DrillNumber)
                    {
                        exists = true;
                        break;
                    }
                }
            }

            // Create a new drill and add it to the drill list.
            if (!exists)
            {
                DrillInfo drillInfo = new DrillInfo();
                drillInfo.DrillNumber = drillNumber;
                drillInfo.DrillSize = drillSize;
                drillInfo.DrillCount = 0;
                drillInfo.DrillUnit = drillUnit;

                drillInfoList.Add(drillInfo);
            }

            return;
        }
    }
}


