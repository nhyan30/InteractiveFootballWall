using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Room Settings")]
    public GameObject roomBoxPrefab;
    public WallGenerator wallGenerator;
    public float wallThickness = 0.5f;   // depth of each wall
    public float roomDepth = 2f;         // how deep the room extends in Z
    public float zOffset = 0f;           // custom Z-axis offset for positioning the entire room

    private GameObject backWall, leftWall, rightWall, topWall, bottomWall;

    void Start()
    {
        GenerateRoom();
    }

    void GenerateRoom()
    {
        Vector2 wallSize = wallGenerator.wallSize;
        Vector3 wallPos = wallGenerator.wallPosition;

        if (roomBoxPrefab == null)
        {
            Debug.LogError("RoomGenerator: No tilePrefab assigned in WallGenerator.");
            return;
        }

        // Calculate center of the back wall based on WallGenerator + apply zOffset
        Vector3 center = wallPos + new Vector3(wallSize.x / 2f, wallSize.y / 2f, zOffset);

        // --- Back wall (only one) ---
        backWall = CreateWallTile("BackWall", roomBoxPrefab,
            new Vector3(wallSize.x, wallSize.y, 1f),
            center + new Vector3(0, 0, -roomDepth));

        // --- Main walls ---
        leftWall = CreateWallTile("LeftWall", roomBoxPrefab,
            new Vector3(wallThickness, wallSize.y, roomDepth),
            center + new Vector3(-wallSize.x / 2f - wallThickness / 2f, 0, -roomDepth / 2f));

        rightWall = CreateWallTile("RightWall", roomBoxPrefab,
            new Vector3(wallThickness, wallSize.y, roomDepth),
            center + new Vector3(wallSize.x / 2f + wallThickness / 2f, 0, -roomDepth / 2f));

        topWall = CreateWallTile("TopWall", roomBoxPrefab,
            new Vector3(wallSize.x, wallThickness, roomDepth),
            center + new Vector3(0, wallSize.y / 2f + wallThickness / 2f, -roomDepth / 2f));

        bottomWall = CreateWallTile("BottomWall", roomBoxPrefab,
            new Vector3(wallSize.x, wallThickness, roomDepth),
            center + new Vector3(0, -wallSize.y / 2f - wallThickness / 2f, -roomDepth / 2f));

        // --- Duplicate walls to extend room forward (positive Z) ---
        float forwardZ = roomDepth;

        CreateWallTile("LeftWall_Extended", roomBoxPrefab,
            new Vector3(wallThickness, wallSize.y, roomDepth),
            leftWall.transform.position + new Vector3(0, 0, forwardZ));

        CreateWallTile("RightWall_Extended", roomBoxPrefab,
            new Vector3(wallThickness, wallSize.y, roomDepth),
            rightWall.transform.position + new Vector3(0, 0, forwardZ));

        CreateWallTile("TopWall_Extended", roomBoxPrefab,
            new Vector3(wallSize.x, wallThickness, roomDepth),
            topWall.transform.position + new Vector3(0, 0, forwardZ + 0.25f));

        CreateWallTile("BottomWall_Extended", roomBoxPrefab,
            new Vector3(wallSize.x, wallThickness, roomDepth),
            bottomWall.transform.position + new Vector3(0, 0, forwardZ + 0.25f));
    }

    private GameObject CreateWallTile(string name, GameObject prefab, Vector3 scale, Vector3 position)
    {
        GameObject wall = Instantiate(prefab, position, Quaternion.identity, transform);
        wall.name = name;
        wall.transform.localScale = scale;

        // Add physics boundaries
        var rb = wall.GetComponent<Rigidbody>();
        if (!rb)
            rb = wall.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Ensure it has a collider
        if (!wall.GetComponent<Collider>())
            wall.AddComponent<BoxCollider>();

        return wall;
    }
}