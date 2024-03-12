using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatronBehaviour : BTAgent
{
    [SerializeField] GameObject[] _artarray;
    [SerializeField] GameObject _frontdoor;
    [SerializeField] GameObject _backdoor;
    [SerializeField] GameObject _home;

    [SerializeField]
    [Range(0, 1000)]
    private int boredom = 150;
    public bool isTicket = false;
    public bool isWaiting = false;

    public override void Start()
    {
        base.Start();
        RamdomSelector selectObject = new RamdomSelector("Random Select Object to Steal");
        BehaviourTree whileBored = new BehaviourTree();

        for(int count = 0; count < _artarray.Length; count++)
        {
            Leaf getobj = new Leaf("Go to " + _artarray[count], count, GoToArt);
            selectObject.AddChild(getobj);
        }
        Leaf gotofrontdoor = new Leaf("Go to Front Door", GoToFrontDoor);
        Leaf gotohome = new Leaf("Go to Home", GoToHome);
        Leaf isBored = new Leaf("Is Bored", IsBored);
        Leaf isOpen = new Leaf("Is Open", IsOpen);

        Sequence viewArts = new Sequence("View Art");

        whileBored.AddChild(isBored);
        Loop loop = new Loop("Loop Node", whileBored);
        
        loop.AddChild(selectObject);

        viewArts.AddChild(isOpen);
        viewArts.AddChild(gotofrontdoor);
        viewArts.AddChild(loop);
        viewArts.AddChild(gotohome);

        Leaf noTicket = new Leaf("Wait for Ticket", NoTicket);
        Leaf isWaiting = new Leaf("Waiting for worker", IsWaiting);
        BehaviourTree waitForTicket = new BehaviourTree();
        waitForTicket.AddChild(noTicket);

        Loop getTicket = new Loop("Ticket", waitForTicket);
        getTicket.AddChild(isWaiting);
        viewArts.AddChild(getTicket);

        BehaviourTree galleryOpenCondition = new BehaviourTree();
        galleryOpenCondition.AddChild(isOpen);

        DepSequence bePatron = new DepSequence("Be an art Patron", galleryOpenCondition, _navAgent);
        bePatron.AddChild(viewArts);

        Selector viewArtWithFallback = new Selector("View Art with Fallback");
        viewArtWithFallback.AddChild(bePatron);
        viewArtWithFallback.AddChild(gotohome);

        _tree.AddChild(viewArtWithFallback);

        StartCoroutine("IncreaseBoredom");
    }

    IEnumerator IncreaseBoredom()
    {
        while(true)
        {
            boredom = Mathf.Clamp(boredom + 20, 0, 1000);
            yield return new WaitForSeconds(Random.Range(1, 5));
        }        
    }

    public Node.Status GoToArt(int index)
    {
        if (!_artarray[index].activeSelf) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(_artarray[index].transform.position);
        if (status == Node.Status.SUCCESS) boredom = Mathf.Clamp(boredom - 50, 0, 1000);

        return status;
    }

    public Node.Status GoToFrontDoor()
    {
        Node.Status status = GoToDoor(_frontdoor);
        return status;
    }

    public Node.Status GoToHome()
    {
        Node.Status status = GoToLocation(_home.transform.position);
        isWaiting = false;
        return status;
    }

    public Node.Status IsBored()
    {
        if (boredom < 100)
            return Node.Status.FAILURE;
        else
            return Node.Status.SUCCESS;
    }

    public Node.Status NoTicket()
    {
        if (isTicket || IsOpen() == Node.Status.FAILURE) return Node.Status.FAILURE;
        else return Node.Status.SUCCESS;
    }

    public Node.Status IsWaiting()
    {
        if (BlackBoard.Instance.RegisterPatron(this.gameObject) == this.gameObject)
        {
            isWaiting = true;
            return Node.Status.SUCCESS;
        }
        else return Node.Status.FAILURE;
    }
}
