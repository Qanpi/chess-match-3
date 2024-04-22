using UnityEngine;

public class Dot : MonoBehaviour
{
    private void OnEnable()
    {
        SetEnabled(false);
    }

    public void SetEnabled(bool visible)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = visible;
    }
}
