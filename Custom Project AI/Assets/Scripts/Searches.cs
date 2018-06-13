using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Searches {
    
    public static Path DijkstraSearch(SparseGraph _graph, int sourceIdx, List<int> targetIdxList)
    {
        //Function variables
        bool stop = false;
        int targetIdx = 0;
        HashSet<int> closed = new HashSet<int>();
        Dictionary<int, int> route = new Dictionary<int, int>();
        List<QueueItem> open = new List<QueueItem>();
        int steps = 0;

        open.Add(new QueueItem(sourceIdx, 0.0));
        route[sourceIdx] = sourceIdx;

        //Loop while still searching
        while (open.Count > 0)
        {
            //Pop first item from queue
            QueueItem currentItem = open[0];
            open.RemoveAt(0);

            steps += 1;
            int leaf = currentItem.item;
            double cost = currentItem.cost;

            closed.Add(leaf);

            //Check if the current leaf is any of the target boxes
            foreach (var idx in targetIdxList)
            {
                if (leaf == idx)
                {
                    stop = true;
                    targetIdx = idx;
                    break;
                }
            }

            if (stop == true)
            {
                break;
            }
            else
            {
                List<int> neighbourIdxs = _graph.GetNeighbours(leaf);
                foreach (var dest in neighbourIdxs)
                {
                    //Make sure the node hasn't been already checked
                    if (!closed.Contains(dest))
                    {
                        double cost_f = cost + _graph.GetEdge(leaf, dest).cost;

                        int index = open.FindIndex(item => item.item == dest);
                        if (index >= 0)
                        {
                            //If node is open see if cost is cheaper or not
                            if (open[index].cost <= cost_f)
                                continue;
                            else
                                open.RemoveAt(index);
                        }

                        route[dest] = leaf;
                        open.Add(new QueueItem(dest, cost_f));
                    }
                }
            }
        }

        return new Path(_graph, route, targetIdx, steps);
    }

    public static Path AStarSearch(SparseGraph _graph, int sourceIdx, int targetIdx)
    {
        //Function variables
        HashSet<int> closed = new HashSet<int>();
        Dictionary<int, int> route = new Dictionary<int, int>();
        List<QueueItem> open = new List<QueueItem>();
        int steps = 0;

        open.Add(new QueueItem(sourceIdx, _graph.CostH(sourceIdx, targetIdx)));
        route[sourceIdx] = sourceIdx;

        //Loop while still searching
        while (open.Count > 0)
        {
            //Pop first queue item
            QueueItem currentItem = open[0];
            open.RemoveAt(0);

            steps += 1;
            int leaf = currentItem.item;
            double cost_f = currentItem.cost;

            closed.Add(leaf);

            //Check if current leaf is target node
            if (leaf == targetIdx)
            {
                break;
            }
            else
            {
                double cost = cost_f - _graph.CostH(leaf, targetIdx);

                List<int> neighbourIdxs = _graph.GetNeighbours(leaf);
                foreach (var dest in neighbourIdxs)
                {
                    if (!closed.Contains(dest))
                    {
                        //Calculate cost factoring in manhattan distance
                        double cost_g = cost + _graph.GetEdge(leaf, dest).cost;
                        double cost_h = _graph.CostH(dest, targetIdx);
                        cost_f = cost_g + cost_h;

                        int index = open.FindIndex(item => item.item == dest);
                        if (index >= 0)
                        {
                            //Compare cost to see if it's cheaper
                            if (open[index].cost <= cost_f)
                                continue;
                            else
                                open.RemoveAt(index);
                        }

                        route[dest] = leaf;
                        open.Add(new QueueItem(dest, cost_f));
                    }
                }
            }

        }

        return new Path(_graph, route, targetIdx, steps);
    }
}
