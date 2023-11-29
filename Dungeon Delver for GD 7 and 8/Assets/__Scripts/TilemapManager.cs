using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;  // We need this to make Tiles & Tilemaps work


public class TilemapManager : MonoBehaviour
{
    static public Tile[] DELVER_TILES;

    [Header("Inscribed")]                                                   // a
    public Tilemap visualMap;                                                 // a
    
    private TileBase[] visualTileBaseArray;


    void Awake()
    {
        LoadTiles();
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowTiles();
    }

    /// <summary>
    /// Load all the Tiles from the Resources/Tiles_Visual folder into an array.
    /// </summary>
    void LoadTiles()
    {
        int num;

        // Load all of the Sprites from DelverTiles
        Tile[] tempTiles = Resources.LoadAll<Tile>("Tiles_Visual");

        // The order of the Tiles is not guaranteed, so arrange them by number
        DELVER_TILES = new Tile[tempTiles.Length];
        for (int i = 0; i < tempTiles.Length; i++)
        {
            string[] bits = tempTiles[i].name.Split( '_' );                        // b
            if (int.TryParse(bits[1], out num))
            {                              // c
                DELVER_TILES[num] = tempTiles[i];
            }
            else
            {
                Debug.LogError("Failed to parse num of: " + tempTiles[i].name);      // d
            }
        }
        Debug.Log("Parsed " + DELVER_TILES.Length + " tiles into TILES_VISUAL.");



    }

    void ShowTiles()
    {
        visualTileBaseArray = GetMapTiles();                                  // b
        visualMap.SetTilesBlock(MapInfo.GET_MAP_BOUNDS(), visualTileBaseArray);
    }

    /// <summary>
    /// Use MapInfo.MAP to create a TileBase[] array holding the tiles to fill
    ///  the visualMap Tilemap.
    /// </summary>
    /// <returns>The TileBases for visualMap</returns>

    public TileBase[] GetMapTiles()
    {
        int tileNum;
        Tile tile;
        TileBase[] mapTiles = new TileBase[MapInfo.W * MapInfo.H];
        for (int y = 0; y < MapInfo.H; y++)
        {
            for (int x = 0; x < MapInfo.W; x++)
            {
                tileNum = MapInfo.MAP[x, y];                                // c
                tile = DELVER_TILES[tileNum];                               // d
                mapTiles[y * MapInfo.W + x] = tile;                         // e
            }
        }
        return mapTiles;
    }
}
