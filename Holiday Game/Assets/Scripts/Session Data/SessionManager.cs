using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager
{
    public SessionMap map;
    private PlayerData playerData = new PlayerData();
    public LevelData currentLevel;
    public SessionMap.MapNode currentNode;
    public static GameData data;
    public static int money;
    public int difficulty = 0;

    public void SavePlayerData(Player player)
    {
        foreach (Upgrade i in player.inventory)
        {
            playerData.inventory.Add(i);
        }
    }

    public void SetChosenCharacter(PlayableCharacterData chosenCharacter)
    {
        Debug.Log("Setting Character: " + chosenCharacter.prefab.name);
        if (!playerData.chosenCharacter)
        {
            playerData.chosenCharacter = chosenCharacter;
            playerData.inventory = new List<Upgrade>();
            foreach (Upgrade i in chosenCharacter.inventory)
            {
                playerData.inventory.Add(i);
            }
        }
    }

    public void GenerateMap(int length, int maxBranches, float nodeDifference, int iterations)
    {
        map = new SessionMap(length, maxBranches, nodeDifference, iterations);
    }

    public Player GetPlayerInstance()
    {
        Player player;
        if (!playerData.chosenCharacter)
        {
            SetChosenCharacter(ResourceManager.characters[0]);
        }
        player = GameObject.Instantiate<Player>(playerData.chosenCharacter.prefab);
        foreach (Upgrade i in playerData.inventory)
        {
            player.inventory.Add(i);
        }
        player.SetXPWithoutLevelUp(playerData.experience);
        return player;
    }

    public void LevelComplete(int gems, Player player)
    {
        difficulty++;
        playerData.inventory.Clear();
        foreach (Upgrade u in player.inventory)
        {
            playerData.inventory.Add(u);
        }
        playerData.experience = player.XP;
        data.currency += gems;
        currentLevel = null;
        currentNode.isComplete = true;
        for (int i = 0; i < map.nodeArr.Length; i++)
        {
            foreach (SessionMap.MapNode n in map.nodeArr[i])
            {
                if (!n.isComplete && !n.isLocked)
                {
                    n.isLocked = true;
                }
            }
        }
        foreach (SessionMap.MapNode n in currentNode.connections)
        {
            n.isLocked = false;
        }
        MapManager.session = this;
        SaveManager.SaveFile(data.id, data);
        SceneManager.LoadSceneAsync("MapScene");
    }

    public void ToTitle()
    {
        SaveManager.SaveFile(data.id, data);
        SceneManager.LoadSceneAsync("StartScene");
    }

    private struct PlayerData
    {
        public PlayableCharacterData chosenCharacter;
        public List<Upgrade> inventory;
        public float experience;
    }
}
