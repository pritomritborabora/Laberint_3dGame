// /**
// * Copyright 2015 IBM Corp. All Rights Reserved.
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *      http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// *
// */

// using UnityEngine;
// using System.Collections;
// using IBM.Watson.DeveloperCloud.Logging;
// using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
// using IBM.Watson.DeveloperCloud.Utilities;
// using IBM.Watson.DeveloperCloud.DataTypes;
// using System.Collections.Generic;
// using UnityEngine.UI;
// using UnityEngine.AI;
// using TMPro;
// public class ExampleStreaming : MonoBehaviour
// {
//     #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
//     [Space(10)]
//     [Tooltip("The service URL (optional). This defaults to \"https://stream.watsonplatform.net/speech-to-text/api\"")]
//     [SerializeField]
//     private string _serviceUrl;
//     [Tooltip("Text field to display the results of streaming.")]
//     public Text ResultsField;
//     [Header("CF Authentication")]
//     [Tooltip("The authentication username.")]
//     [SerializeField]
//     private string _username;
//     [Tooltip("The authentication password.")]
//     [SerializeField]
//     private string _password;
//     [Header("IAM Authentication")]
//     [Tooltip("The IAM apikey.")]
//     [SerializeField]
//     private string _iamApikey;
//     [Tooltip("The IAM url used to authenticate the apikey (optional). This defaults to \"https://iam.bluemix.net/identity/token\".")]
//     [SerializeField]
//     public string _IamAccessToken;
//     [SerializeField]
//     private string _iamUrl;

//     [Header("Parameters")]
//     // https://www.ibm.com/watson/developercloud/speech-to-text/api/v1/curl.html?curl#get-model
//     [Tooltip("The Model to use. This defaults to en-US_BroadbandModel")]
//     [SerializeField]
//     private string _recognizeModel;
//     #endregion

//     //   public Text ResultsField;
//     private int _recordingRoutine = 0;
//     private string _microphoneID = null;
//     private AudioClip _recording = null;
//     private int _recordingBufferSize = 1;
//     private int _recordingHZ = 22050;
//     AudioSource[] audioData;
//     AudioSource theme;
//     AudioSource TempAudioSpurce;
//     // GameObject[] points;
//     ShowPath path;
//     private SpeechToText _service;
//     Animator anim;
//     GameObject enemy;
//     private NavMeshAgent agent;
//     GameObject wall;
//     GameObject[] riddles;
//     GameObject ClosestRiddle;
//     int navigation_help_counter = 3;
//     private TextMeshPro textmesh;
//     GameObject[] portals;
//     GameObject portal;
//     GameObject end;
//     GameObject begin;
//     public TMP_FontAsset BangersSDF;
//     public Material BangersSDFMaterial; 


//     void Start()
//     {
//         LogSystem.InstallDefaultReactors();
//         Runnable.Run(CreateService());
//         audioData = GetComponents<AudioSource>();
//         riddles = GameObject.FindGameObjectsWithTag("riddle");
//         path = GetComponent<ShowPath>();
//         enemy = GameObject.Find("Enemy");
//         end = GameObject.Find("End");
//         begin = GameObject.Find("Begin");
//         //  portal = GameObject.Find("Portal");
//         portals = GameObject.FindGameObjectsWithTag("portal");
//         foreach (GameObject gos in portals)
//         {
//             gos.SetActive(false);
//         }
//         //  portal.SetActive(false);
//         //  wall = GameObject.Find(("TwoSided(4)"));
//         anim = enemy.GetComponent<Animator>();
//         agent = enemy.GetComponent<NavMeshAgent>();
//         //     points = GameObject.FindGameObjectsWithTag("floortrack");
//         //      foreach (GameObject go in points)
//         //       {
//         //        go.SetActive(false);
//         //      }
//     }

//     private IEnumerator CreateService()
//     {
//         //  Create credential and instantiate service
//         Credentials credentials = null;
//         if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
//         {
//             //  Authenticate using username and password
//             credentials = new Credentials(_username, _password, _serviceUrl);
//         }
//         else if (!string.IsNullOrEmpty(_iamApikey))
//         {
//             //  Authenticate using iamApikey
//             TokenOptions tokenOptions = new TokenOptions()
//             {
//                 IamApiKey = _iamApikey,
//                 //   IamUrl = _iamUrl
//                 IamAccessToken = _IamAccessToken

//             };

//             credentials = new Credentials(tokenOptions, _serviceUrl);

//             //  Wait for tokendata
//             while (!credentials.HasIamTokenData())
//                 yield return null;
//         }
//         else
//         {
//             throw new WatsonException("Please provide either username and password or IAM apikey to authenticate the service.");
//         }

//         _service = new SpeechToText(credentials);
//         _service.StreamMultipart = true;

//         Active = true;
//         StartRecording();
//     }

