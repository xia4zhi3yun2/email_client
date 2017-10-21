using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using OpenPop.Pop3;
using OpenPop.Mime;
using MailSend;

namespace MailClient
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        void RefreshData()
        {
            CPublic.client.Disconnect();
            CPublic.client.Connect(CPublic.POPserver, CPublic.POPport, false);
            CPublic.client.Authenticate(CPublic.user, CPublic.password);


            int messageNum = CPublic.client.GetMessageCount();

            listMail.Items.Clear();
            listMail.BeginUpdate();

            for (int i = messageNum; i >= 1; i--)
            {
                OpenPop.Mime.Message message = CPublic.client.GetMessage(i);
                Mail mail = new Mail();
                mail.sender = message.Headers.From.Address;
                mail.receiver = message.Headers.From.DisplayName; ;
                mail.subject = message.Headers.Subject;
                mail.uid = CPublic.client.GetMessageUid(i);
                mail.dateTime = message.Headers.DateSent;


                ListViewItem first = new ListViewItem();
                int recent = messageNum - i + 1;
                first.SubItems.Add(recent.ToString());
                first.SubItems.Add(mail.receiver);
                first.SubItems.Add(mail.dateTime.ToString());
                first.SubItems.Add(mail.subject);
                first.SubItems.Add(mail.body);

                listMail.Items.Add(first);


                try
                {
                    DataSet ds = new DataSet();
                    string sqlStr = "select * from mail where uid = '" + mail.uid + "'";
                    ds = CDataBase.GetDataFromDB(sqlStr);
                    if (ds == null)
                    {
                        listMail.Items[recent - 1].ForeColor = Color.Red;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
               
            }
            listMail.EndUpdate();

        }


        private void FrmMain_Load(object sender, EventArgs e)
        {
            RefreshData();
            label1.Text = "发件人：无";
            label2.Text = "发送时间：无";
            label3.Text = "邮件主题：无";
        }

       
        private void listMail_Click(object sender, EventArgs e)
        {
            if (this.listMail.SelectedItems.Count > 0)
            {
                int messageNum = CPublic.client.GetMessageCount();

                int listNum = Convert.ToInt32(listMail.SelectedItems[0].SubItems[1].Text.Trim());

                int recentIndex = messageNum - listNum + 1;

                OpenPop.Mime.Message message = CPublic.client.GetMessage(recentIndex);


                Mail mail = new Mail();
                mail.sender = message.Headers.From.Address;
                mail.receiver = message.Headers.From.DisplayName;
                mail.subject = message.Headers.Subject;
                mail.uid = CPublic.client.GetMessageUid(recentIndex);
                mail.dateTime = message.Headers.DateSent;


                try
                {
                    DataSet ds = new DataSet();
                    string sqlStr = "select * from mail where uid = '" + mail.uid + "'";
                    ds = CDataBase.GetDataFromDB(sqlStr);
                    if (ds == null)
                    {
                        sqlStr = "insert into mail values('" + mail.uid + "')";
                        CDataBase.UpdateDB(sqlStr);
                    }
                    listMail.Items[listNum - 1].ForeColor = Color.Black;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);


                }
                label1.Text = "发件人：" + mail.sender;
                label2.Text = "发送时间：" + mail.dateTime;
                label3.Text = "邮件主题：" + mail.subject;


                MessagePart mp = message.MessagePart;
                if (mp.IsText)
                {
                    mail.body = mp.GetBodyAsText();
                }
                else if (mp.IsMultiPart)
                {
                    MessagePart plainTextPart = message.FindFirstPlainTextVersion();
                    if (plainTextPart != null)
                    {

                        mail.body = plainTextPart.GetBodyAsText();
                    }
                    else
                    {
                        List<MessagePart> textVersions = message.FindAllTextVersions();
                        if (textVersions.Count >= 1)
                            mail.body = textVersions[0].GetBodyAsText();
                        else
                            mail.body = "<<OpenPop>> Cannot find a text version body in this message.";
                    }
                }
                txtMail.Text = mail.body;

            }

        }

      


        private void btnDelete_Click(object sender, EventArgs e)
        {
            int num = listMail.CheckedItems.Count;  //需要删除邮件数量
            int sum = CPublic.client.GetMessageCount();
            for (int i = 0; i < num; i++)
            {
                int messageid = Convert.ToInt32(this.listMail.CheckedItems[i].SubItems[1].Text);
                int mid = sum - messageid + 1;
                string uid = CPublic.client.GetMessageUid(mid);
                CPublic.client.DeleteMessage(mid);

                string sqlStr = "delete from mail where uid = '" + uid + "'";
                CDataBase.UpdateDB(sqlStr);


            }

            RefreshData();
            MessageBox.Show("删除所选邮件成功！");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtContent.Text = "";
            txtReceiver.Text = "";
            txtTopic.Text = "";
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SmtpMail send = new SmtpMail();
            Cursor.Current = Cursors.WaitCursor;
            send.Charset = "GB2312";
            send.eSmtp = true;
            send.MailServerUserName = CPublic.user;
            send.MailServerPassWord = CPublic.password;
            send.Send(CPublic.SMTPserver, CPublic.user, CPublic.user, txtReceiver.Text.Trim(), txtTopic.Text.Trim(), txtContent.Text);
            Cursor.Current = Cursors.Default;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
            MessageBox.Show("刷新成功！");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
