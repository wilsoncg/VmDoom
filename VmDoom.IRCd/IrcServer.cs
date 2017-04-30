using Akka;
using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VmDoom.IRCd.Message;

namespace VmDoom.IRCd
{
    public class IrcServer : ReceiveActor
    {
        public IrcServer()
        {
            Receive<IrcMessage>(m => { return; });
        }
    }  

    public class User
    {
        public string Host { get; set; }
        public string Nickname { get; set; }
        public string Username { get; set; }
        public string Realname { get; set; }
    }

    public class Newnick
    {
        public string Old { get; set; }
        public string New { get; set; }
    }
}
