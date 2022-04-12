using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace Card_scanner
{
    public partial class Form1 : Form
    {
        String imgPath = "";
        Image<Bgr, Byte> img1 = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imgPath = openFileDialog1.FileName;
                img1 = new Image<Bgr, Byte>(imgPath);
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
                Image<Gray, Byte> gray = toGray(img1);
                imageBox2.Image = gray;
            }
   
        }

        public static Image<Gray, Byte> toGray(Image<Bgr, Byte> image0)
        {
            Image<Gray, Byte> image1 = image0.Convert<Gray, Byte>();
            return image1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}