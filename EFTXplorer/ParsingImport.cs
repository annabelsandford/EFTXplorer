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
    public partial class ParsingImport : Form
    {
        public ParsingImport()
        {
            InitializeComponent();
        }

        private void ParsingImport_Load(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            label1.Text = form1.ImportFilePath;
        }
    }
}
