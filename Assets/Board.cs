#nullable enable

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int rows = 8;
    [SerializeField] private int columns = 8;

    private GameObject[,] tiles;

    private readonly List<Tile> selection = new();

    private (Tile first, Tile second)? nextMatch;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new GameObject[rows, columns];
        Shuffle();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Tile t = GetTile(i, j);
                RecursiveMatch(t);
            }
        }

        EnsureNextMatch();
    }

    private void EnsureNextMatch()
    {
        nextMatch = FindMatch();
        while (nextMatch is null)
        {
            Shuffle();
            nextMatch = FindMatch();
        }

        Debug.Log(nextMatch.Value.first);
        Debug.Log(nextMatch.Value.second);
    }

    private void Shuffle()
    {
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

        void RookLegalMoves(Tile tile)
        {
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
        }

        void BishopLegalMoves(Tile tile)
        {
            //TODO: refactor
            int d1 = r - c;
            int d2 = r + c;

            if (d1 >= 0)
            {
                for (int i = 0; d1 + i < rows && i < columns; i++)
                {
                    Tile t = GetTile(d1 + i, i);
                    if (t == tile) continue;
                    tiles.Add(t);
                }
                for (int i = 0; d2 - i >= 0 && i < columns; i++)
                {
                    if (d2 - i >= rows) continue;
                    Tile t = GetTile(d2 - i, i);
                    if (t == tile) continue;
                    tiles.Add(t);
                }
            }
            else
            {
                d1 = Math.Abs(d1);
                for (int i = 0; i < columns && d1 + i < rows; i++)
                {
                    Tile t = GetTile(i, d1 + i);
                    if (t == tile) continue;
                    tiles.Add(t);
                }
                for (int i = 0; i < columns && d2 - i >= 0; i++)
                {
                    if (d2 - i >= rows) continue;
                    Tile t = GetTile(d2 - i, i);
                    if (t == tile) continue;
                    tiles.Add(t);
                }
            }
        }

        //TODO: maybe use a yield generator? 
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

                        if (x < 0 || x >= columns || y < 0 || y >= rows) continue;
                        Tile t = GetTile(x, y);

                        tiles.Add(t);
                    }
                }

                break;

            case TileType.Queen:
                RookLegalMoves(tile);
                BishopLegalMoves(tile);
                break;

            case TileType.Rook:
                RookLegalMoves(tile);
                break;

            case TileType.Bishop:
                BishopLegalMoves(tile);
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

        RecursiveMatch(tile1);
        RecursiveMatch(tile2);

        //TODO: unswap if no matches found
        //SwapTiles(tile1, tile2);

        EnsureNextMatch();
    }

    private void SwapTiles(Tile tile1, Tile tile2)
    {
        TileType tempType = tile1.Type;
        tile1.Type = tile2.Type;
        tile2.Type = tempType;

        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        //FIXME: ugly
        Color tempColor = renderer1.color;
        renderer1.color = renderer2.color;
        renderer2.color = tempColor;

        Sprite temp = renderer2.sprite;
        renderer2.sprite = renderer1.sprite;
        renderer1.sprite = temp;
    }

    private Tile GetTile(int R, int C)
    {
        //if (R < 0 || R >= rows || C < 0 || C >= columns) return null;
        return tiles[R, C].GetComponent<Tile>();
    }

    private void RecursiveMatch(Tile tile)
    {
        var matched = CheckForMatch(tile);
        if (matched is not null)
        {
            foreach (Tile t in matched)
            {
                t.RefreshTileType();
                RecursiveMatch(t);
            }
        }
    }

    private List<Tile>? CheckForMatch(Tile tile)
    {
        var connections = FindConnections(tile);

        const int threshold = 3;
        if (connections.horizontal.Count >= threshold)
        {
            return connections.horizontal;
        }

        if (connections.vertical.Count >= threshold)
        {
            return connections.vertical;
        }

        return null;
    }

    private (Tile first, Tile second)? FindMatch()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Tile t = GetTile(i, j);

                var moves = GetLegalMoves(t);

                foreach (Tile m in moves)
                {
                    SwapTiles(t, m);

                    if (CheckForMatch(m) is not null || CheckForMatch(t) is not null)
                    {
                        SwapTiles(t, m);
                        return (t, m);
                    }

                    SwapTiles(t, m);
                }
            }
        }

        return null;
    }

    private (List<Tile> horizontal, List<Tile> vertical) FindConnections(Tile tile)
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

        return (horizontalConnections, verticalConnections);
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
}
