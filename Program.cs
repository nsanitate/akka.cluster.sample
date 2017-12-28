using System;
using Akka.Actor;
using Akka.Configuration;

namespace Samples.Cluster.Simple
{
    class Program
    {
        private static void Main(string[] args)
        {
            StartUp(args.Length == 0 ? new String[] { "2551", "2552", "0" } : args);
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        public static void StartUp(string[] ports)
        {
            var fallback = ConfigurationFactory.ParseString(@"
				akka {
                    actor {
                        provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
                    }
                    
                    remote {
                        log-remote-lifecycle-events = DEBUG
                        dot-netty.tcp {
                            hostname = ""localhost""
                            port = 0
                        }
                    }
                    cluster {
                        seed-nodes = [
                            ""akka.tcp://ClusterSystem@localhost:2551"",
                            ""akka.tcp://ClusterSystem@localhost:2552""
                        ]
                    }
                }
            ");

            foreach (var port in ports)
            {
                //Override the configuration of the port
                var config =
                    ConfigurationFactory.ParseString("akka.remote.dot-netty.tcp.port=" + port)
                        .WithFallback(fallback);

                //create an Akka system
                var system = ActorSystem.Create("ClusterSystem", config);

                //create an actor that handles cluster domain events
                system.ActorOf(Props.Create(typeof(SimpleClusterListener)), "clusterListener");
            }
        }
    }
}
