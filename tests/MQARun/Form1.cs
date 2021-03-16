
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MQARun
{
    

    public partial class Form1 : Form
    {
        CustomApplication ca;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ca = new CustomApplication();
            ca.ownerHandle = Handle;
            ca.Run();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ca.SendMessage(1);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == CustomApplication.WM_MESSAGE_DISPATCH)
            {
                richTextBox1.AppendText("Dispatch message " + m.WParam.ToString() + Environment.NewLine);
            }
            base.WndProc(ref m);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ca.SendMessage(1);
            ca.SendMessage(1);
            ca.SendMessage(1);
            ca.SendMessage(1);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ca.Stop();
        }
    }
}
