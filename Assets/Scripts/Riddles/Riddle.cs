using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// Riddle class for the riddles. 
public class Riddle : MonoBehaviour {		
	public string riddleText;
	public string answer;
    public string rewardText;
    public string hint;    
    public string riddleType;    
    public bool timetoMove = false;    

    private bool solved = false;
    private TMP_FontAsset defaultFont;
    private Material DefaultFontMaterial;
    private float defaultFontSize;
    private Vector3 start;    
    private Vector3 end;    
    private float lerptime = 4;
    private float currentLerptime = 0;
    private float MoveWallDistance = 3f;        
    private TextMeshPro _TextMesh;    
    
    void Start() {
        riddleType = this.name;
        _TextMesh = transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        defaultFont = _TextMesh.font;
        DefaultFontMaterial = _TextMesh.fontSharedMaterial;
        defaultFontSize = _TextMesh.fontSize;        
    }

    void Update() {		
        if (timetoMove)
        {   
            MoveRiddleWall();
            if (transform.position == end)
            {
                timetoMove = false;
            }
        }
	}

    public bool Solved
    {
        get 
        {
            return solved;
        }
        set 
        {
            solved = value;
            SetRiddleText("Solved", 10);
        }
    }

	public void SetRiddleText(string text, float fontSize) {
        _TextMesh.text = text;
        _TextMesh.fontSize = fontSize;
        _TextMesh.font = defaultFont;
        _TextMesh.fontSharedMaterial = DefaultFontMaterial;
    }

    private void MoveRiddleWall()
    {
        currentLerptime += Time.deltaTime;        
        if (currentLerptime >= lerptime)
        {
            currentLerptime = lerptime;
        }
        transform.position = Vector3.Lerp(
            transform.position, 
            transform.position + Vector3.up * MoveWallDistance, 
            currentLerptime / lerptime
        );
    }
}
