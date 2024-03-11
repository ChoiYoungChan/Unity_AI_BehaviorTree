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

        Sequence viewArts = new Sequence("View Art");
        Selector bePatron = new Selector("Be an art Patron");

        whileBored.AddChild(isBored);
        Loop loop = new Loop("Loop Node", whileBored);
        
        loop.AddChild(selectObject);

        viewArts.AddChild(isBored);
        viewArts.AddChild(gotofrontdoor);
        viewArts.AddChild(loop);
        viewArts.AddChild(gotohome);

        bePatron.AddChild(viewArts);

        _tree.AddChild(bePatron);

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
        return status;
    }

    public Node.Status IsBored()
    {
        if (boredom < 100)
            return Node.Status.FAILURE;
        else
            return Node.Status.SUCCESS;
    }

}
