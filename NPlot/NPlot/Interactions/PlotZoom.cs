//
// NPlot - A charting library for .NET
// 
// PlotZoom.cs
//
// Copyright (C) Hywel Thomas, Matt Howlett and others 2003-2013
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
// 3. Neither the name of NPlot nor the names of its contributors may
//    be used to endorse or promote products derived from this software without
//    specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
// OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
// OF THE POSSIBILITY OF SUCH DAMAGE.
//
using System;
using System.Drawing;

namespace NPlot
{
    /// <summary>
    /// Mouse Scroll (wheel) increases or decreases both axes scaling factors
    /// Zoom direction is Up/+ve/ZoomIn or Down/-ve/ZoomOut.  If the mouse
    /// pointer is inside the plot area, its position is used as the focus point
    /// of the zoom, otherwise the centre of the plot is used as the default
    /// </summary>
    public class PlotZoom : Interaction
    {
        private double sensitivity_ = 1.0;  // default value
        private Rectangle focusRect = Rectangle.Empty;
        private Point pF = Point.Empty;
   
        /// <summary>
        /// Mouse Scroll (wheel) method for AxisZoom interaction
        /// </summary>
        public override bool DoMouseScroll (int X, int Y, int direction, Modifier keys, InteractivePlotSurface2D ps)
        {
            double proportion = 0.1*sensitivity_;   // use initial zoom of 10%
            double focusX = 0.5, focusY = 0.5;      // default focus point
                
            // Zoom direction is +1 for Up/ZoomIn, or -1 for Down/ZoomOut
            proportion *= -direction;

            // delete previous focusPoint drawing - this is all a bit 'tentative'
            ps.QueueDraw (focusRect);

            Rectangle area = ps.PlotAreaBoundingBoxCache;
            if (area.Contains(X,Y)) {
                pF.X = X;
                pF.Y = Y;
                focusX = (double)(X - area.Left)/(double)area.Width;
                focusY = (double)(area.Bottom - Y)/(double)area.Height;
            }

            // Zoom in/out for all defined axes
            ps.CacheAxes();
            ps.ZoomXAxes (proportion,focusX);
            ps.ZoomYAxes (proportion,focusY);

            int x = pF.X-10;
            int y = pF.Y-10;

            focusRect = new Rectangle (x, y, 21, 21);
            // draw new focusRect
            ps.QueueDraw (focusRect);
                
            return (true);
        }

        /// <summary>
        /// MouseMove method for PlotScroll
        /// </summary>
        public override bool DoMouseMove (int X, int Y, Modifier keys, InteractivePlotSurface2D ps)
        {
            // delete previous focusPoint drawing
            ps.QueueDraw (focusRect);
            return false;
        }

        public override void DoDraw (Graphics g, Rectangle dirtyRect)
        {
            DrawFocus (g);
        }

        /// <summary>
        /// Sensitivity factor for axis zoom
        /// </summary>
        /// <value></value>
        public double Sensitivity
        {
            get { return sensitivity_; }
            set { sensitivity_ = value; }
        }

        private void DrawFocus (Graphics g)
        {
            // Draw the Focus-point when zooming
            if (focusRect != Rectangle.Empty) {
                using (Pen rPen = new Pen (Color.White)) {
                    g.DrawRectangle (rPen, focusRect);
                }
            }
        }

    } // Mouse Wheel Zoom
    
}
