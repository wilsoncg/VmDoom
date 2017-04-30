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

        private IList<String> _params = new string[] { };
        public IList<String> Params
        {
            get { return _params; }
        }
        
        public Newnick Newnick { get; private set; }    
        
        public Prefix Prefix { get; set; }
    }

    public class MapCommand
    {
        public MapCommand(string command)
        {
            if (command.ToUpper() == "NICK")
                Command = Command.Nick;
            if (command.ToUpper() == "USER")
                Command = Command.User;
        }

        public Command Command { get; private set; }
    }

    public enum Command
    {
        Unknown,
        Nick,
        User
    }

    public class Parser
    {
        public IrcMessage TryParse(string input)
        {
            Parser<Char> special = 
                Parse.Char('-')
                .Or(Parse.Char('['))
                .Or(Parse.Char(']'))
                .Or(Parse.Char('\\'))
                .Or(Parse.Char('^'))
                .Or(Parse.Char('`'))
                .Or(Parse.Char('{'))
                .Or(Parse.Char('}'));

            Parser<char> Colon = Parse.Char(':');

            Parser<string> serverOrNick =
                (from letters in Parse.Upper.Many().Text()
                 select letters).Token();

            Parser<Prefix> prefix =
                (Colon
                    .Then(_ =>
                    (from p in serverOrNick
                     from space in Parse.WhiteSpace.Once().Text()
                     select new Prefix(p))));

            //Parser<Prefix> prefix =
            //    Colon.Then(_ => 
            //        (from s in serverOrNick
            //         select new Prefix(s)));

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

            var m = new IrcMessage();
            Parser<IrcMessage> message =
                (from c in command
                 select new IrcMessage { Command = c })
                 .Or
                (from p in prefix
                 from c in command
                 select new IrcMessage { Prefix = p, Command = c });
            //.Then(_ => command);

            return message.Parse(input);
            //return new IrcMessage
            //{
            //    Prefix = prefix.Parse(input),
            //    Command = command.Parse(input)
            //};
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
