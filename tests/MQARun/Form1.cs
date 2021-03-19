
using MessageQueueApplication.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MQARun
{
    public partial class Form1 : Form
    {
        CustomApplication ca;
        MQMessageStorage mq;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mq = MQMessageStorage.CreateFromWndHandle(Handle);

            ca = new CustomApplication();
            ca.ownerHandle = Handle;
            ca.OwnerMQ = mq;
            ca.Run();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ca.MQNotify(0);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == CustomApplication.WM_MESSAGE_DISPATCH)
            {
                MQMessage message = mq.ExtractMessage();
                if (message != null)
                {
                    CustomObj obj = (CustomObj)message.ExtractObject();
                    if (obj != null)
                    {
                        richTextBox1.AppendText("Message type: " + message.Code + ". " + obj.text + Environment.NewLine);
                    }
                }
            }
            if (m.Msg == CustomApplication.WM_MESSAGE_TIMER)
            {
                richTextBox1.AppendText("Timer message" + Environment.NewLine);
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
            ca.Terminate();
        }
    }
}
