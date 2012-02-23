using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AcsServer
{
    public partial class AcsServerMainForm : Form
    {
        AcsServerMain obj;
        public AcsServerMainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Include the code to start here
            obj = new AcsServerMain();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            obj.stop();
        }
    }
}