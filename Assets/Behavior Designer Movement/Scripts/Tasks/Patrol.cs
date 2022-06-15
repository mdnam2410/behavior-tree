using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Patrol around the specified waypoints using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=7")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]
    public class Patrol : NavMeshMovement
    {
        [Tooltip("Should the agent patrol the waypoints randomly?")]
        public SharedBool randomPatrol = false;
        [Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
        public SharedFloat waypointPauseDuration = 0;
        [Tooltip("The waypoints to move to")]
        public SharedGameObjectList waypoints;
        // custom
        [Tooltip("Send event when start")]
        public SharedString sendEventWhenStart;
        [Tooltip("Send event when stop")]
        public SharedString sendEventWhenStop;
        //

        // The current index that we are heading towards within the waypoints array
        private int waypointIndex;
        private float waypointReachedTime;
        // custom
        private bool _isMoving = false;
        public SharedBool hasTime;
        public SharedFloat limitTime;
        private float _currentTime = 0;
        //

        // if is stucked, should change next target position
        private float _maxDistanceToCheckStuck = 0.3f;
        private float _maxTimeToCheckStuck = 3f;
        private float _currentTimeToCheckStuck = 0;
        private Vector3 _prePosition = Vector3.zero;
        //

        public override void OnStart()
        {
            base.OnStart();
            _prePosition = transform.position;
            _currentTime = 0;
            _currentTimeToCheckStuck = 0;
            _isMoving = false;
            // initially move towards the closest waypoint
            float distance = Mathf.Infinity;
            float localDistance;
            for (int i = 0; i < waypoints.Value.Count; ++i) {
                if ((localDistance = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) < distance) {
                    distance = localDistance;
                    waypointIndex = i;
                }
            }
            waypointReachedTime = -1;
            SetDestination(Target());
            if (!HasArrived())
            {
                SendStart();
            }
            else
            {
                MoveNextPoint();
            }
        }

        // Patrol around the different waypoints specified in the waypoint array. Always return a task status of running. 
        public override TaskStatus OnUpdate()
        {
            if (waypoints.Value.Count == 0) {
                return TaskStatus.Failure;
            }
            if (hasTime.Value)
            {
                _currentTime += Time.deltaTime;
                if(_currentTime >= limitTime.Value)
                {
                    return TaskStatus.Success;
                }
            }
            if (HasArrived() || !_isMoving) {
                if(waypointReachedTime <= 0)
                {
                    waypointReachedTime = Time.time;
                }
                // wait the required duration before switching waypoints.
                if (waypointReachedTime + waypointPauseDuration.Value <= Time.time) {
                    MoveNextPoint();
                }
                else
                {
                    if (_isMoving)
                    {
                        SendStop();
                    }
                }
            }
            var position = transform.position;
            // check stuck
            if (_isMoving)
            {
                if(Vector3.Distance(_prePosition, position) <= _maxDistanceToCheckStuck)
                {
                    _currentTimeToCheckStuck += Time.deltaTime;
                    if(_currentTimeToCheckStuck >= _maxTimeToCheckStuck)
                    {
                        _currentTimeToCheckStuck = 0;
                        //MoveNextPoint();
                        SendStop();
                        waypointReachedTime = -1;
                    }
                }
                else
                {
                    _prePosition = position;
                    _currentTimeToCheckStuck = 0;
                }
            }
            //
            return TaskStatus.Running;
        }

        // Return the current waypoint index position
        private Vector3 Target()
        {
            if (waypointIndex >= waypoints.Value.Count) {
                return transform.position;
            }
            return waypoints.Value[waypointIndex].transform.position;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();

            randomPatrol = false;
            waypointPauseDuration = 0;
            waypoints = null;
        }

        protected void MoveNextPoint()
        {
            _prePosition = transform.position;
            waypointReachedTime = -1;
            if (randomPatrol.Value)
            {
                if (waypoints.Value.Count == 1)
                {
                    waypointIndex = 0;
                }
                else
                {
                    // prevent the same waypoint from being selected
                    var newWaypointIndex = waypointIndex;
                    while (newWaypointIndex == waypointIndex)
                    {
                        newWaypointIndex = Random.Range(0, waypoints.Value.Count);
                    }
                    waypointIndex = newWaypointIndex;
                }
            }
            else
            {
                waypointIndex = ( waypointIndex + 1 ) % waypoints.Value.Count;
            }
            SetDestination(Target());
            SendStart();
        }

        private void SendStart()
        {
            if (sendEventWhenStart != null && sendEventWhenStart.Value != null)
            {
                GetComponent<BehaviorTree>().SendEvent(sendEventWhenStart.Value);
            }
            _isMoving = true;
        }

        private void SendStop()
        {
            if (sendEventWhenStop != null && sendEventWhenStop.Value != null)
            {
                GetComponent<BehaviorTree>().SendEvent(sendEventWhenStop.Value);
            }
            _isMoving = false;
        }

        // Draw a gizmo indicating a patrol 
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (waypoints == null || waypoints.Value == null) {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            for (int i = 0; i < waypoints.Value.Count; ++i) {
                if (waypoints.Value[i] != null) {
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
                    UnityEditor.Handles.SphereCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1);
#else
                    UnityEditor.Handles.SphereHandleCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1, EventType.Repaint);
#endif
                }
            }
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}