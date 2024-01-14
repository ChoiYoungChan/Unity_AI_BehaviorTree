using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string name)
    {
        _name = name;
    }

    public override Status Process()
    {
        Status childstatus = children[_currentChild].Process();
        if(childstatus == Status.RUNNING) return Status.RUNNING;
        if (childstatus == Status.FAILURE) return childstatus;

        _currentChild++;
        if(_currentChild >= children.Count)
        {
            _currentChild = 0;
            return Status.SUCCESS;
        }

        return Status.RUNNING;
    }
}
