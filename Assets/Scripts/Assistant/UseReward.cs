using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The script is used to implement the voice commands for the rewards obtained by solving the riddles.
public class UseReward : MonoBehaviour {
    public int navigationhelpCounter = 0;
    public int XrayhelpCounter = 0;
    public string navigateKeyword = "navigate";
    public string xRayKeyword = "x. ray";
    public string distanceKeyword = "distance";
    public string[] keyWords;
    
    private AudioSource[] audioData;
    private GameObject[] riddles;
    private GameObject enemy;
    private ShowPath path;
    private GameObject end;
    private GameObject begin;
    private RiddleController _riddle_ctrl;
    private Riddle riddle;
    private GameObject[] TwoSidedWall;
    private AssistantAgent Assistant;
    private MaterialCOntroller _materialController;
    private GameObject[] portals;
    
    // Use this for initialization
    void Start () {
        keyWords = new string[] { navigateKeyword, xRayKeyword, distanceKeyword };
        audioData = GetComponents<AudioSource>();
        riddles = GameObject.FindGameObjectsWithTag("riddle");
        path = GetComponent<ShowPath>();
        enemy = GameObject.Find("Enemy");
        end = GameObject.Find("End");
        begin = GameObject.Find("Begin");
        _riddle_ctrl = GetComponent<RiddleController>();
        Assistant = GetComponent<AssistantAgent>();
        TwoSidedWall = GameObject.FindGameObjectsWithTag("insidewall");
    }

    public void GenerateReward(Riddle riddle)
    {
        switch (riddle.riddleType)
        {
            case "portal":                
                activatePortals();
                break;
            case "navigation":                         
                navigationhelpCounter = navigationhelpCounter + 1;
                break;
            case "xray":                
                XrayhelpCounter = XrayhelpCounter + 1;
                break;
            case "door":
                riddle.timetoMove = true;
                break;
        }
    }

    public void  activatePortals()
    {
        GameObject portal = GameObject.FindGameObjectsWithTag("portal")[0];
        portal.transform.GetChild(0).gameObject.SetActive(true);
        portal.transform.GetChild(3).gameObject.SetActive(true);
    }

    public void Use(string playerQuery)
    {
        if (playerQuery.ToLower().Contains(navigateKeyword)) // Navigation help 
        {
            if (navigationhelpCounter > 0)
            {
                //  StartCoroutine(LowerThemeVolume(audioData[2]));
                audioData[1].Play();
                path.enabled = true;
                navigationhelpCounter = navigationhelpCounter - 1;
            }
            else
            {
                //StartCoroutine(LowerThemeVolume(audioData[6]));
                audioData[2].Play();
            }
        }
        if (playerQuery.ToLower().Contains(xRayKeyword))  // See through walls or Xray walls help
        {
            if (XrayhelpCounter > 0)
                foreach (GameObject gos in TwoSidedWall)
                {
                    _materialController = gos.GetComponent<MaterialCOntroller>();
                    _materialController.Activate = true;
                    XrayhelpCounter = XrayhelpCounter - 1;
                }
            else Assistant.SetResultFieldText("No X-Ray vision help available");
        }        
        if (playerQuery.ToLower().Contains(distanceKeyword))
        {
            string distance_to_goal = Vector3.Distance(transform.position, end.transform.position).ToString();
            string distance_to_begin = Vector3.Distance(transform.position, begin.transform.position).ToString() ;
            string distance_text = "Distance to goal : " + distance_to_goal + "       Distance Covered : " + distance_to_begin;            
            Assistant.SetResultFieldText(distance_text);
        }
    }
    IEnumerator LowerThemeVolume(AudioSource audio)
     {
         AudioClip clip = audio.clip;
         float time = clip.length;
         if (audioData[7])
         {
            audioData[7].volume = 0.027f;
            yield return new WaitForSeconds(time);
             audioData[7].volume = 0.123f;
         }
     }
 }

