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

        private void button2_Click(object sender, EventArgs e)
        {
            obj.stop();
        }

        private void AcsServerMainForm_Load(object sender, EventArgs e)
        {
            obj = new AcsServerMain();
        }

        private void AcsServerMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                this.WindowState = FormWindowState.Minimized;

                //MessageBox.Show("Application Cannot be Closed by User");
                e.Cancel = true;

            }
            
        }



    }
}