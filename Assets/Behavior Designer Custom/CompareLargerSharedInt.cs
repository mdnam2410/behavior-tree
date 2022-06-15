using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class CompareLargerSharedInt : Conditional
{
    [Tooltip("The first variable to compare larger")]
    public SharedInt variable;
    [Tooltip("The variable to compare to")]
    public SharedInt compareTo;

    public SharedBool hasEqual;

    public override TaskStatus OnUpdate()
    {
        if (hasEqual.Value)
        {
            return variable.Value >= compareTo.Value ? TaskStatus.Success : TaskStatus.Failure;
        }
        else
        {
            return variable.Value > compareTo.Value ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
        

    public override void OnReset()
    {
        variable = 0;
        compareTo = 0;
    }
}
