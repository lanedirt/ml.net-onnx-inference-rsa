using Lanedirt.MLnet.DataStructures;
using Lanedirt.MLnet.Onnx;
using Microsoft.ML;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Lanedirt.MLnet
{
    /// <summary>
    /// Wrapper class to do ML.NET image object detection and localization.
    /// </summary>
    public class RsaLogic
    {
        /// <summary>
        /// Relative path to ONNX model location.
        /// </summary>
        readonly string _modelFilePath = @"Model\rsa.onnx";
        readonly MLContext _mlContext;
        readonly OnnxModelScorer _scorer;

        /// <summary>
        /// Constructor, create mlContext and scorer object for reuse.
        /// </summary>
        public RsaLogic()
        {
            // Create a MLContext object.
            _mlContext = new MLContext();

            // Creates a new model scorer object. This is an expensive operation.
            // This scorer will therefore be reused by this object to improve performance
            // when processing more than 1 image in a row.
            List<MLImageData> MLImages = new List<MLImageData>();
            IDataView imageDataView = _mlContext.Data.LoadFromEnumerable(MLImages);
            _scorer = new OnnxModelScorer(imageDataView, _modelFilePath, _mlContext);
        }

        /// <summary>
        /// Returns class labels Dictionary with translation between ID and label that this model outputs.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> GetClassLabels()
        {
            Dictionary<int, string> classLabels = new Dictionary<int, string>();
            classLabels.Add(1, "rsa");
            classLabels.Add(2, "lcd");
            classLabels.Add(3, "lbl1");
            classLabels.Add(4, "lbl2");
            classLabels.Add(5, "rsaback");

            return classLabels;
        }

        /// Detect objects in provided image.
        /// </summary>
        /// <param name="inputImage">Bitmap of image to detect objects in.</param>
        /// <returns>RsaLogicReturn object with detected objects metadata and annotated image to visualize the detections.</returns>
        public MLResult DetectObjectsInImage(Bitmap inputImage)
        {
            List<BoundingBox> bbList = new List<BoundingBox>();
            Bitmap annotatedImage = inputImage;
            Dictionary<int, string> classLabels = GetClassLabels();

            // Our trained model expects the input images to be 320x320.
            // However here we resize the image ourselves because the Onnx transform does not
            // retain aspect ratio. We resize here to 321x321 (with aspect ratio),
            // and then let the Onnx pipeline resize the image to the final 320x320..
            // If we remove the Onnx image transform the ML.net logic throws an error.
            // @TODO: see if we can remove the Onnx model imageresize in the future to improve performance.
            int imageInputHeight = 321;
            int imageInputWidth = 321;
            Bitmap inputImageResized = ResizeImageWithAspectRatio(inputImage, imageInputWidth, imageInputHeight);
            int inputImageResizedWidth = inputImageResized.Width;
            int inputImageResizedHeight = inputImageResized.Height;

            // Now place the resized picture (with aspect ratio) in top left of black canvas 321x321
            // The output we will get will be a 1:1 ratio image, with any "empty" pixels black.
            Bitmap bitmapOnnx = PlaceImageOnExactSizeCanvas(inputImageResized, imageInputWidth, imageInputHeight);

            // Use model to score data (this does the actual detection of objects in the image)
            Dictionary<string, IEnumerable<float[]>> probabilities = _scorer.Score(bitmapOnnx);

            // Get the return values from the scoring.
            var detectionBoxes = probabilities.Where(x => x.Key == "detection_boxes").First().Value.ToList();
            var detectionClasses = probabilities.Where(x => x.Key == "detection_classes").First().Value.ToList();
            var detectionScores = probabilities.Where(x => x.Key == "detection_scores").First().Value.ToList();

            // The detectionBoxes list is in a Python numpy representation.
            // There are 400 rows, while the actual list should be 100 rows with 4 values each..
            // Here we convert these 400 records back into 100 records by taking the values chronologically
            // so with an array like [1,2,3,4,5,6,7,8] it will be converted to [1,2,3,4],[5,6,7,8] etc.
            int i = 0;
            int rowSizeNumpy = 4;
            var newDetectionBoxes = new List<List<float>>();
            var newDetectionBox = new List<float>();
            foreach (var entry in detectionBoxes[0])
            {
                if (i > 0 && i % rowSizeNumpy == 0)
                {
                    newDetectionBoxes.Add(newDetectionBox);
                    newDetectionBox = new List<float>();
                }
                newDetectionBox.Add(entry);
                i++;
            }
            if (newDetectionBox.Count > 0)
            {
                newDetectionBoxes.Add(newDetectionBox);
            }

            for (int imageCount = 0; imageCount < detectionBoxes.Count; imageCount++)
            {
                // Define the minimum confidence score of objects to draw a boundingbox for..
                // This defaults to 0.5 which means the model must be at least 50%
                // confident what is detected is correct.
                float minScoreThreshold = 0.5F;

                int index = -1;
                foreach (var score in detectionScores[imageCount])
                {
                    index++;
                    if (score >= minScoreThreshold)
                    {
                        var bbox = newDetectionBoxes[index];
                        var detectedObjectClassId = detectionClasses[imageCount][index];

                        // Figure out the ratio between the original inputImage and the resized
                        // image, as the object coordinates are based on the resized image
                        // and we want the coordinates based on the original image.
                        double ratioX = (double)annotatedImage.Width / inputImageResizedWidth;
                        double ratioY = (double)annotatedImage.Height / inputImageResizedHeight;

                        // Add bounding boxes to list.
                        var bb = new BoundingBox
                        {
                            Id = index,
                            Label = classLabels.GetValueOrDefault((int)detectedObjectClassId) ?? "unknown",
                            XStart = (int)((float)bbox[1] * 320 * ratioX),
                            XEnd = (int)((float)bbox[3] * 320 * ratioX),
                            YStart = (int)((float)bbox[0] * 320 * ratioY),
                            YEnd = (int)((float)bbox[2] * 320 * ratioY),
                            Score = score
                        };
                        bbList.Add(bb);

                        // Draw the bounding box on the annotated image which will be returned to the caller.
                        DrawBoundingBox(bb, annotatedImage);
                    }
                }
            }

            // Return bounding box list and annotated image to caller.
            var ret = new MLResult()
            {
                BboxList = bbList,
                AnnotatedImage = annotatedImage,
            };
            return ret;
        }

        /// <summary>
        /// Draw a boundingbox on an image.
        /// </summary>
        private void DrawBoundingBox(BoundingBox bbox, Bitmap annotatedImage) {
            using (Graphics thumbnailGraphic = Graphics.FromImage(annotatedImage))
            {
                thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Draw label string on image
                Font drawFont = new Font("Arial", 36, FontStyle.Bold);
                SizeF size = thumbnailGraphic.MeasureString(bbox.Label ?? "unknown", drawFont);
                SolidBrush fontBrush = new SolidBrush(Color.Red);
                Point atPoint = new Point(bbox.XStart, bbox.YStart - (int)size.Height - 1);
                thumbnailGraphic.DrawString(bbox.Label ?? "unknown", drawFont, fontBrush, atPoint);

                // Draw bounding box on image
                Pen pen = new Pen(Color.GreenYellow, 6f);
                thumbnailGraphic.DrawRectangle(pen, bbox.XStart, bbox.YStart, bbox.XEnd - bbox.XStart, bbox.YEnd - bbox.YStart);
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height, keeping aspect ratio intact.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImageWithAspectRatio(Image image, int width, int height)
        {
            // Figure out the ratio
            double ratioX = (double)width / image.Width;
            double ratioY = (double)height / image.Height;
            // Use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // Now calculate the new height and width
            int newHeight = Convert.ToInt32(image.Height * ratio);
            int newWidth = Convert.ToInt32(image.Width * ratio);

            var destRect = new Rectangle(0, 0, newWidth, newHeight);
            var destImage = new Bitmap(newWidth, newHeight);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Fill the image to the exact specified size. The input image will be placed in the top left
        /// corner of the canvas, all of the "empty" pixels will be set to black.
        /// </summary>
        /// <param name="inputImageResized"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static Bitmap PlaceImageOnExactSizeCanvas(Bitmap inputImageResized, int width, int height)
        {
            Bitmap bitmapOnnx = new Bitmap(width, height);
            bitmapOnnx.SetResolution(72, 72);
            using (Graphics graph = Graphics.FromImage(bitmapOnnx))
            {
                Rectangle ImageSize = new Rectangle(0, 0, width, height);
                graph.FillRectangle(Brushes.Black, ImageSize);
                graph.DrawImageUnscaled(inputImageResized, 0, 0);
            }

            return bitmapOnnx;
        }
    }
}
