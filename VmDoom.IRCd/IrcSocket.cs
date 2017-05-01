using Akka.Actor;
using Akka.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VmDoom.IRCd
{
    public class IrcDaemon : UntypedActor
    {
        private IActorRef _server;

        public IrcDaemon(IActorRef server) : this(IPAddress.Any, 6667, server) { }

        public IrcDaemon(IPAddress ip, int port, IActorRef server)
        {
            _server = server;
            Context.System.Tcp().Tell(new Tcp.Bind(Self, new IPEndPoint(ip, port)));
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Bound)
            {
                var bound = message as Tcp.Bound;
                Trace.WriteLine($"Listening on {bound.LocalAddress}");
            }
            else if (message is Tcp.Connected)
            {
                var connection = Context.ActorOf(Akka.Actor.Props.Create(() => new IrcConnection(Sender, _server)));
                Sender.Tell(new Tcp.Register(connection));
            }
            else Unhandled(message);
        }

        public static Props Props(IActorRef ircServer)
        {
            return Akka.Actor.Props.Create(() => new IrcDaemon(ircServer));
        }
    }

    public class IrcConnection : UntypedActor
    {
        private IActorRef _connection;
        private IActorRef _server;

        public IrcConnection(IActorRef connection, IActorRef server)
        {
            _connection = connection;
            _server = server;
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Received)
            {
                var received = message as Tcp.Received;
                var text = Encoding.UTF8.GetString(received.Data.ToArray());

                // pass data to IrcServer for processing
                var t = Task.Run(async () =>
                {
                    // Using Ask will send a message to the receiving Actor as with Tell, 
                    // and the receiving actor must reply with Sender.Tell(reply, Self) 
                    // in order to complete the returned Task with a value.
                    var t1 = _server.Ask<string>(text, TimeSpan.FromSeconds(2));
                    await Task.WhenAll(t1);
                    return t1.Result;
                });
                t.PipeTo(Self);
            }

            if(message is string)
            {
                var received = message as string;
                _connection.Tell(Tcp.Write.Create(ByteString.FromString(received)));
            }

            else Unhandled(message);
        }
    }
}
