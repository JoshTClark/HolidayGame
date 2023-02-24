using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public int spawnChance;
    public List<ChestContent> contents = new List<ChestContent>();


    /// <summary>
    /// Checks if the chest should randomly spawn
    /// </summary>
    private void Start()
    {
        int spawn = Random.Range(0, 100);

        if (spawn > spawnChance)
        {
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Does the logic for the player colliding with and opening a chest
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            GameManager.instance.ChestPickup(this);
            this.gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public class ChestContent
    {
        public enum ChestContentType
        {
            RandomAll = 0,
            RandomItem = 1,
            RandomWeapon = 2,
            Preset = 3
        }

        public ChestContentType contentType = ChestContentType.RandomAll;
        public ItemDef presetItem;
    }
}
