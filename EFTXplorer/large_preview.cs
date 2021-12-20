using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFTXplorer
{
    public partial class large_preview : Form
    {
        bool contextExit = false;
        string path = null;

        string orig_complete = null;
        string orig_justpath = null;
        string orig_name = null;

        Form1 frm1 = new Form1();
        public large_preview(string qs, string fn)
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(large_preview_FormClosing);

            path = qs;
            orig_complete = fn;
            orig_justpath = Path.GetDirectoryName(fn);
            orig_name = Path.GetFileName(fn);
            this.Text += " (" + orig_name + ")";
        }

        private void large_preview_Load(object sender, EventArgs e)
        {
            if (frm1.is_startup_file == false)
            {
                contextMenuStrip1.Visible = false;
                contextMenuStrip1.Enabled = false;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }

            if (File.Exists(path))
            {
                Random rd = new Random();
                int rand_num = rd.Next(0, 9999);
                string tempFilePath = Path.Combine(Path.GetTempPath(), "tmp.op." + rand_num + ".bmp");
                File.Copy(path, tempFilePath);
                File.Delete(path);
                //MessageBox.Show("" + frm1.is_startup_file.ToString());
                pictureBox1.Image = new Bitmap(tempFilePath);
            }

            toolStripStatusLabel1.Text = orig_complete;
        }

        private void large_preview_FormClosing(object sender, EventArgs e)
        {
            if (frm1.is_startup_file == true)
            {
                if (contextExit == false)
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
            else
            {
                //MessageBox.Show("False: " + frm1.is_startup_file.ToString());
            }
        }

        private void openFull_Click(object sender, EventArgs e)
        {
            contextExit = true;
            frm1.FileExplorerUsed = orig_complete;
            this.Close();
        }
    }
}
