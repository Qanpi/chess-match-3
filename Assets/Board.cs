using System;
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

    private List<Tile> GetLegalMoves(Tile tile)
    {
        int r = tile.Y, c = tile.X;

        List<Tile> tiles = new List<Tile>();

        switch (tile.Type)
        {
            case TileType.Knight:
                for (int i = -1; i <= 1; i += 2)
                {
                    for (int j = -1; j <= 1; j += 2)
                    {
                        if (r - i >= 0 && r - i < rows && c - j * 2 >= 0 && c - j * 2 < columns)
                        {
                            Tile t1 = GetTile(r - i, c - j * 2);
                            tiles.Add(t1);
                        }
                        if (r - i * 2 >= 0 && r - i * 2 < rows && c - j >= 0 && c - j < columns)
                        {
                            Tile t2 = GetTile(r - i * 2, c - j);
                            tiles.Add(t2);
                        }
                    }
                }

                break;

            case TileType.King:
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int x = r - i, y = c - j;
                        if (x < 0 || x >= columns || y < 0 || y >= rows) continue;

                        Tile t = GetTile(x, y);
                        if (t == tile) continue;

                        tiles.Add(t);
                    }
                }
                break;

            case TileType.Pawn:
                for (int i = -1; i <= 1; i += 2)
                {
                    for (int j = -1; j <= 1; j += 2)
                    {
                        int x = r - i, y = c - j;

                        if (x < 0 || x >= columns || x < 0 || y >= rows) continue;
                        Tile t = GetTile(x, y);

                        tiles.Add(t);
                    }
                }

                break;

            //FIXME: add bishop func.
            case TileType.Queen:
            case TileType.Rook:
                for (int i = 0; i < rows; i++)
                {
                    Tile t = GetTile(i, c);
                    if (t == tile) continue;

                    tiles.Add(t);
                }

                for (int i = 0; i < columns; i++)
                {
                    Tile t = GetTile(r, i);
                    if (t == tile) continue;

                    tiles.Add(t);
                }
                break;

            case TileType.Bishop:
                //TODO: refactor
                int d1 = r - c;
                int d2 = r + c;

                if (d1 >= 0)
                {
                    for (int i = 0; d1 + i < rows && i < columns; i++)
                    {
                        Tile t = GetTile(d1 + i, i);
                        tiles.Add(t);
                    }
                    for (int i = 0; d2 - i >= 0 && i < columns; i++)
                    {
                        if (d2 - i >= rows) continue;
                        Tile t = GetTile(d2 - i, i);
                        tiles.Add(t);
                    }
                }
                else
                {
                    d1 = Math.Abs(d1);
                    for (int i = 0; i < columns && d1 + i < rows; i++)
                    {
                        Tile t = GetTile(i, d1 + i);
                        tiles.Add(t);
                    }
                    for (int i = 0; i < columns && d2 - i >= 0; i++)
                    {
                        if (d2 - i >= rows) continue;
                        Tile t = GetTile(d2 - i, i);
                        tiles.Add(t);
                    }
                }

                break;

            default:
                throw new ArgumentException("Invalid tile type.");
        }

        return tiles;
    }

    private void ShowHints(Tile selected)
    {
        List<Tile> tiles = GetLegalMoves(selected);
        foreach (Tile t in tiles)
        {
            t.SetHint(true);
        }
    }

    private void HideHints()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Tile t = GetTile(i, j);
                t.SetHint(false);
            }
        }
    }

    private void Tile_OnClick(object sender, System.EventArgs e)
    {
        Tile tile = (Tile)sender;

        if (selection.Count == 0)
        {
            ShowHints(tile);
        }

        selection.Add(tile);

        if (selection.Count == 2)
        {
            Move(selection[0], selection[1]);

            HideHints();
            selection.Clear();
        }
    }

    private void Move(Tile tile1, Tile tile2)
    {
        if (tile1 == tile2) return;

        List<Tile> legal = GetLegalMoves(tile1);
        if (legal.Contains(tile2) == false) return;

        SwapTiles(tile1, tile2);

        //unswap if no matches
        if (!checkForMatch(tile1) && !checkForMatch(tile2))
        {
            //SwapTiles(tile1, tile2);
        }
    }

    private void SwapTiles(Tile tile1, Tile tile2)
    {
        TileType tempType = tile1.Type;
        tile1.Type = tile2.Type;
        tile2.Type = tempType;

        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        Sprite temp = renderer2.sprite;
        renderer2.sprite = renderer1.sprite;
        renderer1.sprite = temp;
    }

    private Tile? GetTile(int R, int C)
    {
        if (R < 0 || R >= rows || C < 0 || C >= columns) return null;
        return tiles[R, C].GetComponent<Tile>();
    }

    private bool checkForMatch(Tile tile)
    {
        int x = tile.X, y = tile.Y;

        var horizontalConnections = new List<Tile>();
        var verticalConnections = new List<Tile>();

        //HORIZONTAL 
        for (int i = 0; x - i >= 0; i++)
        {
            Tile adjacent = GetTile(y, x - i);

            if (adjacent.Type != tile.Type)
            {
                break;
            }

            horizontalConnections.Add(adjacent);
        }

        for (int i = 1; x + i < columns; i++)
        {
            Tile adjacent = GetTile(y, x + i);

            if (adjacent.Type != tile.Type)
            {
                break;
            }

            horizontalConnections.Add(adjacent);
        }

        //VERTICAL
        for (int i = 0; y - i >= 0; i++)
        {
            Tile adjacent = GetTile(y - i, x);

            if (adjacent.Type != tile.Type)
            {
                break;
            }

            verticalConnections.Add(adjacent);
        }

        for (int i = 1; y + i < rows; i++)
        {
            Tile adjacent = GetTile(y + i, x);

            if (adjacent.Type != tile.Type)
            {
                break;
            }

            verticalConnections.Add(adjacent);
        }

        bool match = false;

        if (horizontalConnections.Count >= 3)
        {
            match = true;
            foreach (Tile connected in horizontalConnections)
            {
                connected.RefreshTileType();
            }
        }

        if (verticalConnections.Count >= 3)
        {
            match = true;
            foreach (Tile connected in verticalConnections)
            {
                connected.RefreshTileType();
            }
        }

        return match;
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
