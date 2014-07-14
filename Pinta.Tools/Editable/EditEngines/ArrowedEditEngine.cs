﻿// 
// ArrowedEditEngine.cs
//  
// Author:
//       Andrew Davis <andrew.3.1415@gmail.com>
// 
// Copyright (c) 2014 Andrew Davis, GSoC 2014
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Cairo;
using Pinta.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Unix;

namespace Pinta.Tools
{
	public abstract class ArrowedEditEngine : BaseEditEngine
    {
        private Gtk.SeparatorToolItem arrowSep;
        private ToolBarLabel arrowLabel;
        private Gtk.CheckButton showArrowOneBox, showArrowTwoBox;
        private bool showOtherArrowOptions;

        private ToolBarComboBox arrowSize;
        private ToolBarLabel arrowSizeLabel;
        private ToolBarButton arrowSizeMinus, arrowSizePlus;

        private ToolBarComboBox arrowAngleOffset;
        private ToolBarLabel arrowAngleOffsetLabel;
        private ToolBarButton arrowAngleOffsetMinus, arrowAngleOffsetPlus;

        private ToolBarComboBox arrowLengthOffset;
        private ToolBarLabel arrowLengthOffsetLabel;
        private ToolBarButton arrowLengthOffsetMinus, arrowLengthOffsetPlus;


        /// <summary>
        /// Set the Arrow options for the current shape to their respective values in the toolbar.
        /// </summary>
        private void SetArrowOptions()
        {
            LineCurveSeriesEngine selEngine = (LineCurveSeriesEngine)SelectedShapeEngine;

            if (selEngine != null)
            {
                selEngine.Arrow1.Show = showArrowOneBox.Active;
                selEngine.Arrow2.Show = showArrowTwoBox.Active;

                showOtherArrowOptions = showArrowOneBox.Active || showArrowTwoBox.Active;

                if (showOtherArrowOptions)
                {
                    Double.TryParse((arrowSize.ComboBox as Gtk.ComboBoxEntry).Entry.Text, out selEngine.Arrow1.ArrowSize);
                    Double.TryParse((arrowAngleOffset.ComboBox as Gtk.ComboBoxEntry).Entry.Text, out selEngine.Arrow1.AngleOffset);
                    Double.TryParse((arrowLengthOffset.ComboBox as Gtk.ComboBoxEntry).Entry.Text, out selEngine.Arrow1.LengthOffset);

                    selEngine.Arrow1.ArrowSize = Utility.Clamp(selEngine.Arrow1.ArrowSize, 1d, 100d);
                    selEngine.Arrow2.ArrowSize = selEngine.Arrow1.ArrowSize;
                    selEngine.Arrow1.AngleOffset = Utility.Clamp(selEngine.Arrow1.AngleOffset, -89d, 89d);
                    selEngine.Arrow2.AngleOffset = selEngine.Arrow1.AngleOffset;
                    selEngine.Arrow1.LengthOffset = Utility.Clamp(selEngine.Arrow1.LengthOffset, -100d, 100d);
                    selEngine.Arrow2.LengthOffset = selEngine.Arrow1.LengthOffset;
                }
            }
        }

        /// <summary>
        /// Set the Arrow options in the toolbar to their respective values for the current shape.
        /// </summary>
        private void SetToolbarArrowOptions()
        {
            LineCurveSeriesEngine selEngine = (LineCurveSeriesEngine)SelectedShapeEngine;

            if (selEngine != null)
            {
                showArrowOneBox.Active = selEngine.Arrow1.Show;
                showArrowTwoBox.Active = selEngine.Arrow2.Show;

                if (showOtherArrowOptions)
                {
                    (arrowSize.ComboBox as Gtk.ComboBoxEntry).Entry.Text = selEngine.Arrow1.ArrowSize.ToString();
                    (arrowAngleOffset.ComboBox as Gtk.ComboBoxEntry).Entry.Text = selEngine.Arrow1.AngleOffset.ToString();
                    (arrowLengthOffset.ComboBox as Gtk.ComboBoxEntry).Entry.Text = selEngine.Arrow1.LengthOffset.ToString();
                }
            }
        }


        #region ToolbarEventHandlers

        void arrowSizeMinus_Clicked(object sender, EventArgs e)
        {
            double newSize = 10d;

            if (Double.TryParse(arrowSize.ComboBox.ActiveText, out newSize))
            {
                --newSize;

                if (newSize < 1d)
                {
                    newSize = 1d;
                }
            }
            else
            {
                newSize = 10d;
            }

            (arrowSize.ComboBox as Gtk.ComboBoxEntry).Entry.Text = newSize.ToString();
        }

