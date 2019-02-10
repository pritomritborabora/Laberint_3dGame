using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceWallback : MonoBehaviour
{
    GameObject target;
    //Transform target;
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
    Renderer rend;
    GameObject wallToOpen;
    // Use this for initialization
    void Start()
    {
        GameObject eg = GameObject.Find("End");
        GameObject pl = GameObject.Find("Player");
        GameObject en = GameObject.Find("Enemy");
        this.EndGate = eg.transform;
        this.player1 = pl.transform;
        this.enemy1 = en.transform;
        if (Physics.Raycast(this.player1.position, (this.EndGate.position - this.player1.position).normalized, out hit))
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
            Debug.DrawRay(this.player1.position, (this.EndGate.position - this.player1.position).normalized * hit.distance, Color.red);
            Debug.Log("MazeBT Remove for Player: Did Hit");
            Debug.Log(hit.collider.name);

            this.wallName = wall.name;
            Debug.Log(wallName);
            //GameObject wallToOpen = GameObject.Find(wallName);
            this.wallToOpen = wall;
            Debug.Log("MazeBT First Line is " + wallToOpen);
            start = this.wallToOpen.transform.position; //start position of the wall
            end = this.wallToOpen.transform.position + Vector3.down * this.MoveWallDistance;

       
        }
    }
    // Update is called once per frame
    void Update()
    {
        this.currentLerptime += Time.deltaTime;
        if (this.currentLerptime >= this.lerptime)
        {
            this.currentLerptime = this.lerptime;
        }
        float perc = this.currentLerptime / this.lerptime;
        wallToOpen.transform.position = Vector3.Lerp(start, end, perc);
    }
}
