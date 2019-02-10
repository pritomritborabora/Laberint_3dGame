using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class EnemyRoam : MonoBehaviour
{

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    Animator anim;
    AnimatorStateInfo info;
    bool clipInfo;
    void Start()
        {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = true;
        anim.SetInteger("Walk", 1);
        GotoNextPoint();

    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }


    void Update()
    {

        // Choose the next destination point when the agent gets
        // close to the current one.
        //    

        //   {

        //}
        info = anim.GetCurrentAnimatorStateInfo(0);
        clipInfo = info.IsName("Twist");
    //    Debug.Log(clipInfo);

        if (clipInfo == false)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GotoNextPoint();
        }
    }
}