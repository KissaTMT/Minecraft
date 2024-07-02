using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChunkData
{
    public Vector2Int ChunkPosition;
    public ChunkRenderer ChunkRenderer;
    public BlockType[,,] Blocks;
}
