using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(string name)
    {
        _name = name;
    }

    public override Status Process()
    {
        Status childstatus = children[_currentChild].Process();
        if (childstatus == Status.RUNNING) return Status.RUNNING;
        if (childstatus == Status.SUCCESS)
        {
            _currentChild = 0;
            return Status.SUCCESS;
        }
        _currentChild++;
        if(_currentChild >= children.Count)
        {
            _currentChild = 0;
            return Status.FAILURE;
        }
        return Status.RUNNING;
    }
}
