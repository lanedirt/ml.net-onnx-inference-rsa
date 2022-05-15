using System.Drawing;

namespace Lanedirt.MLnet.DataStructures
{
    /// <summary>
    /// Return format of ML detection logic which contains bounding boxes and an annotated image.
    /// </summary>
    public class MLResult
    {
        /// <summary>
        /// Will contain list of bounding boxes (detected objects) metadata.
        /// </summary>
        public List<BoundingBox> BboxList { get; set; } = new List<BoundingBox>();

        /// <summary>
        /// Will contain the original image with added (drawed on) bounding box annotations (detected objects).
        /// </summary>
        public Bitmap? AnnotatedImage { get; set; }
    }
}
