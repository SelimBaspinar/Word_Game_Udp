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
    public partial class Client : Form
    {
        IPEndPoint ep;
        UdpClient client = new UdpClient();
        HashSet<string> words = new HashSet<string>();
        int time = 60;
        Boolean sendstatus;
        string winstatus;
        bool startstatus = false;
        public Client()
        {
            InitializeComponent();
        }
        void udpClient()
        {
            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000); // endpoint where server is listening
            client.Connect(ep);

                
            while (true)
            {
                read();
            }
        }
        void write(bool user, bool timestat, string txt)
        {

            if (user == false && timestat == true && winstatus == "You Lose")
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes("Win Status:" + "You Win");
                client.Send(sendBytes, sendBytes.Length);
                MessageBox.Show("You Lose" + "-> " + lblotheruser.Text + " Win");
                winstatus = "";

            }

            if (user == false && timestat == false)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes(txt);
                client.Send(sendBytes, sendBytes.Length);

                if (lblyword.InvokeRequired)
                {
                    lblyword.Invoke(new MethodInvoker(delegate { lblyword.Text = textBox1.Text.ToUpper(); }));
                }
                else
                {
                    lblyword.Text = textBox1.Text.ToUpper();
                }



            }

            else if (user == false && timestat == true)
            {
                if (sendstatus == true)
                {
                    Byte[] sendBytes = Encoding.ASCII.GetBytes("Time:");
                    client.Send(sendBytes, sendBytes.Length);
                }
            }

            else if (user == true)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes("ClientUserName:" + lblname.Text);
                client.Send(sendBytes, sendBytes.Length);
            }
        }
        void read()
        {
            try {
                string receivedData = Encoding.ASCII.GetString(client.Receive(ref ep));

                if (receivedData.Contains("HostUserName:"))
                {
                    if (lblotheruser.InvokeRequired)
                    {
                        lblotheruser.Invoke(new MethodInvoker(delegate { lblotheruser.Text = receivedData.Substring(13); }));
                    }
                    else
                    {
                        lblotheruser.Text = receivedData.Substring(13);

                    }
                    if(lblserverstatus.InvokeRequired)
                    {
                        lblserverstatus.Invoke(new MethodInvoker(delegate { lblserverstatus.Text = "Connection established"; }));
                    }
                    else
                    {
                        lblserverstatus.Text = "Connection established";

                    }
                    if (startstatus == true)
                    {
                        if (label4.InvokeRequired)
                        {
                            label4.Invoke(new MethodInvoker(delegate { label4.Text = lblotheruser.Text + "'s Turn"; }));
                        }
                        else
                        {
                            label4.Text = lblotheruser.Text + "'s Turn";

                        }
                        startstatus = false;
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

                else if (receivedData.Contains("You Win"))
                {
                    winstatus = "You Win";
                    MessageBox.Show("You Win");
                }
                else if (receivedData.Contains("Time:"))
                {
                    try
                    {
                        time = Convert.ToInt32(receivedData.Substring(5));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                else
                {
                    if (lbloword.InvokeRequired)
                    {
                        lbloword.Invoke(new MethodInvoker(delegate { lbloword.Text = receivedData.ToUpper(); }));
                    }
                    else
                    {
                        lbloword.Text = receivedData.ToUpper();
                    }
                    words.Add(lbloword.Text.ToLower());

                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                if (lblserverstatus.InvokeRequired)
                {
                    lblserverstatus.Invoke(new MethodInvoker(delegate { lblserverstatus.Text = "Host is Waiting"; }));
                }
                else
                {
                    lblserverstatus.Text = "Host is Waiting";
                }

            }
            

        }
        private void Client_Load(object sender, EventArgs e)
        {

            var taskListener = Task.Factory.StartNew(() =>
                          udpClient());
            lblserverstatus.Text = "Connection is waiting...";
    
            toolStripTextBox1.Text = Properties.Settings.Default.UserName;
            lblname.Text = Properties.Settings.Default.UserName;

            timer2.Start();
            timer1.Start();
            timer4.Start();
            timer5.Start();
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
                                          write(false, false, textBox1.Text));
                            words.Add(txt);
                            label4.Text = lblotheruser.Text + "'s Turn";
                            sendstatus = true;

                        }
                        else
                        {
                            MessageBox.Show("Kelime Son İki harf ile başlamıyor");
                        }
                    }
                    else
                    {
                        var taskListener = Task.Factory.StartNew(() =>
                               write(false, false, textBox1.Text));
                        words.Add(txt);
                        label4.Text = lblotheruser.Text + "'s Turn";
                        sendstatus = true;


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
            if (label4.Text == "Your Turn" && sendstatus == false && time == 0)
            {
                winstatus = "You Lose";
                var taskListener = Task.Factory.StartNew(() =>
                                        write(false, true, textBox1.Text));
                timer1.Stop();
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Receive receive = new Receive();
            receive.Show();
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            lblname.Text = toolStripTextBox1.Text;
            Properties.Settings.Default.UserName = toolStripTextBox1.Text;
            Properties.Settings.Default.Save();
            var taskListener = Task.Factory.StartNew(() =>
                                           write(true, false, textBox1.Text));
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            var task = Task.Factory.StartNew(() =>
                                          write(true, false, textBox1.Text));



        }

        private void lbloword_TextChanged(object sender, EventArgs e)
        {
            label4.Text = "Your Turn";
            sendstatus = false;
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            lbltime.Text = time.ToString();
            if (label4.Text == "Your Turn")
            {
                textBox1.Enabled = true;
                button1.Enabled = true;
            }
            else
            {
                textBox1.Enabled = false;
                button1.Enabled = false;
            }
        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            if (label4.Text != "Opponent's Turn")
                lblserverstatus.Text = "Connection is Successful";
        }
    }
}
