using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace VmDoom.IRCd.Message
{
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
                Colon.Then(_ => Parse.AnyChar.Many().Text())
                .Or(Parse.CharExcept(' ').Many().Text());

            Parser<IEnumerable<string>> mparams =
                (mparam.Token()).Many();

            Parser<IrcMessage> message =
                (from c in command
                 from space in Parse.WhiteSpace.Once().Text()
                 from mps in mparams
                 select new IrcMessage { Command = c, Params = mps.ToList() })
                 .Or
                 (from p in prefix
                  from c in command
                  from space in Parse.WhiteSpace.Once().Text()
                  from mps in mparams
                  select new IrcMessage { Prefix = p, Command = c, Params = mps.ToList() });

            return message.Parse(input);
        }
    }
}
