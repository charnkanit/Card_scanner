using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace CardScannerV0_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                Image<Bgr, Byte> img = new Image<Bgr, byte>(openFileDialog1.FileName);
                imageBox1.Image = img;
                //imageBox2.Image = Thresholdfunction(GrayConvertFunction(img));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void imageBox2_Click(object sender, EventArgs e)
        {

        }
    }
}