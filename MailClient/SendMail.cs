using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace MailSend
{
    public class SmtpMail
    {
        public SmtpMail()
        {
            
        }

        
        // 回车
        private string enter = "\r\n";    
        //设定语言代码 
        private string _charset = "GB2312";
        // 发件人地址 
        private string _from = "";
        // 发件人姓名 
        private string _fromName = "";
        // 收件人列表 
        private string Recipient;
        // 邮件服务器域名 
        private string mailserver = "";
        // 邮件服务器端口号 
        private int mailserverport = 25;
        // SMTP认证时使用的用户名 
        private string username = ""; 
        // SMTP认证时使用的密码
        private string password = "";
        // 是否需要SMTP验证 
        private bool ESmtp = false;
        // 邮件发送优先级，可设置为"High","Normal","Low"或"1","3","5" 
        private string priority = "Normal";
        // 邮件主题
        private string _subject;
        // 邮件正文
        private string _body;
        // 错误消息反馈
        private string errmsg;
        // TcpClient对象，用于连接服务器 
        private TcpClient tc;
        // NetworkStream对象 
        private NetworkStream ns;
        // SMTP错误代码哈希表 
        private Hashtable ErrCodeHT = new Hashtable();
        // SMTP正确代码哈希表 
        private Hashtable RightCodeHT = new Hashtable();


        
        //邮件主题 
         public string Subject
        {
            get
            {
                return this._subject;
            }
            set
            {
                this._subject = value;
            }
        }
        
        // 邮件正文
        public string Body
        {
            get
            {
                return this._body;
            }
            set
            {
                this._body = value;
            }
        }

        // 发件人地址  
        public string From
        {
            get
            {
                return _from;
            }
            set
            {
                this._from = value;
            }
        }

        // 设定语言代码 
        public string Charset
        {
            get
            {
                return this._charset;
            }
            set
            {
                this._charset = value;
            }
        }
         
        //发件人姓名 
        public string FromName
        {
            get
            {
                return this._fromName;
            }
            set
            {
                this._fromName = value;
            }
        }

        // 收件人姓名  
        public string RecipientName
        {
            get
            {
                return this.Recipient;
            }
            set
            {
                this.Recipient = value;
            }
        }

        // 邮件服务器域名和验证信息  
        public string MailDomain
        {
            set
            {
                mailserver = value;

            }


        }

        // 邮件服务器端口号 
        public int MailDomainPort
        {
            set
            {
                mailserverport = value;
            }
        }
   
        // SMTP认证时使用的用户名 
        public string MailServerUserName
        {
            set
            {
                if (value.Trim() != "")
                {
                    username = value.Trim();
                    ESmtp = true;
                }
                else
                {
                    username = "";
                    ESmtp = false;
                }
            }
        }

        // SMTP认证时使用的密码 
        public string MailServerPassWord
        {
            set
            {
                password = value;
            }
        }

        // 错误消息反馈 
        public string ErrorMessage
        {
            get
            {
                return errmsg;
            }
        }

        public bool eSmtp
        {
            get
            {
                return this.ESmtp;
            }
            set
            {
                this.ESmtp = value;
            }
        }



        
        // 发送邮件方法，所有参数均通过属性设置 
        public bool Send()
        {
            if (mailserver.Trim() == "")
            {
                MessageBox.Show("必须指定SMTP服务器");
                return false;
            }

            return SendEmail();

        }


     
        // 发送邮件方法  
        // <param name="smtpserver">smtp服务器信息

        public bool Send(string smtpserver)
        {
            MailDomain = smtpserver;
            return Send();
        }


        // 发送邮件方法 

        public bool Send(string smtpserver, string from, string fromname, string to, string

            subject, string body)
        {
            MailDomain = smtpserver;
            From = from;
            FromName = fromname;
            Recipient = to;
            //Html = html;
            Subject = subject;
            Body = body;
            return Send();
        }


        void Dispose()
        {
            if (ns != null) ns.Close();
            if (tc != null) tc.Close();
        }

        /// <summary> 
        /// SMTP回应代码哈希表 
        /// </summary> 
        private void SMTPCodeAdd()
        {
            ErrCodeHT.Add("500", "邮箱地址错误");
            ErrCodeHT.Add("501", "参数格式错误");
            ErrCodeHT.Add("502", "命令不可实现");
            ErrCodeHT.Add("503", "服务器需要SMTP验证");
            ErrCodeHT.Add("504", "命令参数不可实现");
            ErrCodeHT.Add("421", "服务未就绪，关闭传输信道");
            ErrCodeHT.Add("450", "要求的邮件操作未完成，邮箱不可用（例如，邮箱忙）");
            ErrCodeHT.Add("550", "要求的邮件操作未完成，邮箱不可用（例如，邮箱未找到，或不可访问）");
            ErrCodeHT.Add("451", "放弃要求的操作；处理过程中出错");
            ErrCodeHT.Add("551", "用户非本地，请尝试<forward-path>");
            ErrCodeHT.Add("452", "系统存储不足，要求的操作未执行");
            ErrCodeHT.Add("552", "过量的存储分配，要求的操作未执行");
            ErrCodeHT.Add("553", "邮箱名不可用，要求的操作未执行（例如邮箱格式错误）");
            ErrCodeHT.Add("432", "需要一个密码转换");
            ErrCodeHT.Add("534", "认证机制过于简单");
            ErrCodeHT.Add("538", "当前请求的认证机制需要加密");
            ErrCodeHT.Add("454", "临时认证失败");
            ErrCodeHT.Add("530", "需要认证");

            RightCodeHT.Add("220", "服务就绪");
            RightCodeHT.Add("250", "要求的邮件操作完成");
            RightCodeHT.Add("251", "用户非本地，将转发向<forward-path>");
            RightCodeHT.Add("354", "开始邮件输入，以<enter>.<enter>结束");
            RightCodeHT.Add("221", "服务关闭传输信道");
            RightCodeHT.Add("334", "服务器响应验证Base64字符串");
            RightCodeHT.Add("235", "验证成功");
        }


        /// <summary> 
        /// 将字符串编码为Base64字符串 
        /// </summary> 
        /// <param name="str">要编码的字符串</param> 
        private string Base64Encode(string str)
        {
            byte[] barray;
            barray = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(barray);
        }


        /// <summary> 
        /// 发送SMTP命令 
        /// </summary> 
        private bool SendCommand(string str)
        {
            byte[] WriteBuffer;
            if (str == null || str.Trim() == String.Empty)
            {
                return true;
            }

            WriteBuffer = Encoding.Default.GetBytes(str);
            try
            {
                ns.Write(WriteBuffer, 0, WriteBuffer.Length);
            }
            catch
            {
                errmsg = "网络连接错误";
                return false;
            }
            return true;
        }

        /// <summary> 
        /// 接收SMTP服务器回应 
        /// </summary> 
        private string RecvResponse()
        {
            int StreamSize;
            string ReturnValue = String.Empty;
            byte[] ReadBuffer = new byte[1024];
            try
            {
                StreamSize = ns.Read(ReadBuffer, 0, ReadBuffer.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return "false";
            }

            if (StreamSize == 0)
            {
                return ReturnValue;
            }
            else
            {
                ReturnValue = Encoding.Default.GetString(ReadBuffer).Substring(0, StreamSize);
                return ReturnValue;
            }
        }

        /// <summary> 
        /// 与服务器交互，发送一条命令并接收回应。 
        /// </summary> 
        /// <param name="str">一个要发送的命令</param> 
        /// <param name="errstr">如果错误，要反馈的信息</param> 
        private bool Dialog(string str, string errstr)
        {

            if (str == null || str.Trim() == "")
            {
                return true;
            }
            if (SendCommand(str))
            {
                string RR = RecvResponse();
                if (RR == "false")
                {
                    return false;
                }
                try
                {
                    string RRCode = RR.Substring(0, 3);
                    if (RightCodeHT[RRCode] != null)
                    {
                        return true;
                    }
                    else
                    {
                        if (ErrCodeHT[RRCode] != null)
                        {
                            errmsg += (RRCode + ErrCodeHT[RRCode].ToString());
                            errmsg += enter;
                        }
                        else
                        {
                            errmsg += RR;
                        }
                        errmsg += errstr;
                        return false;
                    }
                }
                catch
                {
                    MessageBox.Show("发送的附件超过本服务器对个人软件的支持！", "请检查附件的大小");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary> 
        /// 与服务器交互，发送一组命令并接收回应。 
        /// </summary> 

        private bool Dialog(string[] str, string errstr)
        {
            for (int i = 0; i < str.Length; i++)
            {
                //如果在身份验证阶段有一个不成功，就返回错误标志位
                if (!Dialog(str[i], ""))
                {
                    errmsg += enter;
                    errmsg += errstr;
                    return false;
                }
            }

            //身份验证全部正确的话，则返回正确标志位
            return true;
        }

        /// <summary> 
        /// SendEmail 
        /// </summary> 
        /// <returns></returns> 
        private bool SendEmail()
        {

            //建立TCP连接
            try
            {
                //socket功能类，使用远程主机的主机名和端口号创建 TcpClient并自动尝试一个连接。
                tc = new TcpClient(mailserver, mailserverport);
            }
            catch
            {
                MessageBox.Show("连接失败", "请确认");
                return false;
            }

            // TcpClient使用 GetStream 方法返回网络流
            ns = tc.GetStream();
            SMTPCodeAdd();

            //验证网络连接是否正确 
            if (RightCodeHT[RecvResponse().Substring(0, 3)] == null)
            {
                errmsg = "网络连接失败";
                return false;
            }


            string[] SendBuffer;
            string SendBufferstr;

            //ESmtp=1,进行SMTP验证 
            if (ESmtp)
            {
                SendBuffer = new String[4];
                
                //发送端发送EHLO命令与SMTP建立服务器建立连接
                SendBuffer[0] = "EHLO " + mailserver + enter;
                
                //认证命令,用这个命令表示身份验证开始
                SendBuffer[1] = "AUTH LOGIN" + enter;
                //发送用户名和密码，使用base64对数据进行编码
                SendBuffer[2] = Base64Encode(username) + enter;
                SendBuffer[3] = Base64Encode(password) + enter;
                //调用Dialog函数与服务器交互，发送命令并接收回应
                if (!Dialog(SendBuffer, "SMTP服务器验证失败，请核对用户名和密码。"))
                {
                    MessageBox.Show("SMTP服务器验证失败，请核对用户名和密码。");
                    return false;
                }
            }
            
            //不需要验证
            else
            {
                SendBufferstr = "HELO " + mailserver + enter;
                if (!Dialog(SendBufferstr, ""))
                    return false;
            }

            //客户端将邮件发送方的地址传送给SMTP服务器 
            SendBufferstr = "MAIL FROM:<" + From + ">" + enter;
            if (!Dialog(SendBufferstr, "发件人地址错误，或不能为空"))
            {
                MessageBox.Show("发件人地址错误，或不能为空");
                return false;
            }


            //分割传过来的收件人的地址（收件人列表中可能有多个对象）
            string split = ",";
            string[] address = Regex.Split(RecipientName, split);
            //提交收件人地址给服务器
            SendBuffer = new string[address.Length];
            for (int i = 0; i < SendBuffer.Length; i++)
            {
                SendBuffer[i] = "RCPT TO:<" + address[i] + ">" + enter;
            }
            if (!Dialog(SendBuffer, "收件人地址有误"))
            {
                MessageBox.Show("收件人地址有误");
                return false;
            }


            //传送邮件数据
            SendBufferstr = "DATA" + enter;
            if (!Dialog(SendBufferstr, ""))
                return false;
            //发送方、接收方、主题、主体
            SendBufferstr = "From:" + FromName + "<" + From + ">" + enter;
            SendBufferstr += "To:<" + RecipientName + ">" + enter;
            SendBufferstr += ((Subject == String.Empty || Subject == null) ? "Subject:" : ((Charset == "") ? ("Subject:" +
                Subject) : ("Subject:" + "=?" + Charset.ToUpper() + "?B?" + Base64Encode(Subject) + "?="))) + enter;
            SendBufferstr += "X-Priority:" + priority + enter;
            SendBufferstr += "X-Mailer: ArgentSwan Mail Sender" + enter;
            SendBufferstr += "MIME-Version: 1.0" + enter;
            SendBufferstr += "Content-Type: text/plain;" + enter;
            SendBufferstr += ((Charset == "") ? (" charset=\"iso-8859-1\"") : (" charset=\"" +
            Charset.ToLower() + "\"")) + enter;
            SendBufferstr += "Content-Transfer-Encoding: base64" + enter + enter;
            SendBufferstr += Base64Encode(Body) + enter;
            //以.标示发送结束
            SendBufferstr += enter + "." + enter;
            if (!Dialog(SendBufferstr, "错误信件信息"))
                return false;

            //关闭流对象
            SendBufferstr = "QUIT" + enter;
            if (!Dialog(SendBufferstr, "断开连接时错误"))
                return false;
            MessageBox.Show("邮件发送成功！");
            //关闭连接
            ns.Close();
            tc.Close();
            return true;
        }

    }
}
