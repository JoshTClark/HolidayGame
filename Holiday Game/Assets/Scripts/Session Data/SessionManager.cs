using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SessionManager
{
    public SessionMap map;
    private PlayerData playerData = new PlayerData();

    public void SavePlayerData(Player player)
    {
        playerData.inventory = player.inventory;
    }

    public void SetChosenCharacter(PlayableCharacterData chosenCharacter)
    {
        if (!playerData.chosenCharacter)
        {
            playerData.chosenCharacter = chosenCharacter;
            playerData.inventory = chosenCharacter.inventory;
        }
    }

    public void GenerateMap(int length, int maxBranches, float nodeDifference, int iterations)
    {
        map = new SessionMap(length, maxBranches, nodeDifference, iterations);
    }

    public Player GetPlayerInstance()
    {
        Player player = GameObject.Instantiate<Player>(playerData.chosenCharacter.prefab);
        player.inventory = playerData.inventory;
        return player;
    }

    private struct PlayerData
    {
        public PlayableCharacterData chosenCharacter;
        public List<Upgrade> inventory;
    }
}
