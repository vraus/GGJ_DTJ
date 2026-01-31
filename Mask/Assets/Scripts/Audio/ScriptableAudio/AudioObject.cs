using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Object", menuName = "Assets/New Audio Object")]
public class AudioObject : ScriptableObject
{
    public AudioClip audioClip;
    public String subtitle;
}
