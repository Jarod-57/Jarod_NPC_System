using System;
using UnityEngine;

public class GunshotEventManager : MonoBehaviour
{
    public static event Action<Vector3> OnGunshotFired;


    public static void GunshotFired(Vector3 position)
    {
        if (OnGunshotFired != null) OnGunshotFired(position);
    }

}
