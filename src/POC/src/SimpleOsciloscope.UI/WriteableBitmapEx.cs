using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace SimpleOsciloscope.UI
{
    public static class WriteableBitmapEx
    {
        public unsafe static void SetPixel(this BitmapContext ctx,int x,int y,byte r,byte g,byte b)
        {
            ctx.Pixels[y * ctx.Width + x] = -16777216 | (r << 16) | (g << 8) | b;
        }

        /// <summary>
        /// Draws a polyline. Add the first point also at the end of the array if the line should be closed.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="points">The points of the polyline in x and y pairs, therefore the array is interpreted as (x1, y1, x2, y2, ..., xn, yn).</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="thickness">The thickness for the line.</param>
        public static void DrawPolylineAa(this WriteableBitmap bmp, int[] points, Color color, int thickness)
        {
            var col = ConvertColor(color);
            bmp.DrawPolylineAa(points, col, thickness);
        }

        /// <summary>
        /// Draws a polyline anti-aliased. Add the first point also at the end of the array if the line should be closed.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="points">The points of the polyline in x and y pairs, therefore the array is interpreted as (x1, y1, x2, y2, ..., xn, yn).</param>
        /// <param name="color">The color for the line.</param>
        public static void DrawPolylineAa(this WriteableBitmap bmp, int[] points, int color, int thickness)
        {
            using (var context = bmp.GetBitmapContext())
            {
                // Use refs for faster access (really important!) speeds up a lot!
                var w = context.Width;
                var h = context.Height;
                var x1 = points[0];
                var y1 = points[1];

                for (var i = 2; i < points.Length; i += 2)
                {
                    var x2 = points[i];
                    var y2 = points[i + 1];

                    global::System.Windows.Media.Imaging.WriteableBitmapExtensions.DrawLineAa(context, w, h, x1, y1, x2, y2, color, thickness);
                    x1 = x2;
                    y1 = y2;
                }
            }
        }

        public static int ConvertColor(double opacity, Color color)
        {
            if (opacity < 0.0 || opacity > 1.0)
            {
                throw new ArgumentOutOfRangeException("opacity", "Opacity must be between 0.0 and 1.0");
            }

            color.A = (byte)(color.A * opacity);

            return ConvertColor(color);
        }

        public static int ConvertColor(Color color)
        {
            var col = 0;

            if (color.A != 0)
            {
                var a = color.A + 1;
                col = (color.A << 24)
                  | ((byte)((color.R * a) >> 8) << 16)
                  | ((byte)((color.G * a) >> 8) << 8)
                  | ((byte)((color.B * a) >> 8));
            }

            return col;
        }

        /// <summary>
        /// Draws a filled text.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="formattedText">The text to be rendered</param>
        /// <param name="x">The x-coordinate of the text origin</param>
        /// <param name="y">The y-coordinate of the text origin</param>
        /// <param name="color">the color.</param>
        public static void FillText(this WriteableBitmap bmp, FormattedText formattedText, int x, int y, Color color)
        {
            var _textGeometry = formattedText.BuildGeometry(new System.Windows.Point(x, y));
            FillGeometry(bmp, _textGeometry, color);
        }

        /// <summary>
        /// Draws a filled geometry.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="geometry">The geometry to be rendered</param>
        /// <param name="color">the color.</param>
        public static void FillGeometry(WriteableBitmap bmp, Geometry geometry, Color color)
        {

            if (geometry is GeometryGroup gp)
            {
                foreach (var itm in gp.Children)
                    FillGeometry(bmp, itm, color);
            }
            else if (geometry is PathGeometry pg)
            {
                var polygons = new List<int[]>();

                var poly = new List<int>();

                foreach (var fig in pg.Figures)
                {
                    ToWriteableBitmapPolygon(fig, poly);
                    polygons.Add(poly.ToArray());
                }

                bmp.FillPolygonsEvenOdd(polygons.ToArray(), color);
            }

        }

        #region Draw Text

        /// <summary>
        /// Draws an outlined text.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="formattedText">The text to be rendered</param>
        /// <param name="x">The x-coordinate of the text origin</param>
        /// <param name="y">The y-coordinate of the text origin</param>
        /// <param name="color">the color.</param>
        public static void DrawText(this WriteableBitmap bmp, FormattedText formattedText, int x, int y, Color col)
        {
            var _textGeometry = formattedText.BuildGeometry(new System.Windows.Point(x, y));
            DrawGeometry(bmp, _textGeometry, col);
        }


        /// <summary>
        /// Draws an outlined text.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="formattedText">The text to be rendered</param>
        /// <param name="x">The x-coordinate of the text origin</param>
        /// <param name="y">The y-coordinate of the text origin</param>
        /// <param name="color">the color.</param>
        /// <param name="thickness">the thickness.</param>
        public static void DrawTextAa(this WriteableBitmap bmp, FormattedText formattedText, int x, int y, Color color, int thickness)
        {
            var _textGeometry = formattedText.BuildGeometry(new System.Windows.Point(x, y));
            DrawGeometryAa(bmp, _textGeometry, color, thickness);
        }

        /// <summary>
        /// Draws outline of a geometry.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="geometry">The geometry to be rendered</param>
        /// <param name="color">the color.</param>
        private static void DrawGeometry(WriteableBitmap bmp, Geometry geometry, Color col)
        {

            if (geometry is GeometryGroup gp)
            {
                foreach (var itm in gp.Children)
                    DrawGeometry(bmp, itm, col);
            }
            else if (geometry is PathGeometry pg)
            {
                var polygons = new List<int[]>();

                var poly = new List<int>();

                foreach (var fig in pg.Figures)
                {
                    ToWriteableBitmapPolygon(fig, poly);
                    polygons.Add(poly.ToArray());
                }

                foreach (var item in polygons)
                    bmp.DrawPolyline(item, col);

            }

        }

        private static void DrawGeometryAa(WriteableBitmap bmp, Geometry geometry, Color col, int thickness)
        {

            if (geometry is GeometryGroup gp)
            {
                foreach (var itm in gp.Children)
                    DrawGeometryAa(bmp, itm, col, thickness);
            }
            else if (geometry is PathGeometry pg)
            {
                var polygons = new List<int[]>();

                var poly = new List<int>();

                foreach (var fig in pg.Figures)
                {
                    ToWriteableBitmapPolygon(fig, poly);
                    polygons.Add(poly.ToArray());
                }

                foreach (var item in polygons)
                    bmp.DrawPolylineAa(item, col, thickness);

            }

        }

        //converts the PathFigure (consis of curve, line etc) to int array polygon
        private static void ToWriteableBitmapPolygon(PathFigure fig, List<int> buf)
        {
            if (buf.Count != 0) buf.Clear();

            {
                var geo = fig;

                var lastPoint = geo.StartPoint;

                buf.Add((int)lastPoint.X);
                buf.Add((int)lastPoint.Y);

                foreach (var seg in geo.Segments)
                {
                    var flag = false;

                    if (seg is PolyBezierSegment pbs)
                    {
                        flag = true;

                        for (int i = 0; i < pbs.Points.Count; i += 3)
                        {
                            var c1 = pbs.Points[i];
                            var c2 = pbs.Points[i + 1];
                            var en = pbs.Points[i + 2];

                            var pts = ComputeBezierPoints((int)lastPoint.X, (int)lastPoint.Y, (int)c1.X, (int)c1.Y, (int)c2.X, (int)c2.Y, (int)en.X, (int)en.Y);

                            buf.AddRange(pts);

                            lastPoint = en;
                        }
                    }

                    if (seg is PolyLineSegment pls)
                    {
                        flag = true;

                        for (int i = 0; i < pls.Points.Count; i++)
                        {
                            var en = pls.Points[i];

                            buf.Add((int)en.X);
                            buf.Add((int)en.Y);

                            lastPoint = en;
                        }
                    }

                    if (seg is LineSegment ls)
                    {
                        flag = true;

                        var en = ls.Point;

                        buf.Add((int)en.X);
                        buf.Add((int)en.Y);

                        lastPoint = en;
                    }

                    if (seg is BezierSegment bs)
                    {
                        flag = true;

                        var c1 = bs.Point1;
                        var c2 = bs.Point2;
                        var en = bs.Point3;

                        var pts = ComputeBezierPoints((int)lastPoint.X, (int)lastPoint.Y, (int)c1.X, (int)c1.Y, (int)c2.X, (int)c2.Y, (int)en.X, (int)en.Y);

                        buf.AddRange(pts);

                        lastPoint = en;
                    }

                    if (!flag)
                    {
                        throw new Exception("Error in rendering text, PathSegment type not supported");
                    }
                }
            }

        }

        /// <summary>
        /// Draws a filled, cubic Beziér spline defined by start, end and two control points.
        /// </summary>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="cx1">The x-coordinate of the 1st control point.</param>
        /// <param name="cy1">The y-coordinate of the 1st control point.</param>
        /// <param name="cx2">The x-coordinate of the 2nd control point.</param>
        /// <param name="cy2">The y-coordinate of the 2nd control point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        private static List<int> ComputeBezierPoints(int x1, int y1, int cx1, int cy1, int cx2, int cy2, int x2, int y2)
        {
            // Determine distances between controls points (bounding rect) to find the optimal stepsize
            var minX = Math.Min(x1, Math.Min(cx1, Math.Min(cx2, x2)));
            var minY = Math.Min(y1, Math.Min(cy1, Math.Min(cy2, y2)));
            var maxX = Math.Max(x1, Math.Max(cx1, Math.Max(cx2, x2)));
            var maxY = Math.Max(y1, Math.Max(cy1, Math.Max(cy2, y2)));

            // Get slope
            var lenx = maxX - minX;
            var len = maxY - minY;
            if (lenx > len)
            {
                len = lenx;
            }

            // Prevent division by zero
            var list = new List<int>();
            if (len != 0)
            {
                // Init vars
                var step = StepFactor / len;
                int tx = x1;
                int ty = y1;

                // Interpolate
                for (var t = 0f; t <= 1; t += step)
                {
                    var tSq = t * t;
                    var t1 = 1 - t;
                    var t1Sq = t1 * t1;

                    tx = (int)(t1 * t1Sq * x1 + 3 * t * t1Sq * cx1 + 3 * t1 * tSq * cx2 + t * tSq * x2);
                    ty = (int)(t1 * t1Sq * y1 + 3 * t * t1Sq * cy1 + 3 * t1 * tSq * cy2 + t * tSq * y2);

                    list.Add(tx);
                    list.Add(ty);
                }

                // Prevent rounding gap
                list.Add(x2);
                list.Add(y2);
            }
            return list;
        }

        private const float StepFactor = 2f;

        #endregion


    }
}
