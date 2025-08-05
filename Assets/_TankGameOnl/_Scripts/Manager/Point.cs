using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Point : ZuSingleton<Point>
{
    public List<Transform> points;

    void Start() => this.LoadPoints();
    void Reset() => this.LoadPoints();


    protected virtual void LoadPoints()
    {
        if (this.points.Count > 0) return;

        foreach (Transform point in transform)
        {
            this.points.Add(point);
        }
    }

    public virtual Transform GetRandomPoint()
    {
        int index = Random.Range(0, points.Count);
        return points[index];
    }
}
