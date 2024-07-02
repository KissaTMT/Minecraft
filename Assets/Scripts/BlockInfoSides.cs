using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Test/Sides Block")]
public class BlockInfoSides : BlockInfo
{
    public Vector2 PixelsOffsetUp;
    public Vector2 PixelsOffsetDown;
    public override Vector2 GetPixelOffset(Vector3 normal)
    {
        if (normal == Vector3.up) return PixelsOffsetUp; 
        if (normal == Vector3.down) return PixelsOffsetDown; 
        return base.GetPixelOffset(normal);
    }
}