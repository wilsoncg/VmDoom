using Akka;
using Akka.Actor;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmDoom.IRCd
{
    public class IrcServer : ReceiveActor
    {
        public IrcServer()
        {
            Receive<IrcMessage>(m => { return; });
        }
    }

    public class IrcMessage
    {
        public IrcMessage()
        {
        }

        public string Command { get; set; }

        public User User { get; private set; }
        
        public Newnick Newnick { get; private set; }    
        
        public Prefix Prefix { get; set; }
    }

    public class Parser
    {
        public IrcMessage TryParse(string input)
        {
            //Parser<string> servername = 
            //    Parse.Letter.AtLeastOnce().Text().Token();

            //Parser<string> nick =
            //    Parse.Letter.AtLeastOnce().Text().Token();

           Parser<string> servername =
                (from letters in Parse.Letter.Many().Text()
                 select letters).Token();

            Parser<string> nick =
                (from letters in Parse.LetterOrDigit.AtLeastOnce().Text()
                 select letters).Token();

            Parser<Prefix> prefix =
                from s in servername
                from n in nick
                select new Prefix(s, n);

            return new IrcMessage
            {
                Prefix = prefix.Parse(input)
            };
        }
    }

    public class Prefix
    {
        public Prefix(string servername, string nick)
        {
            Servername = servername;
            Nick = nick;
        }

        public string Servername { get; private set; }
        public string Nick { get; private set; }
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
