using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventObject : ObjectScript
{
    public UnityEvent whenKilledDo = new UnityEvent();
    public UnityEvent whenDamagedDo = new UnityEvent();

    protected override void DeleteObject() 
    {
        whenKilledDo.Invoke();
        Destroy(gameObject);
    }

    public override void ApplyDamage(float _value)
    {
        whenDamagedDo.Invoke();
        base.ApplyDamage(_value);
    }
}