        void arrowSizePlus_Clicked(object sender, EventArgs e)
        {
            double newSize = 10d;

            if (Double.TryParse(arrowSize.ComboBox.ActiveText, out newSize))
            {
                ++newSize;

                if (newSize > 100d)
                {
                    newSize = 100d;
                }
            }
            else
            {
                newSize = 10d;
            }

            (arrowSize.ComboBox as Gtk.ComboBoxEntry).Entry.Text = newSize.ToString();
        }

        void arrowAngleOffsetMinus_Clicked(object sender, EventArgs e)
        {
            double newAngle = 0d;

            if (Double.TryParse(arrowAngleOffset.ComboBox.ActiveText, out newAngle))
            {
                --newAngle;

                if (newAngle < -89d)
                {
                    newAngle = -89d;
                }
            }
            else
            {
                newAngle = 0d;
            }

            (arrowAngleOffset.ComboBox as Gtk.ComboBoxEntry).Entry.Text = newAngle.ToString();
        }

        void arrowAngleOffsetPlus_Clicked(object sender, EventArgs e)
        {
            double newAngle = 0d;

            if (Double.TryParse(arrowAngleOffset.ComboBox.ActiveText, out newAngle))
            {
                ++newAngle;

                if (newAngle > 89d)
                {
                    newAngle = 89d;
                }
            }
            else
            {
                newAngle = 0d;
            }

            (arrowAngleOffset.ComboBox as Gtk.ComboBoxEntry).Entry.Text = newAngle.ToString();
        }

        void arrowLengthOffsetMinus_Clicked(object sender, EventArgs e)
        {
            double newLength = 10d;

            if (Double.TryParse(arrowLengthOffset.ComboBox.ActiveText, out newLength))
            {
                --newLength;

                if (newLength < -100d)
                {
                    newLength = -100d;
                }
            }
            else
            {
                newLength = 10d;
            }

            (arrowLengthOffset.ComboBox as Gtk.ComboBoxEntry).Entry.Text = newLength.ToString();
        }

        void arrowLengthOffsetPlus_Clicked(object sender, EventArgs e)
        {
            double newLength = 10d;

            if (Double.TryParse(arrowLengthOffset.ComboBox.ActiveText, out newLength))
            {
                ++newLength;

                if (newLength > 100d)
                {
                    newLength = 100d;
                }
            }
            else
            {
                newLength = 10d;
            }

            (arrowLengthOffset.ComboBox as Gtk.ComboBoxEntry).Entry.Text = newLength.ToString();
        }

        #endregion ToolbarEventHandlers


