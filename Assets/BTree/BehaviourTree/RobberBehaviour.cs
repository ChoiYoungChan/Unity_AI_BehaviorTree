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
    [SerializeField] GameObject[] _art;
    GameObject _pickUp;
    #endregion

    #region public
    [Range(0, 1000)]
    public int _money = 100;

    Leaf goToBackDoor;
    Leaf goToFrontDoor;

    #endregion

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        Sequence steal = new Sequence("Steal Something");
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 2);
        Leaf goToPainting = new Leaf("Go To Painting", GoToPainting, 1);
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);

        Leaf goToAr1 = new Leaf("Go To Art 1", GoToArt1);
        Leaf goToAr2 = new Leaf("Go To Art 2", GoToArt2);
        Leaf goToAr3 = new Leaf("Go To Art 3", GoToArt3);

        goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor, 2);
        goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor, 1);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        PSelector opdnDoor = new PSelector("Open Door");
        PSelector selectObject = new PSelector("Select Object To Steal");
        RamdomSelector rselectObject = new RamdomSelector("Select Object To Steal");

        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney);

        opdnDoor.AddChild(goToFrontDoor);
        opdnDoor.AddChild(goToBackDoor);

        steal.AddChild(invertMoney);
        steal.AddChild(opdnDoor);

        selectObject.AddChild(goToDiamond);
        selectObject.AddChild(goToPainting);

        rselectObject.AddChild(goToAr1);
        rselectObject.AddChild(goToAr2);
        rselectObject.AddChild(goToAr3);

        steal.AddChild(rselectObject);

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

    public Node.Status GoToArt1()
    {
        if (!_art[1].activeSelf) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(_art[1].transform.position);
        if (status == Node.Status.SUCCESS)
        {
            _art[1].transform.parent = this.gameObject.transform;
            _pickUp = _art[1];
        }
        return status;
        //return GoToLocation(_diamond.transform.position);
    }

    public Node.Status GoToArt2()
    {
        if (!_art[2].activeSelf) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(_art[2].transform.position);
        if (status == Node.Status.SUCCESS)
        {
            _art[2].transform.parent = this.gameObject.transform;
            _pickUp = _art[2];
        }
        return status;
        //return GoToLocation(_diamond.transform.position);
    }

    public Node.Status GoToArt3()
    {
        if (!_art[0].activeSelf) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(_art[0].transform.position);
        if (status == Node.Status.SUCCESS)
        {
            _art[0].transform.parent = this.gameObject.transform;
            _pickUp = _diamond;
        }
        return status;
        //return GoToLocation(_diamond.transform.position);
    }

    public Node.Status HasMoney()
    {
        if (_money < 500)
            return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToBackDoor()
    {
        Node.Status s = GoToDoor(_backdoor);
        if (s == Node.Status.FAILURE)
            goToBackDoor._priority = 10;
        else
            goToBackDoor._priority = 1;
        return s;
    }

    public Node.Status GoToFrontDoor()
    {
        Node.Status s = GoToDoor(_frontdoor);
        if (s == Node.Status.FAILURE)
            goToFrontDoor._priority = 10;
        else
            goToFrontDoor._priority = 1;
        return s;
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
