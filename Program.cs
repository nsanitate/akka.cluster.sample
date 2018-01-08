using System;
using Akka.Actor;
using Akka.Configuration;

namespace Sample.Cluster.Simple
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
                        dot-netty.tcp {
                            hostname = ""127.0.0.1""
                            port = 0
                        }
                    }
                    cluster {
                        seed-nodes = [
                            ""akka.tcp://ClusterSystem@127.0.0.1:2551""
                            ""akka.tcp://ClusterSystem@127.0.0.1:2552""]

                        # auto downing is NOT safe for production deployments.
                        # you may want to use it during development, read more about it in the docs.
                        auto-down-unreachable-after = 10s
                    }
                }
            ");

            foreach (var port in ports)
            {
                // Override the configuration of the port
                var config = ConfigurationFactory.ParseString("akka.remote.dot-netty.tcp.port=" + port)
                  .WithFallback(fallback);

                // Create an Akka system
                var system = ActorSystem.Create("ClusterSystem", config);

                // Create an actor that handles cluster domain events
                system.ActorOf(Props.Create(typeof(SimpleClusterListener)), "clusterListener");
            }
        }
    }
}
