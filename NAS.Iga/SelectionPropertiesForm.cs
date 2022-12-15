﻿// SelectionPropertiesFrm.cs - Builds and displays the selected objects properties.

/*  Copyright (C) 2015-2019 Milton Neal <milton200954@gmail.com>

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
using System.Text;
using System.Windows.Forms;

using GerberVS;

namespace GerberView
{
    public partial class SelectionPropertiesForm : Form
    {
        private SelectionInformation selectionInfo;
        StringBuilder selectionText = new StringBuilder();
        private GerberApertureType apertureType;
        private int apertureNumber = 0;
        private double parameter0 = 0.00;
        private double parameter1 = 0.00;
        private double x, y = 0.00;

        public SelectionPropertiesForm(SelectionInformation selectionInfo)
        {
            InitializeComponent();
            this.selectionInfo = selectionInfo;
        }

        private void SelectionListFrm_Load(object sender, EventArgs e)
        {
            textBox1.Text = String.Empty;
            selectionText.Append("File: " + selectionInfo.FileInfo.FileName + Environment.NewLine);
            foreach (GerberNet net in selectionInfo.SelectedNodeArray.SelectedNetList)
            {
                if (net.ApertureState == GerberApertureState.On)
                {
                    switch (net.Interpolation)
                    {
                        case GerberInterpolation.RegionStart:
                            selectionText.Append(Environment.NewLine);
                            selectionText.Append("Object type: Region Fill"  + Environment.NewLine);
                            break;

                        case GerberInterpolation.Linear:
                        //case GerberInterpolation.DrillSlot:
                            if (net.BoundingBox != null)
                            {
                                selectionText.Append(Environment.NewLine);
                                /*if (net.Interpolation == GerberInterpolation.DrillSlot)
                                {
                                    selectionText.Append("Object type: Drill Slot" + Environment.NewLine);
                                }

                                else
                                {
                                    selectionText.Append("Object type: Line" + Environment.NewLine);
                                }*/

                                selectionText.Append("Object type: Line" + Environment.NewLine);
                                apertureNumber = net.Aperture;
                                selectionText.Append("  Aperture used: " + "D" + apertureNumber.ToString() + Environment.NewLine);
                                apertureType = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].ApertureType;
                                selectionText.Append("  Aperture type: " + apertureType.ToString() + Environment.NewLine);
                                parameter0 = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].Parameters()[0] * 1000;
                                if (apertureType == GerberApertureType.Rectangle || apertureType == GerberApertureType.Oval)
                                {
                                    parameter1 = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].Parameters()[1] * 1000;
                                    selectionText.Append("  Dimension: " + parameter0.ToString("0.000") + " x " + parameter1.ToString("0.000") + Environment.NewLine);
                                }

                                else
                                    selectionText.Append("  Diameter: " + parameter0.ToString("0.000") + Environment.NewLine);

                                x = net.StartX * 1000;
                                y = net.StartY * 1000;
                                PointD start = new PointD(x, y);
                                selectionText.Append("  Start: (" + x.ToString("0.000") + ", " + y.ToString("0.000") + ")");
                                selectionText.Append(Environment.NewLine);
                                x = net.EndX * 1000;
                                y = net.EndY * 1000;
                                PointD stop = new PointD(x, y);
                                selectionText.Append("  Stop: (" + x.ToString("0.000") + ", " + y.ToString("0.000") + ")");
                                selectionText.Append(Environment.NewLine);
                                double length = GetLineLength(start, stop);
                                selectionText.Append("  Length: " + length);
                                selectionText.Append(Environment.NewLine);
                                selectionText.Append("  Level Name: ");
                                if (net.Level.LevelName == String.Empty)
                                    selectionText.Append("<Unnamed Level>");

                                else
                                    selectionText.Append(net.Level.LevelName);

                                selectionText.Append(Environment.NewLine);
                                selectionText.Append("  Net Label: ");
                                if (net.Label == String.Empty)
                                    selectionText.Append("<Unlabeled Net>");

                                else
                                    selectionText.Append(net.Label);

                                selectionText.Append(Environment.NewLine);
                            }
                            break;

                        case GerberInterpolation.ClockwiseCircular:
                        case GerberInterpolation.CounterclockwiseCircular:
                            if (net.BoundingBox != null)
                            {
                                selectionText.Append(Environment.NewLine);
                                selectionText.Append("Object type: Arc" + Environment.NewLine);
                                apertureNumber = net.Aperture;
                                selectionText.Append("  Aperture used: " + "D" + apertureNumber.ToString() + Environment.NewLine);
                                apertureType = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].ApertureType;
                                selectionText.Append("  Aperture type: " + apertureType.ToString() + Environment.NewLine);
                                parameter0 = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].Parameters()[0] * 1000;
                                if (apertureType == GerberApertureType.Rectangle || apertureType == GerberApertureType.Oval)
                                {
                                    parameter1 = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].Parameters()[1] * 1000;
                                    selectionText.Append("  Dimension: " + parameter0.ToString("0.000") + " x " + parameter1.ToString("0.000") + Environment.NewLine);
                                }

                                else
                                    selectionText.Append("  Diameter: " + parameter0.ToString("0.000") + Environment.NewLine);

                                x = net.StartX * 1000;
                                y = net.StartY * 1000;
                                selectionText.Append("  Start: (" + x.ToString("0.000") + ", " + y.ToString("0.000") + ")");
                                selectionText.Append(Environment.NewLine);
                                x = net.EndX * 1000;
                                y = net.EndY * 1000;
                                selectionText.Append("  Stop: (" + x.ToString("0.000") + ", " + y.ToString("0.000") + ")");
                                selectionText.Append(Environment.NewLine);
                                x = net.CircleSegment.CenterX * 1000;
                                y = net.CircleSegment.CenterY * 1000;
                                selectionText.Append("  Centre: (" + x.ToString("0.000") + ", " + y.ToString("0.000") + ")");
                                selectionText.Append(Environment.NewLine);
                                x = net.CircleSegment.StartAngle;
                                y = net.CircleSegment.SweepAngle;
                                selectionText.Append("  Start: "+ x.ToString("0.0000") + ", Sweep: " + y.ToString("0.0000"));
                                selectionText.Append(Environment.NewLine);
                                selectionText.Append("  Direction: ");
                                selectionText.Append(net.Interpolation == GerberInterpolation.ClockwiseCircular ? "CW" : "CCW");
                                selectionText.Append(Environment.NewLine);
                                selectionText.Append("  Level Name: ");
                                if (net.Level.LevelName == String.Empty)
                                    selectionText.Append("<Unnamed Level>");

                                else
                                    selectionText.Append(net.Level.LevelName);

                                selectionText.Append(Environment.NewLine);
                                selectionText.Append("  Net Label: ");
                                if (net.Label == String.Empty)
                                    selectionText.Append("<Unlabeled Net>");

                                else
                                    selectionText.Append(net.Label);

                                selectionText.Append(Environment.NewLine);

                            }
                            break;
                    }
                }

                if (net.ApertureState == GerberApertureState.Flash)
                {
                    apertureNumber = net.Aperture;
                    selectionText.Append(Environment.NewLine);
                    selectionText.Append("Object type: Flashed Aperture" + Environment.NewLine);
                    selectionText.Append("  Aperture used: " + "D" + apertureNumber.ToString() + Environment.NewLine);
                    apertureType = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].ApertureType;
                    if (apertureType != GerberApertureType.Macro)
                    {
                        selectionText.Append("  Aperture type: " + apertureType.ToString() + Environment.NewLine);
                        switch (apertureType)
                        {
                            case GerberApertureType.Circle:
                                parameter0 = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].Parameters()[0] * 1000;
                                selectionText.Append("  Diameter: " + parameter0.ToString("0.000") + Environment.NewLine);
                                break;

                            case GerberApertureType.Rectangle:
                            case GerberApertureType.Oval:
                                parameter0 = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].Parameters()[0] * 1000;
                                parameter1 = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].Parameters()[1] * 1000;
                                selectionText.Append("  Dimension: " + parameter0.ToString("0.000") + " x " + parameter1.ToString("0.000") + Environment.NewLine);
                                break;
                        }

                    }

                    else
                    {
                        if (selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].ApertureMacro != null)
                        {
                            apertureType = selectionInfo.FileInfo.Image.ApertureArray()[apertureNumber].SimplifiedMacroList[0].ApertureType;
                            selectionText.Append("  Aperture type: " + apertureType.ToString() + Environment.NewLine);
                        }
                    }

                    x = net.EndX * 1000;
                    y = net.EndY * 1000;
                    selectionText.Append("  Location: (" + x.ToString("0.000") + ", " + y.ToString("0.000") + ")");
                    selectionText.Append(Environment.NewLine);
                    selectionText.Append("  Level Name: ");
                    if (net.Level.LevelName == String.Empty)
                        selectionText.Append("<Unnamed Level>");

                    else
                        selectionText.Append(net.Level.LevelName);

                    selectionText.Append(Environment.NewLine);
                    selectionText.Append("  Net Label: ");
                    if (net.Label == String.Empty)
                        selectionText.Append("<Unlabeled Net>");

                    else
                        selectionText.Append(net.Label);

                    selectionText.Append(Environment.NewLine);
                }
            }

            textBox1.Text = selectionText.ToString();
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
        }

        private double GetLineLength(PointD start, PointD stop)
        {
            double result = Math.Sqrt(Math.Pow((stop.Y - start.Y), 2) + Math.Pow((stop.X - start.X), 2));
            return Math.Round(result, 1);
        }

    }
}
