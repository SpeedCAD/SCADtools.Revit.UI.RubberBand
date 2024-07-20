using Autodesk.Revit.DB;

namespace SCADtools.Revit.UI.DB
{
    /// <summary>
    /// Represents a pair of points in 3-dimensional space.
    /// </summary>
    /// <remarks>
    /// This class is used to store a start point and an end point, each represented by an
    /// XYZ coordinate. It can be used to define a segment or a line between two points.
    /// </remarks>
    public class XYZPair
    {
        /// <summary>
        /// Gets the 3D start point.
        /// </summary>
        public XYZ StartPoint { get; }
        /// <summary>
        /// Gets the 3D end point.
        /// </summary>
        public XYZ EndPoint { get; }

        /// <summary>
        /// Creates an XYZPair with the supplied points.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        public XYZPair(XYZ startPoint, XYZ endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
    }
}
