using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCOntroller : MonoBehaviour {

    public Material[] material;
    Renderer rend;
    public bool Activate = false ;
	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[0];
	}
	
	// Update is called once per frame
    IEnumerator XrayVision()
    {
        Activate = false;
        rend.sharedMaterial = material[1];
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            yield return new WaitForSeconds(10);
            rend.sharedMaterial = material[0];
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            
    }
	void Update () {
        if (Activate)
        {
     
            StartCoroutine(XrayVision());
        }

    }
}
