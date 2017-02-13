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
    public class IrcConnection : UntypedActor
    {
        private IActorRef _server;

        public IrcConnection(IActorRef server) : this(IPAddress.Any, 6667, server) { }

        public IrcConnection(IPAddress ip, int port, IActorRef server)
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
                var connection = Context.ActorOf(Props.Create(() => new IrcSocket(Sender, _server)));
                Sender.Tell(new Tcp.Register(connection));
            }
            else Unhandled(message);
        }
    }

    public class IrcSocket : UntypedActor
    {
        private IActorRef _connection;

        private IActorRef _server;

        public IrcSocket(IActorRef connection, IActorRef server)
        {
            _connection = connection;
            _server = server;
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Received)
            {
                var received = message as Tcp.Received;
                // pass data to IrcServer for processing
                var t = Task.Run(async () =>
                {
                    var t1 = _server.Ask<string>(received.Data, TimeSpan.FromSeconds(2));
                    await Task.WhenAll(t1);
                    return t1.Result;
                });
                t.PipeTo(_server, Self);
                _connection.Tell(Tcp.Write.Create(received.Data));
            }
            else Unhandled(message);
        }
    }
}
