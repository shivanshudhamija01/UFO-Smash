using UnityEngine;
[CreateAssetMenu(fileName = "SoundInfo", menuName = "ScriptableObjects/SoundData")]
public class SoundInfo : ScriptableObject
{
    public SoundType id;
    public AudioClip audioClip;
}
