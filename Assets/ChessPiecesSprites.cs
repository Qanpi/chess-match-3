﻿using Unity.VisualScripting;
using UnityEngine;

public static class ChessPiecesSprites
{
    public static Sprite[] Sprites { get; private set; }
    public static int Length { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Sprites = Resources.LoadAll<Sprite>("Sprites/ChessPieces/");
        Length = Sprites.Length;
    }

} 
