using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "DrunkState", menuName = "DrunkStates/Create Drunk State", order = 1)]
public class DrunkState : ScriptableObject
{
    public enum State
    {
        Normal = 0,
        Tipsy = 1,
        Boozy = 2,
        Drunk = 3,
        Hammered = 4
    }

    public double minBoundary;
    public double maxBoundary;
    public State state;
    public VolumeProfile profile;
    public float minLD;
    public float maxLD;
    public float minDOF;
    public float maxDOF;
    public float minMB;
    public float maxMB;

    public bool IsInBoundary(float value)
    {
        double rounded = Math.Round(value, 1);
        return rounded >= minBoundary && rounded <= maxBoundary;
    }


}
public static class IComparableExtension
{
    public static bool InRange<T>(this T value, T from, T to) where T : IComparable<T>
    {
        return value.CompareTo(from) >= 1 && value.CompareTo(to) <= -1;
    }
}