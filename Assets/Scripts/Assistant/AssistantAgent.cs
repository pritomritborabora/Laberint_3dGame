using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class AssistantAgent : MonoBehaviour
{
    // STT Config
    public string usernameSTT;
    public string passwordSTT;
    public string urlSTT;

    // Chatbot Config
    public string usernameTone;
    public string passwordTone;
    public string urlTone;
    public string urlChat;    
    
    public string communicatingResponse = "....communicating....awaiting connection from shuttle....";
    
    // Internal variables
    private RiddleController _riddleCtrl;
    private SpeechToTextController _sttCtrl;
    private UseReward _useReward;
    private ChatBotController _chatCtrl;
    private AudioClip recording;
    private float startRecordingTime;
    private string[] _helpWords = {"tell me more", "help"};
    private bool awaitingResponse = false;

    public TextMeshProUGUI ResultsField;
    GameObject panel;
    HelperTextTyping chatText;

    void Start()
    {
        _sttCtrl = new SpeechToTextController(usernameSTT, passwordSTT, urlSTT);
        _sttCtrl.Start(OnSTTResult);

        _chatCtrl = new ChatBotController(usernameTone, passwordTone, urlTone, urlChat);
        _chatCtrl.Start(OnChatbotReply);
        
        _riddleCtrl = GetComponent<RiddleController>();

        panel = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        ResultsField = panel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        chatText = panel.transform.GetChild(0).gameObject.GetComponent<HelperTextTyping>();
        _useReward = GetComponent<UseReward>();

        SetResultFieldText("...hello...can you hear me?...hold down left control and speak into the mic to answer...");
    }

    void Update() 
    {
        if (awaitingResponse == true) SetResultFieldText(communicatingResponse);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SetResultFieldText(communicatingResponse);
            awaitingResponse = true;
            StartRecording();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl)) StopRecording();
    }

    public void StopRecording()
    {
        //End the recording when the mouse comes back up, then play it
        Microphone.End("");
        Debug.Log("Microphone end");
        //Trim the audioclip by the length of the recording
        float timeelapsed = Time.time - startRecordingTime;
        if (timeelapsed > 1.0f) 
        {   
            Debug.Log("Creating audio clip from mic");
            AudioClip recordingNew = AudioClip.Create(recording.name, (int)((Time.time - startRecordingTime) * recording.frequency), recording.channels, recording.frequency, false);
            float[] data = new float[(int)((Time.time - startRecordingTime) * recording.frequency)];
            recording.GetData(data, 0);
            recordingNew.SetData(data, 0);
            Debug.Log("Processing audio");
            _sttCtrl.ProcessAudio(recordingNew);
        } 
        else 
        {
            SetResultFieldText("...");
            awaitingResponse = false;
        }       
    }

    public void StartRecording()
    {
        Debug.Log("Recording audio");
        //Get the max frequency of a microphone, if it's less than 44100 record at the max frequency, else record at 44100
        int minFreq;
        int maxFreq;
        int freq = 44100;
        Microphone.GetDeviceCaps("", out minFreq, out maxFreq);
        
        if (maxFreq < 44100) freq = maxFreq;

        //Start the recording, the length of 300 gives it a cap of 5 minutes
        recording = Microphone.Start("", false, 300, 44100);
        startRecordingTime = Time.time;
    }

    private void OnSTTResult(string result)
    {                   
        Debug.Log("Handling STT Result " + result);
        awaitingResponse = false;
        Riddle closestRiddle = _riddleCtrl.closestRiddle;
        string displayText = null;

        if (closestRiddle && ArrayContains(result, _helpWords))
        {               
            Debug.Log("Launching Hint");
            displayText = closestRiddle.hint;            
        } 
        else if (closestRiddle && _riddleCtrl.SolveClosestRiddle(result))
        {
            Debug.Log("Solved Riddle");                        
            _useReward.GenerateReward(closestRiddle);
            displayText = closestRiddle.rewardText;            
        } 
        else if (ArrayContains(result, _useReward.keyWords))
        {
            Debug.Log("Using Reward");
            _useReward.Use(result);
            displayText = "This should come in handy";
        }
        
        if (displayText != null) 
        {
            Debug.Log("Displaying text");
            SetResultFieldText(displayText);  
        }        
        else
        {               
            Debug.Log("Sending to chat");
            _chatCtrl.SendMessage(result);
        }        
    }

    private bool ArrayContains(string result, string[] array) 
    {
        bool contains = false;
        
        foreach (var word in array)
        {
            if (result.ToLower().Contains(word)) contains = true;
        }

        return contains;
    }

    private void OnChatbotReply(string reply) 
    {        
        SetResultFieldText("..." + reply);
    }

    public void SetResultFieldText(string text)
    {
        ResultsField.text = text;
        chatText.enabled = true;
    }
}