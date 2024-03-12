using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : BTAgent
{
    [SerializeField] GameObject office;
    public GameObject patron;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Leaf patronStillWaiting = new Leaf("Is Patron Waiting", PatronWaiting);
        Leaf allocatePatron = new Leaf("Allocate Patron", AllocatePatron);
        Leaf goToPatron = new Leaf("Go To Patron", GotoPatron);
        Leaf goToOffice = new Leaf("go To Office", GoToOffice);

        Sequence getPatron = new Sequence("Find a Patron");
        getPatron.AddChild(allocatePatron);

        BehaviourTree waiting = new BehaviourTree();
        DepSequence moveToPatron = new DepSequence("Moving To Patron", waiting, _navAgent);
        waiting.AddChild(patronStillWaiting);
        moveToPatron.AddChild(goToPatron);
        getPatron.AddChild(moveToPatron);

        Selector beWorker = new Selector("Be a Worker");
        beWorker.AddChild(getPatron);
        beWorker.AddChild(goToOffice);

        _tree.AddChild(beWorker);
    }

    public Node.Status PatronWaiting()
    {
        if (patron == null) return Node.Status.FAILURE;
        if (patron.GetComponent<PatronBehaviour>().isWaiting) return Node.Status.SUCCESS;

        return Node.Status.FAILURE;
    }

    public Node.Status AllocatePatron()
    {
        if (BlackBoard.Instance.patron.Count == 0) return Node.Status.FAILURE;
        patron = BlackBoard.Instance.patron.Pop();
        if (patron == null) return Node.Status.FAILURE;

        return Node.Status.SUCCESS;
    }

    public Node.Status GotoPatron()
    {
        if (patron == null) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(patron.transform.position);

        if(status == Node.Status.SUCCESS)
        {
            patron.GetComponent<PatronBehaviour>().isTicket = true;
            patron = null;
        }
        return status;
    }

    public Node.Status GoToOffice()
    {
        Node.Status status = GoToLocation(office.transform.position);
        return status;
    }

}
