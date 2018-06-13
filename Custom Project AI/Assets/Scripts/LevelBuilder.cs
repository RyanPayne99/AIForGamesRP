using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBuilder : MonoBehaviour {
    //Public variables
    public int width, height;

    //Local variables
    UIManager uiManager;
    OpenSimplexNoise genE, genM1, genM2;
    double e, m1, m2;

    private void Start()
    {
        GameInfo.width = width;
        GameInfo.height = height;

        GameInfo.middlePosition = new Vector3(width, height, 0);
        GameInfo.basePosition = new Vector3(width, height + 4, 0);
        GameInfo.startPosition = new Vector3(width, height + 2, 0);

        uiManager = gameObject.GetComponent<UIManager>();
    }

    public void BuildLevel() {
        GenerateLevelInfo();
        uiManager.Initiate();
        GameInfo.ResetGraph();
        //SpawnEnemies();
    }

    public void ResetLevel()
    {
        if (GameObject.Find("Game Container")) DestroyImmediate(GameObject.Find("Game Container"));
        GameInfo.Reset();
    }

    private double NoiseE(double nx, double ny) { return genE.Evaluate(nx, ny) / 2 + 0.5; }
    private double NoiseM1(double nx, double ny) { return genM1.Evaluate(nx, ny) / 2 + 0.5; }
    private double NoiseM2(double nx, double ny) { return genM2.Evaluate(nx, ny) / 2 + 0.5; }

    private void GenerateLevelInfo()
    {
        //General allocations
        var rndE = new System.Random(DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute);
        var rndM1 = new System.Random(DateTime.Now.Millisecond * DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute);
        var rndM2 = new System.Random(DateTime.Now.Millisecond * DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Hour);

        genE = new OpenSimplexNoise(rndE.Next());
        genM1 = new OpenSimplexNoise(rndM1.Next());
        genM2 = new OpenSimplexNoise(rndM2.Next());

        ResetLevel();

        GameObject gameContainer = new GameObject("Game Container");

        //Loop for generating the tiles
        for (double i = 1; i <= height; i++)
        {
            for (double j = 1; j <= width; j++)
            {
                var position = new Vector3((float)j * 2, (float)i * 2, 0);
                var rotation = new Quaternion(0, 0, 0, 0);
                GameObject tile = Instantiate((GameObject)Resources.Load("Prefabs/Tile", typeof(GameObject)), position, rotation, gameContainer.transform);

                GenerateNoise(i, j);
                tile.GetComponent<Tile>().Initiate(GetTileKind(), ExtrasCheck());

                GameInfo.tiles.Add(tile);
            }
        }


        int count = 0;
        foreach (var tile in GameInfo.tiles)
        {
            Tile tileInfo = tile.GetComponent<Tile>();

            tileInfo.id = count;
            count++;

            if (tileInfo.content == "coin")
            {
                GameInfo.coinTiles.Add(tile);
            }
        }
    }

    private string GetTileKind()
    {
        if (e < 0.03) return "water";
        if (e < 0.10)
        {
            if (m1 < 0.03) return "swamp";
            return "sand";
        }
        

        if (m2 < 0.01) return "tainted_grass";
        if (m2 > 0.42) return "dirt";
        return "grass";
    }

    private void GenerateNoise(double i, double j)
    {
        double nx = j / width - 0.5;
        double ny = i / height - 0.5;

        e = NoiseE(2.5 * nx, 2.5 * ny);
        e = Math.Pow(e, 3.00);

        m1 = NoiseM1(2.5 * nx, 2.5 * ny);
        m1 = Math.Pow(m1, 3.00);

        m2 = NoiseM2(2.5 * nx, 2.5 * ny);
        m2 = Math.Pow(m2, 3.00);
    }

    private string ExtrasCheck()
    {
        float chance = UnityEngine.Random.value;

        if (chance < 0.005) return "coin";
        else if (chance < 0.015) return "wall";

        return "none";
    }

    public void SpawnEnemies()
    {
        foreach (var enemy in GameInfo.enemies)
        {
            Destroy(enemy);
        }
        GameInfo.enemies.Clear();

        for (var i = 0; i < 10; i++)
        {
            float chance = UnityEngine.Random.value;
            string prefab = "";
            if (chance < 0.75)
                prefab = "Prefabs/Zombie Type 1";
            else
                prefab = "Prefabs/Zombie Type 2";

            GameObject enemy = Instantiate((GameObject)Resources.Load(prefab, 
                typeof(GameObject)), GameObject.Find("Game Container").transform);
            enemy.transform.position = new Vector3(UnityEngine.Random.Range(0, GameInfo.width * 2), UnityEngine.Random.Range(0, GameInfo.height * 2));
            GameInfo.enemies.Add(enemy);
        }
    }
}
