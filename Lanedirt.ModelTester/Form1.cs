using Lanedirt.MLnet;
using Lanedirt.MLnet.DataStructures;
using System.Text.Json;

namespace Lanedirt.ModelTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// btnLoadImage OnClick handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckPathExists = true;
            ofd.Filter = "JPEG image (*.jpg)|";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Display uploaded image in picture box.
                pictureBoxOriginal.Image = new Bitmap(ofd.FileName);

                // Prepare parameters for model scoring.
                var userBitmap = new Bitmap(ofd.FileName);
                userBitmap.SetResolution(72, 72);

                var rsaLogic = new RsaLogic();
                MLResult rsaReturn = rsaLogic.DetectObjectsInImage(userBitmap);

                // Display boundingbox altered image in picture box.
                pictureBoxBoundingBoxes.Image = rsaReturn.AnnotatedImage;

                // Convert boundingbox list object to JSON and show it in the textbox.
                JsonSerializerOptions opt = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                richTextBoxJson.Text = JsonSerializer.Serialize(rsaReturn.BboxList, opt);
            }
        }
    }
}