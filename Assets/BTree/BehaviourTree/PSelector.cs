using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSelector : Node
{
    bool _isOdernode = false;
    Node[] _nodeArray;
    public PSelector(string name)
    {
        _name = name;
    }

    public void OrderNode()
    {
        _nodeArray = children.ToArray();
        Sort(_nodeArray, 0, (children.Count - 1));
        children = new List<Node>(_nodeArray);
    }

    public override Status Process()
    {
        if(!_isOdernode)
        {
            OrderNode();
            _isOdernode = true;
        }

        Status childstatus = children[_currentChild].Process();
        if (childstatus == Status.RUNNING) return Status.RUNNING;
        if (childstatus == Status.SUCCESS)
        {
            _currentChild = 0;
            _isOdernode = false;
            return Status.SUCCESS;
        }


        _currentChild++;
        if(_currentChild >= children.Count)
        {
            _currentChild = 0;
            _isOdernode = false;
            return Status.FAILURE;
        }
        return Status.RUNNING;
    }

    public int Partition(Node[] array, int low, int high)
    {
        Node pivot = array[high];
        int lowIndex = (low - 1);

        for(int count = low; count < high; count++)
        {
            if(array[count]._priority <= pivot._priority)
            {
                lowIndex++;

                Node temp = array[lowIndex];
                array[lowIndex] = array[count];
                array[count] = temp;
            }
        }
        Node tempNode = array[lowIndex + 1];
        array[lowIndex + 1] = array[high];
        array[high] = tempNode;

        return (lowIndex + 1);
    }

    public void Sort(Node[] array, int low, int high)
    {
        if(low < high)
        {
            int partitionIndex = Partition(array, low, high);
            Sort(array, low, (partitionIndex - 1));
            Sort(array, (partitionIndex + 1), high);
        }
    }
}
