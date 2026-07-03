using UnityEngine;

public class RefreshLevel1Colliders : MonoBehaviour
{
    private BoxCollider2D[] colliders;

    private void Start()
    {
        colliders = FindObjectsByType<BoxCollider2D>(FindObjectsSortMode.None);
    }

    private void LateUpdate()
    {
        foreach (BoxCollider2D box in colliders)
        {
            if (box == null)
                continue;

            box.enabled = false;
            box.enabled = true;
        }
    }
}