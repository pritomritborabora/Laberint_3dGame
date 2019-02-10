using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RemoveFirstWallEnemy : MonoBehaviour {
    GameObject target;
    //Transform target;
    EnemyController enemy1control;
    Transform EndGate;
    Transform enemy1;
    Transform player1;
    GameObject wall;
    string wallName;
    Vector3 start;
    Vector3 end;
    string planeName;
    GameObject Light;
    GameObject light;
    RaycastHit hit;
    public Material material;
    GameObject LightWall;
    private float MoveWallDistance = 3f;
    private float lerptime = 2;
    private float currentLerptime = 0;
    float distance_player_wall;
    bool isCoroutineStarted = false;
    public bool FoundWall = false;
    Renderer rend;
    GameObject wallToOpen;
    int openTime = 500;
    int waitTime = 0;
    int action = -1; // -1 none, 0 for removing, 1 for placing it back
    // Use this for initialization
    void Start()
    {
      
        FoundWall = false;
        
    }
            // Update is called once per frame
      void Update () {
   
        if (!FoundWall)
        {
            FindWall();
            FoundWall = true;
        }
       /*if(this.wallToOpen.transform.position != end)
        {
            MoveWall();
        }
        if (this.wallToOpen.transform.position == end)
        {
            FoundWall = false;
            this.enabled = false;
        } */
        if (this.action != -1){
             MoveWall();
        }
        if (this.action == -1 ){
            FoundWall = false;
            this.enabled = false;
        }



    }

    IEnumerator RemoveWall()
    {
      
        this.currentLerptime += Time.deltaTime;
        if (this.currentLerptime >= this.lerptime)
        {
            this.currentLerptime = this.lerptime;
        }
        float perc = this.currentLerptime / this.lerptime;
        this.wallToOpen.transform.position = Vector3.Lerp(this.start, this.end, perc);
        yield return null;
    }
    void FindWall()
    {
        this.currentLerptime = 0;
        GameObject eg = GameObject.Find("End");
        GameObject pl = GameObject.Find("Player");
        GameObject en = GameObject.Find("Enemy");
        this.enemy1control =  en.GetComponent<EnemyController>();
        this.EndGate = eg.transform;
        this.player1 = pl.transform;
        this.enemy1 = en.transform;
        if (Physics.Raycast(this.enemy1.position, (this.player1.position - this.enemy1.position).normalized, out hit))
        {
            GameObject wall = GameObject.Find(hit.collider.name);
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!this is the hit.collider object" + wall.name);

            if (hit.collider.name.Contains("Plane"))
            {  //This is if the collider did not find a wall but a Plane
                Transform parent1 = hit.transform.parent;
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!change parent1 " + parent1.name);
                LightWall = parent1.gameObject;
                int children = LightWall.transform.childCount;
                for (int i = 0; i < children; ++i)
                {
                    light = LightWall.transform.GetChild(i).gameObject;
                    rend = light.GetComponent<Renderer>();
                    rend.sharedMaterial = material;
                }
                Transform parent2 = parent1.parent;
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!change parent2 " + parent2.name);
                wall = parent2.gameObject;
            }
            Debug.DrawRay(this.enemy1.position, (this.player1.position - this.enemy1.position).normalized * hit.distance, Color.red);
            Debug.Log("MazeBT Remove for Player: Did Hit");
            Debug.Log(hit.collider.name);

            this.wallName = wall.name;
            Debug.Log(wallName);
            //GameObject wallToOpen = GameObject.Find(wallName);
            this.wallToOpen = wall;
            Debug.Log("MazeBT First Line is " + this.wallToOpen);
            this.start = this.wallToOpen.transform.position; //start position of the wall
            this.end = this.wallToOpen.transform.position + Vector3.up * this.MoveWallDistance;
            this.action = 0;
            this.enemy1control.wallIsRemoved = true;
            this.enemy1control.wallRemovedId = this.wallToOpen;
            //Debug.Break();

        }
    }
    void MoveWall()
    {
        this.currentLerptime += Time.deltaTime;
        if (this.currentLerptime >= this.lerptime)
        {
            this.currentLerptime = this.lerptime;
        }
        float perc = 0;
        if (this.action != 2) perc = this.currentLerptime / this.lerptime;

        if (this.action == 0) {
            wallToOpen.transform.position = Vector3.Lerp(start, end, perc);
            //Debug.Log("------------------------- RemoveWall Opening wall for Enemy" + wallToOpen.transform.position);
            if ( wallToOpen.transform.position == end){  
                this.action = 2; 
                this.currentLerptime = 0;
                this.waitTime = 0;
                //Debug.Log("RemoveWall Opening wall for Enemy finished" );
                // Debug.Break();
            }
        }
        else{
            if (this.action == 2) {
                //Debug.Log("-----------------------  RemoveWall waiting" );
                this.waitTime += 1;
                if(this.waitTime > this.openTime){
                    this.currentLerptime = 0;
                    this.action = 1;
                    //Debug.Log("-----------------------  RemoveWall finish waiting" );
                }
            } 
            if (this.action == 1) {
                wallToOpen.transform.position = Vector3.Lerp(start, end, 1 - perc);
                //Debug.Log("------------------------- RemoveWall Closing wall For Enmy" + wallToOpen.transform.position);
                if ( wallToOpen.transform.position == start){  
                    this.action = -1;
                    this.enemy1control.wallIsRemoved = false;
                    this.enemy1control.wallRemovedId = null;
                    //Debug.Log("------------------------- RemoveWall Finished closing wall Enemy "); 
                }
            }
        }
        //wallToOpen.transform.position = Vector3.Lerp(start, end, perc);
    }
  }