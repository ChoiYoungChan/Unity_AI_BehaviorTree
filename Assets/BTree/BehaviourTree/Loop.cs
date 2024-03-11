using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : Node
{
    BehaviourTree _dependancy;
    public Loop(string name, BehaviourTree dependancy)
    {
        _name = name;
        _dependancy = dependancy;
    }

    public override Status Process()
    {
        if(_dependancy.Process() == Status.FAILURE) return Status.SUCCESS;

        Status childstatus = children[_currentChild].Process();
        if (childstatus == Status.RUNNING) return Status.RUNNING;
        if (childstatus == Status.FAILURE) return childstatus;

        _currentChild++;
        if (_currentChild >= children.Count) _currentChild = 0;

        return Status.RUNNING;
    }
}
