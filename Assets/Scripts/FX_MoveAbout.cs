using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FX_MoveAbout : MonoBehaviour
{
    [SerializeField] AnimationCurve movementX, movementY, movementZ;
    public float speed=1, amplitude=1;
    Vector3 originPosition=Vector3.zero;

    private void Awake()
    {
        originPosition = transform.localPosition;
    }

    void Update()
    {
        transform.position = originPosition + new Vector3  (movementX.Evaluate(Time.time * speed) * amplitude,
                                                            movementY.Evaluate(Time.time * speed) * amplitude,
                                                            movementZ.Evaluate(Time.time * speed) * amplitude);
    }
}
