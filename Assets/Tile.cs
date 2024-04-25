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
    [SerializeField] private Dot dotPrefab;
    private Dot dot;

    //do i need these coords? 
    public int X;
    public int Y;

    private int typeId;
    public TileType Type { get => (TileType)typeId; set => typeId = (int)value; }

    public event EventHandler OnClick;

    public void RefreshTileType()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        //TODO: weighted random for tile types
        Type = (TileType)UnityEngine.Random.Range(0, ChessPiecesSprites.Length);
        spriteRenderer.sprite = ChessPiecesSprites.Sprites[typeId];

        Color color; 
        switch(Type)
        {
            case TileType.Bishop:
                color = Color.red;
                break;
            case TileType.King:
                color = Color.yellow;
                break;
            default:
                color = Color.white;
                break;
        }

        spriteRenderer.color = color;
    }

    public void SetHint(bool visible)
    {
        dot.SetEnabled(visible);
    }

    private void OnEnable()
    {
        RefreshTileType();

        dot = Instantiate(dotPrefab, this.transform); 
    }

    private void OnMouseDown()
    {
        OnClick?.Invoke(this, EventArgs.Empty);
    }

    override public string ToString()
    {
        return $"{Type} at row {Y}, column {X}";
    }

}
