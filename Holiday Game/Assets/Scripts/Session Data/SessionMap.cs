<<<<<<< HEAD:Holiday Game/Assets/Scripts/Data Holders/SessionMap.cs
using System.Collections.Generic;
using UnityEngine;

public class SessionMap
{
    public MapNode[][] nodeArr;
    public MapNode startingNode;
    public MapNode endingNode;

    public SessionMap(int length, int maxBranches, float nodeDifference, int iterations)
    {
        // Generating Map
        startingNode = new MapNode();
        startingNode.isEmpty = false;
        endingNode = new MapNode();
        endingNode.isEmpty = false;
        nodeArr = new MapNode[length][];
        for (int y = 0; y < length; y++)
        {
            nodeArr[y] = new MapNode[maxBranches];
            for (int x = 0; x < maxBranches; x++)
            {
                nodeArr[y][x] = new MapNode();
                nodeArr[y][x].level = y;
                nodeArr[y][x].branch = x;
            }
        }

        nodeArr[0][(maxBranches - 1) / 2] = startingNode;
        startingNode.level = 0;
        startingNode.branch = (maxBranches - 1) / 2;
        nodeArr[length - 1][(maxBranches - 1) / 2] = endingNode;
        endingNode.level = length - 1;
        endingNode.branch = (maxBranches - 1) / 2;

        for (int i = 0; i < iterations; i++)
        {
            MapNode prevNode = startingNode;
            float divider = 1.0f / maxBranches;
            float perlinX = Random.Range(0.0f, 10000.0f);
            float perlinY = Random.Range(0.0f, 10000.0f);
            for (int level = 1; level < length - 1; level++)
            {
                float perlin = Mathf.PerlinNoise(perlinX, perlinY);
                int branch = 0;

                while (perlin >= divider * branch)
                {
                    branch++;
                }
                branch--;
                if (nodeArr[level][branch].isEmpty)
                {
                    nodeArr[level][branch].isEmpty = false;
                }

                if (!prevNode.connections.Contains(nodeArr[level][branch]))
                {
                    prevNode.connections.Add(nodeArr[level][branch]);
                }

                prevNode = nodeArr[level][branch];
                perlinX += nodeDifference;
            }
            prevNode.connections.Add(endingNode);
        }
    }

    public class MapNode
    {
        public int level;
        public int branch;
        public bool isEmpty = true;
        public List<MapNode> connections = new List<MapNode>();
        public string scene = "GenericMap";
    }
}
=======
using System.Collections.Generic;
using UnityEngine;

public class SessionMap
{
    public MapNode[][] nodeArr;
    public MapNode startingNode;
    public MapNode endingNode;

    public SessionMap(int length, int maxBranches, float nodeDifference, int iterations)
    {
        // Generating Map
        startingNode = new MapNode();
        startingNode.isEmpty = false;
        endingNode = new MapNode();
        endingNode.isEmpty = false;
        nodeArr = new MapNode[length][];
        for (int y = 0; y < length; y++)
        {
            nodeArr[y] = new MapNode[maxBranches];
            for (int x = 0; x < maxBranches; x++)
            {
                nodeArr[y][x] = new MapNode();
                nodeArr[y][x].level = y;
                nodeArr[y][x].branch = x;
            }
        }

        nodeArr[0][(maxBranches - 1) / 2] = startingNode;
        startingNode.level = 0;
        startingNode.branch = (maxBranches - 1) / 2;
        nodeArr[length - 1][(maxBranches - 1) / 2] = endingNode;
        endingNode.level = length - 1;
        endingNode.branch = (maxBranches - 1) / 2;

        for (int i = 0; i < iterations; i++)
        {
            MapNode prevNode = startingNode;
            float divider = 1.0f / maxBranches;
            float perlinX = Random.Range(0.0f, 10000.0f);
            float perlinY = Random.Range(0.0f, 10000.0f);
            for (int level = 1; level < length - 1; level++)
            {
                float perlin = Mathf.PerlinNoise(perlinX, perlinY);
                int branch = 0;

                while (perlin >= divider * branch)
                {
                    branch++;
                }
                branch--;
                if (nodeArr[level][branch].isEmpty)
                {
                    nodeArr[level][branch].isEmpty = false;
                }

                if (!prevNode.connections.Contains(nodeArr[level][branch]))
                {
                    prevNode.connections.Add(nodeArr[level][branch]);
                }

                prevNode = nodeArr[level][branch];
                perlinX += nodeDifference;
            }
            prevNode.connections.Add(endingNode);
        }
    }

    public class MapNode
    {
        public int level;
        public int branch;
        public bool isEmpty = true;
        public List<MapNode> connections = new List<MapNode>();
    }
}
>>>>>>> main:Holiday Game/Assets/Scripts/Session Data/SessionMap.cs
