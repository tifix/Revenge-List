using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrderInLayer : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer sprite;
    [Tooltip("Leave ticked for moving entities - uncheck to save set order."), SerializeField] bool isSortingUpdating = true;

    void Awake()
    {
        if (sprite == null) //Safeguard for automatic assigning with safeguard against no SpriteRenderer
        {
            if (TryGetComponent<SpriteRenderer>(out SpriteRenderer SR)) sprite = SR;
            else Debug.LogWarning("Sprite Ordering script on object <"+ gameObject.name + "> which has no sprite renderer!");
        }

        sprite.sortingOrder = (int)transform.position.z;
    }

    void Update()
    {
        sprite.sortingOrder = (int)transform.position.z;
        if (!isSortingUpdating) DestroyImmediate(this);              //For static objects, the layer shouldn't update for performance sake - destroy script once redundant
    }
}
