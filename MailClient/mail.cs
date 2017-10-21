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
    class Mail
    {
        public string sender;
        public string receiver;
        public string subject;
        public string uid;
        public DateTime dateTime;
        public string body;
    }
}
