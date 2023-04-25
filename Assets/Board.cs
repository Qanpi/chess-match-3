using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private int Width;

    [SerializeField]
    private int Height;

    // Start is called before the first frame update
    void Start()
    {
        FillBoard();
    }

    private TileChangeData? previouslySelected;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var tilePosition = tilemap.WorldToCell(worldPoint);
            PieceTile tile = tilemap.GetTile<PieceTile>(tilePosition);
            

            if (tile != null)
            {
                TileChangeData data = new()
                {
                    tile = tile,
                    position = tilePosition,
                    color = Color.white,
                };

                if (previouslySelected.HasValue)
                {
                    tile.Selected = true;
                    Swap(previouslySelected.Value, data);

                    previouslySelected = null;
                } 
                else
                {
                    //tile.Selected = true;
                    //tilemap.RefreshTile(tilePosition);
                 
                    previouslySelected = data;
                }
            }
        }
    }

    void Swap(TileChangeData tile1, TileChangeData tile2)
    {
        ((PieceTile)tile1.tile).Selected = false;
        ((PieceTile)tile2.tile).Selected = false;

        if (Vector3.Distance(tile1.position, tile2.position) != 1)
        {
            //tilemap.RefreshTile(tile1.position);
            return;
        }

        tilemap.SwapTile(tile1.tile, tile2.tile);
        tilemap.SwapTile(tile2.tile, tile1.tile);
        //tilemap.SetTile(tile1.position, tile2.tile);
        //tilemap.SetTile(tile2.position, tile1.tile);
    }

    void FillBoard()
    {
        for (int i=0; i<Width; i++)
        {
            for (int j=0; j<Height; j++)
            {
                PieceTile tile = PieceTile.CreateInstance<PieceTile>();
                tilemap.SetTile(new Vector3Int(i, j), tile);
            }
        }
    }
}
