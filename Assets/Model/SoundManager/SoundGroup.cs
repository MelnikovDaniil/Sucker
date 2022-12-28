using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundGroup
{
    public string groupName;

    public List<AudioClip> audioClips = new List<AudioClip>();
}