        public override void HandleBuildToolBar(Gtk.Toolbar tb)
        {
            base.HandleBuildToolBar(tb);


            #region Show Arrows

            //Arrow separator.

            if (arrowSep == null)
            {
                arrowSep = new Gtk.SeparatorToolItem();

                showOtherArrowOptions = false;
            }

            tb.AppendItem(arrowSep);


            if (arrowLabel == null)
            {
                arrowLabel = new ToolBarLabel(string.Format(" {0}: ", Catalog.GetString("Arrow")));
            }

            tb.AppendItem(arrowLabel);


            //Show arrow 1.

            showArrowOneBox = new Gtk.CheckButton("1");

            showArrowOneBox.Toggled += (o, e) =>
            {
                //Determine whether to change the visibility of Arrow options in the toolbar based on the updated Arrow showing/hiding.
                if (!showArrowOneBox.Active && !showArrowTwoBox.Active)
                {
                    if (showOtherArrowOptions)
                    {
                        tb.Remove(arrowSizeLabel);
                        tb.Remove(arrowSizeMinus);
                        tb.Remove(arrowSize);
                        tb.Remove(arrowSizePlus);
                        tb.Remove(arrowAngleOffsetLabel);
                        tb.Remove(arrowAngleOffsetMinus);
                        tb.Remove(arrowAngleOffset);
                        tb.Remove(arrowAngleOffsetPlus);
                        tb.Remove(arrowLengthOffsetLabel);
                        tb.Remove(arrowLengthOffsetMinus);
                        tb.Remove(arrowLengthOffset);
                        tb.Remove(arrowLengthOffsetPlus);

                        showOtherArrowOptions = false;
                    }
                }
                else
                {
                    if (!showOtherArrowOptions)
                    {
                        tb.Add(arrowSizeLabel);
                        tb.Add(arrowSizeMinus);
                        tb.Add(arrowSize);
                        tb.Add(arrowSizePlus);
                        tb.Add(arrowAngleOffsetLabel);
                        tb.Add(arrowAngleOffsetMinus);
                        tb.Add(arrowAngleOffset);
                        tb.Add(arrowAngleOffsetPlus);
                        tb.Add(arrowLengthOffsetLabel);
                        tb.Add(arrowLengthOffsetMinus);
                        tb.Add(arrowLengthOffset);
                        tb.Add(arrowLengthOffsetPlus);

                        showOtherArrowOptions = true;
                    }
                }

                LineCurveSeriesEngine selEngine = (LineCurveSeriesEngine)SelectedShapeEngine;

                if (selEngine != null)
                {
                    selEngine.Arrow1.Show = showArrowOneBox.Active;

                    DrawActiveShape(false, false, true, false);
                }
            };

            tb.AddWidgetItem(showArrowOneBox);


            //Show arrow 2.

            showArrowTwoBox = new Gtk.CheckButton("2");

            showArrowTwoBox.Toggled += (o, e) =>
            {
                //Determine whether to change the visibility of Arrow options in the toolbar based on the updated Arrow showing/hiding.
                if (!showArrowOneBox.Active && !showArrowTwoBox.Active)
                {
                    if (showOtherArrowOptions)
                    {
                        tb.Remove(arrowSizeLabel);
                        tb.Remove(arrowSizeMinus);
                        tb.Remove(arrowSize);
                        tb.Remove(arrowSizePlus);
                        tb.Remove(arrowAngleOffsetLabel);
                        tb.Remove(arrowAngleOffsetMinus);
                        tb.Remove(arrowAngleOffset);
                        tb.Remove(arrowAngleOffsetPlus);
                        tb.Remove(arrowLengthOffsetLabel);
                        tb.Remove(arrowLengthOffsetMinus);
                        tb.Remove(arrowLengthOffset);
                        tb.Remove(arrowLengthOffsetPlus);

                        showOtherArrowOptions = false;
                    }
                }
                else
                {
                    if (!showOtherArrowOptions)
                    {
                        tb.Add(arrowSizeLabel);
                        tb.Add(arrowSizeMinus);
                        tb.Add(arrowSize);
                        tb.Add(arrowSizePlus);
                        tb.Add(arrowAngleOffsetLabel);
                        tb.Add(arrowAngleOffsetMinus);
                        tb.Add(arrowAngleOffset);
                        tb.Add(arrowAngleOffsetPlus);
                        tb.Add(arrowLengthOffsetLabel);
                        tb.Add(arrowLengthOffsetMinus);
                        tb.Add(arrowLengthOffset);
                        tb.Add(arrowLengthOffsetPlus);

                        showOtherArrowOptions = true;
                    }
                }

                LineCurveSeriesEngine selEngine = (LineCurveSeriesEngine)SelectedShapeEngine;

                if (selEngine != null)
                {
                    selEngine.Arrow2.Show = showArrowTwoBox.Active;

					DrawActiveShape(false, false, true, false);
                }
            };

            tb.AddWidgetItem(showArrowTwoBox);

            #endregion Show Arrows


            #region Arrow Size

            if (arrowSizeLabel == null)
            {
                arrowSizeLabel = new ToolBarLabel(string.Format(" {0}: ", Catalog.GetString("Size")));
            }

            if (arrowSizeMinus == null)
            {
                arrowSizeMinus = new ToolBarButton("Toolbar.MinusButton.png", "", Catalog.GetString("Decrease arrow size"));
                arrowSizeMinus.Clicked += new EventHandler(arrowSizeMinus_Clicked);
            }

            if (arrowSize == null)
            {
                arrowSize = new ToolBarComboBox(65, 7, true,
                    "3", "4", "5", "6", "7", "8", "9", "10", "12", "15", "18",
                    "20", "25", "30", "40", "50", "60", "70", "80", "90", "100");

                arrowSize.ComboBox.Changed += (o, e) =>
                {
                    if (arrowSize.ComboBox.ActiveText.Length < 1)
                    {
                        //Ignore the change until the user enters something.
                        return;
                    }
                    else
                    {
                        double newSize = 10d;

                        if (arrowSize.ComboBox.ActiveText == "-")
                        {
                            //The user is trying to enter a negative value: change it to 1.
                            newSize = 1d;
                        }
                        else
                        {
                            if (Double.TryParse(arrowSize.ComboBox.ActiveText, out newSize))
                            {
                                if (newSize < 1d)
                                {
                                    //Less than 1: change it to 1.
                                    newSize = 1d;
                                }
                                else if (newSize > 100d)
                                {
                                    //Greater than 100: change it to 100.
                                    newSize = 100d;
                                }
                            }
                            else
                            {
                                //Not a number: wait until the user enters something.
                                return;
                            }
                        }

                        (arrowSize.ComboBox as Gtk.ComboBoxEntry).Entry.Text = newSize.ToString();

                        LineCurveSeriesEngine selEngine = (LineCurveSeriesEngine)SelectedShapeEngine;

                        if (selEngine != null)
                        {
                            selEngine.Arrow1.ArrowSize = newSize;
                            selEngine.Arrow2.ArrowSize = newSize;

							DrawActiveShape(false, false, true, false);
                        }
                    }
                };
            }

            if (arrowSizePlus == null)
            {
                arrowSizePlus = new ToolBarButton("Toolbar.PlusButton.png", "", Catalog.GetString("Increase arrow size"));
                arrowSizePlus.Clicked += new EventHandler(arrowSizePlus_Clicked);
            }

            #endregion Arrow Size


            #region Angle Offset

            if (arrowAngleOffsetLabel == null)
            {
                arrowAngleOffsetLabel = new ToolBarLabel(string.Format(" {0}: ", Catalog.GetString("Angle")));
            }

            if (arrowAngleOffsetMinus == null)
            {
                arrowAngleOffsetMinus = new ToolBarButton("Toolbar.MinusButton.png", "", Catalog.GetString("Decrease angle offset"));
                arrowAngleOffsetMinus.Clicked += new EventHandler(arrowAngleOffsetMinus_Clicked);
            }

            if (arrowAngleOffset == null)
            {
                arrowAngleOffset = new ToolBarComboBox(65, 9, true,
                    "-30", "-25", "-20", "-15", "-10", "-5", "0", "5", "10", "15", "20", "25", "30");

                arrowAngleOffset.ComboBox.Changed += (o, e) =>
                {
                    if (arrowAngleOffset.ComboBox.ActiveText.Length < 1)
                    {
                        //Ignore the change until the user enters something.
                        return;
                    }
                    else if (arrowAngleOffset.ComboBox.ActiveText == "-")
                    {
                        //The user is trying to enter a negative value: ignore the change until the user enters more.
                        return;
                    }
                    else
                    {
                        double newAngle = 15d;

                        if (Double.TryParse(arrowAngleOffset.ComboBox.ActiveText, out newAngle))
                        {
                            if (newAngle < -89d)
                            {
                                //Less than -89: change it to -89.
                                newAngle = -89d;
                            }
                            else if (newAngle > 89d)
                            {
                                //Greater than 89: change it to 89.
                                newAngle = 89d;
                            }
                        }
                        else
                        {
                            //Not a number: wait until the user enters something.
                            return;
                        }

                        (arrowAngleOffset.ComboBox as Gtk.ComboBoxEntry).Entry.Text = newAngle.ToString();

                        LineCurveSeriesEngine selEngine = (LineCurveSeriesEngine)SelectedShapeEngine;

                        if (selEngine != null)
                        {
                            selEngine.Arrow1.AngleOffset = newAngle;
                            selEngine.Arrow2.AngleOffset = newAngle;

							DrawActiveShape(false, false, true, false);
                        }
                    }
                };
            }

            if (arrowAngleOffsetPlus == null)
            {
                arrowAngleOffsetPlus = new ToolBarButton("Toolbar.PlusButton.png", "", Catalog.GetString("Increase angle offset"));
                arrowAngleOffsetPlus.Clicked += new EventHandler(arrowAngleOffsetPlus_Clicked);
            }

            #endregion Angle Offset


            #region Length Offset

            if (arrowLengthOffsetLabel == null)
            {
                arrowLengthOffsetLabel = new ToolBarLabel(string.Format(" {0}: ", Catalog.GetString("Length")));
            }

            if (arrowLengthOffsetMinus == null)
            {
                arrowLengthOffsetMinus = new ToolBarButton("Toolbar.MinusButton.png", "", Catalog.GetString("Decrease length offset"));
                arrowLengthOffsetMinus.Clicked += new EventHandler(arrowLengthOffsetMinus_Clicked);
            }

            if (arrowLengthOffset == null)
            {
                arrowLengthOffset = new ToolBarComboBox(65, 8, true,
                    "-30", "-25", "-20", "-15", "-10", "-5", "0", "5", "10", "15", "20", "25", "30");

                arrowLengthOffset.ComboBox.Changed += (o, e) =>
                {
                    if (arrowLengthOffset.ComboBox.ActiveText.Length < 1)
                    {
                        //Ignore the change until the user enters something.
                        return;
                    }
                    else if (arrowLengthOffset.ComboBox.ActiveText == "-")
                    {
                        //The user is trying to enter a negative value: ignore the change until the user enters more.
                        return;
                    }
                    else
                    {
                        double newLength = 10d;

                        if (Double.TryParse(arrowLengthOffset.ComboBox.ActiveText, out newLength))
                        {
                            if (newLength < -100d)
                            {
                                //Less than -100: change it to -100.
                                newLength = -100d;
                            }
                            else if (newLength > 100d)
                            {
                                //Greater than 100: change it to 100.
                                newLength = 100d;
                            }
                        }
                        else
                        {
                            //Not a number: wait until the user enters something.
                            return;
                        }

                        (arrowLengthOffset.ComboBox as Gtk.ComboBoxEntry).Entry.Text = newLength.ToString();

                        LineCurveSeriesEngine selEngine = (LineCurveSeriesEngine)SelectedShapeEngine;

                        if (selEngine != null)
                        {
                            selEngine.Arrow1.LengthOffset = newLength;
                            selEngine.Arrow2.LengthOffset = newLength;

							DrawActiveShape(false, false, true, false);
                        }
                    }
                };
            }

            if (arrowLengthOffsetPlus == null)
            {
                arrowLengthOffsetPlus = new ToolBarButton("Toolbar.PlusButton.png", "", Catalog.GetString("Increase length offset"));
                arrowLengthOffsetPlus.Clicked += new EventHandler(arrowLengthOffsetPlus_Clicked);
            }

            #endregion Length Offset


            if (showOtherArrowOptions)
            {
                tb.Add(arrowSizeLabel);
                tb.Add(arrowSizeMinus);
                tb.Add(arrowSize);
                tb.Add(arrowSizePlus);
                tb.Add(arrowAngleOffsetLabel);
                tb.Add(arrowAngleOffsetMinus);
                tb.Add(arrowAngleOffset);
                tb.Add(arrowAngleOffsetPlus);
                tb.Add(arrowLengthOffsetLabel);
                tb.Add(arrowLengthOffsetMinus);
                tb.Add(arrowLengthOffset);
                tb.Add(arrowLengthOffsetPlus);
            }
        }

