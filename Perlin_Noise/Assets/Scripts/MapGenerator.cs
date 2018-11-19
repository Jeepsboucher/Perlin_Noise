using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour {
    public enum GenerationType
    {
        random, perlinNoise
    }
    public GenerationType generationType;
    public int mapWidth;
    public int mapheight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public int seed;
    public Vector2 offset;

    public Tilemap tilemap;
    public TerrainType[] regions;
    public TerrainType[] ores;
	
    private TileBase FindTileFromRegion(float valeur)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if(valeur <= regions[i].value)
            {
                return regions[i].tile;
            }
        }
        return regions[0].tile;
    }

    private TileBase FindTileFromOre(float valeur)
    {
        for (int i = 0; i < ores.Length; i++)
        {
            if (valeur <= ores[i].value)
            {
                return ores[i].tile;
            }
        }
        return ores[0].tile;
    }

    public void SetTileMap(TileBase[] customTileMap)
    {
        for (int y = 0; y < mapheight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x,y,0), customTileMap[y * mapWidth + x]);
            }
        }
    }

    public void GenerateMapRandom()
    {
        TileBase[] customTileMap = new TileBase[mapWidth * mapheight];
        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float rnd = Random.Range(0f, 1f);
                customTileMap[y * mapWidth + x] = FindTileFromRegion(rnd);
            }
        }
        SetTileMap(customTileMap);
    }

    private void OnValidate()
    {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }

    public void GenerateMap()
    {
        if(generationType == GenerationType.random)
        {
            GenerateMapRandom();
        }
        else if (generationType == GenerationType.perlinNoise)
        {
            GenerateMapPerlinNoise();
        }
    }

    public void GenerateMapPerlinNoise()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth,mapheight,seed,noiseScale,octaves,persistance,lacunarity,offset);
        float[,] noiseMapOre = Noise.GenerateNoiseMap(mapWidth, mapheight, seed + 1, noiseScale, octaves, persistance, lacunarity, offset);

        TileBase[] customTileMap = new TileBase[mapWidth * mapheight];
        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float value = noiseMap[x, y];
                if(value > regions[1].value)
                {
                    float valueOre = noiseMapOre[x, y];
                    customTileMap[y * mapWidth + x] = FindTileFromOre(valueOre);
                }
                else
                {
                    customTileMap[y * mapWidth + x] = FindTileFromRegion(value);
                }

            }
        }
        SetTileMap(customTileMap);
    }
}
[Serializable]
public struct TerrainType {
    public string name;
    public float value;
    public TileBase tile;
}
