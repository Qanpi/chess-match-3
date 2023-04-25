using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class PiecesDatabase
{
    public static Sprite[] Sprites { get; private set; }
    public static int Length { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Sprites = Resources.LoadAll<Sprite>("Sprites/");
        Length = Sprites.Length;
    }
    public static Sprite GetRandomPiece()
    {
        return Sprites[Random.Range(0, Length)];
    }
} 

