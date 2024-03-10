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
    private int borendom = 150;

    public override void Start()
    {
        base.Start();
        RamdomSelector selectObject = new RamdomSelector("Random Select Object to Steal");
        for(int count = 0; count < _artarray.Length; count++)
        {
            Leaf getobj = new Leaf("Go to " + _artarray[count], count, GoToArt);
            selectObject.AddChild(getobj);
        }
        Leaf gotofrontdoor = new Leaf("Go to Front Door", GoToFrontDoor);
        Leaf gotohome = new Leaf("Go to Home", GoToHome);
        Leaf isBored = new Leaf("Is Bored", IsBored);

        Sequence viewArts = new Sequence("View Art");
        Selector bePatron = new Selector("Be an art Patron");

        viewArts.AddChild(isBored);
        viewArts.AddChild(gotofrontdoor);
        viewArts.AddChild(selectObject);
        viewArts.AddChild(gotohome);

        bePatron.AddChild(viewArts);

        _tree.AddChild(bePatron);
    }

    public Node.Status GoToArt(int index)
    {
        if (!_artarray[index].activeSelf) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(_artarray[index].transform.position);
        if (status == Node.Status.SUCCESS) borendom = Mathf.Clamp(borendom - 500, 0, 1000);

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
        return status;
    }

    public Node.Status IsBored()
    {
        if (borendom < 100)
            return Node.Status.FAILURE;
        else
            return Node.Status.SUCCESS;
    }

}
