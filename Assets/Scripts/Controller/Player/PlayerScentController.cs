using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScentController : MonoBehaviour {

    // Use this for initialization
    public GameObject Scent;
    public float period;

    void Start()
    {
        InvokeRepeating("DropScent", 0f, period);

    }
    // Update is called once per frame
    void Update()
    {
    }


    void DropScent()
    {
        Vector3 position = GameObject.Find("Player").transform.position;
        Quaternion rotation = Scent.transform.rotation;
        Instantiate(Scent, position, rotation);
    }

}

