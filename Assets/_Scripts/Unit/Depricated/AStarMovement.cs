using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.UnitTask
{
    public abstract class AStarMovement : Action
    {
        [UnityEngine.Serialization.FormerlySerializedAs("maxSpeed")]
        public SharedFloat m_MaxSpeed = 5;
        [UnityEngine.Serialization.FormerlySerializedAs("maxAcceleration")]
        public SharedFloat m_MaxAcceleration = 10;
        [UnityEngine.Serialization.FormerlySerializedAs("endReachedDistance")]
        public SharedFloat m_EndReachedDistance = 1f;
        [UnityEngine.Serialization.FormerlySerializedAs("slowdownDistance")]
        public SharedFloat m_SlowdownDistance = 1.5f;
        [UnityEngine.Serialization.FormerlySerializedAs("whenCloseToDestination")]
        public SharedBool m_WhenCloseToDestinationStop = true;

        protected AIPath _aiPath;
        protected AIDestinationSetter _destination;


        public override void OnAwake()
        {
            _aiPath = GetComponent<AIPath>();
            _destination = GetComponent<AIDestinationSetter>();
        }

        public override void OnStart()
        {
            InitAIPath();
        }

        protected void InitAIPath(float maxSpeed_ = 5f, float maxAcceleration_ = 10f, float endReachedDistance_ = 1f, 
                                    float slowdownDistance_ = 1.5f, bool whenCloseToDestinationStop_ = true)
        {
            _aiPath.isStopped = false;
            _aiPath.maxSpeed = maxSpeed_;
            _aiPath.maxAcceleration = maxAcceleration_;
            _aiPath.endReachedDistance = endReachedDistance_;
            _aiPath.slowdownDistance = slowdownDistance_;
            if(whenCloseToDestinationStop_) _aiPath.whenCloseToDestination = CloseToDestinationMode.Stop;
            else _aiPath.whenCloseToDestination = CloseToDestinationMode.ContinueToExactDestination;
        }
        protected void SetDestination(Transform destination_)
        {
            _destination.target = destination_;
        }

        protected void SetRandomDestination(int distance_)
        {
            _destination.target = null;
            _aiPath.SetPath(RandomPath.Construct(transform.position, distance_));
        }


        protected void Stop()
        {
            _destination.target = null;
            _aiPath.isStopped = true;
        }

        protected bool GetIsReachedDest()
        {
            if(_destination.target != null){
                float distance = Vector3.Distance(_destination.target.transform.position, transform.position);
                if(distance <= m_EndReachedDistance.Value) return true;
            }
            else{
                Debug.Log("destination is nul!!");
            }
            return false;
        }

        protected int GetDirection(){
            if(_aiPath.hasPath == false) return 0;
            if(_aiPath.destination.x > transform.position.x) return 1;
            else return -1;
        }

        public override void OnEnd()
        {
            // Stop();
        }

        public override void OnBehaviorComplete()
        {
            Stop();
        }

        public override void OnReset()
        {
            m_MaxSpeed = 5;
            m_MaxAcceleration = 10;
            m_SlowdownDistance = 1.5f;
            m_EndReachedDistance = 1f;
            m_WhenCloseToDestinationStop = true;
        }
    }
}