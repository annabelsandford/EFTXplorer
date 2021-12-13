//            !!!"..?!!.' ......        !!!!!!! 
//           !!! e2$ .<!!!!!!!!!`~!~!!!!!!~! ""!`.` 
//           !!!!^:!!!!!!!!!!!!!!.:!!!!!!!!! *@ !4:'
//          . >! !!!!!!!!!!!!!!!!!:^:!!!!!!!!:  J!: 
//          .!! ,<!!!!!!!!!!!!!...`*."!!!!!!!!!!.~~
//          !!~!!!!!!!!!f !!!! #$$$$$$b`!!!!!L!!!(  
//         !!! ! !!!!! !>b"!!!!. ^$$$*"!!~!4!!!!!!`x 
//        .!!!! !`!! d "= "$N !!f u `!!!~' !!!!!!!!! 
//        !!!!!  !XH.=m" C..^*$!.  .~L:u@ !! !!!!~:` 
//       !!!!!   '`"*:$$P k  $$$$e$R""" mee"<!!!!!  
//      :!!!!"    $N $$$  * x$$$$$$$   <-m.` !!!!!'<! 
//     .!!!!f     "$ $$$.  u$$$$$$$e $ : ee `  !`:!!!`
//     !!!!!.        $$$$$$$$$$$ $$   u$$" r'    !!!!!             ~4
//    !!!!!          "$$$$$$&""%$$$ee$$$ @"      !!!!!h            $b`
//   !!!!!             $$$$     $$$$$$$           !!!!!           @$ 
//  !!!!! X             "&$c   $$$$$"              !!!!!       `e$$
// !!!!! !              $$."***""                   !!!!h     z$$$$$$$$$$$$$$eJ
//!!!!! !!     .....     ^"'$$$            $         !!!!    J$$$$$$$$$$$"
//!!!! !!  .d$$$$$$$$$$e( <  d            4$          ~!!! z$$F$$$$$$$$$$b
//!!! !!  J$$$$$$*****$$$$. "J<=    t'b  `)$b' ,C)`    `!~@$$$$$J'$$$$$$$
//!!~:!   $$$$"e$$$$$$$$c"$N". - ". :F$ ?P"$$$ #$$      .$$$$$$$FL$$$$$$$
//!`:!    $$"$$$$$$$$$$$$$$e $$$.   '>@ z$&$$$eC$"    .d$$$$$$$P      "*$$.
// !!     #$$$$$$$*"zddaaa""e^*F""*. "$ $$P.#$$$$E:: d$$$$$$$$           ^$ 
//!!~      ;$$$$"d$$$$$$$$$$$$$u       $c#d$$@$\$>`x$$$$$$$$"             "c
//!!        ;e?(."$$$$$$$$$$$$$$$$u     "$NJ$$$d"x$$$$$$$$$ 

// Written by Anna (@annie_sandford)

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
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Net;
using System.Text.RegularExpressions;

namespace EFTXplorer
{
    public struct eft_loader
    {
        // Replace with eft_loader_x86.dll for 32-bit
        [DllImport("eft_loader_x64.dll")]
        public static extern IntPtr load_eft_file_bgra(string input, out int width, out int height);
    }

    public partial class Form1 : Form
    {
        public string EFTXVersion = "1.1";
        public string EFTXCopyright = "Jean-Luc Mackail and Annabel Jocelyn Sandford";
        public string PCBitVersion = "";
        string EFTXYear = "2021";

        public string nwln = Environment.NewLine;
        public string ImportFilePath = "";
        public string LastTempFile = "";
        public string RecentOriginalFileName = "";
        string FileExplorerUsed = "";
        public bool FileLoaded = false;

