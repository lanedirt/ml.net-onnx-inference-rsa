using Lanedirt.MLnet.DataStructures;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Drawing;

namespace Lanedirt.MLnet.Onnx
{
    /// <summary>
    /// Model scorer class which contains the pipeline to convert the input image to what the Onnx model expects,
    /// runs the detection logic on the model and then returns the results.
    /// </summary>
    public class OnnxModelScorer
    {
        private IDataView imageMemoryData;
        private readonly string modelLocation;
        private readonly MLContext mlContext;
        private EstimatorChain<Microsoft.ML.Transforms.Onnx.OnnxTransformer>? _pipeline = null;

        /// <summary>
        /// Constructor method to set up internal variables.
        /// </summary>
        /// <param name="imageMemoryData"></param>
        /// <param name="modelLocation"></param>
        /// <param name="mlContext"></param>
        public OnnxModelScorer(IDataView imageMemoryData, string modelLocation, MLContext mlContext)
        {
            this.imageMemoryData = imageMemoryData;
            this.modelLocation = modelLocation;
            this.mlContext = mlContext;
        }

        /// <summary>
        /// Detect objects in the provided input image using the ONNX model.
        /// </summary>
        /// <param name="bitmapOnnx"></param>
        /// <returns></returns>
        public Dictionary<string, IEnumerable<float[]>> Score(Bitmap bitmapOnnx)
        {
            var model = LoadModel();

            List<MLImageData> MLImages = new();
            MLImages.Add(new MLImageData
            {
                Bitmap = bitmapOnnx
            });
            IDataView data = mlContext.Data.LoadFromEnumerable(MLImages);

            return PredictDataUsingModel(data, model);
        }

        /// <summary>
        /// Set the ML.net pipeline which will transform the input image to the format
        /// that the ONNX model expects.
        /// </summary>
        private void SetPipeline()
        {
            if (_pipeline != null)
            {
                // Already set, skip.
                return;
            }

            // Define the pipeline. This pipeline transforms the input image to what the Onnx model expects.
            // For checking Onnx model input and output parameter names, you can use tools like Netron on
            // the .onnx file. Netron is installed by Visual Studio AI Tools.
            _pipeline = mlContext.Transforms.ResizeImages(
                    outputColumnName: "input_tensor",
                    imageWidth: 320, 
                    imageHeight: 320, 
                    inputColumnName: nameof(MLImageData.Bitmap), 
                    resizing: Microsoft.ML.Transforms.Image.ImageResizingEstimator.ResizingKind.IsoPad, 
                    cropAnchor: Microsoft.ML.Transforms.Image.ImageResizingEstimator.Anchor.Left
                )
                .Append(mlContext.Transforms.ExtractPixels(
                    outputColumnName: "input_tensor", 
                    inputColumnName: null, 
                    colorsToExtract: Microsoft.ML.Transforms.Image.ImagePixelExtractingEstimator.ColorBits.Rgb, 
                    orderOfExtraction: Microsoft.ML.Transforms.Image.ImagePixelExtractingEstimator.ColorsOrder.ARGB, 
                    interleavePixelColors: true, 
                    offsetImage: 1, 
                    scaleImage: 1, 
                    outputAsFloatArray: false)
                )
                .Append(mlContext.Transforms.ApplyOnnxModel(
                    shapeDictionary: new Dictionary<string, int[]>()
                        {
                           { "input_tensor", new[] { 1, 320, 320, 3} }
                        },
                    modelFile: modelLocation,
                    outputColumnNames: new[] 
                    { 
                        "detection_anchor_indices",
                        "detection_boxes",
                        "detection_classes",
                        "detection_scores",
                    },
                    inputColumnNames: new[] 
                    { 
                        "input_tensor" 
                    },
                    gpuDeviceId: null,
                    fallbackToCpu: true
                )
            );
        }

        /// <summary>
        /// Load the model and return the ML transformer.
        /// </summary>
        /// <returns></returns>
        private ITransformer LoadModel()
        {
            SetPipeline();
            var model = _pipeline.Fit(this.imageMemoryData);

            return model;
        }

        /// <summary>
        /// Transform method which does the actual prediction of the objects by using the model
        /// and returns the ONNX model output results.
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private Dictionary<string, IEnumerable<float[]>> PredictDataUsingModel(IDataView testData, ITransformer model)
        {
            IDataView scoredData = model.Transform(testData);

            var returns = new Dictionary<string, IEnumerable<float[]>>();
            returns.Add("detection_boxes", scoredData.GetColumn<float[]>("detection_boxes"));
            returns.Add("detection_classes", scoredData.GetColumn<float[]>("detection_classes"));
            returns.Add("detection_scores", scoredData.GetColumn<float[]>("detection_scores"));

            return returns;
        }


    }
}
