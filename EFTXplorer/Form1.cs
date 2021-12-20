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
// I wanna die please send help lmfao

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
using System.Threading;
using TGASharpLib;

namespace EFTXplorer
{
    public struct eft_loader
    {
        // Replace with eft_loader_x86.dll for 32-bit
        [DllImport("eft_loader_x64.dll")]
        public static extern IntPtr load_eft_file_bgra(string input, out int width, out int height, bool swapWH);
        
        [DllImport("eft_loader_x64.dll")]
        public static extern void free_eft_memory(IntPtr eft_ptr);
    }

    //public struct eft_loader_512
    //{
    //    [DllImport("eft_loader_qf.dll")]
    //    public static extern IntPtr load_eft_file_bgra(string input, out int width, out int height);
    //}

    public partial class Form1 : Form
    {
        static Mutex mutex = new Mutex(true, "{46d34fd4-01c0-4f25-ad3e-447f6abbd8e1}");
        public string EFTXVersion = "1.3 ALPHA";
        public string EFTXCopyright = "Annabel Jocelyn Sandford and Jean-Luc Mackail";
        public string PCBitVersion = "";
        string EFTXYear = "2021";

        public string nwln = Environment.NewLine;
        public string ImportFilePath = "";
        public string LastTempFile = "";
        public string RecentOriginalFileName = "";
        public string FileExplorerUsed = "";
        public bool FileLoaded = false;

        bool image_scrambling = false;
        int scramble_count = 0;

        bool is_512_file = false;
        public bool is_startup_file = false;
        string startup_file = "";

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
            adjust_rotate.Enabled = false; adjust_scramble.Enabled = false; adjust_preview.Enabled = true;
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
                consoleBox.AppendText("> Could not clear cache" + nwln);
                //MessageBox.Show("Something went terribly wrong." + nwln + "Exception: Could not clear img-cache (" + c + ")" + nwln + "Suggestion: Try running EFTX as an administrator.", "EFTX Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //System.Windows.Forms.Application.Exit();
            }
            consoleBox.AppendText("(: Initialized!"); // c:

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                //MessageBox.Show("" + args[1]);
                is_startup_file = true;
                startup_file = args[1].ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("Hello");
            label1.Text = "EFTX Version: " + EFTXVersion + " / Authors: " + EFTXCopyright + " " + EFTXYear;

            this.AllowDrop = true;
            this.DragDrop += Form1_DragDrop;
            this.DragEnter += Form1_DragEnter;

            if (is_startup_file == true)
            {
                try
                {
                    if (Path.GetExtension(startup_file.ToLower()).Equals(".eft"))
                    {
                        FileAttributes attr = File.GetAttributes(startup_file);
                        if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                        {
                            FileExplorerImport(Path.GetDirectoryName(startup_file), Path.GetFileName(startup_file));
                            startup_file = "";
                            PreviewFunction();
                            is_startup_file = false;
                        }
                    }
                }
                catch
                {
                    // do nothing lol
                }
            }
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            //consoleBox.AppendText("DragEnter!");
            e.Effect = DragDropEffects.Copy;
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            // start here

