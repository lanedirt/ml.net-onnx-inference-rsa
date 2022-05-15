using Microsoft.ML.Transforms.Image;
using System.Drawing;

namespace Lanedirt.MLnet.DataStructures
{
    /// <summary>
    /// Image input for ML model.
    /// </summary>
    public class MLImageData
    {
        [ImageType(320, 320)]
        public Bitmap Bitmap { get; set; }
    }
}
