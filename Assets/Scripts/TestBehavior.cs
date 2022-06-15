using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner;
using BehaviorDesigner.Runtime;

public class TestBehavior : MonoBehaviour
{
    SharedFloat _duration;
    SharedBool _isA;
    SharedBool _isB;
    SharedBool _isC;
    SharedBool _isA1;
    BehaviorTree _behaviorTree;
    [SerializeField]
    ExternalBehavior ExternalBehavior;

    // Start is called before the first frame update
    void Start()
    {
        _behaviorTree = gameObject.AddComponent<BehaviorTree>();
        _behaviorTree.ExternalBehavior = ExternalBehavior;

        _behaviorTree.RegisterEvent("DoSomething", () =>
        {
            Debug.Log("Do Something");
            _duration.Value = 5;
        });

        _duration = _behaviorTree.GetVariable("Duration") as SharedFloat;
        _isA = _behaviorTree.GetVariable("IsA") as SharedBool;
        _isB = _behaviorTree.GetVariable("IsB") as SharedBool;
        _isC = _behaviorTree.GetVariable("IsC") as SharedBool;
        _isA1 = _behaviorTree.GetVariable("IsA1") as SharedBool;

        _behaviorTree.EnableBehavior();
    }

    public void OnClickA()
    {
        _isA.Value = !_isA.Value;
    }

    public void OnClickB()
    {
        _isB.Value = !_isB.Value;
    }

    public void OnClickC()
    {
        _isC.Value = !_isC.Value;
    }

    public void OnClickD()
    {
        _behaviorTree.SendEvent("ReceiveEventD");
    }

    public void OnClickA1()
    {
        _isA1.Value = !_isA1.Value;
    }
}
