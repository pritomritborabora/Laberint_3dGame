using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindEnemyPoints : MonoBehaviour {

    public GameObject FindSmell(GameObject[] gos , Transform transform)
    {

       // Transform player;
        //gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        //    player = PlayerManager.instance.player.transform;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
}

