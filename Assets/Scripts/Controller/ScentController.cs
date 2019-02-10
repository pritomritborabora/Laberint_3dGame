using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentController : MonoBehaviour
{


    public int intensity;
    public int intensityRate;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("LowerIntensity", 2f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (intensity <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void LowerIntensity()
    {
        intensity = intensity - intensityRate;
    }
}
