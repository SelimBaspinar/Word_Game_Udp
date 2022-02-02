using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordGameUdp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnconnect_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserName = textBox1.Text;
            Properties.Settings.Default.Save();
            Client client = new Client();
            client.Show();

        }

        private void btncreate_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserName = textBox1.Text;
            Properties.Settings.Default.Save();

            Receive receive = new Receive();
            receive.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.UserName;

        }
    }
}
