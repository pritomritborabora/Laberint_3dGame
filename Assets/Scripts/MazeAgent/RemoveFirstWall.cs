using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveFirstWall : MonoBehaviour {
    GameObject target;
    //Transform target;
    bool verbose = false; 
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
    int openTime = 200;
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
        this.EndGate = eg.transform;
        this.player1 = pl.transform;
        this.enemy1 = en.transform;
        if (Physics.Raycast(this.player1.position, (this.EndGate.position - this.player1.position).normalized, out hit))
        {
            GameObject wall = GameObject.Find(hit.collider.name);
            if (this.verbose) Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!this is the hit.collider object" + wall.name);

            if (hit.collider.name.Contains("Plane"))
            {  //This is if the collider did not find a wall but a Plane
                Transform parent1 = hit.transform.parent;
                if (this.verbose) Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!change parent1 " + parent1.name);
                LightWall = parent1.gameObject;
                int children = LightWall.transform.childCount;
                for (int i = 0; i < children; ++i)
                {
                    light = LightWall.transform.GetChild(i).gameObject;
                    rend = light.GetComponent<Renderer>();
                    rend.sharedMaterial = material;
                }
                Transform parent2 = parent1.parent;
                if (this.verbose) Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!change parent2 " + parent2.name);
                wall = parent2.gameObject;
            }
            if (this.verbose) Debug.DrawRay(this.player1.position, (this.EndGate.position - this.player1.position).normalized * hit.distance, Color.red);
            if (this.verbose) Debug.Log("MazeBT Remove for Player: Did Hit");
            if (this.verbose) Debug.Log(hit.collider.name);

            this.wallName = wall.name;
            if (this.verbose) Debug.Log(wallName);
            //GameObject wallToOpen = GameObject.Find(wallName);
            this.wallToOpen = wall;
            if (this.verbose) Debug.Log("MazeBT First Line is " + wallToOpen);
            start = this.wallToOpen.transform.position; //start position of the wall
            end = this.wallToOpen.transform.position + Vector3.up * this.MoveWallDistance;
            this.action = 0;


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
            if (this.verbose) Debug.Log("------------------------- RemoveWall Opening wall " + wallToOpen.transform.position);
            if ( wallToOpen.transform.position == end){  
                this.action = 2;
                this.waitTime = 0; 
                this.currentLerptime = 0;
                if (this.verbose) Debug.Log("RemoveWall Opening wall finished" );
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
                if (this.verbose) Debug.Log("------------------------- RemoveWall Closing wall " + wallToOpen.transform.position);
                if ( wallToOpen.transform.position == start){  
                    this.action = -1;
                    if (this.verbose) Debug.Log("------------------------- RemoveWall Finished closing wall "); 
                }
            }
        }
        //wallToOpen.transform.position = Vector3.Lerp(start, end, perc);
    }
  }