using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* Controls the Enemy AI */

public class EnemyController : MonoBehaviour
{
    private FiniteStateMachine<EnemyController> finiteStateMachine;

    public FiniteStateMachine<EnemyController> FiniteStateMachine
    {
        get
        {
            return finiteStateMachine;
        }
    }

    public List<Transform> points;
    public float lookRadius = 10f;  // Detection range for player
    public bool wallIsRemoved = false;
    public GameObject wallRemovedId;

    Transform target;   // Reference to the player
    public NavMeshAgent agent; // Reference to the NavMeshAgent
    Animator anim;
    public GameObject Scent;

    //private float moveSpeed = 2f; 
    FIndPlayerSmell findplayer = new FIndPlayerSmell();
    private int destPoint = 0;

    public float period;
    //  CharacterCombat combat;

    // Use this for initialization
    void Start()
    {
        //target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.autoBraking = true;

        anim.SetInteger("Condition", 1);
        InvokeRepeating("DropScent", 0f, period);
        finiteStateMachine = new FiniteStateMachine<EnemyController>();
        finiteStateMachine.Configure(this, MovingState.Instance());

        anim.SetInteger("Walk", 1);
        // combat = GetComponent<CharacterCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        finiteStateMachine.Update();
    }

    //{
    // // Distance to the target
    //PlayerSmell = findplayer.FIndPlayer("PlayerSmell");
    //if (PlayerSmell != null)
    //{
    //    float distance = Vector3.Distance(PlayerSmell.transform.position, transform.position);
    //    if (distance <= lookRadius)
    //    {
    //        if (PlayerSmell.activeSelf)
    //        {
    //            FaceTarget();
    //            agent.SetDestination(PlayerSmell.transform.position);


    //            anim.SetInteger("Run", 1);

    //        }
    //        else anim.SetInteger("Run", 0);
    //    }
    //}
    //else anim.SetInteger("Run", 0);

    void DropScent()
    {
        Vector3 position = transform.position;
        Quaternion rotation = Scent.transform.rotation;
        Instantiate(Scent, position, rotation);
    }

    public void ChangeAnimation(string animationDesc, int value)
    {
        anim.SetInteger(animationDesc, value);
    }

    public void ChangeAnimation(string animationDesc, bool value)
    {
        anim.SetBool(animationDesc, value);
    }

    public AnimatorStateInfo GetAnimatorStateInfo(int indx)
    {
        return anim.GetCurrentAnimatorStateInfo(indx);
    }

    // Rotate to face the target
    //void FaceTarget()
    //{
    //    //   Vector3 direction = (PlayerSmell.transform.position - transform.position).normalized;
    //    Vector3 direction = (Scent.transform.position - transform.position).normalized;
    //    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    //    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    //}

    // Show the lookRadius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    public void SetPoint(Vector3 position)
    {
        agent.destination = position;
    }

    public void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Count == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Count;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetCurrentDestination()
    {
        return agent.destination;
    }

    public void SetSpeed(int speed)
    {
        agent.speed = speed;
    }

    public void DestroyScent(GameObject scent)
    {
        Destroy(scent);
    }

    public float DistanceToPlayer()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        GameObject player = gos[0];
        return Vector3.Distance(transform.position, player.transform.position);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision", collision.gameObject);
        if (collision.gameObject.tag == "Player")
        {

            Application.Quit();
            
        }
    }



}