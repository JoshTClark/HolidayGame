using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Playable Character", menuName = Constants.ASSET_MENU_PATH + "Playable Character")]
public class PlayableCharacterData : ScriptableObject
{
    [SerializeField]
    public Player prefab;

    [SerializeField]
    public List<Upgrade> inventory = new List<Upgrade>();
}
