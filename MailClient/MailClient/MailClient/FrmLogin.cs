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
        
        int port = 110;
        public FrmLogin()
        {
            InitializeComponent();
            CPublic.client = new Pop3Client();


            txtUser.Text = "castleliang@163.com";
            txtPassword.Text = "";
            txtPOP.Text = "pop.163.com";
            txtSMTP.Text = "smtp.163.com";

            //txtUser.Text = "xiazhuyun213@126.com";
            //txtPassword.Text = "542b542b";
            //txtPOP.Text = "pop.126.com";
            //txtSMTP.Text = "smtp.126.com";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (CPublic.client.Connected)
            {
                CPublic.client.Disconnect();
                MessageBox.Show("您的邮箱当前已在别处登录！");
                return;
            }
            else
            {
                CPublic.user = txtUser.Text.Trim();
                CPublic.password = txtPassword.Text.Trim();
                CPublic.SMTPserver = txtSMTP.Text.Trim();
                CPublic.POPserver = txtPOP.Text.Trim();


                //connect POP Server
                CPublic.client.Connect(txtPOP.Text, port, false);
                CPublic.client.Authenticate(txtUser.Text, txtPassword.Text);


                MessageBox.Show("登陆成功！");
                this.Hide();
                FrmMain ob_FrmMain = new FrmMain();
                ob_FrmMain.Show();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUser.Text = "";
            txtPassword.Text = "";
            txtPOP.Text = "";
            txtSMTP.Text = "";
            MessageBox.Show("清空成功！");
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
