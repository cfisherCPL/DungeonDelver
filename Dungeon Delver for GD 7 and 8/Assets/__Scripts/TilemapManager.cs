using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;  // We need this to make Tiles & Tilemaps work


public class TilemapManager : MonoBehaviour
{
    static public Tile[] DELVER_TILES;
    static public Dictionary<char, Tile> COLL_TILE_DICT;

    [Header("Inscribed")]                                                   // a
    public Tilemap visualMap;
    public Tilemap collisionMap; // a
    public Tilemap grapTilesMap;

    private TileBase[] visualTileBaseArray;
    private TileBase[] collTileBaseArray;
    private TileBase[] grapTileBaseArray;


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

        // Load all of the Sprites from CollisionTiles
        tempTiles = Resources.LoadAll<Tile>("Tiles_Collision");             // d
        // Collision tiles are stored in a Dictionary for easier access
        COLL_TILE_DICT = new Dictionary<char, Tile>();
        for (int i = 0; i < tempTiles.Length; i++)
        {
            char c = tempTiles[i].name[0];                                    // e
            COLL_TILE_DICT.Add(c, tempTiles[i]);
        }
        Debug.Log("COLL_TILE_DICT contains " + COLL_TILE_DICT.Count + " tiles.");
    

    }

    void ShowTiles()
    {
        visualTileBaseArray = GetMapTiles();                                  // b
        visualMap.SetTilesBlock(MapInfo.GET_MAP_BOUNDS(), visualTileBaseArray);
        collTileBaseArray = GetCollisionTiles();                              // f
        collisionMap.SetTilesBlock(MapInfo.GET_MAP_BOUNDS(), collTileBaseArray);

        grapTileBaseArray = GetGrapTiles();                                   // c
        grapTilesMap.SetTilesBlock(MapInfo.GET_MAP_BOUNDS(), grapTileBaseArray);
        

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

    /// <summary>
    /// Use MapInfo.MAP and MapInfo.COLLISIONS to create a TileBase[] array 
    ///  holding the tiles to fill the collisionMap Tilemap.
    /// </summary>
    /// <returns>The TileBases for collisionMap</returns>
    public TileBase[] GetCollisionTiles()
    {
        Tile tile;
        int tileNum;
        char tileChar;
        TileBase[] mapTiles = new TileBase[MapInfo.W * MapInfo.H];
        for (int y = 0; y < MapInfo.H; y++)
        {
            for (int x = 0; x < MapInfo.W; x++)
            {
                tileNum = MapInfo.MAP[x, y];
                tileChar = MapInfo.COLLISIONS[tileNum];                     // g
                tile = COLL_TILE_DICT[tileChar];
                mapTiles[y * MapInfo.W + x] = tile;
            }
        }
        return mapTiles;
    }

    /// <summary>
    /// Use MapInfo.MAP and MapInfo.GRAP_TILES to create a TileBase[] array 
    ///  holding the tiles to fill the grapTilesMap Tilemap.
    /// </summary>
    /// <returns>The TileBases for grapTilesMap</returns>
    public TileBase[] GetGrapTiles()
    {
        Tile tile;
        int tileNum;
        char tileChar;
        TileBase[] mapTiles = new TileBase[MapInfo.W * MapInfo.H];
        for (int y = 0; y < MapInfo.H; y++)
        {
            for (int x = 0; x < MapInfo.W; x++)
            {
                tileNum = MapInfo.MAP[x, y];
                tileChar = MapInfo.GRAP_TILES[tileNum];                       // d
                if (tileChar == 'U' ) tileChar = '_';                          // e
                tile = COLL_TILE_DICT[tileChar];
                mapTiles[y * MapInfo.W + x] = tile;
            }
        }
        return mapTiles;
    }
}
