using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBackground : MonoBehaviour
{
    private static MusicBackground musicBackground;
    private void Awake() {
        if(musicBackground==null)
        {
            musicBackground = this;
            //DontDestroyOnLoad(musicBackground);
        }
        else{
            Destroy(gameObject);
        }
    }
}
