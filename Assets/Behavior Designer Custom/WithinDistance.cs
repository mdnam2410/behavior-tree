using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    [TaskDescription("Check within the distance")]
    [TaskCategory("Custom")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=18")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WithinDistanceIcon.png")]
    public class WithinDistance : Conditional
    {
        [Tooltip("The object that we are searching for")]
        public SharedGameObject targetObject;
        [Tooltip("The agent has arrived when the destination is less than the specified amount")]
        public SharedFloat distance = 0.2f;
        public SharedBool useY = false;

        public override void OnAwake()
        {
        }

        public override void OnStart()
        {
            
        }

        // returns success if any object is within distance of the current object. Otherwise it will return failure
        public override TaskStatus OnUpdate()
        {
            if (transform == null || targetObject == null || targetObject.Value == null)
                return TaskStatus.Failure;
            Vector3 position1 = transform.position;
            Vector3 position2 = targetObject.Value.transform.position;
            if (!useY.Value)
            {
                position1.y = 0;
                position2.y = 0;
            }
            var d = Vector3.Distance(position1, position2);
            if (d <= distance.Value)
            {
                return TaskStatus.Success;
            }
            
            // no objects are within distance. Return failure
            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetObject = null;
            distance = 0.2f;
        }
    }
}