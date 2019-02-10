using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScentController : MonoBehaviour {

    
    // Use this for initialization
   private GameObject[] points;
    GameObject EnemysmellPoints;
    GameObject prev;
    float Distance;
    bool isfirstCoroutineStarted = false;
    bool isSecondCoroutineStarted = false;
    FindEnemyPoints findSmell = new FindEnemyPoints();
    void Start()
    {
        points = GameObject.FindGameObjectsWithTag("EnemySmell");
        foreach (GameObject go in points)
        {
            go.SetActive(false);
        }

    }
    // Update is called once per frame
    void Update()
    {
        EnemysmellPoints = findSmell.FindSmell(points, transform);
        if (EnemysmellPoints == prev)
        {
            EnemysmellPoints = findSmell.FindSmell(points,transform);
        }
        prev = EnemysmellPoints;

        Distance = Vector3.Distance(EnemysmellPoints.transform.position, transform.position);
        if (Distance < 2)
        {
            if (!isfirstCoroutineStarted)
            {
                StartCoroutine(ActiveEnemySmell1(EnemysmellPoints));
            }

            else //if (!isSecondCoroutineStarted)
            {
                StartCoroutine(ActiveEnemySmell2(EnemysmellPoints));
            }
        }
    }
    IEnumerator ActiveEnemySmell1(GameObject EnemysmellPoints)
    {
        isfirstCoroutineStarted = true;

        EnemysmellPoints.SetActive(true);
        yield return new WaitForSeconds(10);
        EnemysmellPoints.SetActive(false);
        isfirstCoroutineStarted = false;
    }

    IEnumerator ActiveEnemySmell2(GameObject EnemysmellPoints)
    {

        isSecondCoroutineStarted = true;

        EnemysmellPoints.SetActive(true);
        yield return new WaitForSeconds(10);
        EnemysmellPoints.SetActive(false);
        isSecondCoroutineStarted = false;
    }
}
