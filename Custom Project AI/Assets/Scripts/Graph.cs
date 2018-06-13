using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Edge
{
    public int fromIdx;
    public int toIdx;
    public double cost;

    public Edge(int _fromIdx = -1, int _toIdx = -1, double _cost = 0.0)
    {
        fromIdx = _fromIdx;
        toIdx = _toIdx;
        cost = _cost;
    }
}

public class Node
{
    public int idx;

    public Node(int _idx = -1)
    {
        idx = _idx;
    }
}


public class SparseGraph {

    public Dictionary<int, Node> nodes = new Dictionary<int, Node>();
    public Dictionary<int, Dictionary<int, Edge>> edgeList = new Dictionary<int, Dictionary<int, Edge>>();

    public bool diagraph;
    public int nextNodeIdx = 0;

    public SparseGraph(bool _diagraph = true)
    {
        diagraph = _diagraph;
    }

    public double CostH(int idxOne, int idxTwo)
    {
        double x1 = GameInfo.tiles[idxOne].transform.position.x;
        double y1 = GameInfo.tiles[idxOne].transform.position.y;
        
        double x2 = GameInfo.tiles[idxTwo].transform.position.x;
        double y2 = GameInfo.tiles[idxTwo].transform.position.y;

        return (System.Math.Abs(x1 - x2) + System.Math.Abs(y1 - y2)) * 1.0;
    }

    public double PathCost(List<int> _path)
    {
        double result = 0;

        for (int n = 1; n < _path.Count - 2; n++)
        {
            int i = _path[n];
            int j = _path[n + 1];

            result += GetEdge(i, j).cost;
        }

        return result;
    }

    public List<int> GetNeighbours(int nodeIdx)
    {
        List<int> keys = new List<int>(edgeList[nodeIdx].Keys);
        keys.Sort();

        return keys;
    }

    public Edge GetEdge(int fromIdx, int toIdx)
    {
        if (edgeList.ContainsKey(fromIdx))
        {
            if (edgeList[fromIdx].ContainsKey(toIdx))
            {
                return edgeList[fromIdx][toIdx];
            }
        }

        return null;
    }

    public Node AddNode(Node node)
    {
        if (node.idx < 0)
        {
            node.idx = nextNodeIdx;
        }
        nextNodeIdx = node.idx + 1;

        nodes[node.idx] = node;
        edgeList[node.idx] = new Dictionary<int, Edge>();

        return node;
    }

    public void AddEdge(Edge edge)
    {
        if (!(nodes.ContainsKey(edge.fromIdx) && nodes.ContainsKey(edge.toIdx)))
            return;
        Dictionary<int, Edge> edgeInfo = new Dictionary<int, Edge>();
        if (edgeList.ContainsKey(edge.fromIdx))
        {
            edgeInfo = edgeList[edge.fromIdx];
            edgeInfo.Add(edge.toIdx, edge);
            edgeList[edge.fromIdx] = edgeInfo;
        }
        else
        {
            edgeInfo.Add(edge.toIdx, edge);
            edgeList.Add(edge.fromIdx, edgeInfo);
        }

    }
}
