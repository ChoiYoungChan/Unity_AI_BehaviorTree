using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cop : BTAgent
{
    [SerializeField] GameObject[] patrolPoints;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Sequence selectPatrolPoint = new Sequence("Select Patrol Point");
        for(int count = 0; count < patrolPoints.Length; count++)
        {
            Leaf patrol = new Leaf("Patrol : " + patrolPoints[count].name, count, Patrol);
            selectPatrolPoint.AddChild(patrol);
        }
        _tree.AddChild(selectPatrolPoint);
    }

    public Node.Status Patrol(int index)
    {
        Node.Status status = GoToLocation(patrolPoints[index].transform.position);
        return status;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
