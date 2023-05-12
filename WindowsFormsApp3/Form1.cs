using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string TextToSend;

        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());

            foreach(IPAddress address in localIP)
            {
                if(address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ServerIPtextBox.Text = address.ToString();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(ServerPortTextBox.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint IpEnd = new IPEndPoint(IPAddress.Parse(ClientIPtextBox.Text), int.Parse(ClientPortTextBox.Text));

            client.Connect(IpEnd);

            try
            {
                ChatScreenTextBox.AppendText("Connect to server" + "\n");
                STW = new StreamWriter(client.GetStream());
                STR = new StreamReader(client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while(client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.ChatScreenTextBox.Invoke(new MethodInvoker(delegate ()
                    {
                        ChatScreenTextBox.AppendText("You: " + recieve + "\r\n");
                    }));

                    recieve = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if(client.Connected)
            {
                STW.WriteLine(TextToSend);
                this.ChatScreenTextBox.Invoke(new MethodInvoker(delegate ()
                {
                    ChatScreenTextBox.AppendText("Me: " + TextToSend + "\r\n");
                }));
            }
            else
            {
                MessageBox.Show("Sending Failed");
            }

            backgroundWorker2.CancelAsync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(MessageTextBox.Text != "")
            {
                TextToSend = MessageTextBox.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            MessageTextBox.Text = "";
        }
    }
}
