using System;
using Akka.Actor;
using Akka.Configuration;

namespace Sample.Cluster.Simple.Listener
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
            foreach (var port in ports)
            {
                // Override the configuration of the port
                var config = ConfigurationFactory.ParseString("akka.remote.dot-netty.tcp.port=" + port)
                  .WithFallback(Configuration.Configuration.Fallback);

                // Create an Akka system
                var system = ActorSystem.Create("ClusterSystem", config);

                // Create an actor that handles cluster domain events
                system.ActorOf(Listener.Props(), "clusterListener");
            }
        }
    }
}
