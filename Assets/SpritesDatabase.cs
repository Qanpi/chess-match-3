using Unity.VisualScripting;
using UnityEngine;

public static class SpritesDatabase
{
    public static Sprite[] Sprites { get; private set; }
    public static int Length { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Sprites = Resources.LoadAll<Sprite>("Sprites/");
        Length = Sprites.Length;
    }

} 
