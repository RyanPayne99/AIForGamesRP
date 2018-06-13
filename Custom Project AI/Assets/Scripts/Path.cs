using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueItem
{
    public int item;
    public double cost;

    public QueueItem(int _item, double _cost)
    {
        item = _item;
        cost = _cost;
    }
}

public class Path
{
    //Public variables
    public int targetIdx;
    public int midpoindIdx;
    public List<int> pathPoints = new List<int>();
    public string pathCost;
    public string result;

    //Local variables
    Dictionary<int, int> route = new Dictionary<int, int>();
    int sourceIdx;

    public Path(SparseGraph _graph, Dictionary<int, int> _route, int _targetIdx, int _steps)
    {
        //Store everything in the class varaibles
        route = _route;
        targetIdx = _targetIdx;

        //Check if path was complete
        if (route.ContainsKey(targetIdx))
        {
            List<int> _pathPoints = new List<int>();
            int currentIdx = targetIdx;

            while (currentIdx != route[currentIdx])
            {
                _pathPoints.Add(currentIdx);
                currentIdx = route[currentIdx];
            }

            _pathPoints.Add(currentIdx);
            _pathPoints.Reverse();

            result = "Success!";
            sourceIdx = currentIdx;
            pathPoints = _pathPoints;
            pathCost = _graph.PathCost(pathPoints).ToString();
        }
        else
        {
            result = "Failed!";
            pathCost = "~";
        }
    }

    public void Add(Path p)
    {
        //Combine info from two paths
        pathPoints.AddRange(p.pathPoints);
        pathCost = (System.Convert.ToDouble(p.pathCost) + System.Convert.ToDouble(p.pathCost)).ToString();
        midpoindIdx = targetIdx;
        targetIdx = p.targetIdx;
        result = p.result;
    }
}
