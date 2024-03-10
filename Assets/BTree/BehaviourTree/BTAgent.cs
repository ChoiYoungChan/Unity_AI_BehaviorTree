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
    Vector3 _savedLocation;

    // Start is called before the first frame update
    public virtual void Start()
    {
        _navAgent = this.GetComponent<NavMeshAgent>();
        _tree = new BehaviourTree();
        _waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 1.0f));
        StartCoroutine("Behave");
    }

    public Node.Status CanSee(Vector3 target, string tag, float distance, float sightangle)
    {
        Vector3 direction = target - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);
        if(angle < sightangle && direction.magnitude < distance)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(this.transform.position, direction, out hitInfo))
            {
                if(hitInfo.collider.gameObject.CompareTag(tag)) return Node.Status.SUCCESS;
            }
        }
        return Node.Status.FAILURE;
    }

    public Node.Status Flee(Vector3 location, float distance)
    {
        if (_state == ActionState.IDLE) _savedLocation = this.transform.position + (transform.position - location).normalized * distance;
        return GoToLocation(_savedLocation);
    }

    public Node.Status GoToLocation(Vector3 dest)
    {
        float destanceToTarget = Vector3.Distance(dest, this.transform.position);
        if(_state == ActionState.IDLE) {
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

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status status = GoToLocation(door.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>()._isLocked)
            {
                door.GetComponent<NavMeshObstacle>().enabled = false;
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else
            return status;
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
