using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Connection;

public class JsonChatData
{
	public string[] context;
	public string emotion;
}

public class JsonChatResponse
{
	public string response;
}

public class ChatBotController
{    
    public delegate void ResultHandler(string result);

    private ResultHandler _chatbotResponseHandler;
    private ToneAnalyzer _toneAnalyzer;
    private string _urlChat;
    private string[] _chatbotAllowedEmotions = {"anger", "fear", "joy", "sadness"};
    private string _chatbotDefaultEmotion = "neutral";
    private string _versionTone = "2018-12-12"; 
    private float _emotionConfidenceThreshold = 0.5f;   
    private string _currentUserQuery = "";
    private string _currentUserEmotion = "";

    public ChatBotController(string usernameTone, string passwordTone, string urlTone, string urlChat) {
        _urlChat = urlChat;
        _toneAnalyzer = new ToneAnalyzer(new Credentials(usernameTone, passwordTone, urlTone)) {
            VersionDate = _versionTone
        };        
    }

    public void Start(ResultHandler handler) {
        _chatbotResponseHandler = handler;
    }

    public void SendMessage(string message) {
        _currentUserQuery = message;
        _toneAnalyzer.GetToneAnalyze(OnGetToneAnalyze, OnToneFail, message);
    }

    private void OnGetToneAnalyze(ToneAnalysis resp, Dictionary<string, object> customData)
    {
        Debug.Log("Tone analysis:");
        _currentUserEmotion = FilterTopEmotion(resp.DocumentTone.Tones);
        Debug.Log(_currentUserEmotion);
        JsonChatData data = new JsonChatData();
        data.context =  new string[1];
        data.context[0] = _currentUserQuery;
        data.emotion = _currentUserEmotion;         
        Runnable.Run(PostRequest(_urlChat, JsonUtility.ToJson(data)));
        // anger, fear, joy, and sadness (emotional tones); analytical, confident, and tentative
        // {"document_tone":{"tones":[{"score":1.0,"tone_id":"joy","tone_name":"Joy"},{"score":0.97759,"tone_id":"confident","tone_name":"Confident"}]}}
    }

    private string FilterTopEmotion(List<ToneScore> toneScores)
    {
        string emotion = _chatbotDefaultEmotion;
        float maxScore = _emotionConfidenceThreshold;

        foreach (var tone in toneScores)
        {
            float score = (float)tone.Score;
            if (score >= maxScore && _chatbotAllowedEmotions.Contains(tone.ToneId)) {
                maxScore = (float)tone.Score;
                emotion = tone.ToneId;
            }
        }

        return emotion;
    }

    private IEnumerator PostRequest(string url, string json)
    {
        UnityWebRequest uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError) OnChatFail(uwr);
        else 
        {
            Debug.Log("Chatbot reply" + uwr.downloadHandler.text);
            JsonChatResponse data = JsonUtility.FromJson<JsonChatResponse>(uwr.downloadHandler.text);
            _chatbotResponseHandler(data.response); 
        }         
    }

    private void OnChatFail(UnityWebRequest uwr) {
        Debug.Log("Error While Sending message to ChatBot: " + uwr.error);
    }

    private void OnToneFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Debug.Log("Error analyzing tone" + error.ToString());
    }
}
