using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo {

    public static int collectedCount;
    public static List<GameObject> enemies = new List<GameObject>();

    public static List<string> tileKinds = new List<string>
    {
        "stone", //st
        "grass", //g
        "tainted_grass", //tg
        "dirt", //d
        "sand", //sa
        "swamp", //sw
        "water", //wt
        "wall", //wl
    };

    private static double None = double.NaN;
    private static List<List<double>> edgeCostMatrix = new List<List<double>>
    {                       //st   //g    //tg   //d    //sa   //sw   //wt  //wl
        new List<double> {  1.0,   2.0,   2.0,   3.0,   4.0,   6.0,   8.0,  None }, //st
        new List<double> {  2.0,   4.0,   4.0,   5.0,   6.0,   8.0,  10.0,  None }, //g
        new List<double> {  2.0,   4.0,   4.0,   5.0,   6.0,   8.0,  10.0,  None }, //tg
        new List<double> {  3.0,   5.0,   5.0,   6.0,   7.0,   9.0,  11.0,  None }, //d
        new List<double> {  4.0,   6.0,   6.0,   7.0,   8.0,  10.0,  12.0,  None }, //sa
        new List<double> {  6.0,   8.0,   8.0,   9.0,  10.0,  12.0,  14.0,  None }, //sw
        new List<double> {  8.0,  10.0,  10.0,  11.0,  12.0,  14.0,  16.0,  None }, //wt
        new List<double> { None,  None,  None,  None,  None,  None,  None,  None }, //wl
    };

    //Variables for searching and graphing
    public static int width;
    public static int height;
    public static Vector3 middlePosition;
    public static Vector3 basePosition;
    public static List<GameObject> tiles = new List<GameObject>();
    public static List<GameObject> coinTiles = new List<GameObject>();
    public static Vector3 startPosition;
    public static Path path = null;
    private static SparseGraph graph;

    public static void Reset()
    {
        coinTiles.Clear();
        tiles.Clear();
    }

    private static double EdgeCost(string kindOne, string kindTwo)
    {
        int indexOne = tileKinds.FindIndex(x => x == kindOne);
        int indexTwo = tileKinds.FindIndex(x => x == kindTwo);

        //Get edge cost for specific tile kinds
        return edgeCostMatrix[indexOne][indexTwo];
    }

    private static void AddEdge(int fromIdx, int toIdx, double distance = 1.0f)
    {
        Tile fromTile = tiles[fromIdx].GetComponent<Tile>();
        Tile toTile = tiles[toIdx].GetComponent<Tile>();

        if (toTile.kind != "wall")
        {
            //Add edge to graph info
            double cost = EdgeCost(fromTile.kind, toTile.kind);
            graph.AddEdge(new Edge(fromIdx, toIdx, cost * distance));
        }
    }

    public static void ResetGraph()
    {
        //Reset path and graph to nothing
        path = null;
        graph = new SparseGraph();

        //Loop through each tile and add a node to the graph
        foreach (var tileObject in tiles)
        {
            int index = tiles.FindIndex(x => x == tileObject);
            Tile tile = tileObject.GetComponent<Tile>();

            tile.node = graph.AddNode(new Node(index));
        }

        //Loop through each tile and add each edge to the graph
        foreach (var tileObject in tiles)
        {
            int index = tiles.FindIndex(x => x == tileObject);
            Tile tile = tileObject.GetComponent<Tile>();
            if (tile.kind == "wall")
                continue;

            //Tile above
            if (index + width < tiles.Count)
                AddEdge(index, index + width);
            //Tile below
            if (index - width >= 0)
                AddEdge(index, index - width);
            //Tile right
            if ((index % width) + 1 < width)
                AddEdge(index, index + 1);
            //Tile left
            if ((index % width) - 1 >= 0)
                AddEdge(index, index - 1);

            double dist = System.Math.Sqrt(1 + 1);
            int j = index + width;
            //Tile above left
            if (j - 1 < tiles.Count && (j % width) - 1 >= 0)
                AddEdge(index, j - 1, dist);
            //Tile above right
            if (j + 1 < tiles.Count && (j % width) + 1 < width)
                AddEdge(index, j + 1, dist);

            int k = index - width;
            //Tile below left
            if (k - 1 >= 0 && (k % width) - 1 >= 0)
                AddEdge(index, k - 1, dist);
            //Tile below right
            if (k + 1 >= 0 && (k % width) + 1 < width)
                AddEdge(index, k + 1, dist);
        }
    }

    public static void PlanPath()
    {
        int startIdx = 0;
        List<int> targetIdxList = new List<int>();

        //Find the idx of the start tile
        foreach (var tile in tiles)
        {
            if (tile.transform.position == startPosition)
            {
                Tile t = tile.GetComponent<Tile>();
                startIdx = t.id;
            }
        }

        //Get the idxs of each collectable tile
        foreach (var coinTile in coinTiles)
        {
            Tile t = coinTile.GetComponent<Tile>();

            targetIdxList.Add(t.id);
        }

        //Search to the item and then to start
        Path toItem = Searches.DijkstraSearch(graph, startIdx, targetIdxList);
        Path toStart = Searches.AStarSearch(graph, toItem.targetIdx, startIdx);

        //Combine paths
        toItem.Add(toStart);
        path = toItem;
    }
}
