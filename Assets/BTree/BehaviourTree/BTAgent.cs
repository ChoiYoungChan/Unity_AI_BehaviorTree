using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTAgent : MonoBehaviour
{
    public BehaviourTree _tree;
    public NavMeshAgent _navAgent;
    public enum ActionState { IDLE, WORKING };
    public ActionState _state = ActionState.IDLE;
    public Node.Status _treeStatus = Node.Status.RUNNING;
    WaitForSeconds _waitForSeconds;

    // Start is called before the first frame update
    public void Start()
    {
        _navAgent = this.GetComponent<NavMeshAgent>();
        _tree = new BehaviourTree();
        _waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 1.0f));
        StartCoroutine("Behave");
    }

    public Node.Status GoToLocation(Vector3 dest)
    {
        float destanceToTarget = Vector3.Distance(dest, this.transform.position);
        if(_state == ActionState.IDLE)
        {
            _navAgent.SetDestination(dest);
            _state = ActionState.WORKING;
        } else if(Vector3.Distance(_navAgent.pathEndPosition, dest) >= 2) {
            _state = ActionState.IDLE;
            return Node.Status.FAILURE;
        } else if(destanceToTarget < 2) {
            _state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    IEnumerator Behave()
    {
        while(true)
        {
            _treeStatus = _tree.Process();
            yield return _waitForSeconds;
        }
    }
}
