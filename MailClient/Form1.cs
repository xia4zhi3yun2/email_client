using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Net;
using OpenPop.Pop3;
using OpenPop.Mime;


namespace MailClient
{
    public partial class FrmLogin : Form
    {
        Pop3Client client;
        int port = 110;
        public FrmLogin()
        {
            InitializeComponent();
            client = new Pop3Client();
            txtName.Text = "castleliang@163.com";
            txtPassword.Text = "lyx124357689";
            txtPOP.Text = "pop.163.com";
            txtSMTP.Text = "smtp.163.com";
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                client.Disconnect();
                MessageBox.Show("您的邮箱当前已在别处登录！");
                return;
            }
            else
            {
                //connect POP Server
                client.Connect(txtPOP.Text, port, false);

                client.Authenticate(txtName.Text, txtPassword.Text);
                MessageBox.Show("登陆成功！");





            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            txtPassword.Text = "";
            txtPOP.Text = "";
            txtSMTP.Text = "";
            MessageBox.Show("清空成功！");
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmLogin
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "FrmLogin";
            this.ResumeLayout(false);

        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
