using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum TileType {
    Bishop, 
    King,
    Knight, 
    Pawn, 
    Queen, 
    Rook
}

public class Tile : MonoBehaviour
{
    public int X;
    public int Y;
    private int typeId;
    public TileType Type { get => (TileType)typeId; set => typeId = (int)value; }

    public event EventHandler OnClick;

    public void RefreshTileType()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Type = (TileType)UnityEngine.Random.Range(0, SpritesDatabase.Length);
        spriteRenderer.sprite = SpritesDatabase.Sprites[typeId];
    }

    private void OnEnable()
    {
        RefreshTileType(); 
    }

    public bool Selected = false;


    private void OnMouseDown()
    {
        OnClick?.Invoke(this, EventArgs.Empty);
    }

}