            try
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                    //MessageBox.Show("" + fileList[0]);
                    if (Path.GetExtension(fileList[0].ToLower()).Equals(".eft"))
                    {
                        FileAttributes attr = File.GetAttributes(fileList[0]);
                        if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                        {
                            FileExplorerImport(Path.GetDirectoryName(fileList[0]), Path.GetFileName(fileList[0]));
                            //MessageBox.Show("JustPath: " + Path.GetDirectoryName(fileList[0]) + nwln + "JustName: " + Path.GetFileName(fileList[0]));
                        }
                    }
                }
            }
            catch
            {
                // pretend like nothing happened
            }
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
            int is_512 = Convert.ToInt32(file_length) / 8;
            if (is_512 == 16514)
            {
                is_512_file = true;
            }

            if (file_length > 26214400)
            {
                MessageBox.Show("We've detected you are about to import a file possibly larger than 25 MB. This file may take some time to import, please do not force quit EFTX while importing " + Path.GetFileName(LFEPath), "EFTX Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                IntPtr pointer;

                if (is_512_file == true)
                {
                    pointer = eft_loader.load_eft_file_bgra(LFEPath, out width, out height, false);

                    scramble_count = 0;
                    image_scrambling = false;
                    //is_512_file = false;
                }
                else
                {
                    if (scramble_count >= 1)
                    {
                        scramble_count = 0;
                        image_scrambling = false;
                    }
                    if (image_scrambling == true)
                    {
                        pointer = eft_loader.load_eft_file_bgra(LFEPath, out width, out height, false);
                        image_scrambling = false;
                        scramble_count++;
                    }
                    else
                    {
                        pointer = eft_loader.load_eft_file_bgra(LFEPath, out width, out height, true);
                    }
                }
                
                //IntPtr pointer = eft_loader.load_eft_file_bgra(LFEPath, out width, out height, true);
                byte[] image_buffer = new byte[width * height * 4];
                Marshal.Copy(pointer, image_buffer, 0, width * height * 4);
                eft_loader.free_eft_memory(pointer);

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
            richTextBox1.AppendText("File: " + Path.GetFileName(LFEPath) + nwln + "W: " + width + " H: " + height + nwln + previewEFT.Image.PixelFormat.ToString() + nwln);

            contextMenuStrip2.Items[0].Enabled = true; contextMenuStrip2.Items[1].Enabled = true; exportToolStripMenuItem.Enabled = true;
            adjust_rotate.Enabled = true;
            if (is_512_file == true)
            {
                adjust_scramble.Enabled = false;
                is_512_file = false;
            }
            else
            {
                adjust_scramble.Enabled = true;
            }
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
                    adjust_rotate.Enabled = false; adjust_scramble.Enabled = false;
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
                    adjust_rotate.Enabled = false; adjust_scramble.Enabled = false;
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

        private void RotateImageFunc()
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

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearFunction();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearFunction();

            // Create a new instance of the Form2 class
            about aboutForm = new about();

            // Show the settings form
            aboutForm.ShowDialog();
        }

        private void rotateImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotateImageFunc();
        }

        private void ExportImage(string TempPath)
        {
            try
            {
                Bitmap bmp = new Bitmap(TempPath);

                bool exportingTarga = false;
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "TARGA Image (.tga)|*.tga|Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|PNG Image (.png)|*.png|Tiff Image (.tiff)|*.tiff|WMF Image (.wmf)|*.wmf";
                ImageFormat format = ImageFormat.Png;
                sfd.FileName = System.IO.Path.GetFileNameWithoutExtension(RecentOriginalFileName);
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string ext = System.IO.Path.GetExtension(sfd.FileName);
                    switch (ext)
                    {
                        case ".tga":
                            //format = ImageFormat.Jpeg;
                            exportingTarga = true;
                            break;
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
                    if (exportingTarga == false)
                    {
                        consoleBox.AppendText("Exporting " + format + "... ");
                        bmp.Save(sfd.FileName, format);
                    }
                    else
                    {
                        consoleBox.AppendText(nwln + "Intercept: TGA export det.");
                        var bmp32 = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        using (var gr = Graphics.FromImage(bmp32))
                            gr.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                        Random rand = new Random();
                        string tempFilePath32 = Path.Combine(Path.GetTempPath(), "tmp.op." + rand.Next(100, 99999) + ".bmp");
                        bmp32.Save(tempFilePath32);

                        Bitmap bmp32Temp = new Bitmap(tempFilePath32);
                        consoleBox.AppendText(nwln + "STATUS: " + bmp32Temp.PixelFormat.ToString() + nwln + "Saving TGA...");

                        TGA targa = (TGA)bmp32Temp;
                        targa.Save(sfd.FileName);
                        consoleBox.AppendText(" Done!"+ nwln + "Deleting temp files... ");

                        bmp32.Dispose();
                        bmp32Temp.Dispose();
                        File.Delete(tempFilePath32);
                        GC.Collect();
                    }
                    consoleBox.AppendText("Done." + nwln);
                    MessageBox.Show("Successfully exported " + sfd.FileName, "EFTX Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                bmp.Dispose();

                //MessageBox.Show("Would've exported.");
            }
            catch (Exception e)
            {
                MessageBox.Show("Something went terribly wrong." + nwln + "Exception: Cannot export bitmap (" + e + ")" + nwln + "Suggestion: Try running EFTX as an administrator.", "EFTX Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SaveLogfile();
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
            catch
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

        private void SaveLogfile()
        {
            Random rd = new Random();
            int rand_num = rd.Next(0, 9999);
            string tempFilePath = Path.Combine(Path.GetTempPath(), "eftx_console_" + rand_num + ".txt");
            File.WriteAllText(tempFilePath, consoleBox.Text);
            consoleBox.AppendText(nwln + "Log saved to: " + tempFilePath);
        }

        private void PreviewFunction()
        {
            if (!LastTempFile.Equals(""))
            {
                string ExePath = System.Reflection.Assembly.GetEntryAssembly().Location;
                string JustExePath = Path.GetDirectoryName(ExePath);
                File.Copy(LastTempFile, Path.GetDirectoryName(ExePath) + "\\" + Path.GetFileName(LastTempFile) + ".dup.bmp");
                previewEFT.Image.Dispose();
                previewEFT.Image = null;
                //MessageBox.Show("" + Path.GetDirectoryName(ExePath) + "\\" + Path.GetFileName(LastTempFile) + ".dup.bmp");
                large_preview PreviewForm = new large_preview(Path.GetDirectoryName(ExePath) + "\\" + Path.GetFileName(LastTempFile) + ".dup.bmp", FileExplorerUsed);
                PreviewForm.ShowDialog();
                FileExplorerImport(Path.GetDirectoryName(FileExplorerUsed), Path.GetFileName(FileExplorerUsed));
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

        private void adjust_rotate_Click(object sender, EventArgs e)
        {
            RotateImageFunc();
        }

        private void adjust_scramble_Click(object sender, EventArgs e)
        {
            try
            {
                image_scrambling = true;
                FileExplorerImport(Path.GetDirectoryName(FileExplorerUsed), Path.GetFileName(FileExplorerUsed));
            }
            catch
            {
                // just do absolutely nothing lol
            }
        }

        private void adjust_console_Click(object sender, EventArgs e)
        {
            SaveLogfile();
        }

        private void adjust_preview_Click(object sender, EventArgs e)
        {
            PreviewFunction();
        }
    }
}
