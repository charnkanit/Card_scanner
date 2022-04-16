using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;
using NumSharp;
using System.Diagnostics;

namespace Card_scanner
{
    public partial class Form1 : Form
    {
        String imgPath = "";
        Image<Bgr, Byte> img1 = null;
        Image<Gray, Byte> gray = null;
        Image<Gray, Byte> gaus= null;
        Image<Gray, Byte> cropped = null;
        UMat canny = new UMat();
        UMat dilate = new UMat();
        UMat erode = new UMat();
        Mat hier = new Mat();
        Emgu.CV.Util.VectorOfVectorOfPoint contour = null;
        Emgu.CV.Util.VectorOfPoint biggestContour = null;
        float[,] pts1 = null, pts2 = null;
        int[,] kernel = new int[,] { { 1,1,1,1,1},
                                     { 1,1,1,1,1},
                                     { 1,1,1,1,1},
                                     { 1,1,1,1,1},
                                     { 1,1,1,1,1}};
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _Reset();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imgPath = openFileDialog1.FileName;
                img1 = new Image<Bgr, Byte>(imgPath);
                //img1 = img1.Resize(imageBox1.Width,imageBox1.Height, Inter.Linear);
                imageBox1.Image = img1;
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            if (img1 != null)
            {
                if (btnScan.Text == "Convert to Gray")
                {
                    gray = toGray(img1);
                    imageBox2.Image = gray;
                    btnScan.Text = "Apply Gaussian";
                }
                else if (btnScan.Text == "Apply Gaussian")
                {
         
                    gaus = toGaussian(gray);
                    imageBox2.Image = gaus;
                    btnScan.Text = "Apply Canny";
                    trackBar1.Enabled = true;
                    trackBar2.Enabled = true;
                }
                else if (btnScan.Text == "Apply Canny")
                {
                    canny = toCanny(gaus, trackBar1.Value, trackBar2.Value);
                    imageBox2.Image = canny;
                    btnScan.Text = "Apply Dilation";
                }
                else if (btnScan.Text == "Apply Dilation")
                {
                    dilate = toDilate(canny);
                    imageBox2.Image = dilate;
                    btnScan.Text = "Apply Erosion";
                }
                else if (btnScan.Text == "Apply Erosion")
                {
                    erode = toErode(dilate);
                    imageBox2.Image = erode;
                    btnScan.Text = "Find Contour";
                }
                else if (btnScan.Text == "Find Contour")
                {
                    (contour,hier) = findContour(erode);
                    Image<Bgr, Byte> cpImg = img1.Copy();
                    CvInvoke.DrawContours(cpImg, contour, -1, new MCvScalar(255, 0, 0), 10);
                    imageBox2.Image = cpImg;
                    btnScan.Text = "Find Biggest Contour";
                }
                else if (btnScan.Text == "Find Biggest Contour")
                {
                    biggestContour = sortContour(contour, img1);
                    btnScan.Text = "Perspective crop";
                }
                else if (btnScan.Text == "Perspective crop")
                {
                    (pts1, pts2, cropped) = setPers(biggestContour, gray);
                    imageBox2.Image = cropped;
                    trackBar1.Enabled = false;
                    trackBar2.Enabled = false;
                    btnScan.Text = "Adaptive Threshold";
                }
                else if (btnScan.Text == "Adaptive Threshold")
                {
                    btnScan.Text = "Scan Card Info";
                }
                else if (btnScan.Text == "Scan Card Info")
                {
                    btnScan.Enabled = false;
                    btnSave.Enabled = true;
                }
            }

        }

        public static Image<Gray, Byte> toGray(Image<Bgr, Byte> image0)
        {
            Image<Gray, Byte> image1 = image0.Convert<Gray, Byte>();
            return image1;
        }

        public static Image<Gray, Byte> toGaussian(Image<Gray, Byte> image0)
        {
            CvInvoke.GaussianBlur(image0, image0, new Size(5, 5), 1);
            return image0;
        }

