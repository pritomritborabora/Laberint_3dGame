using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Connection;

public class SpeechToTextController 
{
    public delegate void ResultHandler(string result);
    public bool StreamMultipart = false;
    
	private SpeechToText _speechToText;	    	
	private ResultHandler _result_handler;
    private float _confidenceThreshold = 0.1f;
    private int _recordingRoutine = 0;
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingBufferSize = 1;
    private int _recordingHZ = 22050;
	private bool DetectSilence = true;
	private bool EnableWordConfidence = true;
	private bool EnableTimestamps = true;
	private float SilenceThreshold = 0.01f;
	private int MaxAlternatives = 0;
	private bool EnableInterimResults = true;
	private int InactivityTimeout = -1;
	private bool ProfanityFilter = false;
	private bool SmartFormatting = true;
	private bool SpeakerLabels = false;

    public SpeechToTextController(string _username_STT, string _password_STT, string _url_STT) {
		Credentials credentials_STT = new Credentials(_username_STT, _password_STT, _url_STT);
        _speechToText = new SpeechToText(credentials_STT);
        _speechToText.StreamMultipart = StreamMultipart;
        _speechToText.DetectSilence = false;
        _speechToText.SilenceThreshold = 0;
        _speechToText.EnableWordConfidence = EnableWordConfidence;
        _speechToText.EnableTimestamps = EnableTimestamps;
        _speechToText.SilenceThreshold = SilenceThreshold;
        _speechToText.MaxAlternatives = MaxAlternatives;
        _speechToText.EnableInterimResults = EnableInterimResults;
        _speechToText.OnError = OnError;
        _speechToText.InactivityTimeout = InactivityTimeout;
        _speechToText.Keywords = new string[]{"help"}; 
        _speechToText.KeywordsThreshold = 0.1f;
        _speechToText.WordAlternativesThreshold = null;
        _speechToText.ProfanityFilter = ProfanityFilter;
        _speechToText.SmartFormatting = SmartFormatting;
        _speechToText.SpeakerLabels = SpeakerLabels;        
	}
	
	public void Start(ResultHandler handler) {
		_result_handler = handler;
        
        if (StreamMultipart == true) 
        {
            _speechToText.StartListening(OnSttResult, null);
            StartRecording();
        }          
    }

    public void ProcessAudio(AudioClip clip) 
    {               
        Debug.Log("Sending to IBM STT");
        _speechToText.Recognize(OnSttResult, OnFail, clip, null);        
    }

    public void OnFail(RESTConnector.Error error, Dictionary<string, object> customData) 
    {
        Debug.Log("STT Error");
        Debug.Log(error);
    }

    private void StartRecording()
    {
        if (_recordingRoutine == 0)
        {
            Debug.Log("STARTING RECORDING");
            UnityObjectUtil.StartDestroyQueue();
            _recordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    private void StopRecording()
    {
        if (_recordingRoutine != 0)
        {
            Microphone.End(_microphoneID);
            Runnable.Stop(_recordingRoutine);
            _recordingRoutine = 0;
        }
    }

    private void OnError(string error)
    {
        _speechToText.StopListening();
        StopRecording();
        Debug.Log("ExampleStreaming.OnError(): " + error);
    }

    private IEnumerator RecordingHandler()
    {
        Debug.Log("ExampleStreaming.RecordingHandler()" + "devices: {0}" + Microphone.devices);
        _recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);    
        yield return null;      
        if (_recording == null)
        {
            StopRecording();
            yield break;
        }
        bool bFirstBlock = true;
        int midPoint = _recording.samples / 2;
        float[] samples = null;

        while (_recordingRoutine != 0 && _recording != null)
        {
            int writePos = Microphone.GetPosition(_microphoneID);
            if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
            {
                Debug.Log("ExampleStreaming.RecordingHandler()" + "Microphone disconnected.");
                StopRecording();
                yield break;
            }
            if ((bFirstBlock && writePos >= midPoint) || (!bFirstBlock && writePos < midPoint))
            {                
                Debug.Log("Creating audio");
                samples = new float[midPoint];
                _recording.GetData(samples, bFirstBlock ? 0 : midPoint);
                AudioData record = new AudioData();
                record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
                record.Clip.SetData(samples, 0);   
                _speechToText.OnListen(record);             
                bFirstBlock = !bFirstBlock;
            }
            else
            {
                Debug.Log("Waiting");
                int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
                float timeRemaining = (float)remaining / (float)_recordingHZ;
                yield return new WaitForSeconds(timeRemaining);
            }
        }
        yield break;
    }

	private void OnSttResult(SpeechRecognitionEvent recognitionEvent, Dictionary<string, object> customData )
    {
        Debug.Log("STT Result");        
        if (recognitionEvent != null)  //&& recognitionEvent.HasFinalResult()
        {               
            foreach (var result in recognitionEvent.results)
            {        
                Debug.Log("Temp result");           
                Debug.Log(result);
                
                if (result.final) 
                {
                    Debug.Log("Final Result");
                    string transcript = "";
                    double maxConfidence = 0;
                    
                    foreach (var alt in result.alternatives)
                    {                        
                        if (alt.confidence >= maxConfidence) {
                            transcript = alt.transcript;
                            maxConfidence = alt.confidence;
                        }
                    }
                    if (maxConfidence > _confidenceThreshold) {
                        Debug.Log("Transcript" + transcript);
                        _result_handler(transcript);
                    }                    
                }                
            }
        }
    }
}
