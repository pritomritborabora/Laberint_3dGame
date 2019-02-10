using System.Collections;
using UnityEngine;
using TMPro;

public class HelperTextTyping : MonoBehaviour
{                
    private int visibleCount = 0;    
    private bool isCoroutineStarted;
    private TextMeshProUGUI textmesh;
    private AudioSource typingSoundEffect;
    
    void Start()
    {
        typingSoundEffect = this.GetComponent<AudioSource>();
        textmesh = this.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (!isCoroutineStarted) StartCoroutine(type());
    }

    void OnDisable()
    {
        isCoroutineStarted = false;
        visibleCount = 0;        

        if (typingSoundEffect != null && typingSoundEffect.isPlaying) typingSoundEffect.Stop();
    }

	IEnumerator type () {
        // typingSoundEffect.Play();
        isCoroutineStarted = true;
        
        while (true)
        {            
            textmesh.maxVisibleCharacters = visibleCount;

            if (visibleCount == textmesh.textInfo.characterCount) break;

            visibleCount += 1;
            yield return new WaitForSeconds(0.04f);
        }

        typingSoundEffect.Stop();
        this.enabled = !this.enabled;
    }
}
