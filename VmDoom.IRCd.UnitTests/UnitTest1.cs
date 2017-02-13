using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VmDoom.IRCd.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        // <message>  ::= [':' <prefix> <SPACE> ] <command> <params> <crlf>
        // <prefix>   ::= <servername> | <nick> [ '!' <user> ]
        //    [ '@' <host> ]
        // command is either 0 or more letters or 3 numbers seperated by spaces
        // <command>  ::= <letter> { <letter> } | <number> <number> <number>
        // <SPACE>    ::= ' ' { ' ' }
        // <params>   ::= <SPACE> [ ':' <trailing> | <middle> <params> ]
        // <middle>   ::= <Any* non-empty* sequence of octets not including SPACE or NUL or CR or LF, the first of which may not be ':'>
        // <trailing> ::= <Any, possibly* empty*, sequence of octets not including NUL or CR or LF>
        // <crlf>     ::= CR LF

        // More:
        // <target>     ::= <to> [ "," <target> ]
        // <to>         ::= <channel> | <user> '@' <servername> | <nick> | <mask>
        // <channel>    ::= ('#' | '&') <chstring>
        // <servername> ::= <host>
        // <host>       ::= see RFC 952 [DNS:4] for details on allowed hostnames
        // <nick>       ::= <letter> { < letter > | < number > | < special > }
        // <mask>       ::= ('#' | '$') <chstring>
        // <chstring>   ::= <any 8bit code except SPACE, BELL, NUL, CR, LF and comma(',')>
        // <user>       ::= <nonwhite> { <nonwhite> }
        // <letter>     ::= 'a' ... 'z' | 'A' ... 'Z'
        // <number>     ::= '0' ... '9'
        // <special>    ::= '-' | '[' | ']' | '\' | '`' | '^' | '{' | '}'

        // Notes:
        // Optional items are in square brackets [ '!' <user> ]
        // | (read as 'or')
        // { ... } read as zero or more

        // NICK
        // Numeric replies:
        // ERR_NONICKNAMEGIVEN 
        // ERR_ERRONEUSNICKNAME
        // ERR_NICKNAMEINUSE 
        // ERR_NICKCOLLISION
        [TestMethod]
        public void ParseNick()
        {
            var input = "NICK nickname";
            var message = new Parser().TryParse(input);

            Assert.AreEqual("nick", message.Command);
            Assert.AreEqual("nickname", message.User.Nickname);
        }

        [TestMethod]
        public void ParseNickChange()
        {
            var input = ":nickname NICK newnickname";
            var message = new Parser().TryParse(input);

            Assert.AreEqual("changenick", message.Command);
            Assert.AreEqual("newnickname", message.Newnick.New);
            Assert.AreEqual("nickname", message.Newnick.Old);
        }

        // USER
        // Parameters: <username> <hostname> <servername> <realname>
        // Examples:
        // USER guest tolmoon tolsun :Ronnie Reagan
        // Numeric Replies: 
        // ERR_NEEDMOREPARAMS 461 
        // ERR_ALREADYREGISTRED 462
        [TestMethod]
        public void ParseUser()
        {
            var input = "USER ~username host server :realname";
            var userToString = "!~username@host";
            var message = new Parser().TryParse(input);
            var user = message.User;

            Assert.AreEqual("~username", user.Username);
            Assert.AreEqual("host", user.Host);
            Assert.AreEqual("realname", user.Realname);
            Assert.AreEqual("user", message.Command);
            Assert.AreEqual(userToString, message.User.ToString());
        }
    }
}
