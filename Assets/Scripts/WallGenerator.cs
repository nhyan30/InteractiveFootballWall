using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTool.TabSystem;

[HasTabField]
public class WallGenerator : MonoBehaviour
{
    public static WallGenerator Instance;

    [Header("Wall Settings")]
    public Vector2 wallSize = new Vector2(17, 10);
    public Vector3 wallPosition = new Vector3(-8.5f, 0f, -0.7f);

    [Header("Tile Size Settings")]
    public float minTileWidth = 0.1f;
    public float maxTileWidth = 2f;
    public float minTileHeight = 0.1f;
    public float maxTileHeight = 2f;

    [Header("References")]
    public GameObject wallBoxPrefab;

    private List<Rect> cells = new List<Rect>();
    private List<GameObject> allTiles = new List<GameObject>();
    private Coroutine spreadCoroutine;

    [Header("Wave Settings")]
    public float duration = 1.5f;          // total animation time
    public float spreadSpeed = 8f;         // how fast the wave spreads

    [TabField] public bool CameraDir = false;

    private void Awake() => Instance = this;

    void Start()
    {
        GenerateWall();

        foreach (Rect cell in cells)
        {
            Vector3 pos = new Vector3(cell.x + cell.width / 2, cell.y + cell.height / 2, 0);
            GameObject tile = Instantiate(wallBoxPrefab, pos, Quaternion.identity, transform);
            tile.transform.localScale = new Vector3(cell.width, cell.height, 1f);
            allTiles.Add(tile);
        }

        // Center the wall visually
        transform.position = wallPosition;
    }

    void GenerateWall()
    {
        cells.Clear();
        float currentX = 0f;

        while (currentX < wallSize.x - minTileWidth)
        {
            float remainingWidth = wallSize.x - currentX;
            float columnWidth = Random.Range(minTileWidth, Mathf.Min(maxTileWidth, remainingWidth));
            if (currentX + columnWidth > wallSize.x)
                columnWidth = wallSize.x - currentX;

            GenerateRowsForColumn(currentX, columnWidth);
            currentX += columnWidth;
        }
    }

    void GenerateRowsForColumn(float startX, float columnWidth)
    {
        float currentY = 0f;

        while (currentY < wallSize.y - minTileHeight)
        {
            float remainingHeight = wallSize.y - currentY;
            float rowHeight = Random.Range(minTileHeight, Mathf.Min(maxTileHeight, remainingHeight));
            if (currentY + rowHeight > wallSize.y)
                rowHeight = wallSize.y - currentY;

            Rect cell = new Rect(startX, currentY, columnWidth, rowHeight);
            cells.Add(cell);
            currentY += rowHeight;
        }
    }

    // === Color Spread Logic ===
    public void StartSpreadEffect(Vector3 hitPos)
    {
        if (spreadCoroutine != null)
            StopCoroutine(spreadCoroutine);

        spreadCoroutine = StartCoroutine(SpreadWave(hitPos));
    }

    IEnumerator SpreadWave(Vector3 origin)
    {
        float elapsed = 0f;

        // Cache tile positions and original colors
        List<(TileBehaviour, float)> tileData = new List<(TileBehaviour, float)>();
        foreach (GameObject tile in allTiles)
        {
            TileBehaviour tb = tile.GetComponent<TileBehaviour>();
            float dist = Vector3.Distance(origin, tile.transform.position);
            tileData.Add((tb, dist));
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float waveFront = elapsed * spreadSpeed;

            foreach (var (tile, dist) in tileData)
            {
                float distanceFromWave = Mathf.Abs(dist - waveFront);

                // If the tile is near the current wave front, flash white
                if (distanceFromWave < 1.5f)
                {
                    float intensity = Mathf.Clamp01(1f - (distanceFromWave / 1.5f));
                    Color waveColor = Color.Lerp(tile.DefaultColor, Color.white, intensity);
                    tile.SetColor(waveColor);
                }
                // If the wave has already passed, restore its original color
                else if (dist < waveFront)
                {
                    tile.ResetColor();
                }
            }

            yield return null;
        }

        // Ensure all tiles return to normal
        foreach (var (tile, _) in tileData)
            tile.ResetColor();
    }
}
