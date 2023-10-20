// Copyright Cristian Pagán Díaz. All Rights Reserved.

using UnityEngine;

public abstract class ObjectPool : MonoBehaviour
{
    protected abstract void Init(object[] args);
    protected abstract void Spawn();
    protected abstract void Despawn();
}
