using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Playable Character", menuName = Constants.ASSET_MENU_PATH + "Playable Character")]
public class PlayableCharacterData : ScriptableObject
{
    [SerializeField]
    public Player prefab;

    [SerializeField]
    public string characterName;

    [SerializeField, TextArea(15, 20)]
    public string desc;

    [SerializeField]
    public Sprite characterImage;

    [SerializeField]
    public List<ItemDef> inventory = new List<ItemDef>();
}
