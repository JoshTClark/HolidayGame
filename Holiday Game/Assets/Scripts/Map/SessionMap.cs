using System.Collections.Generic;
using UnityEngine;

public class SessionMap
{
    //public MapNode[][] nodeArr;
    public MapNode startingNode;
    public MapNode endingNode;
    public List<MapNode> nodes = new List<MapNode>();

    /// <summary>
    /// Creates a new map with the given parameters
    /// </summary>
    /// <param name="length">The number of levels in the map including the boss and starting stage. Min = 2</param>
    /// <param name="width">The number of possible level choices possible to the left and right of the starting point.
    /// Minimum x of level is -width. Maximum x of a level is +width.
    /// </param>
    /// <param name="nodeDifference">Used in the randomness calculation to determin where the next stage will be.</param>
    /// <param name="iterations">Number of paths to possible to take. 
    /// Specifically the number of times a path is created that starts at the starting stage and ends at the boss stage.
    /// </param>
    public SessionMap(int length, int width, float nodeDifference, int iterations)
    {
        int trueWidth = (width * 2) + 1;

        //Starting and Ending points
        startingNode = new MapNode();
        startingNode.pos = new Vector2(0, 0);
        endingNode = new MapNode();
        endingNode.pos = new Vector2(0, length - 1);
        nodes.Add(startingNode);
        nodes.Add(endingNode);

        /*
        nodeArr = new MapNode[length][];
        for (int y = 0; y < length; y++)
        {
            nodeArr[y] = new MapNode[width];
            for (int x = 0; x < width; x++)
            {
                nodeArr[y][x] = new MapNode();
                nodeArr[y][x].difficulty = y;
                nodeArr[y][x].branch = x;
            }
        }
        nodeArr[0][(width - 1) / 2] = startingNode;
        startingNode.difficulty = 0;
        startingNode.branch = (width - 1) / 2;
        startingNode.isLocked = false;
        nodeArr[length - 1][(width - 1) / 2] = endingNode;
        endingNode.difficulty = length - 1;
        endingNode.branch = (width - 1) / 2;
        */

        for (int i = 0; i < iterations; i++)
        {
            MapNode prevNode = startingNode;
            float perlinX = Random.Range(0.0f, 10000.0f);
            float perlinY = Random.Range(0.0f, 10000.0f);
            for (int y = 1; y < length - 1; y++)
            {
                float perlin = Mathf.Clamp(Mathf.PerlinNoise(perlinX, perlinY), 0f, 1f);
                float x = perlin * trueWidth + 1;
                x -= ((trueWidth + 1) / 2f);
                x = Mathf.Round(x);

                MapNode node = new MapNode();
                node.pos = new Vector2(x, y);
                /*
                if (nodeArr[y][branch].isEmpty)
                {
                    nodeArr[y][branch].isEmpty = false;
                }

                if (!prevNode.connections.Contains(nodeArr[y][branch]))
                {
                    prevNode.connections.Add(nodeArr[y][branch]);
                }
                */
                prevNode.connections.Add(node);
                nodes.Add(node);
                prevNode = node;
                perlinX += nodeDifference;
            }
            prevNode.connections.Add(endingNode);
        }

        foreach (MapNode node in nodes)
        {
            LevelPool pool = ResourceManager.levelPools[0];
            int ranIndex = Random.Range(0, pool.levels.Count);
            node.levelData = ResourceManager.levelPools[0].levels[ranIndex];
        }

        startingNode.levelData = ResourceManager.GetLevelFromSceneName("Tutorial");
        startingNode.isLocked = false;
    }

    public class MapNode
    {
        public int difficulty;
        //public int branch;
        public Vector2 pos = new Vector2();
        //public bool isEmpty = true;
        public List<MapNode> connections = new List<MapNode>();
        public LevelData levelData;
        public bool isComplete = false;
        public bool isLocked = true;
    }
}
