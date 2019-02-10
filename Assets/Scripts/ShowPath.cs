using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ShowPath : MonoBehaviour {
  //  private NavMeshAgent agent;
    private NavMeshPath path;
    private float elapsed = 0.0f;
    private Color c = Color.white;
    GameObject[] checkpoints;
    GameObject checkpoint;

    FindFloorTrack findtrack = new FindFloorTrack();
    GameObject track;
    GameObject[] points;
    Renderer rend;
    Vector3 surfacePoint1;
    Vector3 surfacePoint2;
    float len;
    Collider collider1;
    Collider collider2;
    bool FoundCheckPoint = false;
    float distance_player_checkpoint_first;
    LineRenderer lr; 
    // Use this for initialization
    void Start () {
    path = new NavMeshPath();
    // agent = GetComponent<NavMeshAgent>();
    collider1 = GetComponent<Collider>();
    checkpoints = GameObject.FindGameObjectsWithTag("checkpoint");
    lr = GetComponent<LineRenderer>();
    
    
       
    lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
    lr.startColor = Color.white;
    lr.endColor = Color.white;
    lr.startWidth = 0.2f;
    lr.endWidth = 0.2f;

  //   NavMesh.CalculatePath(transform.position, checkpoint.transform.position, NavMesh.AllAreas, path);
     len = path.corners.Length;
  //   distance_player_checkpoint_first = Vector3.Distance(transform.position, checkpoint.transform.position);
   
     Debug.Log(path.corners.Length);
    }

    // Update is called once per frames
    void Update()
    {   if (!FoundCheckPoint)
        {
            FindNextCheckPoint();

        }
        lr.enabled = true;
        float distance_player_checkpoint = Vector3.Distance(transform.position, checkpoint.transform.position);
       if(distance_player_checkpoint < 1)
        {
   
           this.enabled = false;
           lr.enabled = false;
           FoundCheckPoint = false;
         //  Destroy(checkpoint);
        }
        NavMesh.CalculatePath(transform.position, checkpoint.transform.position, NavMesh.AllAreas, path);
        //  if (path.corners.Length <= len )//&& distance_player_checkpoint <= distance_player_checkpoint_first)
        // {
        Vector3[] positions = new Vector3[path.corners.Length];
           for (int i = 0; i < path.corners.Length ; i++)
            {
            positions[i] = path.corners[i];

                } 
        lr.positionCount = positions.Length;
        lr.SetPositions(positions);
    }


     void FindNextCheckPoint()
    {
        checkpoint = FindCheckpoint(transform);
        FoundCheckPoint = true;
    }

    public GameObject FindCheckpoint(Transform _player_transform)
    {
        GameObject closest = null;        
        Vector3 position = _player_transform.position;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject checkpoint in checkpoints)
        {
            float distance = (checkpoint.transform.position - position).sqrMagnitude;
            if (distance < shortestDistance)
            {
                closest = checkpoint;
            }
        }
        return closest;
    }
      //  }
    }