//     public bool Active
//     {
//         get { return _service.IsListening; }
//         set
//         {
//             if (value && !_service.IsListening)
//             {
//                 _service.RecognizeModel = (string.IsNullOrEmpty(_recognizeModel) ? "en-US_BroadbandModel" : _recognizeModel);
//                 _service.DetectSilence = false;
//                 _service.EnableWordConfidence = true;
//                 _service.EnableTimestamps = true;
//                 _service.SilenceThreshold = 0.1f;
//                 _service.MaxAlternatives = 0;
//                 _service.EnableInterimResults = true;
//                 _service.OnError = OnError;
//                 _service.InactivityTimeout = -1;
//                 _service.ProfanityFilter = false;
//                 _service.SmartFormatting = true;
//                 _service.SpeakerLabels = false;
//                 _service.WordAlternativesThreshold = null;
//                 _service.StartListening(OnRecognize, OnRecognizeSpeaker);
//             }
//             else if (!value && _service.IsListening)
//             {
//                 _service.StopListening();
//             }
//         }
//     }

//     private void StartRecording()
//     {
//         if (_recordingRoutine == 0)
//         {
//             UnityObjectUtil.StartDestroyQueue();
//             _recordingRoutine = Runnable.Run(RecordingHandler());
//         }
//     }

//     private void StopRecording()
//     {
//         if (_recordingRoutine != 0)
//         {
//             Microphone.End(_microphoneID);
//             Runnable.Stop(_recordingRoutine);
//             _recordingRoutine = 0;
//         }
//     }

//     private void OnError(string error)
//     {
//         Active = false;

//         Log.Debug("ExampleStreaming.OnError()", "Error! {0}", error);
//     }

//     private IEnumerator RecordingHandler()
//     {
//         Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
//         _recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
//         yield return null;      // let _recordingRoutine get set..

//         if (_recording == null)
//         {
//             StopRecording();
//             yield break;
//         }

//         bool bFirstBlock = true;
//         int midPoint = _recording.samples / 2;
//         float[] samples = null;

//         while (_recordingRoutine != 0 && _recording != null)
//         {
//             int writePos = Microphone.GetPosition(_microphoneID);
//             if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
//             {
//                 Log.Error("ExampleStreaming.RecordingHandler()", "Microphone disconnected.");

//                 StopRecording();
//                 yield break;
//             }

//             if ((bFirstBlock && writePos >= midPoint)
//               || (!bFirstBlock && writePos < midPoint))
//             {
//                 // front block is recorded, make a RecordClip and pass it onto our callback.
//                 samples = new float[midPoint];
//                 _recording.GetData(samples, bFirstBlock ? 0 : midPoint);

//                 AudioData record = new AudioData();
//                 record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
//                 record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
//                 record.Clip.SetData(samples, 0);

//                 _service.OnListen(record);

//                 bFirstBlock = !bFirstBlock;
//             }
//             else
//             {
//                 // calculate the number of samples remaining until we ready for a block of audio, 
//                 // and wait that amount of time it will take to record.
//                 int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
//                 float timeRemaining = (float)remaining / (float)_recordingHZ;

//                 yield return new WaitForSeconds(timeRemaining);
//             }

//         }

//         yield break;
//     }

//     private void OnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
//     {
//         if (result != null && result.results.Length > 0)
//         {
//             foreach (var res in result.results)
//             {
//                 foreach (var alt in res.alternatives)
//                 {
//                     string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
//                     Log.Debug("ExampleStreaming.OnRecognize()", text);
//                     //  audioData.Play();
//                     //  Log.Debug("ExampleStreaming.OnRecognize()", "mama");
//                     ResultsField.text = text;

//                     if (alt.transcript.Contains("Bob") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         audioData[0].Play();
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("rock and roll") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         audioData[1].Play();
//                         agent.enabled = false;
//                         anim.SetInteger("Dance", 1);
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("navigate ") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         if (navigation_help_counter > 0)
//                         {
//                             StartCoroutine(LowerThemeVolume(audioData[2]));
//                             audioData[2].Play();
//                             path.enabled = true;
//                             navigation_help_counter = navigation_help_counter - 1;
//                         }
//                         else
//                         {
//                             StartCoroutine(LowerThemeVolume(audioData[6]));
//                             audioData[6].Play();
//                         }
//                     }
//                     if (alt.transcript.Contains("steps ") || alt.transcript.Contains("footsteps ") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         string word = "steps";
//                         ClosestRiddle = riddleFinder.FindRiddle(riddles, transform);
//                         GameObject question1 = ClosestRiddle.transform.GetChild(0).gameObject;
//                         textmesh = ClosestRiddle.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>();
//                         string answer = textmesh.text;



//                         if (answer.Equals(word))
//                         {

