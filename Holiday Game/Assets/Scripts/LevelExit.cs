using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    //List of all possible exit locations
    [SerializeField] public List<Vector2> ExitLocations;

    private void Start()
    {
        
        RandomizeExit();
       
    }

    //Randomly sets position of exit to any of possible locations
    public void RandomizeExit()
    {
        int choice = Random.Range(0, ExitLocations.Count);
        this.transform.position = ExitLocations[choice];

    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            GameManager.instance.EndLevel();
        }
    }
}
