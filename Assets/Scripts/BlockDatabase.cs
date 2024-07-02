using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Blocks/db"))]
public class BlockDatabase : ScriptableObject
{
    public BlockInfo[] Blocks;
    private Dictionary<BlockType, BlockInfo> _blockCashed = new Dictionary<BlockType, BlockInfo>();

    private void OnEnable()
    {
        _blockCashed.Clear();
        foreach(var block in Blocks)
        {
            _blockCashed.Add(block.BlockType, block);
        }
    }
    public BlockInfo GetInfo(BlockType type)
    {
        if (_blockCashed.TryGetValue(type, out var info))
        {
            return info;
        }
        else return null;
    }
}
