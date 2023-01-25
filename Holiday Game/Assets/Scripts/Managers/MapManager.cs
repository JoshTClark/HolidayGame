using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static SessionMap;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCam;

    [SerializeField]
    private GameObject mapPoint;

    [SerializeField]
    private LineRenderer mapLine;

    [SerializeField]
    private InputActionReference movement;

    [SerializeField]
    private InputAction mouseClick;

    public static SessionManager session;

    private GameObject selectedNode;

    private Vector2 previousInput;

    private GameObject[][] nodeArr;

    private float camSpeed = 10.0f;
    private float distanceToTarget = 0f;

    private void Start()
    {
        if (session == null)
        {
            session = new SessionManager();
            session.GenerateMap(10, 9, 10, 4);
        }
        SessionMap.MapNode[][] nodes = session.map.nodeArr;
        nodeArr = new GameObject[session.map.nodeArr.Length][];
        for (int y = 0; y < nodes.Length; y++)
        {
            nodeArr[y] = new GameObject[nodes[y].Length];
            for (int x = 0; x < nodes[y].Length; x++)
            {
                CreateMapPoint(nodes[y][x]);
            }
        }
        selectedNode = nodeArr[session.map.startingNode.level][session.map.startingNode.branch];
        mainCam.transform.position = new Vector3(selectedNode.transform.position.x, selectedNode.transform.position.y, -10);
        mouseClick.Enable();
        mouseClick.performed += (InputAction.CallbackContext callback) =>
        {
            Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit)
            {
                //Debug.Log("Mouse selection to new map point");
                if (selectedNode != hit.collider.gameObject)
                {
                    selectedNode = hit.collider.gameObject;
                    distanceToTarget = Vector2.Distance(mainCam.transform.position, selectedNode.transform.position);
                }
            }
        };
    }

    void Update()
    {
        Vector2 vecControls = movement.action.ReadValue<Vector2>();

        if (vecControls.x < 0 && previousInput.x >= 0)
        {

        }
        if (vecControls.x > 0 && previousInput.x <= 0)
        {

        }
        if (vecControls.y < 0 && previousInput.y >= 0)
        {

        }
        if (vecControls.y > 0 && previousInput.y <= 0)
        {

        }

        previousInput = vecControls;

        Vector2 camMovement = new Vector2();
        camMovement.x = selectedNode.transform.position.x - mainCam.transform.position.x;
        camMovement.y = selectedNode.transform.position.y - mainCam.transform.position.y;
        if (distanceToTarget != 0 && Vector2.Distance(mainCam.transform.position, selectedNode.transform.position) > 0.005f)
        {
            camMovement = camMovement.normalized * camSpeed * Time.deltaTime * ((Vector2.Distance(mainCam.transform.position, selectedNode.transform.position) +0.5f) / distanceToTarget);
            mainCam.transform.position = mainCam.transform.position + new Vector3(camMovement.x, camMovement.y, 0);
        }
        else 
        {
            mainCam.transform.position = new Vector3(selectedNode.transform.position.x, selectedNode.transform.position.y, -10);
        }
    }

    private void CreateMapPoint(SessionMap.MapNode node)
    {
        if (!node.isEmpty)
        {
            Vector3 pos1 = NodeToCoords(node);
            GameObject point = Instantiate<GameObject>(mapPoint, pos1, Quaternion.identity);
            point.GetComponent<MapPoint>().scene = node.scene;
            nodeArr[node.level][node.branch] = point;
            foreach (SessionMap.MapNode n in node.connections)
            {
                LineRenderer line = Instantiate<LineRenderer>(mapLine, pos1, Quaternion.identity);
                line.positionCount = 2;
                line.SetPositions(new Vector3[] { pos1, NodeToCoords(n) });
            }
        }
    }

    private Vector3 NodeToCoords(SessionMap.MapNode node)
    {
        Vector3 pos = new Vector3();
        pos.x = node.branch * 2;
        pos.y = node.level * 2;
        pos.z = 0;
        return pos;
    }

    public void StartLevel() 
    {
        SceneManager.LoadSceneAsync(selectedNode.GetComponent<MapPoint>().scene);
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
}
