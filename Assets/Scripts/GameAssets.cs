using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;

    public static GameAssets GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public Sprite pipeHeadSprite;
    public Transform PFPipeBody;
    public Transform PFPipeHead;
    public Transform PFGround;
    public Transform PFCloud1;
    public Transform PFCloud2;
    public Transform PFCloud3;

    public SoundAudioClip[] SoundAudioClipArray;

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}
