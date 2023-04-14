using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static SessionMap;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Image cursor;

    [SerializeField]
    private Camera mainCam;

    [SerializeField]
    private GameObject mapPoint;

    [SerializeField]
    private LineRenderer mapLine;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject mapSelector;

    [SerializeField]
    private Tilemap pathLayer;

    [SerializeField]
    private TileBase tile;

    [SerializeField]
    private GameObject stageInfo, fadeObject;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private TMP_Text levelName, levelGoal;

    [SerializeField]
    private InputActionReference movement;

    [SerializeField]
    private InputAction mouseClick, pressSpace;

    public static SessionManager session;

    private GameObject selectedNode;

    private List<GameObject> nodeObjects;

    private bool followingPlayer = false;
    private bool fading = false;

    private float timer = 0f, animTime = 1.5f;
    private Vector3 playerStartPos;

    Dictionary<int, List<int>> turnPoints = new Dictionary<int, List<int>>();

    private float freeSpeed = 10.0f;

    private static Vector3 prevPlayerPos = new Vector3(0, -5, 0);

    private CamMovement camMovement;

    [SerializeField]
    private RuntimeAnimatorController wizardAnim, knightAnim;

    private void Start()
    {
        // If session is null create a new session and generate a random map
        if (session == null)
        {
            Debug.Log("No session found");
            ResourceManager.Init();
            session = new SessionManager();
            session.GenerateMap(5, 3, 10, 4);
        }

        camMovement = new CamMovement();
        camMovement.cam = mainCam;

        List<MapNode> nodes = session.map.nodes;
        nodeObjects = new List<GameObject>();
        foreach (MapNode m in nodes)
        {
            GameObject point = CreateMapPoint(m);
        }

        selectedNode = null;

        player.GetComponent<Animator>().speed = 0;
        mainCam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        mouseClick.Enable();
        mouseClick.performed += (InputAction.CallbackContext callback) =>
        {
            if (mainCam && !followingPlayer)
            {
                Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit)
                {
                    //Debug.Log("Mouse selection to new map point");
                    if (selectedNode != hit.collider.gameObject && !hit.collider.gameObject.GetComponent<MapPoint>().isLocked && !hit.collider.gameObject.GetComponent<MapPoint>().isComplete)
                    {
                        selectedNode = hit.collider.gameObject;
                        mapSelector.SetActive(true);
                        mapSelector.gameObject.transform.position = selectedNode.transform.position;
                        camMovement.SetTarget(mainCam.transform.position, selectedNode.transform.position);
                        LevelData level = selectedNode.GetComponent<MapPoint>().level;
                        levelName.text = level.name;
                        if (level.isBossLevel)
                        {
                            // If level is boss level show boss display else hide it
                            levelGoal.text = "Defeat the boss";
                        }
                        else if (level.daysToSurvive > 0)
                        {
                            // Display days left to survive
                            levelGoal.text = "Survive for " + level.daysToSurvive + " days";
                        }
                        else if (level.enemiesToDefeat > 0)
                        {
                            // Display enemies remaining
                            levelGoal.text = "Defeat " + level.enemiesToDefeat + " Enemies";
                        }
                        else
                        {
                            // All objective complete
                            levelGoal.text = "Find the exit";
                        }
                    }
                }
            }
        };

        pressSpace.Enable();
        pressSpace.performed += (InputAction.CallbackContext callback) =>
        {
            camMovement.SetTarget(mainCam.transform.position, selectedNode.transform.position);
        };

        if (prevPlayerPos != null)
        {
            player.transform.position = prevPlayerPos;
        }
        if (session.difficulty == 0)
        {
            player.gameObject.transform.position = new Vector2(0, -5);
            prevPlayerPos = new Vector2(0, -5);
        }

        mainCam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);

        if (session != null && session.playerData != null && session.playerData.chosenCharacter != null)
        {
            if (session.playerData.chosenCharacter.index == ResourceManager.CharacterIndex.Wizard)
            {
                player.gameObject.GetComponent<Animator>().runtimeAnimatorController = wizardAnim;
            }
            else if (session.playerData.chosenCharacter.index == ResourceManager.CharacterIndex.Knight)
            {
                player.gameObject.GetComponent<Animator>().runtimeAnimatorController = knightAnim;
            }
        }
    }

    private void OnDisable()
    {
        pressSpace.Disable();
        mouseClick.Disable();
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 cursorPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), mousePos, mainCam, out cursorPos);
        cursor.rectTransform.anchoredPosition = new Vector3(cursorPos.x, cursorPos.y, 0.0f);
        camMovement.Update();

        if (!followingPlayer)
        {
            Vector2 vecControls = movement.action.ReadValue<Vector2>();
            Vector2 camMovement = new Vector2();
            if (vecControls.x < 0)
            {
                camMovement.x = -1 * freeSpeed * Time.deltaTime;
            }
            if (vecControls.x > 0)
            {
                camMovement.x = 1 * freeSpeed * Time.deltaTime;
            }
            if (vecControls.y < 0)
            {
                camMovement.y = -1 * freeSpeed * Time.deltaTime;
            }
            if (vecControls.y > 0)
            {
                camMovement.y = 1 * freeSpeed * Time.deltaTime;
            }

            Vector3 newPos = new Vector3();
            newPos.x = Mathf.Clamp((mainCam.transform.position + new Vector3(camMovement.x, camMovement.y, 0)).x, -20, 20);
            newPos.y = Mathf.Clamp((mainCam.transform.position + new Vector3(camMovement.x, camMovement.y, 0)).y, -6, 25);
            newPos.z = -10;
            mainCam.transform.position = newPos;
        }
        else if (fading)
        {
            if (timer < animTime)
            {
                timer += Time.deltaTime;
                if (timer > animTime)
                {
                    timer = animTime;
                }
                float a = Mathf.Lerp(0, 1f, timer / animTime);
                fadeObject.GetComponent<Image>().color = new Color(0, 0, 0, a);
            }
            else
            {
                ExitScene();
            }
        }
        else if (followingPlayer)
        {
            if (timer < animTime)
            {
                timer += Time.deltaTime;
                if (timer > animTime)
                {
                    timer = animTime;
                }
                player.GetComponent<Animator>().speed = 1f;
                float x = Mathf.Lerp(playerStartPos.x, selectedNode.transform.position.x, timer / animTime);
                float y = Mathf.Lerp(playerStartPos.y, selectedNode.transform.position.y, timer / animTime);

                player.transform.position = new Vector3(x, y, 0);
            }
            else
            {
                player.GetComponent<Animator>().speed = 0f;
                timer = 0f;
                fading = true;
            }
            mainCam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        }

        if (selectedNode == null || followingPlayer)
        {
            stageInfo.SetActive(false);
        }
        else
        {
            stageInfo.SetActive(true);
        }
    }

    private GameObject CreateMapPoint(SessionMap.MapNode node)
    {

        Vector3 pos1 = NodeToCoords(node);
        GameObject point = Instantiate<GameObject>(mapPoint, pos1, Quaternion.identity);
        point.GetComponent<MapPoint>().level = node.levelData;
        point.GetComponent<MapPoint>().isLocked = node.isLocked;
        point.GetComponent<MapPoint>().isComplete = node.isComplete;
        point.GetComponent<MapPoint>().node = node;
        nodeObjects.Add(point);
        foreach (SessionMap.MapNode n in node.connections)
        {
            CreatePath(pos1, NodeToCoords(n));
            LineRenderer line = Instantiate<LineRenderer>(mapLine, pos1, Quaternion.identity);
            line.positionCount = 2;
            line.SetPositions(new Vector3[] { pos1, NodeToCoords(n) });
        }
        return point;
    }

    private Vector3 NodeToCoords(SessionMap.MapNode node)
    {
        Vector3 pos = node.pos * 4;
        pos.z = 0;
        return pos;
    }

    public void StartLevel()
    {
        if (!selectedNode.GetComponent<MapPoint>().isComplete && !selectedNode.GetComponent<MapPoint>().isLocked)
        {
            playerStartPos = player.transform.position;
            followingPlayer = true;
        }
    }

    private void ExitScene()
    {
        prevPlayerPos = player.transform.position;
        session.currentLevel = selectedNode.GetComponent<MapPoint>().level;
        session.currentNode = selectedNode.GetComponent<MapPoint>().node;
        GameManager.session = session;
        SceneManager.LoadScene(selectedNode.GetComponent<MapPoint>().level.scene);
    }

    private void CreatePath(Vector2 m1, Vector2 m2)
    {
        Vector2 currentPos = new Vector2(m1.x, m1.y);
        int turnPoint = Random.Range((int)(m1.y) + 1, (int)(m2.y) - 1);

        if (!turnPoints.ContainsKey((int)m1.y))
        {
            turnPoints.Add((int)m1.y, new List<int>());
        }

        List<int> listPoints = new List<int>();
        turnPoints.TryGetValue((int)m1.y, out listPoints);
        bool isNew = true;
        foreach (int i in listPoints)
        {
            if (turnPoint == i + 1 || turnPoint == i - 1)
            {
                turnPoint = i;
                isNew = false;
                break;
            }
        }
        if (isNew)
        {
            listPoints.Add(turnPoint);
            turnPoints.Remove((int)m1.y);
            turnPoints.Add((int)m1.y, listPoints);
        }

        pathLayer.SetTile(new Vector3Int((int)currentPos.x, (int)currentPos.y, 0), tile);

        while (currentPos.y < turnPoint)
        {
            currentPos.y++;
            pathLayer.SetTile(new Vector3Int((int)currentPos.x, (int)currentPos.y, 0), tile);
        }

        while (currentPos.x < m2.x)
        {
            currentPos.x++;
            pathLayer.SetTile(new Vector3Int((int)currentPos.x, (int)currentPos.y, 0), tile);
        }
        while (currentPos.x > m2.x)
        {
            currentPos.x--;
            pathLayer.SetTile(new Vector3Int((int)currentPos.x, (int)currentPos.y, 0), tile);
        }

        while (currentPos.y < m2.y)
        {
            currentPos.y++;
            pathLayer.SetTile(new Vector3Int((int)currentPos.x, (int)currentPos.y, 0), tile);
        }

        pathLayer.SetTile(new Vector3Int((int)currentPos.x, (int)currentPos.y, 0), tile);
    }

    private void OnDrawGizmos()
    {
        /*
        if (session != null)
        {
            SessionMap.MapNode[][] nodes = session.map.nodeArr;
            for (int y = 0; y < nodes.Length; y++)
            {
                for (int x = 0; x < nodes[y].Length; x++)
                {
                    if (!nodes[y][x].isEmpty)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawSphere(new Vector3(x, y, 0), 0.25f);
                        Gizmos.color = Color.blue;
                        foreach (SessionMap.MapNode node in nodes[y][x].connections)
                        {
                            Gizmos.DrawLine(new Vector3(x, y, 0), new Vector3(node.branch, node.level, 0));
                        }
                    }
                    else 
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(new Vector3(x, y, 0), 0.25f);
                    }
                }
            }
        }
        */
    }

    public class CamMovement
    {
        public float animTime = 0.75f;
        public float time = 0f;
        public Vector2 target;
        public Vector2 start;
        public bool isAnimating = false;
        public Camera cam;

        public void Update()
        {
            if (isAnimating)
            {
                time += Time.deltaTime;

                if (time < animTime)
                {
                    Vector3 newPos = new Vector3(0, 0, cam.gameObject.transform.position.z);
                    newPos.x = Mathf.SmoothStep(start.x, target.x, time / animTime);
                    newPos.y = Mathf.SmoothStep(start.y, target.y, time / animTime);
                    cam.gameObject.transform.position = newPos;
                }
                else
                {
                    cam.gameObject.transform.position = new Vector3(target.x, target.y, cam.gameObject.transform.position.z);
                    isAnimating = false;
                }
            }
        }

        public void SetTarget(Vector2 start, Vector2 target)
        {
            this.target = target;
            this.start = start;
            time = 0;
            isAnimating = true;
        }
    }
}
