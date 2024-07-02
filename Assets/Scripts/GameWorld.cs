using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public const int VIEW_RADIUS = 5;
    public Dictionary<Vector2Int,ChunkData> ChunkDatas = new Dictionary<Vector2Int,ChunkData>();
    public ChunkRenderer ChunkRenderer;
    public TerrainGenerator TerrainGenerator;

    private Camera _cameraMain;
    private Vector2Int _currentPlayerChunk;

    private void Start()
    {
        _cameraMain = Camera.main;
        StartCoroutine(Generate());
    }

    private IEnumerator Generate(bool wait = false)
    {
        for (var x = _currentPlayerChunk.x - VIEW_RADIUS; x < _currentPlayerChunk.x+VIEW_RADIUS; x++)
        {
            for (var z = _currentPlayerChunk.y - VIEW_RADIUS; z < _currentPlayerChunk.y + VIEW_RADIUS; z++)
            {
                var chunkPosition = new Vector2Int(x, z);
                if (ChunkDatas.ContainsKey(chunkPosition)) continue;
                LoadChunk(x, z, chunkPosition);
                if (wait) yield return new WaitForSecondsRealtime(0.2f);
            }
        }
    }

    private void LoadChunk(int x, int z, Vector2Int chunkPosition)
    {
        float xPos = x * ChunkRenderer.CHUNK_WIDTH * ChunkRenderer.BLOCK_SCALE;
        float zPos = z * ChunkRenderer.CHUNK_WIDTH * ChunkRenderer.BLOCK_SCALE;
        var chunkData = new ChunkData();
        chunkData.Blocks = TerrainGenerator.Generate(xPos, zPos);
        chunkData.ChunkPosition = new Vector2Int(x, z);
        ChunkDatas.Add(chunkPosition, chunkData);
        var chunk = Instantiate(ChunkRenderer, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
        chunkData.ChunkRenderer = chunk;
        chunk.ChunkData = chunkData;
        chunk.ParentWorld = this;
    }

    private void Update()
    {
        var playerWorldPos = Vector3Int.FloorToInt(_cameraMain.transform.position / ChunkRenderer.BLOCK_SCALE);
        var playerChunk = GetChunkContainingBlock(playerWorldPos);
        if(playerChunk != _currentPlayerChunk)
        {
            _currentPlayerChunk = playerChunk;
            StartCoroutine(Generate(true));
        }
        InputHandle();
    }

    private void InputHandle()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            var isDestroying = Input.GetMouseButtonDown(0);
            var ray = _cameraMain.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out var hitInfo))
            {
                Vector3 blockCenter;
                if (isDestroying)
                {
                    blockCenter = hitInfo.point - hitInfo.normal * ChunkRenderer.BLOCK_SCALE / 2;
                }
                else
                {
                    blockCenter = hitInfo.point + hitInfo.normal * ChunkRenderer.BLOCK_SCALE / 2;
                }
                var blockWorldPosition = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.BLOCK_SCALE);
                var chunkPosition = GetChunkContainingBlock(blockWorldPosition);
                if (ChunkDatas.TryGetValue(chunkPosition, out var chunkData))
                {
                    var chunkOrigin = new Vector3Int(chunkPosition.x, 0, chunkPosition.y) * ChunkRenderer.CHUNK_WIDTH;
                    chunkData.ChunkRenderer.SpawnBlock(blockWorldPosition - chunkOrigin);
                    if (isDestroying)
                    {
                        chunkData.ChunkRenderer.DestroyBlock(blockWorldPosition - chunkOrigin);
                    }
                    else
                    {
                        chunkData.ChunkRenderer.SpawnBlock(blockWorldPosition - chunkOrigin);
                    }
                }
            }
        }
    }

    public Vector2Int GetChunkContainingBlock(Vector3Int blockPosition)
    {
        var pos = new Vector2Int(blockPosition.x / ChunkRenderer.CHUNK_WIDTH, blockPosition.z / ChunkRenderer.CHUNK_WIDTH);
        if (blockPosition.x < 0) pos.x--;
        if (blockPosition.y < 0) pos.y--;
        return pos;
    }
}
