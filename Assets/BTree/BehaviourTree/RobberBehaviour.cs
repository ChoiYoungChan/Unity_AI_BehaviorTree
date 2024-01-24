using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent
{
    #region private
    [SerializeField] GameObject _diamond;
    [SerializeField] GameObject _painting;
    [SerializeField] GameObject _van;
    [SerializeField] GameObject _backdoor;
    [SerializeField] GameObject _frontdoor;
    GameObject _pickUp;
    #endregion

    #region public
    [Range(0, 1000)]
    public int _money = 100;

    #endregion

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        Sequence steal = new Sequence("Steal Something");
        Leaf goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor);
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 2);
        Leaf goToPainting = new Leaf("Go To Painting", GoToPainting, 1);
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Selector opdnDoor = new Selector("Open Door");
        PSelector selectObject = new PSelector("Select Object To Steal");

        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney);

        opdnDoor.AddChild(goToFrontDoor);
        opdnDoor.AddChild(goToBackDoor);

        steal.AddChild(invertMoney);
        steal.AddChild(opdnDoor);

        selectObject.AddChild(goToDiamond);
        selectObject.AddChild(goToPainting);
        steal.AddChild(selectObject);

        steal.AddChild(goToVan);
        _tree.AddChild(steal);
    }

    public Node.Status GoToDiamond()
    {
        if (!_diamond.activeSelf) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(_diamond.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            _diamond.transform.parent = this.gameObject.transform;
            _pickUp = _diamond;
        }
        return status;
        //return GoToLocation(_diamond.transform.position);
    }

    public Node.Status GoToPainting()
    {
        if (!_painting.activeSelf) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(_painting.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            _painting.transform.parent = this.gameObject.transform;
            _pickUp = _painting;
        }
        return status;
    }

    public Node.Status HasMoney()
    {
        if (_money < 500)
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
            _money += 300;
            _pickUp.SetActive(false);
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
                door.GetComponent<NavMeshObstacle>().enabled = false;
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
}
