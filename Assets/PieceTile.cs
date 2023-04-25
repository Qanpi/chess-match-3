using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class PieceTile : TileBase
{
    private Sprite sprite;
    public void SetRandomSprite()
    {
        sprite = PiecesDatabase.GetRandomPiece();
    }

    public bool Selected = false;

    public void OnEnable()
    {
        SetRandomSprite();
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.flags = TileFlags.LockColor;
        tileData.sprite = sprite;

        if(Selected) {
            tileData.color = Color.red;
        }
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        List<Vector3Int> connected = new();
        DepthFirstSearch(position, tilemap, new List<Vector3Int>(), connected);
        
        if (connected.Count >= 2)
        {
            this.Selected = true;
            foreach (var pos in connected) {
                PieceTile tile = tilemap.GetTile<PieceTile>(pos);

                tile.Selected = true;
                tile.SetRandomSprite();
                Debug.Log("Initiated" + this.sprite.name);
                tilemap.RefreshTile(pos);
            }
        }

        base.RefreshTile(position, tilemap);
    }

    private void DepthFirstSearch(Vector3Int position, ITilemap tilemap, List<Vector3Int> visited, List<Vector3Int> connected)
    {
        PieceTile tile = tilemap.GetTile<PieceTile>(position);

        if (tile == null || visited.Contains(position)|| tile.sprite.name != sprite.name) return;
        visited.Add(position);

        if (tile != this) connected.Add(position);

        Vector3Int[] dirs = new Vector3Int[] { Vector3Int.down, Vector3Int.up, Vector3Int.right, Vector3Int.left };

        foreach (var d in dirs)
        {
            DepthFirstSearch(position - d, tilemap, visited, connected);
        }
    }

    

}
