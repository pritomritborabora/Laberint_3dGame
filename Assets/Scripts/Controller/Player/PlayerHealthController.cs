using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerHealthController : MonoBehaviour {

    public PostProcessingProfile defaultProfile;
    public int chasingDistance;
    private float distanceFromEnemy;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        distanceFromEnemy = chasingDistance;
        GameObject[] gos = GameObject.FindGameObjectsWithTag("enemy");

        foreach (GameObject go in gos)
        {
            Vector3 enemyPosition = go.transform.position;
            float dist = Vector3.Distance(enemyPosition, transform.position);
            if (dist < distanceFromEnemy)
            {
                distanceFromEnemy = dist;
            }
        }

        ColorGradingModel.Settings set = defaultProfile.colorGrading.settings;
        float saturation = set.basic.saturation;
        if (distanceFromEnemy < chasingDistance)
        {
            float newSaturation = (distanceFromEnemy/chasingDistance);
            set.basic.saturation = newSaturation;
            defaultProfile.colorGrading.settings = set;
            Camera.main.GetComponent<PostProcessingBehaviour>().profile = defaultProfile;
        }

    }
}
