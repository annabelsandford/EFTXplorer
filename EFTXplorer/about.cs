using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFTXplorer
{
    public partial class about : Form
    {
        public about()
        {
            InitializeComponent();
        }

        private void about_Load(object sender, EventArgs e)
        {
            Form1 frm1 = new Form1();
            label1.Text = "EFTXplorer Version: " + frm1.EFTXVersion;
            label3.Text = frm1.EFTXCopyright;
            textBox1.Text = "EFTX is licensed through AGPL 3.0" + frm1.nwln + "You can distribute modified versions of EFTX if you keep track of the changes, the date you made them and distribute source code along with modified publications. You must include EFTX's as well as LibEFTs's original authors as the first copyright holders in any deriviates published. Please refrain from republishing unmodified code or executables.";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //GitHub Page
            System.Diagnostics.Process.Start("https://github.com/annabelsandford/EFTXplorer");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //My Twitter
            System.Diagnostics.Process.Start("https://twitter.com/annie_sandford");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //JJ's Twitter
            System.Diagnostics.Process.Start("https://twitter.com/FuzzyQuills");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //License Link
            System.Diagnostics.Process.Start("https://raw.githubusercontent.com/annabelsandford/EFTXplorer/main/agpl-3.0.txt");
        }
    }
}
