using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour {
    public RemoveFirstWall remove;
 //   RemoveFirstWall remove;
	// Use this for initialization
	void Start () {
        remove = GetComponent<RemoveFirstWall>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            remove.enabled= true;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            remove.enabled =false;
        }

    }
}
