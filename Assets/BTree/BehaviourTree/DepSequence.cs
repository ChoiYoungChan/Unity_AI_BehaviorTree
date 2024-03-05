using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DepSequence : Node
{
    BehaviourTree _dependancy;
    NavMeshAgent _agent;
    public DepSequence(string name, BehaviourTree dependancy, NavMeshAgent nav)
    {
        _name = name;
        _dependancy = dependancy;
        _agent = nav;
    }

    public override Status Process()
    {
        if(_dependancy.Process() == Status.FAILURE)
        {
            _agent.ResetPath();
            // reset all children;
            foreach (Node node in children)
                node.Reset();

            return Status.FAILURE;
        }
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
