using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour {
   public  AudioSource myfx;
   public AudioClip hoverfx;
   public AudioClip clickfx;

    /*    void start()
      {
           myfx = GetComponent<AudioSource>();
           hoverfx = myfx.clip;
       } */


    // Use this for initializations
    public void HoverSound()
    {
        myfx.PlayOneShot(hoverfx);
    }

    public void ClickSound()
    {
        myfx.PlayOneShot(clickfx);
    }

    // Update is called once per frame

}
