using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Test/Normal Block")]
public class BlockInfo : ScriptableObject
{
    public BlockType BlockType;
    public Vector2 PixelOffset;
    public AudioClip Clip;
    public float TimeToBreak;

    public virtual Vector2 GetPixelOffset(Vector3 normal)
    {
        return PixelOffset;
    }
}
