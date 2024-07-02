using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public const int CHUNK_WIDTH = 16;
    public const int CHUNK_HEIGHT = 128;
    public const float BLOCK_SCALE = 0.5f;

    public ChunkData ChunkData;
    public GameWorld ParentWorld;
    public BlockDatabase BlockDatabase;

    private Mesh _chunkMesh;
    private List<Vector3> _vertices = new List<Vector3>();
    private List<Vector2> _uvs = new List<Vector2>();
    private List<int> _triangles = new List<int>();

    public void SpawnBlock(Vector3Int position)
    {
        ChunkData.Blocks[position.x, position.y, position.z] = BlockType.Wood;
        RegenerateMesh();
    }
    public void DestroyBlock(Vector3Int position)
    {
        ChunkData.Blocks[position.x, position.y, position.z] = BlockType.Air;
        RegenerateMesh();
    }

    private void Start()
    {
        _chunkMesh = new Mesh();
        RegenerateMesh();
        GetComponent<MeshFilter>().mesh = _chunkMesh;
    }

    private void RegenerateMesh()
    {
        _vertices.Clear();
        _triangles.Clear();
        _uvs.Clear();

        for (var x = 0; x < CHUNK_WIDTH; x++)
        {
            for (var y = 0; y < CHUNK_HEIGHT; y++)
            {
                for (var z = 0; z < CHUNK_WIDTH; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }
        _chunkMesh.triangles = Array.Empty<int>();
        _chunkMesh.vertices = _vertices.ToArray();
        _chunkMesh.uv = _uvs.ToArray();
        _chunkMesh.triangles = _triangles.ToArray();

        _chunkMesh.Optimize();

        _chunkMesh.RecalculateBounds();
        _chunkMesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = _chunkMesh;
    }

    private void GenerateBlock(int x, int y, int z)
    {
        var position = new Vector3Int(x, y, z);

        var blockType = GetBlockAtPotition(position);
        if (blockType == 0) return;

        if (GetBlockAtPotition(position + Vector3Int.right) == 0)
        {
            GenerateRightSide(position);
            AddUvs(blockType, Vector3.right);
        }
        if (GetBlockAtPotition(position + Vector3Int.left) == 0)
        {
            GenerateLeftSide(position);
            AddUvs(blockType, Vector3.left);
        }
        if (GetBlockAtPotition(position + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(position);
            AddUvs(blockType, Vector3.forward);
        }
        if (GetBlockAtPotition(position + Vector3Int.back) == 0)
        {
            GenerateBackSide(position);
            AddUvs(blockType, Vector3.back);
        }
        if (GetBlockAtPotition(position + Vector3Int.up) == 0)
        {
            GenerateTopSide(position);
            AddUvs(blockType, Vector3.up);
        }
        if (GetBlockAtPotition(position + Vector3Int.down) == 0)
        {
            GenerateBottomSide(position);
            AddUvs(blockType, Vector3.down);
        }
    }
    private void AddLastVerticesSquare()
    {
        _triangles.Add(_vertices.Count - 4);
        _triangles.Add(_vertices.Count - 3);
        _triangles.Add(_vertices.Count - 2);

        _triangles.Add(_vertices.Count - 3);
        _triangles.Add(_vertices.Count - 1);
        _triangles.Add(_vertices.Count - 2);
    }
    private void AddUvs(BlockType blockType, Vector3 normal)
    {
        Vector2 uv;

        var info = BlockDatabase.GetInfo(blockType);
        if (info != null) uv = info.GetPixelOffset(normal) / 256;
        else uv = new Vector2(160f / 256, 240f / 256f);
        for (var i = 0; i < 4; i++)
        {
            _uvs.Add(uv);
        }
    }
    private BlockType GetBlockAtPotition(Vector3Int position)
    {
        if (position.x >= 0 && position.x < CHUNK_WIDTH &&
           position.y >= 0 && position.y < CHUNK_HEIGHT &&
           position.z >= 0 && position.z < CHUNK_WIDTH)
        {
            return ChunkData.Blocks[position.x, position.y, position.z];
        }
        else
        {
            if (position.y < 0 || position.y >= CHUNK_HEIGHT) return BlockType.Air;
            var adjacentChunkPosition = ChunkData.ChunkPosition;

            if (position.x < 0)
            {
                adjacentChunkPosition.x--;
                position.x += CHUNK_WIDTH;
            }
            else if(position.x>= CHUNK_WIDTH)
            {
                adjacentChunkPosition.x++;
                position.x -= CHUNK_WIDTH;
            }
            if (position.z < 0)
            {
                adjacentChunkPosition.y--;
                position.z += CHUNK_WIDTH;
            }
            else if (position.z >= CHUNK_WIDTH)
            {
                adjacentChunkPosition.y++;
                position.z -= CHUNK_WIDTH;
            }
            if(ParentWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out var chunk))
            {
                return chunk.Blocks[position.x,position.y,position.z];
            }

            else return BlockType.Air;
        }
    }
    private void GenerateRightSide(Vector3 blockPosition)
    {
        _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);

        AddLastVerticesSquare();
    }
    private void GenerateLeftSide(Vector3 blockPosition)
    {
        _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);

        AddLastVerticesSquare();
    }
    private void GenerateFrontSide(Vector3 blockPosition)
    {
        _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);

        AddLastVerticesSquare();
    }
    private void GenerateBackSide(Vector3 blockPosition)
    {
        _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);

        AddLastVerticesSquare();
    }
    private void GenerateTopSide(Vector3 blockPosition)
    {
        _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);

        AddLastVerticesSquare();
    }
    private void GenerateBottomSide(Vector3 blockPosition)
    {
        _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
        _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);

        AddLastVerticesSquare();
    }
}
public enum BlockType : byte
{
    Air,
    Stone,
    Dirt,
    Grass,
    Wood
}
