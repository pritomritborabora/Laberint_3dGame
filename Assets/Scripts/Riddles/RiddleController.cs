using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RiddleController : MonoBehaviour
{
    public float maxRiddleDistance = 10.0f;
    public float _RiddleDistanceThreshold = 3.0f;
    public Material material;
    
    private GameObject[] riddles;

    void Start()
    {
        riddles = GameObject.FindGameObjectsWithTag("riddle");
    }

    void Update()
    {
        if (closestRiddleObject != null) AnimateRiddle(closestRiddleObject, transform);
    }

    private float CalcRiddleDistance(GameObject riddle, Transform _transform)
    {
        return (riddle.transform.position - _transform.position).sqrMagnitude;
    }

    private GameObject closestRiddleObject 
    {
        get 
        {
            return FindClosestRiddle(transform, riddles);
        }
    }

    public Riddle closestRiddle
    {
        get
        {
            if (closestRiddleObject && CalcRiddleDistance(closestRiddleObject, transform) <= _RiddleDistanceThreshold)
            {
                return closestRiddleObject.GetComponent<Riddle>();
            }
            else
            {
                return null;
            }
        }
    }

    public float getClosestRiddleDistance()
    {
        return Vector3.Distance(transform.position, closestRiddleObject.transform.position);
    }

    public bool closestRiddleSolved()
    {
        return closestRiddle.Solved;
    }

    public GameObject FindClosestRiddle(Transform _player_transform, GameObject[] riddles)
    {
        float riddleDistance;
        GameObject closestRiddle = null;
        float _closestRiddleDistance = Mathf.Infinity;

        foreach (GameObject riddle in riddles)
        {
            riddleDistance = CalcRiddleDistance(riddle, _player_transform);

            if (riddleDistance < _closestRiddleDistance)
            {
                _closestRiddleDistance = riddleDistance;
                closestRiddle = riddle;
            }
        }
        return closestRiddle;
    }

    void AnimateRiddle(GameObject riddleObj, Transform _playerTransform)
    {
        Animator anim = riddleObj.GetComponent<Animator>();
        Riddle riddle = riddleObj.GetComponent<Riddle>();
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float distance = CalcRiddleDistance(riddleObj, _playerTransform);

        if (distance < _RiddleDistanceThreshold && !riddle.Solved)
        {
            anim.SetInteger("moveback", 0);
            anim.SetInteger("appear", 1);
            info = anim.GetCurrentAnimatorStateInfo(0);
            if (riddle.name != "door")
            {
                Renderer rend = riddleObj.GetComponent<Renderer>();
                rend.sharedMaterial = material;
            }
            if (info.IsName("end") == true) DisplayRiddleText(riddleObj, true);
        }
        if (info.IsName("end") == true && distance > _RiddleDistanceThreshold)
        {
            anim.SetInteger("moveback", 1);
            anim.SetInteger("appear", 0);
            DisplayRiddleText(riddleObj, false);
        }
    }

    void DisplayRiddleText(GameObject riddle, bool active)
    {
        riddle.transform.GetChild(0).gameObject.GetComponent<TypeTextEffect>().enabled = active;
        riddle.transform.GetChild(0).gameObject.SetActive(active);
    }
    // Solve riddles by matching the player voice input to the riddle answers. If Correct, make the rewards available for the player.
    public bool SolveClosestRiddle(string userAnswer)
    {             
        bool justSolved = false;
        if (closestRiddle && closestRiddle.Solved == false && 
            userAnswer.ToLower().Contains(closestRiddle.answer.ToLower())) 
        {
            closestRiddle.Solved = true;
            justSolved = true;
        }         
        return justSolved;
    }
}