using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordGameUdp
{
    public partial class Receive : Form
    {
        public Receive()
        {
            InitializeComponent();
        }
        UdpClient udpServer;
        IPEndPoint remoteEP;
        HashSet<string> words = new HashSet<string>();
        bool sendstatus = false;
        bool startstatus = false;
        string winstatus;
        int time = 60;


        void connectserver()
        {
            udpServer = new UdpClient(11000);
            remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
            while (true)
            {
                Read();
            }

        }
        void Read()
        {
            Byte[] receiveBytes = udpServer.Receive(ref remoteEP);
            string data = Encoding.ASCII.GetString(receiveBytes);
            if (data.Contains("ClientUserName:"))
            {
                if (lblotheruser.InvokeRequired)
                {
                    lblotheruser.Invoke(new MethodInvoker(delegate { lblotheruser.Text = data.Substring(15); }));
                }
                else
                {
                    lblotheruser.Text = data.Substring(15);

                }
                if (lblserverstatus.InvokeRequired)
                {
                    lblserverstatus.Invoke(new MethodInvoker(delegate { lblserverstatus.Text = "Connection established"; }));
                }
                else
                {
                    lblserverstatus.Text = "Connection established";

                }
                if (sendstatus == true)
                {
                    if (label4.InvokeRequired)
                    {
                        label4.Invoke(new MethodInvoker(delegate { label4.Text = lblotheruser.Text + "'s Turn"; }));
                    }
                    else
                    {
                        label4.Text = lblotheruser.Text + "'s Turn";

                    }
                }
            }
            else if (data.Contains("Host is waiting"))
            {
                if (lblserverstatus.InvokeRequired)
                {
                    lblserverstatus.Invoke(new MethodInvoker(delegate { lblserverstatus.Text = "Connection established"; }));
                }
                else
                {
                    lblserverstatus.Text = "Connection established";

                }
            }
            else if (data.Contains("Client is waiting"))
            {
                if (lblserverstatus.InvokeRequired)
                {
                    lblserverstatus.Invoke(new MethodInvoker(delegate { lblserverstatus.Text = "Connection is waiting..."; }));
                }
                else
                {
                    lblserverstatus.Text = "Connection is waiting...";

                }
            }
            else if (data.Contains("Time"))
            {
                time = 60;

            }
            else if (data.Contains("You Win"))
            {
                winstatus = "You Win";
                MessageBox.Show("You Win");
            }
            else if (data.Contains("HostUserName:"))
            {
                if (lblserverstatus.InvokeRequired)
                {
                    lblserverstatus.Invoke(new MethodInvoker(delegate { lblserverstatus.Text = "Connection is Waiting"; }));
                }
                else
                {
                    lblserverstatus.Text = "Connection is Waiting";
                }
             
            }
            else
            {
                if (lbloword.InvokeRequired)
                {
                    lbloword.Invoke(new MethodInvoker(delegate { lbloword.Text = data.ToUpper(); }));
                }
                else
                {
                    lbloword.Text = data.ToUpper();
                }

                words.Add(lbloword.Text.ToLower());
            }



        }
        public void Send(bool user, bool timestat, string txt)
        {
            if (user == false && timestat == false && winstatus == "You Lose")
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes("Win Status:" + "You Win");
                udpServer.Send(sendBytes, sendBytes.Length, remoteEP);
                MessageBox.Show("You Lose" + "-> " + lblotheruser.Text + " Win");
                winstatus = "";
                button1.Enabled = true;
            }

            if (timestat == true)
            {

                if (time == 0)
                {
                    Byte[] sendBytes = Encoding.ASCII.GetBytes("Time:" + time.ToString());
                    udpServer.Send(sendBytes, sendBytes.Length, remoteEP);
                    time = 60;
                    timer3.Stop();
                    button1.Enabled = true;

                }
                else
                {
                    Byte[] sendBytes = Encoding.ASCII.GetBytes("Time:" + time.ToString());
                    udpServer.Send(sendBytes, sendBytes.Length, remoteEP);
                    time--;
                }

            }



            if (user == false && timestat == false)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes(txt);
                udpServer.Send(sendBytes, sendBytes.Length, remoteEP);
                if (lblyword.InvokeRequired)
                {
                    lblyword.Invoke(new MethodInvoker(delegate { lblyword.Text = textBox1.Text.ToUpper(); }));
                }
                else
                {
                    lblyword.Text = textBox1.Text.ToUpper();
                }

            }

            else if (user == true)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes("HostUserName:" + lblname.Text);
                udpServer.Send(sendBytes, sendBytes.Length, remoteEP);
            }

        }
        private void Receive_Load(object sender, EventArgs e)
        {
            startstatus = false;
            var taskListener = Task.Factory.StartNew(() =>
                                      connectserver());
          
            lblname.Text = Properties.Settings.Default.UserName;
            toolStripTextBox1.Text = Properties.Settings.Default.UserName;
            timer2.Start();
            timer1.Start();
            timer4.Start();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Start();
            startstatus = true;
            var taskListener = Task.Factory.StartNew(() =>
                                        Send(false, false, textBox1.Text));
            timer3.Start();
            button2.Enabled = false;
          


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string txt = textBox1.Text.ToLower();
            if (textBox1.Text != "")
            {
                if (words.Contains(textBox1.Text))
                {
                    MessageBox.Show("Bu kelime daha önce kullanıldı.");
                }
                else
                {
                    if (lbloword.Text != "")
                    {

                        if (txt.StartsWith(lbloword.Text.Substring(lbloword.Text.Length - 2).ToLower()))
                        {
                            var taskListener = Task.Factory.StartNew(() =>
                               Send(false, false, txt));
                            sendstatus = true;
                            words.Add(txt);
                            label4.Text = lblotheruser.Text + "'s Turn";
                            time = 60;
                        }
                        else
                        {
                            MessageBox.Show("Kelime Son İki harf ile başlamıyor");
                        }
                    }
                    else
                    {
                        var taskListener = Task.Factory.StartNew(() =>
                               Send(false, false, txt));
                        sendstatus = true;
                        words.Add(txt);
                        label4.Text = lblotheruser.Text + "'s Turn";
                        time = 60;
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen Bir Kelime Giriniz");
            }


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (winstatus == "You Win")
            {
                button2.Enabled = true;
            }

            if (label4.Text == "Your Turn" && startstatus == true)
            {
                textBox1.Enabled = true;
                button1.Enabled = true;
                if (sendstatus == false && time == 0)
                {
                    winstatus = "You Lose";
                    textBox1.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = true;
                    var taskListener = Task.Factory.StartNew(() =>
                                            Send(false, false, textBox1.Text));
                    timer1.Stop();

                }
            }
            else
            {
                textBox1.Enabled = false;
                button1.Enabled = false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            var task = Task.Factory.StartNew(() =>
                                          Send(true, false, textBox1.Text));

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            var taskListener = Task.Factory.StartNew(() =>
                                    Send(false, true, textBox1.Text));
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
     
            lbltime.Text = time.ToString();
            if (startstatus == true)
            {
                button1.Enabled = true;
                textBox1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                textBox1.Enabled = false;
            }
        }


        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            lblname.Text = toolStripTextBox1.Text;
            Properties.Settings.Default.UserName = toolStripTextBox1.Text;
            Properties.Settings.Default.Save();
            var taskListener = Task.Factory.StartNew(() =>
                                           Send(true, false, textBox1.Text));
        }

        private void Receive_FormClosing(object sender, FormClosingEventArgs e)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes("Host terminated.");
            udpServer.Send(msg, msg.Length, remoteEP);


            lblserverstatus.Dispose();
        }

        private void lbloword_TextChanged(object sender, EventArgs e)
        {
            label4.Text = "Your Turn";
            sendstatus = false;
            time = 60;
            timer3.Start();
        }

        private void Receive_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
