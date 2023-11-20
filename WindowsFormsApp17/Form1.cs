using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Svg;


namespace WindowsFormsApp17
{
    public partial class Form1 : Form
    {
        private const string ImagesFolder = "images";
        private const string PdfFileName = "producttt.pdf";

        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                string imagesFolderPath = Path.Combine(Application.StartupPath, "images");
                string imagePath = Path.Combine(imagesFolderPath, "gold2Button.png");
                string carImagePath = @"C:\full\path\to\your\project\images\Air Conditioning.svg";


                foreach (Control control in panel1.Controls)
                {
                    if (control is FlowLayoutPanel flowLayoutPanel)
                    {
                        foreach (Control innerControl in flowLayoutPanel.Controls)
                        {
                            if (innerControl is PictureBox pictureBox)
                            {
                                pictureBox.Image = System.Drawing.Image.FromFile(carImagePath);
                                int width = pictureBox.Width;
                                int height = pictureBox.Height;
                                int x = (width - pictureBox.Image.Width) / 2;
                                int y = (height - pictureBox.Image.Height) / 2;
                                pictureBox.BackgroundImage = System.Drawing.Image.FromFile(imagePath);
                                pictureBox.Location = new Point(x, y);
                                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                            }
                        }
                    }
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Save panel as SVG
            string svgPath = Path.Combine(ImagesFolder, "panelImage.svg");
            GenerateAndSaveSVG(panel1, svgPath);

            // Convert SVG to PDF
            string pdfPath = Path.Combine(Application.StartupPath, PdfFileName);
            ConvertSvgToPdf(svgPath, pdfPath);

            MessageBox.Show("PDF file saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GenerateAndSaveSVG(Panel panel, string outputPath)
        {
            try
            {
                int desiredWidth = panel.Width;
                int desiredHeight = panel.Height;

                using (Bitmap bitmap = new Bitmap(desiredWidth, desiredHeight))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    panel.DrawToBitmap(bitmap, new Rectangle(0, 0, desiredWidth, desiredHeight));

                    using (MemoryStream stream = new MemoryStream())
                    {
                        System.Drawing.Imaging.EncoderParameters encoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                        encoderParameters.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                        ImageCodecInfo encoderInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(enc => enc.FormatID == ImageFormat.Png.Guid);
                        bitmap.Save(stream, encoderInfo, encoderParameters);
                        byte[] imageBytes = stream.ToArray();
                        string base64 = Convert.ToBase64String(imageBytes);
                        StringBuilder svgBuilder = new StringBuilder();
                        svgBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
                        svgBuilder.AppendLine($"<svg width=\"{desiredWidth}\" height=\"{desiredHeight}\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");
                        svgBuilder.AppendLine($"<image width=\"{desiredWidth}\" height=\"{desiredHeight}\" xlink:href=\"data:image/png;base64,{base64}\" />");
                        svgBuilder.AppendLine("</svg>");
                        File.WriteAllText(outputPath, svgBuilder.ToString(), Encoding.UTF8);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Switch+IconConfigurator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConvertSvgToPdf(string svgPath, string pdfPath)
        {
            try
            {
                using (var writer = new PdfWriter(pdfPath))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);
                        var pdfImage = new iText.Layout.Element.Image(ImageDataFactory.Create(svgPath));
                        document.Add(pdfImage);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Switch+IconConfigurator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}





