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

        public Command Command { get; set; }

        public User User { get; private set; }
        
        public Newnick Newnick { get; private set; }    
        
        public Prefix Prefix { get; set; }
    }

    public class MapCommand
    {
        public MapCommand(string command)
        {
            if (command.ToUpper() == "NICK")
                Command = Command.Nick;
        }

        public Command Command { get; private set; }
    }

    public enum Command
    {
        Unknown,
        Nick
    }

    public class Parser
    {
        public IrcMessage TryParse(string input)
        {
            //Parser<string> servername = 
            //    Parse.Letter.AtLeastOnce().Text().Token();

            //Parser<string> nick =
            //    Parse.Letter.AtLeastOnce().Text().Token();

            Parser<char> Colon = Parse.Char(':');

            Parser<string> serverOrNick =
                (from letters in Parse.Letter.Many().Text()
                 select letters).Token();

            Parser<Prefix> prefix =
                Colon.Then(_ => 
                    (from s in serverOrNick
                     select new Prefix(s)));

            Parser<string> numCommand = 
                Parse.Digit.AtLeastOnce().Text().Token();
            Parser<string> textCommand =
                Parse.Letter.Many().Text().Token();

            //Parser<Command> command =
            //    prefix
            //    .Then(p => Parse.Return(Command.Unknown))
            //        .Return(numCommand)
            //    .Or(_ => textCommand)
            //        .Return(new MapCommand(textCommand).Command)

            Parser<Command> command =
                numCommand
                .Or(textCommand)
                .Select(x => new MapCommand(x).Command);                

            return new IrcMessage
            {
                Prefix = prefix.Parse(input),
                Command = command.Parse(input)
            };
        }
    }

    public class Prefix
    {
        public Prefix(string servernameORnick)
        {
            ServernameOrNick = servernameORnick;
        }

        public string ServernameOrNick { get; private set; }
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