        public Form1()
        {
            InitializeComponent();
            if (Environment.Is64BitOperatingSystem == true)
            {
                PCBitVersion = "64";
            }
            else
            {
                PCBitVersion = "32";
            }
            exportToolStripMenuItem.Enabled = false; // Disable Export Menu Item because there's nothing loaded yet
            contextMenuStrip1.Items[0].Enabled = false; contextMenuStrip1.Items[2].Enabled = false; contextMenuStrip2.Items[0].Enabled = false; contextMenuStrip2.Items[1].Enabled = false;
            this.Text = "EFTX " + EFTXVersion; // Match Window Title w version number

            consoleBox.AppendText("Analyzing Cache... " + nwln);
            try
            {
                foreach (var file in Directory.GetFiles(Path.GetTempPath(), "*.bmp"))
                {
                    File.Delete(file);
                    consoleBox.AppendText("> Deleted " + file + nwln);
                }
            }
            catch (Exception c)
            {
                MessageBox.Show("Something went terribly wrong." + nwln + "Exception: Could not clear img-cache (" + c + ")" + nwln + "Suggestion: Try running EFTX as an administrator.", "EFTX Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
            consoleBox.AppendText("(: Initialized!"); // c:
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("Hello");
            label1.Text = "EFTX Version: " + EFTXVersion + " / Copyright " + EFTXCopyright + " " + EFTXYear;
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LFEImport();
        }

        private void LFEImport()
        {
            OpenFileDialog dialogfile = new OpenFileDialog();
            dialogfile.Filter = "EFT Files (*.eft)|*.eft";
            dialogfile.FilterIndex = 1;
            dialogfile.Multiselect = false;

            if (dialogfile.ShowDialog() == DialogResult.OK)
            {
                string sFileName = dialogfile.FileName;
                string[] arrAllFiles = dialogfile.FileNames;

                //MessageBox.Show("Path: " + dialogfile.FileName + nwln + "Folder: " + LastFolder + nwln + "JustPath: " + JustPath);
                ImportFilePath = dialogfile.FileName;
                if (ImportFilePath.Equals(""))
                {
                    consoleBox.AppendText(nwln + "ERR: Filepath null");
                    MessageBox.Show("Exception: EFT Filepath Empty");
                }
                else
                {
                    consoleBox.AppendText(nwln + "> Filepath valid...");
                    aboutToolStripMenuItem.Enabled = false;
                    importToolStripMenuItem.Enabled = false;

                    LoadFileExplorer(dialogfile.FileName);
                    ParseImage(dialogfile.FileName);
                }
            }
        }

        private void LoadFileExplorer(string LFEPath)
        {
            string LastFolder = new DirectoryInfo(LFEPath).Parent.Name;
            string JustPath = Path.GetDirectoryName(LFEPath);

            aboutToolStripMenuItem.Enabled = false;
            importToolStripMenuItem.Enabled = false;

            richTextBox1.Clear();
            treeView1.Nodes.Clear();
            consoleBox.AppendText(nwln + ">> Clearing Nodes..");
            // Create Parent Tree Node
            TreeNode FileExplorer;
            FileExplorer = treeView1.Nodes.Add(LastFolder);
            FileExplorer.ImageIndex = 1;
            FileExplorer.SelectedImageIndex = 1;

            // Try to populate it lmfao
            string[] fileArray = Directory.GetFiles(JustPath, "*.eft");
            foreach (var item in fileArray)
            {
                FileExplorer.Nodes.Add(item, Path.GetFileName(item));
            }
            treeView1.Nodes.Find(Path.GetFileName(LFEPath), true);
            consoleBox.AppendText(nwln + "> Populating Tree..");
            FileExplorerUsed = LFEPath;
            aboutToolStripMenuItem.Enabled = true;
            importToolStripMenuItem.Enabled = true;
            consoleBox.AppendText(nwln + "> Done!" + nwln);
            contextMenuStrip1.Items[0].Enabled = true; contextMenuStrip1.Items[2].Enabled = true;
        }

        private void ParseImage(string LFEPath)
        {
            long file_length = new System.IO.FileInfo(LFEPath).Length;
            if (file_length > 1008576)
            {
                MessageBox.Show("We've detected you are about to import a file possibly larger than 10mb. This file may take some time to import, please do not force quit EFTX while importing " + Path.GetFileName(LFEPath), "EFTX Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            consoleBox.AppendText("Try Image.Dispose()... ");
            
            try
            {
                if (previewEFT.Image != null)
                {
                    previewEFT.Image.Dispose();
                    previewEFT.Image = null;
                    consoleBox.AppendText(" Success." + nwln);
                }
                else
                {
                    consoleBox.AppendText(" Nothing." + nwln);
                }
            }
            catch
            {
                consoleBox.AppendText(" Catch." + nwln);
            }
            consoleBox.AppendText("Check img-cache." + nwln);
            if (!LastTempFile.Equals(""))
            {
                if (File.Exists(LastTempFile))
                {
                    try
                    {
                        File.Delete(LastTempFile);
                        LastTempFile = "";
                        RecentOriginalFileName = "";
                        consoleBox.AppendText("Cleared img-cache successfully." + nwln);
                    }
                    catch (Exception b)
                    {
                        MessageBox.Show("Something went terribly wrong." + nwln + "Exception: Could not clear img-cache (" + b + ")" + nwln + "Suggestion: Try running EFTX as an administrator.", "EFTX Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        System.Windows.Forms.Application.Exit();
                    }
                }
                else
                {
                    consoleBox.AppendText("img-cache file already gone. Clear." + nwln);
                    LastTempFile = "";
                    RecentOriginalFileName = "";
                }
            }
            else
            {
                consoleBox.AppendText("img-cache empty. Continue." + nwln);
            }
            int width = 0; int height = 0;
            try
            {
                Random rd = new Random();
                int rand_num = rd.Next(0, 9999);
                string tempFilePath = Path.Combine(Path.GetTempPath(), "tmp.op."+rand_num+".bmp");
                consoleBox.AppendText("Converting... ");
                IntPtr pointer = eft_loader.load_eft_file_bgra(LFEPath, out width, out height);
                byte[] image_buffer = new byte[width * height * 4];
                Marshal.Copy(pointer, image_buffer, 0, width * height * 4);
                //MessageBox.Show("W: " + width + " H: " + height);
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppRgb);
                BitmapData bmpData = bmp.LockBits(
                                       new Rectangle(0, 0, bmp.Width, bmp.Height),
                                       ImageLockMode.WriteOnly, bmp.PixelFormat);
                Marshal.Copy(image_buffer, 0, bmpData.Scan0, width * height * 4);
                //bmpData.Scan0 = pointer;
                bmp.UnlockBits(bmpData);
                bmp.Save(tempFilePath); // Black Magic happening right here
                LastTempFile = tempFilePath;
                consoleBox.AppendText("Done." + nwln);
                previewEFT.Image = new Bitmap(tempFilePath);

                //image_buffer = null;
                bmp.Dispose();
                GC.Collect();
            }
            catch (Exception e)
            {
                MessageBox.Show("Something went terribly wrong." + nwln + "Exception: " + e, "EFTX Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }

            string LastFolder = new DirectoryInfo(LFEPath).Parent.Name;
            string JustPath = Path.GetDirectoryName(LFEPath);
            RecentOriginalFileName = Path.GetFileName(LFEPath);

            richTextBox1.Clear();
            richTextBox1.AppendText("File: " + Path.GetFileName(LFEPath) + nwln + "W: " + width + " H: " + height + nwln + "RGBA EFT/VALID" + nwln);

            contextMenuStrip2.Items[0].Enabled = true; contextMenuStrip2.Items[1].Enabled = true; exportToolStripMenuItem.Enabled = true;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!FileExplorerUsed.Equals(""))
            {
                try
                {
                    consoleBox.AppendText(nwln + "Try Image.Dispose()... ");
                    try
                    {
                        previewEFT.Image.Dispose();
                        previewEFT.Image = null;
                        consoleBox.AppendText(" Success." + nwln);
                    }
                    catch
                    {
                        consoleBox.AppendText(" Catch." + nwln);
                    }
                    contextMenuStrip2.Items[0].Enabled = false; contextMenuStrip2.Items[1].Enabled = false; exportToolStripMenuItem.Enabled = false;
                    consoleBox.AppendText(nwln + FileExplorerUsed);
                    LoadFileExplorer(FileExplorerUsed);
                    ParseImage(FileExplorerUsed);
                }
                catch (Exception a)
                {
                    MessageBox.Show("Something went wrong while trying to reload the file tree (" + a + ")");
                }
                //MessageBox.Show("Reload " + FileExplorerUsed);
            }
            else
            {
                MessageBox.Show("No folder to reload. Abort.");
            }
        }

        private void importNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LFEImport();
        }

        private void ClearFunction()
        {
            if (!FileExplorerUsed.Equals(""))
            {
                try
                {
                    contextMenuStrip2.Items[0].Enabled = false; contextMenuStrip2.Items[1].Enabled = false; exportToolStripMenuItem.Enabled = false;
                    previewEFT.Image.Dispose();
                    previewEFT.Image = null;
                    richTextBox1.Clear();
                    treeView1.Nodes.Clear();
                    consoleBox.AppendText(nwln + "Cleared All Nodes." + nwln);

                    consoleBox.AppendText("Check img-cache." + nwln);
                    if (!LastTempFile.Equals(""))
                    {
                        if (File.Exists(LastTempFile))
                        {
                            try
                            {
                                File.Delete(LastTempFile);
                                LastTempFile = "";
                                RecentOriginalFileName = "";
                                consoleBox.AppendText("Cleared img-cache successfully." + nwln);
                            }
                            catch (Exception b)
                            {
                                MessageBox.Show("Something went terribly wrong." + nwln + "Exception: Could not img-cache cache (" + b + ")" + nwln + "Suggestion: Try running EFTX as an administrator.", "EFTX Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                System.Windows.Forms.Application.Exit();
                            }
                        }
                        else
                        {
                            consoleBox.AppendText("img-cache file already gone. Clear." + nwln);
                            LastTempFile = "";
                            RecentOriginalFileName = "";
                        }
                    }
                    else
                    {
                        consoleBox.AppendText("img-cache already empty. Continue." + nwln);
                    }

                    contextMenuStrip1.Items[0].Enabled = false; contextMenuStrip1.Items[2].Enabled = false;
                    FileExplorerUsed = "";

                    consoleBox.AppendText("Force GC.Collect()... ");
                    GC.Collect();
                    consoleBox.AppendText("Done!" + nwln + "All clear! (:" + nwln);
                }
                catch (Exception a)
                {
                    MessageBox.Show("Something went wrong while trying to clear the file tree (" + a + ")");
                }
                //MessageBox.Show("Reload " + FileExplorerUsed);
            }
            else
            {
                //MessageBox.Show("No folder to reload. Abort.");
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearFunction();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a new instance of the Form2 class
            about aboutForm = new about();

            // Show the settings form
            aboutForm.Show();
        }

        private void rotateImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            consoleBox.AppendText(nwln + "Rotating.");
            
            if (previewEFT.Image != null)
            {
                try
                {
                    previewEFT.Image.Dispose();
                    previewEFT.Image = null;
                    consoleBox.AppendText(".. ");
                    Bitmap bmp = new Bitmap(LastTempFile);
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    bmp.Save(LastTempFile);

                    previewEFT.Image = new Bitmap(LastTempFile);
                    GC.Collect();
                    consoleBox.AppendText("Done!");
                }
                catch (Exception f)
                {
                    MessageBox.Show("Something went terribly wrong." + nwln + "Exception: Cannot rotate bitmap (" + f + ")" + nwln + "Suggestion: Try running EFTX as an administrator.", "EFTX Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Windows.Forms.Application.Exit();
                }
            }
            else
            {
                MessageBox.Show("There is no image.");
            }
        }

        private void ExportImage(string TempPath)
        {
            try
            {
                Bitmap bmp = new Bitmap(TempPath);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png|Tiff Image (.tiff)|*.tiff|Wmf Image (.wmf)|*.wmf";
                ImageFormat format = ImageFormat.Png;
                sfd.FileName = System.IO.Path.GetFileNameWithoutExtension(RecentOriginalFileName);
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string ext = System.IO.Path.GetExtension(sfd.FileName);
                    switch (ext)
                    {
                        case ".jpeg":
                            format = ImageFormat.Jpeg;
                            break;
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                        case ".gif":
                            format = ImageFormat.Gif;
                            break;
                        case ".tiff":
                            format = ImageFormat.Tiff;
                            break;
                        case ".wmf":
                            format = ImageFormat.Wmf;
                            break;
                    }
                    consoleBox.AppendText("Exporting " + format + "... ");
                    bmp.Save(sfd.FileName, format);
                    consoleBox.AppendText("Done." + nwln);
                    MessageBox.Show("Successfully exported " + sfd.FileName, "EFTX Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                bmp.Dispose();

                //MessageBox.Show("Would've exported.");
            }
            catch (Exception e)
            {
                MessageBox.Show("Something went terribly wrong." + nwln + "Exception: Cannot export bitmap (" + e + ")" + nwln + "Suggestion: Try running EFTX as an administrator.", "EFTX Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
        }

        private void exportImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (previewEFT.Image != null)
            {
                ExportImage(LastTempFile);
            }
            else
            {
                MessageBox.Show("There is no image.");
            }
        }



        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (previewEFT.Image != null)
            {
                ExportImage(LastTempFile);
            }
            else
            {
                MessageBox.Show("There is no image.");
            }
        }

        private void searchUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearFunction();
            bool internet_connection = false;
            bool update_found = false;
            try
            {
                Ping myPing = new Ping();
                String host = "github.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                internet_connection = true;
            }
            catch (Exception a)
            {
                MessageBox.Show("You either are not connected to the internet or the GitHub servers are down. If you have an internet connection and this doesn't work, please try again later.", "EFTX Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (internet_connection == true)
            {
                try
                {
                    Random rd = new Random();
                    int rand_num = rd.Next(0, 99999);
                    string latest_version = string.Empty;
                    WebRequest request = WebRequest.Create("https://raw.githubusercontent.com/annabelsandford/EFTXplorer/main/updates/latest.txt?dummy=" + rand_num);
                    WebResponse response = request.GetResponse();
                    Stream data = response.GetResponseStream();
                    
                    using (StreamReader sr = new StreamReader(data))
                    {
                        latest_version = sr.ReadToEnd();
                    }
                    if (!EFTXVersion.ToString().Equals(latest_version))
                    {
                        // this here isnt the latest version
                        update_found = true;
                        DialogResult result = MessageBox.Show("We've found an EFTXplorer Update!" + nwln + "This version: " + EFTXVersion.ToString() + "/ New version: " + latest_version + nwln + "Do you want to update?", "EFTX Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (result == DialogResult.Yes)
                        {
                            // Create a new instance of the Form2 class
                            update updateForm = new update();
                            //ClearFunction();
                            // Show the settings form
                            //updateForm.Show();
                            updateForm.ShowDialog();
                        }
                    }
                    else
                    {
                        // current version (no update)
                        MessageBox.Show("No update found. (This: " + EFTXVersion + "/ Serv: " + latest_version + ")", "EFTX Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception b)
                {
                    MessageBox.Show("Something went terribly wrong." + nwln + "Exception: Couldn't WebRequest Latest Version (" + b + ")", "EFTX Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FileExplorerImport(string Path, string Filename)
        {
            //string sFileName = dialogfile.FileName;
            //string[] arrAllFiles = dialogfile.FileNames;
            //MessageBox.Show("Path: " + dialogfile.FileName + nwln + "Folder: " + LastFolder + nwln + "JustPath: " + JustPath);
            ImportFilePath = Path + "\\" + Filename;
            if (ImportFilePath.Equals(""))
            {
                consoleBox.AppendText(nwln + "ERR: Filepath null");
                MessageBox.Show("Exception: EFT Filepath Empty");
            }
            else
            {
                consoleBox.AppendText(nwln + "> Filepath valid...");
                aboutToolStripMenuItem.Enabled = false;
                importToolStripMenuItem.Enabled = false;
                LoadFileExplorer(ImportFilePath);
                ParseImage(ImportFilePath);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //MessageBox.Show(e.Node.Text);
        }
        void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string LastFolder = new DirectoryInfo(FileExplorerUsed).Parent.Name;
            string JustPath = Path.GetDirectoryName(FileExplorerUsed);
            if (!e.Node.Text.Equals(LastFolder))
            {
                if (File.Exists(JustPath + "\\" + e.Node.Text))
                {
                    FileExplorerImport(JustPath, e.Node.Text);
                }
                //MessageBox.Show(JustPath);
            }
            else
            {
                MessageBox.Show("Unfortunately you cannot import folders lol");
            }
            
            //MessageBox.Show(e.Node.Text);
        }
    }
}
