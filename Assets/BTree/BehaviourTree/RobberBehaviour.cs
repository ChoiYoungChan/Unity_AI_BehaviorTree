using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    #region private
    BehaviourTree _tree;

    [SerializeField] GameObject _diamond;
    [SerializeField] GameObject _van;
    [SerializeField] GameObject _backdoor;
    [SerializeField] GameObject _frontdoor;
    NavMeshAgent _navAgent;

    #endregion

    #region public
    enum ActionState { IDLE, WORKING };
    ActionState _state = ActionState.IDLE;
    Node.Status _treeStatus = Node.Status.RUNNING;

    [Range(0, 1000)]
    public int _money = 100;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _navAgent = this.GetComponent<NavMeshAgent>();

        _tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal Something");
        Leaf goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor);
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);

        Selector opdnDoor = new Selector("Open Door");

        opdnDoor.AddChild(goToFrontDoor);
        opdnDoor.AddChild(goToBackDoor);

        steal.AddChild(hasGotMoney);
        steal.AddChild(opdnDoor);
        steal.AddChild(goToDiamond);
        //steal.AddChild(goToFrontDoor);
        steal.AddChild(goToVan);
        _tree.AddChild(steal);
    }

    public Node.Status GoToDiamond()
    {
        Node.Status status = GoToLocation(_diamond.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            _diamond.transform.parent = this.gameObject.transform;
        }
        return status;
        //return GoToLocation(_diamond.transform.position);
    }

    public Node.Status HasMoney()
    {
        if (_money >= 500)
            return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToBackDoor()
    {
        return GoToDoor(_backdoor);
    }

    public Node.Status GoToFrontDoor()
    {
        return GoToDoor(_frontdoor);
    }

    public Node.Status GoToVan()
    {
        Node.Status status = GoToLocation(_van.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            _money += 500;
            _diamond.SetActive(false);
        }
        return status;
        //return GoToLocation(_van.transform.position);
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status status = GoToLocation(door.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>()._isLocked)
            {
                door.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else
            return status;
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

    // Update is called once per frame
    void Update()
    {
        if(_treeStatus != Node.Status.SUCCESS)
            _treeStatus = _tree.Process();
    }
}
