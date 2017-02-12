using Akka;
using Akka.Actor;

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
        public Prefix Prefix { get; private set; }
    }

    public class Prefix
    {
        public string Servername { get; private set; }
    }
    // server
    //
    // connect NICK/USER

// <message>  ::= [':' <prefix> <SPACE> ] <command> <params> <crlf>
// <prefix>   ::= <servername> | <nick> [ '!' <user> ]
//    [ '@' <host> ]
// <command>  ::= <letter> { <letter> } | <number> <number> <number>
// <SPACE>    ::= ' ' { ' ' }
// <params>   ::= <SPACE> [ ':' <trailing> | <middle> <params> ]
// <middle>   ::= <Any* non-empty* sequence of octets not including SPACE or NUL or CR or LF, the first of which may not be ':'>
// <trailing> ::= <Any, possibly* empty*, sequence of octets not including NUL or CR or LF>
// <crlf>     ::= CR LF
}

//class Program
//{
//    static void Main(string[] args)
//    {
//        // Create a new actor system (a container for your actors)
//        var system = ActorSystem.Create("MySystem");

//        // Create your actor and get a reference to it.
//        // This will be an "ActorRef", which is not a
//        // reference to the actual actor instance
//        // but rather a client or proxy to it.
//        var greeter = system.ActorOf<GreetingActor>("greeter");

//        // Send a message to the actor
//        greeter.Tell(new Greet("World"));

//        // This prevents the app from exiting
//        // before the async work is done
//        Console.ReadLine();
//    }
//}
