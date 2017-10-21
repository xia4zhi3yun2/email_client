using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPop.Pop3;
using OpenPop.Mime;

namespace MailClient
{
    class CPublic
    {
        public static Pop3Client client;
        public static string user;
        public static string password;
        public static string POPserver;
        public static int POPport = 110;
        public static string SMTPserver;
        public static int SMTPport = 25;
    }
}
