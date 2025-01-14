using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NavController : MonoBehaviour
{
    public static NavController instance;


    public List<NavNode> nodes = new List<NavNode>();
    
    [SerializeField]
    Rooms targetRoom;
    [SerializeField]
    NavNode targetNode;

    [SerializeField]
    GameObject navNode;
    [SerializeField]
    GameObject doorsBetweenRooms;

    [SerializeField]
    Transform spawnDoorsUnder;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
        InvokeRepeating("UpdateWeigths", 1, 1);
    }

    public Queue<NavNode> GetPathTo(Vector2 from, Vector2 to)
    {
        Queue<NavNode> path = new Queue<NavNode>();

        NavNode currentNode = FindClosestWaypoint(from);
        NavNode toNode = FindClosestWaypoint(to);


        if (currentNode == null || toNode == null)
        {
            path.Enqueue(currentNode);
            return path;
        }

        SortedList<float, NavNode> openList = new SortedList<float, NavNode>();
        List<NavNode> closedList = new List<NavNode>();
        openList.Add(0, currentNode);

        currentNode.previous = null;
        currentNode.distance = 0f;

        while (openList.Count > 0)
        {
            currentNode = openList.Values[0];
            openList.RemoveAt(0);

            float distance = currentNode.distance;
            closedList.Add(currentNode);

            if (toNode == currentNode)
            {
                while (currentNode.previous != null)
                {
                    path.Enqueue(currentNode);
                    currentNode = currentNode.previous;
                }
                path.Enqueue(currentNode);
            }
            else
            {
                currentNode.GetConnectedNodes().ForEach(node =>
                {
                    if (!(closedList.Contains(node) || openList.ContainsValue(node)))
                    {
                        node.previous = currentNode;
                        node.distance = currentNode.distance + Vector2.Distance(node.transform.position, currentNode.transform.position);
                        float distanceToTarget = node.distance + Vector2.Distance(node.transform.position, toNode.transform.position);
                        openList.Add(distanceToTarget, node);
                    }
                });
            }
        }
        path = new Queue<NavNode>(path.Reverse());
        return path;
    }

    public Queue<NavNode> GetPathToScientist(Vector2 from)
    {
        Queue<NavNode> path = new Queue<NavNode>();
        NavNode currentNode = FindClosestWaypoint(from, true);
        if(currentNode == null) return path;

        path.Enqueue(currentNode);

        while (currentNode.nextNode != null)
        {

            currentNode = currentNode.nextNode;
            path.Enqueue(currentNode);
        }

        return path;
    }

    [SerializeField]
    LayerMask navNodeLayerMask;
    [SerializeField]
    LayerMask navNodeLayerMaskWithoutDoor;


    public NavNode FindClosestWaypoint(Vector2 target, bool toScientist = false)
    {
        NavNode closest = null;
        float closestDist = Mathf.Infinity;
        foreach (NavNode waypoint in nodes)
        {
            var dist = ((Vector2)waypoint.transform.position - target).magnitude + (toScientist ? waypoint.distanceToScientist : 0);
            if (dist < closestDist)
            {
                if (!Physics2D.Linecast(waypoint.transform.position, target, ~navNodeLayerMask))
                {
                    closest = waypoint;
                    closestDist = dist;
                }
            }
        }
        if (closest != null)
        {
            return closest;
        }
        return null;
    }



    public void CreateNavMesh()
    {

    }



    public void UpdateWeigths()
    {
        List<NavNode> closedList = new List<NavNode>();
        List<NavNode> openList = new List<NavNode>();

        NavNode target = targetNode;

        target.distanceToScientist = 0;
        openList.Add(target);

        while (openList.Count > 0)
        {
            target = openList[0];
            openList.RemoveAt(0);
            closedList.Add(target);

            float distanceObs = target.distanceToScientistWithObsticles;

            target.GetConnectedNodes().ForEach(n =>
            {
                if(!(openList.Contains(n) || closedList.Contains(n)))
                {
                    float dist = Vector2.Distance(target.transform.position, n.transform.position);
                    n.distanceToScientistWithObsticles = distanceObs + dist + n.obstaclesWeigths;
                    openList.Add(n);
                }
            });
            openList.Sort((first, second) => first.distanceToScientistWithObsticles.CompareTo(second.distanceToScientistWithObsticles));
        }
    }

    public void UpdateStaticWeigths()
    {
        List<NavNode> closedList = new List<NavNode>();
        List<NavNode> openList = new List<NavNode>();

        NavNode target = targetNode;

        target.distanceToScientist = 0;
        openList.Add(target);

        while (openList.Count > 0)
        {
            target = openList[0];
            openList.RemoveAt(0);
            closedList.Add(target);

            float distanceWithout = target.distanceToScientist;

            target.GetConnectedNodes().ForEach(n =>
            {
                if (!(openList.Contains(n) || closedList.Contains(n)))
                {
                    float dist = Vector2.Distance(target.transform.position, n.transform.position);
                    n.distanceToScientist = distanceWithout + dist;
                    n.nextNode = target;
                    openList.Add(n);
                }
            });
            openList.Sort((first, second) => first.distanceToScientist.CompareTo(second.distanceToScientist));
        }
    }


    struct DoorNav
    {
        public StairsScript script;
        public NavNode node;
    };


    [ContextMenu("Remove nodes")]
    void RemoveNodes()
    {
        NavNode[] allNodes = FindObjectsOfType<NavNode>();
        for (int i = 0; i < allNodes.Length; i++)
        {
            DestroyImmediate(allNodes[i].gameObject);
        }
    }

    #if UNITY_EDITOR

    [ContextMenu("Create navmesh and connect doors from premade map")]
    void createNavMeshPremade()
    {
        List<Rooms> allRooms = FindObjectsOfType<Rooms>().ToList();
        targetRoom = allRooms.Find(x => x.startingRoom == true);

        if(targetRoom == null)
        {
            Debug.LogError("Create room marked as starting");
            return;
        }

        nodes.Clear();
        NavNode[] allNodes = FindObjectsOfType<NavNode>();
        for (int i = 0; i < allNodes.Length; i++)
        {
            DestroyImmediate(allNodes[i].gameObject);
        }

        

        // In room nodes
        allRooms.ForEach(room =>
        {
            List<DoorMarker> doors = room.GetComponentsInChildren<DoorMarker>().ToList();
            List<NavNode> nodesInRoom = new List<NavNode>();
            doors.ForEach(door =>
            {
                nodesInRoom.Add(Instantiate(navNode, door.transform.position, Quaternion.identity, door.transform).GetComponent<NavNode>());
            });

            List<StairsScript> stairs = room.GetComponentsInChildren<StairsScript>().ToList();

            stairs.ForEach(stairs =>
            {
                NavNode temp = Instantiate(navNode, stairs.transform.position, Quaternion.identity, stairs.transform).GetComponent<NavNode>();
                temp.navNodeType = NavNode.NavNodeType.Stairs;
                nodesInRoom.Add(temp);
            });

            if(room == targetRoom)
            {
                Transform bunkerDoor = room.GetComponentInChildren<ScientistRoomDoor>().transform;
                nodesInRoom.Add(Instantiate(navNode, bunkerDoor.transform.position, Quaternion.identity, bunkerDoor.transform).GetComponent<NavNode>());
                targetNode = nodesInRoom[nodesInRoom.Count - 1];
            }

            nodesInRoom.ForEach(nodeOne =>
            {
                nodesInRoom.ForEach(nodeTwo =>
                {
                    if(nodeOne != nodeTwo)
                    {
                        nodeOne.AddConnectedNode(nodeTwo);
                        nodeTwo.AddConnectedNode(nodeOne);
                    }
                });
            });
            nodes.AddRange(nodesInRoom);
        });

        //Stairs
        List<StairsScript> stairs = FindObjectsOfType<StairsScript>().ToList();

        stairs.ForEach(stairs =>
        {
            stairs.GetComponentInChildren<NavNode>().AddConnectedNode(stairs.GetConnected().GetComponentInChildren<NavNode>());
        });


        //Connect doors and remove redundant nodes

        List<DoorMarker> doors = FindObjectsOfType<DoorMarker>().ToList();
        List<DoorsBetweenRooms> doorsToDestroy = FindObjectsOfType<DoorsBetweenRooms>().ToList();

        doorsToDestroy.ForEach(doorToDestroy =>
        {
            DestroyImmediate(doorToDestroy);
        });

        doors.ForEach(door =>
        {
            NavNode myNode = door.GetComponentInChildren<NavNode>();
            if(myNode != null)
            {
                DoorMarker connected = doors.Find(toFind => Vector2.Distance(door.transform.position, toFind.transform.position) < 0.3f && door != toFind);
                NavNode redundant = connected.GetComponentInChildren<NavNode>();

                Instantiate(doorsBetweenRooms, door.transform.position, Quaternion.identity, spawnDoorsUnder).GetComponent<DoorsBetweenRooms>().SetParrents(door.GetComponentInParent<Rooms>(),
                    connected.GetComponentInParent<Rooms>(), myNode);

                nodes.Remove(redundant);
                redundant.GetConnectedNodes().ForEach(nodeToUpdate =>
                {
                    nodeToUpdate.RemoveConnectedNode(redundant);
                    nodeToUpdate.AddConnectedNode(myNode);
                });
                myNode.AddConnectedNode(redundant.GetConnectedNodes());
                
                DestroyImmediate(redundant.gameObject);
            }
        });




        UpdateStaticWeigths();
    }

    #endif
}
