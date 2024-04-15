using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int rows = 8;
    [SerializeField] private int columns = 8;

    private GameObject[,] tiles;
    private readonly List<Tile> selection = new();

    // Start is called before the first frame update
    void Start()
    {
        tiles = new GameObject[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                tiles[i, j] = Instantiate(tilePrefab, this.transform);

                Tile tile = GetTile(i, j);
                tile.Y = i;
                tile.X = j;
                tile.OnClick += Tile_OnClick;
            }
        }
    }

    private void Tile_OnClick(object sender, System.EventArgs e)
    {
        Tile tile = (Tile)sender;

        selection.Add(tile);

        if (selection.Count == 2)
        {
            Swap(selection[0], selection[1]);

            selection.Clear();
        }
    }

    private void Swap(Tile tile1, Tile tile2)
    {
        if (Mathf.Abs(tile1.X - tile2.X) + Mathf.Abs(tile1.Y - tile2.Y) > 1) return;

        SwapSprites(tile1, tile2);

        TileType temp = tile1.Type;
        tile1.Type = tile2.Type;
        tile2.Type = temp;

        checkForMatch(tile1);
        checkForMatch(tile2);
    }

    private void SwapSprites(Tile tile1, Tile tile2)
    {
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        Sprite temp = renderer2.sprite;
        renderer2.sprite = renderer1.sprite;
        renderer1.sprite = temp;
    }

    private Tile GetTile(int R, int C)
    {
        //if (R >= rows || R < 0 || C >= columns || C < 0) return null;

        return tiles[R, C].GetComponent<Tile>();
    }

    private void checkForMatch(Tile tile)
    {
        int x = tile.X, y = tile.Y;

        var horizontalConnections = new List<Tile>();
        var verticalConnections = new List<Tile>();

        //HORIZONTAL 
        for (int i = 0; x - i >= 0; i++)
        {
            Tile adjacent = GetTile(y, x - i );

            if (adjacent.Type != tile.Type)
            {
                break;
            } 

            horizontalConnections.Add(adjacent); 
        }

        for (int i = 1;  x + i < columns; i++)
        {
            Tile adjacent = GetTile(y, x + i );

            if (adjacent.Type != tile.Type)
            {
                break;
            } 

            horizontalConnections.Add(adjacent); 
        }

        //VERTICAL
        for (int i = 0; y - i >= 0; i++)
        {
            Tile adjacent = GetTile(y - i, x );

            if (adjacent.Type != tile.Type)
            {
                break;
            } 

            verticalConnections.Add(adjacent); 
        }

        for (int i = 1;  y + i < rows; i++)
        {
            Tile adjacent = GetTile(y + i, x);

            if (adjacent.Type != tile.Type)
            {
                break;
            } 

            verticalConnections.Add(adjacent); 
        }

        if (horizontalConnections.Count >= 3)
        {
            foreach (Tile connected in horizontalConnections)
            {
                connected.RefreshTileType();
            }
        }

        if (verticalConnections.Count >= 3)
        {
            foreach (Tile connected in verticalConnections)
            {
                connected.RefreshTileType();
            }
        }

        //VERTICAL 
        //for (int j = y - 1; j >= 0; j--)
        //{
        //    Tile adjacent = GetTile(x, j);

        //    if (adjacent.Type == tile.Type)
        //    {
        //        vertical++;
        //    }
        //}

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            /*var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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
            }*/
        }
    }

    //void Swap(TileChangeData tile1, TileChangeData tile2)
    //{
    /*((PieceTile)tile1.tile).Selected = false;
    ((PieceTile)tile2.tile).Selected = false;

    if (Vector3.Distance(tile1.position, tile2.position) != 1)
    {
        //tilemap.RefreshTile(tile1.position);
        return;
    }

    tilemap.SwapTile(tile1.tile, tile2.tile);
    tilemap.SwapTile(tile2.tile, tile1.tile);
    //tilemap.SetTile(tile1.position, tile2.tile);
    //tilemap.SetTile(tile2.position, tile1.tile);*/
    //}

    void Shuffle()
    {
        /*for (int i=0; i<Width; i++)
        {
            for (int j=0; j<Height; j++)
            {
                PieceTile tile = PieceTile.CreateInstance<PieceTile>();
                tilemap.SetTile(new Vector3Int(i, j), tile);
            }
        }*/
    }
}
