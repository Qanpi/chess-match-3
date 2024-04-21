using UnityEngine;

public class Dot : MonoBehaviour
{
    private void OnEnable()
    {
        Toggle(false);
    }

    public void Toggle(bool visible)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = visible;
    }
}
