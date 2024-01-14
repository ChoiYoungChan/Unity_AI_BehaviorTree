using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : Node
{
    public BehaviourTree()
    {
        _name = "Tree";
    }

    public BehaviourTree(string name)
    {
        _name = name;
    }

    public override Status Process()
    {
        return children[_currentChild].Process();

    }

    struct NodeLevel
    {
        public int level;
        public Node node;
    }

    public void PrintTree()
    {
        string treeprint = "";
        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
        Node currentNode = this;
        nodeStack.Push(new NodeLevel { level = 0, node = currentNode } );

        while(nodeStack.Count != 0)
        {
            NodeLevel nextNode = nodeStack.Pop();
            treeprint += new string('-', nextNode.level) + nextNode.node._name + "\n";
            for(int count = nextNode.node.children.Count - 1; count >= 0; count--)
            {
                nodeStack.Push(new NodeLevel { level = nextNode.level + 1, node = nextNode.node.children[count] });
            }
        }
        Debug.LogError(treeprint);
    }
}
