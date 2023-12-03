using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileSwap
{                  
    public int fromTileNum;
    public GameObject swapPrefab;
    public GameObject guaranteedDrop;
    public int toTileNum = 1; // Default 1 is ignored due to a Unity bug
}

public class TileSwapManager : MonoBehaviour
{
    private static TileSwapManager S;
    private static Dictionary<int, TileSwap> TILE_SWAP_DICT;

    public List<TileSwap> tileSwapList;

    void Awake()
    {
        S = this;
    }

    public static void SWAP_TILES(int[,] map)
    {                                  // a
        if (TILE_SWAP_DICT == null) S.BuildTileSwapDict();                       // b
        
        int fromTileNum;
        TileSwap tSwap;
        // Iterate through each tileNum in the map
        for (int i = 0; i < map.GetLength(0); i++)
        {                             // c
            for (int j = 0; j < map.GetLength(1); j++)
            {
                fromTileNum = map[i, j];
                
                if (TILE_SWAP_DICT.ContainsKey(fromTileNum))
                {                 // d
                    tSwap = TILE_SWAP_DICT[fromTileNum];
                    map[i, j] = tSwap.toTileNum;                                   // e
                    
                    // More will be added here
                }
            }
        }
    }

    void BuildTileSwapDict()
    {                                                     // f
        TILE_SWAP_DICT = new Dictionary<int, TileSwap>();
        foreach (TileSwap swap in tileSwapList)
        {
            if (TILE_SWAP_DICT.ContainsKey(swap.fromTileNum))
            {                // g
                Debug.LogError("More than one TileSwap with a From # of " + swap.fromTileNum);
            }
            else
            {
                TILE_SWAP_DICT.Add(swap.fromTileNum, swap);                      // h
            }
        }
    }


}
