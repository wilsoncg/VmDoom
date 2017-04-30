﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VmDoom.IRCd.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        // <message>  ::= [':' <prefix> <SPACE> ] <command> <params> <crlf>
        // <prefix>   ::= <servername> | <nick> [ '!' <user> ] [ '@' <host> ]
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
        public void MessageIsNickCommand()
        {
            var input = "NICK nickname";
            var message = new Parser().TryParse(input);

            Assert.IsNull(message.Prefix);
            Assert.AreEqual(Command.Nick, message.Command);
            Assert.IsNotNull(message.Params);
            Assert.IsFalse(message.Params.Count == 0);
            Assert.AreEqual("nickname", message.Params[0]);
        }

        [TestMethod]
        public void MessageIsPrefixWithNickCommand()
        {
            var input = ":nickname NICK newnickname";
            var message = new Parser().TryParse(input);

            Assert.AreEqual("nickname", message.Prefix.ServernameOrNick);
            Assert.AreEqual(Command.Nick, message.Command);
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
        public void MessageIsUserCommand()
        {
            var input = "USER ~username host server :realname";
            var userToString = "!~username@host";
            var message = new Parser().TryParse(input);

            Assert.AreEqual(Command.User, message.Command);
            Assert.IsTrue(message.Params.Count == 4);
            Assert.AreEqual("~username", message.Params[0]);
            Assert.AreEqual("host", message.Params[1]);
            Assert.AreEqual("server", message.Params[2]);
            Assert.AreEqual("realname", message.Params[3]);
        }
    }
}
