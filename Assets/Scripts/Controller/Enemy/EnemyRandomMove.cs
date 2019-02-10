using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRandomMove : MonoBehaviour {
    public float roamRadius;
    UnityEngine.AI.NavMeshAgent agent;
    Animator anim;
    // Use this for initialization
    void Start () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {
        anim.SetInteger("Condition", 1);
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position*10;
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
        Vector3 finalPosition = hit.position;
        agent.SetDestination(finalPosition);
    }
}
