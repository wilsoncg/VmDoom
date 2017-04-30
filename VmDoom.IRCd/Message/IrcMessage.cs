using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmDoom.IRCd.Message
{
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

    public class Prefix
    {
        public Prefix(string servernameORnick)
        {
            ServernameOrNick = servernameORnick;
        }

        public string ServernameOrNick { get; private set; }
    }

    public enum Command
    {
        Unknown,
        Nick,
        User
    }
}
