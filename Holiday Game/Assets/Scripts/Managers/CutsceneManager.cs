using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager
{
    private Cutscene currentScene = Cutscene.None;
    private List<GameObject> sceneObjects = new List<GameObject>();
    private ArrayList storedObjects = new ArrayList();
    private float timer = 0f;

    public enum Cutscene
    {
        None,
        BossGemScene
    }

    public void Update()
    {
        switch (currentScene)
        {
            case Cutscene.BossGemScene:
                // Getting params from the params list
                Player player = null;
                BossGem bossGem = null;
                Camera cam = null;
                foreach (GameObject i in sceneObjects)
                {
                    if (i.GetComponent<Player>())
                    {
                        player = i.GetComponent<Player>();
                    }
                    else if (i.GetComponent<BossGem>())
                    {
                        bossGem = i.GetComponent<BossGem>();
                    }
                    else if (i.GetComponent<Camera>())
                    {
                        cam = i.GetComponent<Camera>();
                    }
                }

                // Needed variables for the cutscene
                Vector3 gemPos = bossGem.gameObject.transform.position;
                Vector3 playerPos = player.gameObject.transform.position;
                Vector3 camPos = cam.gameObject.transform.position;
                Vector3 targetPosition = new Vector3(playerPos.x, playerPos.y + 3.5f, 0);
                Vector3 camOrigPos = (Vector3)(storedObjects[0]);
                Vector3 gemOrigPos = (Vector3)(storedObjects[1]);
                float gemScale = 3.5f;

                // Update the timer
                timer += Time.deltaTime;

                // Moving camera
                float camMoveTime = 1f;
                if (timer <= camMoveTime)
                {
                    Vector3 newCamPos = new Vector3();
                    newCamPos.x = targetPosition.x;
                    newCamPos.z = camPos.z;
                    newCamPos.y = Mathf.SmoothStep(camOrigPos.y, targetPosition.y, timer/camMoveTime);

                    // Setting positions
                    cam.transform.position = newCamPos;
                }

                // Moving gems
                float gemMoveTime = 4f;
                if (timer <= gemMoveTime)
                {
                    Vector3 newGemPos = new Vector3();
                    newGemPos.x = Mathf.SmoothStep(gemOrigPos.x, targetPosition.x, timer / gemMoveTime);
                    newGemPos.z = 0f;
                    newGemPos.y = Mathf.SmoothStep(gemOrigPos.y, targetPosition.y, timer / gemMoveTime);

                    Vector3 newGemScale = new Vector3();
                    newGemScale.x = Mathf.SmoothStep(1f, gemScale, timer / gemMoveTime);
                    newGemScale.y = Mathf.SmoothStep(1f, gemScale, timer / gemMoveTime);
                    newGemScale.z = 1f;

                    // Setting positions
                    bossGem.gameObject.transform.position = newGemPos;
                    bossGem.gameObject.transform.localScale = newGemScale;
                }
                break;
        }
    }

    /// <summary>
    /// Starts a cutscene
    /// </summary>
    /// <param name="cutscene">The cutscene to start</param>
    /// <param name="gameObjects">Required params for the cutscene</param>
    public void DoCutscene(Cutscene cutscene, params GameObject[] gameObjects)
    {
        switch (cutscene)
        {
            case Cutscene.BossGemScene:
                timer = 0;
                storedObjects.Clear();
                sceneObjects.Clear();
                sceneObjects.AddRange(gameObjects);
                currentScene = cutscene;
                foreach (GameObject i in sceneObjects)
                {
                    if (i.GetComponent<Camera>())
                    {
                        storedObjects.Add(new Vector3(i.transform.position.x, i.transform.position.y, i.transform.position.z));
                    }
                }
                foreach (GameObject i in sceneObjects)
                {
                    if (i.GetComponent<BossGem>())
                    {
                        i.AddComponent<Vibrate>();
                        storedObjects.Add(new Vector3(i.transform.position.x, i.transform.position.y, i.transform.position.z));
                    }
                }
                break;
        }
    }
}
