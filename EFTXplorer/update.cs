using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFTXplorer
{
    public partial class update : Form
    {
        string download_folder = "";
        string upd_file_name = "";
        Form1 frm1 = new Form1();

        public update()
        {
            InitializeComponent();
            this.ControlBox = false;
            button1.Enabled = false;
        }

        private void update_Load(object sender, EventArgs e)
        {
            //Form1 frm1 = new Form1();
            label1.Text = "Updating EFTXplorer... (" + frm1.PCBitVersion + "-Bit)";

            Random rd = new Random();
            int rand_num = rd.Next(0, 99999);
            upd_file_name = rand_num + "_eftxplorer_" + frm1.PCBitVersion + ".rar";
            string GetDownloadFolderPath()
            {
                return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
            }
            download_folder = GetDownloadFolderPath();
            //MessageBox.Show(GetDownloadFolderPath());
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(
                    // Param1 = Link of file
                    new System.Uri("https://github.com/annabelsandford/EFTXplorer/releases/latest/download/eftxplorer_"+frm1.PCBitVersion+".rar"),
                    // Param2 = Path to save
                    download_folder + "\\" + upd_file_name
                );
            }
        }
        // Event to track the progress
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            if (progressBar1.Value >= 100)
            {
                label1.Text = "New version downloaded to:" + frm1.nwln + download_folder + "\\" + upd_file_name;
                button1.Text = "Close";
                try
                {
                    Process.Start(download_folder);
                    Process.Start(download_folder + "\\" + upd_file_name);
                }
                catch (Win32Exception a)
                {
                    //The system cannot find the file specified...
                    MessageBox.Show(a.Message);
                }
                button1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
