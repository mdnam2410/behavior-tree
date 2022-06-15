using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Check to see if the any objects are within sight of the agent.")]
    [TaskCategory("Movement")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=11")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class CanSeeObjectNearest : Conditional
    {
       
        [Tooltip("The object that we are searching for")]
        public SharedGameObject targetObject;
        [Tooltip("The objects that we are searching for")]
        public SharedGameObjectList targetObjects;
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedFloat fieldOfViewAngle = 90;
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 1000;
        [Tooltip("The distance that the agent can detect, ignore view angle")]
        public SharedFloat viewDistanceIgnoreViewAngle = 1000;
        [Tooltip("Need Check has path")]
        public SharedBool needCheckHasPath = true;

        private NavMeshAgent _navMeshAgent = null;

        public override void OnStart()
        {
            base.OnStart();
            _navMeshAgent = transform.GetComponent<NavMeshAgent>();
        }

        // Returns success if an object was found otherwise failure
        public override TaskStatus OnUpdate()
        {
            if (targetObjects.Value != null && targetObjects.Value.Count > 0)
            { // If there are objects in the group list then search for the object within that list
                GameObject objectFound = null;
                float minDistance = -1;
                Vector3 positionOffset = new Vector3(0, 1, 0);
                var position = transform.position;
                var forward = transform.forward;
                for (int i = 0; i < targetObjects.Value.Count; ++i)
                {
                    var targetTransform = targetObjects.Value[i].transform;
                    if (!targetTransform.gameObject.activeInHierarchy)
                        continue;
                    var targetPosition = targetTransform.position;
                    float distance = 0;
                    if (
                        WithinSight(position, forward, targetPosition, 360, viewDistanceIgnoreViewAngle.Value, false, out distance) ||
                        ( viewDistanceIgnoreViewAngle.Value < viewDistance.Value && WithinSight(position, forward, targetPosition, fieldOfViewAngle.Value, viewDistance.Value, false, out distance)))
                    {
                        // This object is within sight. Set it to the objectFound GameObject if the angle is less than any of the other objects
                        if (minDistance < 0 || minDistance > distance)
                        {
                            minDistance = distance;
                            objectFound = targetTransform.gameObject;
                        }
                    }
                }
                if (targetObject.Value == null)
                {
                    targetObject.Value = objectFound;
                }
                else
                {
                    if (objectFound != null)
                    {
                        targetObject.Value = objectFound;
                    }
                    else
                    {
                        targetObject.Value = targetObjects.Value.Any(x => x == targetObject.Value) ? targetObject.Value : null;
                    }
                }
                if (targetObject.Value != null)
                {
                    // Return success if an object was found
                    return TaskStatus.Success;
                }
            }
            if (targetObject.Value != null)
            {
                // Return success if an object was found
                return targetObjects.Value.Any(x => x == targetObject.Value) ? TaskStatus.Success : TaskStatus.Failure;
            }
            // An object is not within sight so return failure
            return TaskStatus.Failure;
        }

        // Reset the public variables
        public override void OnReset()
        {
            fieldOfViewAngle = 90;
            viewDistance = 1000;
        }

        // Draw the line of sight representation within the scene window
        public override void OnDrawGizmos()
        {
            MovementUtility.DrawLineOfSight(Owner.transform, Vector3.zero, fieldOfViewAngle.Value, 0, viewDistance.Value, false);
        }

        public override void OnBehaviorComplete()
        {
            MovementUtility.ClearCache();
        }

        protected bool WithinSight(Vector3 position, Vector3 forward, Vector3 targetPosition, float fieldOfViewAngle, float limitViewDistance, bool useRaycastToFind, out float distance)
        {
            var direction = targetPosition - position;
            distance = direction.magnitude;
            if (distance > limitViewDistance)
            {
                return false;
            }
            var angle = Vector3.Angle(direction, forward);
            direction.y = 0;
            if(angle >= fieldOfViewAngle * 0.5f)
            {
                return false;
            }

            if (needCheckHasPath.Value && _navMeshAgent != null)
            {
                var navMeshPath = new NavMeshPath();
                _navMeshAgent.CalculatePath(targetPosition, navMeshPath);
                return navMeshPath.status == NavMeshPathStatus.PathComplete;
            }

            //if (useRaycastToFind)
            //{
            //    return LineInSight(targetTransform);
            //}
            return true;
        }

        protected bool LineInSight(Transform targetTransform)
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position, targetTransform.position, out hit, ~ignoreLayerMask))
            {
                if (hit.transform.IsChildOf(targetTransform) || targetTransform.IsChildOf(hit.transform))
                {
                    return true;
                }
            }
            return false;
        }
    }
}