using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace vegas2movieStudio
{
    public partial class main : Form
    {
        string txtIdle = "Status: Idle";
        string txtDone = "Status: Done";
        string txtDrop = "Drop files here";
        public main()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragDrop += new DragEventHandler(this.main_DragDrop);
            this.DragEnter += new DragEventHandler(this.main_DragEnter);
            //
            lbDropFiles.Font = new Font(lbDropFiles.Font.FontFamily, 32);
            lbStatus.Font = new Font(lbDropFiles.Font.FontFamily, 22);
            lbDropFiles.ForeColor = Color.Red;
            lbStatus.ForeColor = Color.Blue;

            lbDropFiles.Text = txtDrop;
            lbStatus.Text = txtIdle;
            //
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "SonyVegas 2 MovieStudio - sornram9254.com";
        }

        private void main_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private async void main_DragDrop(object sender, DragEventArgs e)
        {
            var vegFile = "";
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file in files)
            {
                vegFile = file;
            }
            try
            {
                veg2vf(vegFile);
                await Task.Delay(2000);
                lbStatus.Text = txtIdle;
            }
            catch
            {
                lbDropFiles.Text = "Error!";
            }
        }
        private void veg2vf(string dropFiles)
        {
            // bit : 24-39, 70
            int[] position = { 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 70 };
            byte[] hexValue = { 0xF6, 0x1B, 0x3C, 0x53, 0x35, 0xD6, 0xF3, 0x43, 0x8A, 0x90, 0x64, 0xB8, 0x87, 0x23, 0x1F, 0x7F, 0x0C }; //11=0x0B, 12=0x0C, 13=0x0D

            File.Copy(dropFiles, dropFiles + ".vf", true);
            // ref: http://stackoverflow.com/a/1955780/3703855
            var posHex = position.Zip(hexValue, (n, w) => new { POS = n, HEX = w });
            using (var stream = new FileStream(dropFiles + ".vf", FileMode.Open, FileAccess.Write))
            {
                foreach (var hexReplace in posHex)
                {
                    // ref: http://stackoverflow.com/a/3217953/3703855
                    stream.Position = hexReplace.POS;
                    stream.WriteByte(hexReplace.HEX);
                }
            }
            lbStatus.Text = txtDone;
        }
    }
}
