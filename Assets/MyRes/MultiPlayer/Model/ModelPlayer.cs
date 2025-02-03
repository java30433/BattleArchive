using System.Collections.Generic;
using UnityEngine;

class ModelPlayer
{
    public Vector3 Position;
    public float RotationY;
    public float Speed;
    public bool IsAiming;
    public bool IsReloading;
    public short Health;
    public bool NeedDestroy;

    public List<BasePlayerEvent> EventList = new();
}