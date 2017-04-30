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

        public Prefix Prefix { get; set; }

        public Command Command { get; set; }

        private IList<string> _params = new string[] { };
        public IList<string> Params
        {
            get { return _params; }
            set { _params = value; }
        }
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
                (from letters in Parse.Letter.Many().Text()
                 select letters).Token();

            Parser<Prefix> prefix =
                (Colon
                    .Then(_ =>
                    (from p in serverOrNick
                     select new Prefix(p))));

            Parser<string> numCommand = 
                Parse.Digit.AtLeastOnce().Text();
            Parser<string> textCommand =
                Parse.Letter.Many().Text();

            Parser<Command> command =
                numCommand
                .Or(textCommand)
                .Select(x => new MapCommand(x).Command);

            Parser<string> mparam =
                Parse.AnyChar.Many().Text().Token();

            Parser<List<String>> mparams =
                mparam.Many().Select(mp => mp.ToList());

            var m = new IrcMessage();
            Parser<IrcMessage> message =
                (from c in command
                 from space in Parse.WhiteSpace.Once().Text()
                 from mps in mparams
                 select new IrcMessage { Command = c, Params = mps })
                 .Or
                 (from p in prefix
                  from c in command
                  from space in Parse.WhiteSpace.Once().Text()
                  from mps in mparams
                  select new IrcMessage { Prefix = p, Command = c, Params = mps });

            //(prefix.Then(p => command).Then(c => mparams)
            //    .Return(
            //        (p, c, mparams) => 
            //        { m.Prefix = p; m.Command = c; m.Params = mparams; }
            //    ));
                 //from c in command
                 //select (Prefix p, Command c) => { m.Prefix = p; m.Command = c; });

            return message.Parse(input);
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
