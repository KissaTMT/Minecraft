using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public float BaseHeight = 8;
    public NoiseOctaveSettings[] OctaveSettings;
    public NoiseOctaveSettings DomainWarp;
    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }
    private FastNoiseLite[] _octaveNoises;
    private FastNoiseLite _warpNoise;

    private void Awake()
    {
        _octaveNoises = new FastNoiseLite[OctaveSettings.Length];
        for(var i=0;i<OctaveSettings.Length;i++)
        {
            _octaveNoises[i] = new FastNoiseLite();
            _octaveNoises[i].SetNoiseType(OctaveSettings[i].NoiseType);
            _octaveNoises[i].SetFrequency(OctaveSettings[i].Frequency);
        }
        _warpNoise = new FastNoiseLite();
        _warpNoise.SetNoiseType(DomainWarp.NoiseType);
        _warpNoise.SetFrequency(DomainWarp.Frequency);
        _warpNoise.SetDomainWarpAmp(DomainWarp.Amplitude);
    }
    private float GetHeight(float x, float y)
    {
        _warpNoise.DomainWarp(ref x, ref y);
        var height = BaseHeight;

        for(var i = 0; i < OctaveSettings.Length; i++)
        {
            var noise = _octaveNoises[i].GetNoise(x, y);
            height += noise * OctaveSettings[i].Amplitude / 2;
        }
        return height;
    }
    public BlockType[,,] Generate(float xOffset, float zOffset)
    {
        var result = new BlockType[ChunkRenderer.CHUNK_WIDTH, ChunkRenderer.CHUNK_HEIGHT, ChunkRenderer.CHUNK_WIDTH];

        for(var x = 0; x < ChunkRenderer.CHUNK_WIDTH; x++)
        {
            for(var z = 0;z < ChunkRenderer.CHUNK_WIDTH; z++)
            {
                var height = GetHeight(x * ChunkRenderer.BLOCK_SCALE + xOffset, z * ChunkRenderer.BLOCK_SCALE + zOffset);
                var grassLayerHight = 1;
                for(var y = 0; y < height/ ChunkRenderer.BLOCK_SCALE; y++)
                {
                    if(height - y * ChunkRenderer.BLOCK_SCALE < grassLayerHight) result[x, y, z] = BlockType.Dirt;
                    else result[x,y,z] = BlockType.Stone;
                }
            }
        }

        return result;
    }
}
