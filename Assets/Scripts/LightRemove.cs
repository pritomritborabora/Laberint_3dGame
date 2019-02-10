using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRemove : MonoBehaviour {
    GameObject player;
    Vector3 start;
    Vector3 end;
    private float Move_distance = 4f;
    private float lerptime = 3;
    private float currentLerptime = 0;
    private Vector3 wallDir;
    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        start = transform.position;
        end  = transform.position + Vector3.right * Move_distance;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        wallDir = (transform.position - player.transform.position);
        float angle = Vector3.Dot(player.transform.forward, wallDir);
        //    if (angle > 0.8)
        //     {
        if (distance < 4)
        {
            currentLerptime += Time.deltaTime; ;
            if (currentLerptime >= lerptime)
            {
                currentLerptime = lerptime;
            }
            float perc = currentLerptime / lerptime;
            transform.position = Vector3.Lerp(start, end, perc);

        }
    //    if (transform.position == end)
      //  {
      //      currentLerptime = 0;

            if (distance > 7 && transform.position!= start)
            {

                currentLerptime += Time.deltaTime; ;
                if (currentLerptime >= lerptime)
                {
                    currentLerptime = lerptime;
                }
                float perc = currentLerptime / lerptime;
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.left * Move_distance, perc);

                //   }
            
        }
    }
}
