using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamdomSelector : Node
{
    bool _isShuffled = false;
    public RamdomSelector(string name)
    {
        _name = name;
    }

    public override Status Process()
    {
        if (!_isShuffled)
        {
            children.Shuffle();
            _isShuffled = true;
        }
        
        Status childstatus = children[_currentChild].Process();
        if (childstatus == Status.RUNNING) return Status.RUNNING;
        if (childstatus == Status.SUCCESS)
        {
            _currentChild = 0;
            _isShuffled = false;
            return Status.SUCCESS;
        }
        _currentChild++;
        if (_currentChild >= children.Count)
        {
            _currentChild = 0;
            _isShuffled = false;
            return Status.FAILURE;
        }
        return Status.RUNNING;
    }
}