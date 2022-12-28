using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundGroupComponent : SoundManagerComponent
{
    public List<SoundGroup> soundGroups;

    public void PlaySoundFromGroup(string groupName)
    {
        var group = soundGroups.FirstOrDefault(x => x.groupName == groupName);
        if (group == null || !group.audioClips.Any())
        {
            Debug.LogWarning($"{gameObject.name} does not contains sounds for \"{groupName}\" Group");
        }
        else
        {
            PlayRandomClip(group.audioClips);
        }
    }
}
