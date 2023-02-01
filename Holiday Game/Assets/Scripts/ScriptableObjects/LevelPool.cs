using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Pool", menuName = Constants.ASSET_MENU_PATH + "Level Pool")]
public class LevelPool: ScriptableObject
{
    [SerializeField]
    public List<LevelData> levels = new List<LevelData>();
}
