using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using static Akka.Cluster.ClusterEvent;

namespace Sample.Cluster.Simple
{
    public class SimpleClusterListener2 : UntypedActor
    {
        protected ILoggingAdapter Log;
        protected Akka.Cluster.Cluster Cluster;

        public SimpleClusterListener2()
        {
            Log = Logging.GetLogger(Context.System, this);
            Cluster = Akka.Cluster.Cluster.Get(Context.System);
        }

        /// <summary>
        /// subscribe to cluster changes
        /// </summary>
        protected override void PreStart()
        {
            Cluster.Subscribe(Self, typeof(IMemberEvent), typeof(UnreachableMember));
        }

        /// <summary>
        /// re-subscribe when restart
        /// </summary>
        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CurrentClusterState state:
                    Log.Info("Current members: {0}", state.Members);
                    break;
                case MemberUp mUp:
                    Log.Info("Member is Up: {0}", mUp.Member);
                    break;
                case UnreachableMember mUnreachable:
                    Log.Info("Member detected as unreachable: {0}", mUnreachable.Member);
                    break;
                case MemberRemoved mRemoved:
                    Log.Info("Member is Removed: {0}", mRemoved.Member);
                    break;
                case IMemberEvent mEvent:
                    // ignore
                    break;
            }
        }
    }
}