//                             navigation_help_counter = navigation_help_counter + 1;
//                             TextMeshPro textmesh1 = ClosestRiddle.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
//                             textmesh1.fontSize = 17;
//                             textmesh1.font = BangersSDF;
//                             textmesh1.fontSharedMaterial = BangersSDFMaterial;
//                             textmesh1.text = "You have Unlocked a Navigation hint!";
//                             if (ClosestRiddle.activeSelf)
//                             {
//                                 StartCoroutine(DeactivateRiddle(ClosestRiddle));
//                             }
//                             //   ClosestRiddle.transform.GetChild(0).gameObject.SetActive(false);
//                             //   ClosestRiddle.transform.GetChild(2).gameObject.Destroy();
//                         }
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("television ") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         string word = "television";
//                         ClosestRiddle = riddleFinder.FindRiddle(riddles, transform);
//                         GameObject question = ClosestRiddle.transform.GetChild(0).gameObject;
//                         textmesh = ClosestRiddle.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>();
//                         string answer = textmesh.text;
//                         GameObject portal = GameObject.Find("Portal");


//                         if (answer.Equals(word))
//                         {


//                             TextMeshPro textmesh1 = ClosestRiddle.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
//                             textmesh1.fontSize = 16;
//                             textmesh1.font = BangersSDF;
//                             textmesh1.fontSharedMaterial = BangersSDFMaterial;
//                             textmesh1.text = "You have Unlocked a Portal!";
//                          //   textmesh1.fontSize = 17;
//                           //  textmesh1.font = BangersSDF;
//                          //   textmesh1.fontSharedMaterial = BangersSDFMaterial;
//                             foreach (GameObject gos in portals)
//                             {
//                                 gos.SetActive(true);
//                                 //     
//                             }
//                             if (ClosestRiddle.activeSelf)
//                             {
//                                 StartCoroutine(DeactivateRiddle(ClosestRiddle));
//                             }
//                             //   ClosestRiddle.transform.GetChild(0).gameObject.SetActive(false);
//                             //   ClosestRiddle.transform.GetChild(2).gameObject.Destroy();
//                         }
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("tips ") || alt.transcript.Contains("dips ") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         ClosestRiddle = riddleFinder.FindRiddle(riddles, transform);
//                         TempAudioSpurce = ClosestRiddle.GetComponent<AudioSource>();
//                         StartCoroutine(LowerThemeVolume(TempAudioSpurce));
//                         TempAudioSpurce.Play();
//                     }
//                     if (alt.transcript.Contains("distance") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         float distance_goal = Vector3.Distance(end.transform.position, transform.position);
//                         float distance_Covered = Vector3.Distance(begin.transform.position, transform.position);
//                         ClosestRiddle = riddleFinder.FindRiddle(riddles, transform);
//                         textmesh = ClosestRiddle.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
//                         textmesh.text = "Distance to Goal    " + distance_goal.ToString() + "              Distance covered   " + distance_Covered.ToString();
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("enemy") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         float distance = Vector3.Distance(enemy.transform.position, transform.position);
//                         ClosestRiddle = riddleFinder.FindRiddle(riddles, transform);
//                         textmesh = ClosestRiddle.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
//                         textmesh.text = "DIstance to Enemy : " + distance.ToString();
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("here ") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {

//                         path.enabled = false;
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("bored ") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         audioData[3].Play();
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("sure") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         audioData[4].Play();
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("tell ") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         audioData[5].Play();
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("xray") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {

//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("stop ") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens

//                     {
//                         foreach (AudioSource audio in audioData)
//                         {
//                             if (audio.isPlaying)
//                             {
//                                 audio.Stop();
//                                 agent.enabled = true;
//                                 anim.SetInteger("Dance", 0);
//                             }
//                         }

//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                     if (alt.transcript.Contains("finish") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
//                     {
//                         Active = false;
//                         this.enabled = false;
//                         //  _testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">I love the color of the sky too!</express-as></speak>";
//                         //   Runnable.Run(Examples());

//                     }
//                 }

//                 if (res.keywords_result != null && res.keywords_result.keyword != null)
//                 {
//                     foreach (var keyword in res.keywords_result.keyword)
//                     {
//                         Log.Debug("ExampleStreaming.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
//                     }
//                 }

//                 if (res.word_alternatives != null)
//                 {
//                     foreach (var wordAlternative in res.word_alternatives)
//                     {
//                         Log.Debug("ExampleStreaming.OnRecognize()", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
//                         foreach (var alternative in wordAlternative.alternatives)
//                             Log.Debug("ExampleStreaming.OnRecognize()", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
//                     }
//                 }
//             }
//         }
//     }

//     private void OnRecognizeSpeaker(SpeakerRecognitionEvent result, Dictionary<string, object> customData)
//     {
//         if (result != null)
//         {
//             foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
//             {
//                 Log.Debug("ExampleStreaming.OnRecognize()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
//             }
//         }
//     }

//     IEnumerator DeactivateRiddle(GameObject riddle)
//     {
//         yield return new WaitForSeconds(4);
//         riddle.SetActive(false);
//     }
//     IEnumerator LowerThemeVolume(AudioSource audio)
//     {
//         AudioClip clip = audio.clip;
//         float time = clip.length;
//         if (audioData[7])
//         {
//             audioData[7].volume = 0.027f;
//             yield return new WaitForSeconds(time);
//             audioData[7].volume = 0.123f;
//         }
//     }
// }
