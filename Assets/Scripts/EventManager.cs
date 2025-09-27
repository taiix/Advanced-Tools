using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent<List<Vector3>> OnPathReceived = new();
}
