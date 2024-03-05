using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    public delegate Status MultiTick(int val);
    public MultiTick MultiProcessMethod;

    public int index;

    public Leaf() { }

    public Leaf(string name, Tick processMethod)
    {
        _name = name;
        ProcessMethod = processMethod;
    }

    public Leaf(string name, int number, MultiTick processMethod)
    {
        _name = name;
        MultiProcessMethod = processMethod;
        index = number;
    }

    public Leaf(string name, Tick processMethod, int priority)
    {
        _name = name;
        ProcessMethod = processMethod;
        _priority = priority;
    }

    public override Status Process()
    {
        if(ProcessMethod != null)
            return ProcessMethod();
        else if (MultiProcessMethod != null)
            return MultiProcessMethod(index);

        return Status.FAILURE;
    }
}
