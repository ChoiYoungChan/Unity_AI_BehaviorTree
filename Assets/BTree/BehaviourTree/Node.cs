using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum Status { SUCCESS, RUNNING, FAILURE};
    public Status _status;
    public List<Node> children = new List<Node>();

    public int _currentChild = 0;
    public string _name;
    public int _priority;

    public Node() { }

    public Node(string name)
    {
        this._name = name;
    }

    public Node(string name, int priority)
    {
        this._name = name;
        _priority = priority;
    }

    public virtual Status Process()
    {
        return children[_currentChild].Process();
    }

    public void AddChild(Node child)
    {
        this.children.Add(child);
    }
}
