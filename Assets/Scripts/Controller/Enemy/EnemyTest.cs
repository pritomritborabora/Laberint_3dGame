using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* Controls the Enemy AI */

public class EnemyTest : MonoBehaviour
{

    public float lookRadius = 10f;  // Detection range for player

    Transform target;   // Reference to the player
    NavMeshAgent agent; // Reference to the NavMeshAgent
    Animator anim;
    GameObject PlayerSmell;
    GameObject prev;
    private float moveSpeed = 2f;
    private float distance;

    public Transform[] points;
    private int destPoint = 0;
    FIndPlayerSmell findplayer = new FIndPlayerSmell();
    //  CharacterCombat combat;

    // Use this for initialization
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.SetInteger("Condition", 1);
        GotoNextPoint();
        // combat = GetComponent<CharacterCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        
            PlayerSmell = findplayer.FIndPlayer("PlayerSmell");

            if (PlayerSmell != null )
            {
                 distance = Vector3.Distance(PlayerSmell.transform.position, transform.position);
                if (distance <= lookRadius)
                {
                    FollowSmell(PlayerSmell);
                }
            }
            else {
             if (anim.GetCurrentAnimatorStateInfo(0).IsName("run"))

              {
                       anim.SetInteger("Run", 0);
                  }

            if (!agent.pathPending && agent.remainingDistance < 1f)
            {   GotoNextPoint();
              }
                //   
            }
        }
    
    void FollowSmell(GameObject PLayerSmell)
    {
        if (PlayerSmell.activeSelf)
        {   
            anim.SetInteger("Run", 1);
            FaceTarget();
            agent.SetDestination(PlayerSmell.transform.position);


            

        } else anim.SetInteger("Run", 0);
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

    // Rotate to face the target
    void FaceTarget()
    {
       
        Vector3 direction = (PlayerSmell.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Show the lookRadius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}