        public override void HandleAfterUndo()
        {
            base.HandleAfterUndo();

            //Update the toolbar's arrow options.
            SetToolbarArrowOptions();
        }

        public override void HandleAfterRedo()
        {
            base.HandleAfterRedo();

            //Update the toolbar's arrow options.
            SetToolbarArrowOptions();
        }


        public ArrowedEditEngine(BaseTool passedOwner): base(passedOwner)
        {
			//Set the new shape's arrow options to be the same as what's in the toolbar settings.
			SetArrowOptions();
        }

        protected override void drawExtras(Rectangle? dirty, Context g)
        {
			ShapeEngine activeEngine = ActiveShapeEngine;
			
			if (activeEngine != null && activeEngine.ControlPoints.Count > 0)
			{
				//Draw the arrows for the currently active shape.

				PointD[] genPoints = activeEngine.GeneratedPoints;

				for (int i = 0; i < activeEngine.ControlPoints.Count; ++i)
				{
					LineCurveSeriesEngine activeLCSEngine = (LineCurveSeriesEngine)activeEngine;

					if (activeLCSEngine.Arrow1.Show)
					{
						if (genPoints.Length > 1)
						{
							dirty = dirty.UnionRectangles(activeLCSEngine.Arrow1.Draw(
								g, activeLCSEngine.OutlineColor, genPoints[0], genPoints[1]));
						}
					}

					if (activeLCSEngine.Arrow2.Show)
					{
						if (genPoints.Length > 1)
						{
							dirty = dirty.UnionRectangles(activeLCSEngine.Arrow2.Draw(
								g, activeLCSEngine.OutlineColor, genPoints[genPoints.Length - 1], genPoints[genPoints.Length - 2]));
						}
					}
				}
			}

			base.drawExtras(dirty, g);
        }
    }
}
