using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsToEarn : MonoBehaviour
{
    public int points { get { return _points; } }

    [SerializeField] private int _points;
}