        public static UMat toCanny(Image<Gray, Byte> image0, int lower, int upper)
        {
            UMat result = new UMat();
            CvInvoke.Canny(image0, result, lower, upper);
            return result;
        }

        public static UMat toDilate(UMat image0)
        {
            UMat result = new UMat();
            Mat element = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Dilate(image0, result, element, new Point(-1, -1), 2, 0, new MCvScalar(255, 255, 255));
            return result;
        }

        public static UMat toErode(UMat image0)
        {
            UMat result = new UMat();
            Mat element = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Erode(image0, result, element, new Point(-1, -1), 1, 0, new MCvScalar(255, 255, 255));
            return result;
        }

        public static (Emgu.CV.Util.VectorOfVectorOfPoint, Mat) findContour(UMat image0)
        {
            Emgu.CV.Util.VectorOfVectorOfPoint contour = new Emgu.CV.Util.VectorOfVectorOfPoint();
            Mat hier = new Mat();
            CvInvoke.FindContours(image0, contour, hier, Emgu.CV.CvEnum.RetrType.Tree, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            return (contour,hier);
        }
        
        public Emgu.CV.Util.VectorOfPoint sortContour(Emgu.CV.Util.VectorOfVectorOfPoint contour, Image<Bgr, Byte> image0)
        {
            Dictionary<int, double> dict  = new Dictionary<int, double>();
            Dictionary<int, double> biggest = new Dictionary<int, double>();
            if (contour.Size > 0)
            {
                for (int i = 0; i < contour.Size; i++)
                {
                    double area = CvInvoke.ContourArea(contour[i]);
                    dict.Add(i, area);
                }
            }
            var item = dict.OrderByDescending(v => v.Value).Take(3);
            int key = 0;
            foreach (var it in item)
            {
                key = int.Parse(it.Key.ToString());
            }
            CvInvoke.DrawContours(image0, contour, key, new MCvScalar(0, 255, 0), 10);
            imageBox2.Image = image0;
            return contour[key];
        }

        public (float[,], float[,], Image<Gray, Byte>) setPers(Emgu.CV.Util.VectorOfPoint contour, Image<Gray, Byte> image0)
        {
            float width = image0.Width;
            float height = image0.Height;
            var cvt = contour.ToArray();
            var p1 = cvt[2];
            var p2 = cvt[3];
            var p3 = cvt[0];
            var p4 = cvt[1];
            float[,] scrp = { { p1.X, p1.Y }, { p2.X, p2.Y }, { p3.X, p3.Y }, { p4.X, p4.Y } };
            float[,] dstp = { { 0, 0 }, { width, 0 }, { 0, height }, { width, height } };
            Matrix<float> c1 = new Matrix<float>(scrp);
            Matrix<float> c2 = new Matrix<float>(dstp);
            Mat matr = CvInvoke.GetPerspectiveTransform(c1, c2);
            Image<Gray, Byte> res = image0.Copy();
            CvInvoke.WarpPerspective(image0, res, matr, new Size(image0.Width, image0.Height));
            label9.Text = res.Width.ToString();
            label10.Text = res.Height.ToString();
            return (scrp, dstp, res);
        }

        public void _Reset()
        {
            img1 = null;
            gray = null;
            gaus = null;
            canny = null;
            imageBox2.Image = null;
            btnScan.Text = "Convert to Gray";
            trackBar1.Value = 100;
            trackBar2.Value = 255;
            trackBar1.Enabled = false;
            trackBar2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            canny = toCanny(gaus, trackBar1.Value, trackBar2.Value);
            imageBox2.Image = canny;
            btnScan.Text = "Apply Dilation";
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            canny = toCanny(gaus, trackBar1.Value, trackBar2.Value);
            imageBox2.Image = canny;
            btnScan.Text = "Apply Dilation";
        }
    }
}