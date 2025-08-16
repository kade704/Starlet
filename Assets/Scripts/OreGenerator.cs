using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OreInfo
{
    public ItemAsset oreItem;
    public Color color;
}

public class OreGenerator : MonoBehaviour
{
    [SerializeField] private Ore[] oreShapes;
    [SerializeField] private OreInfo[] oreInfos;

    private const int MAP_SIZE = 600;
    private const int MIN_RADIUS = 20;

    private bool IsValid(Vector2 candidate, float mapSize, float minRadius, float cellSize, Vector2?[,] grid)
    {
        if (candidate.x < 0 || candidate.x >= mapSize || candidate.y < 0 || candidate.y >= mapSize)
        {
            return false;
        }

        int gridX = (int)(candidate.x / cellSize);
        int gridY = (int)(candidate.y / cellSize);
        int searchStartX = Mathf.Max(0, gridX - 2);
        int searchEndX = Mathf.Min(gridX + 2, grid.GetLength(0) - 1);
        int searchStartY = Mathf.Max(0, gridY - 2);
        int searchEndY = Mathf.Min(gridY + 2, grid.GetLength(1) - 1);

        for (int x = searchStartX; x <= searchEndX; x++)
        {
            for (int y = searchStartY; y <= searchEndY; y++)
            {
                Vector2? neighbor = grid[x, y];
                if (neighbor.HasValue)
                {
                    float sqrDst = (candidate - neighbor.Value).sqrMagnitude;
                    if (sqrDst < minRadius * minRadius)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    void Start()
    {
        float cellSize = MIN_RADIUS / Mathf.Sqrt(2);
        int gridSize = Mathf.CeilToInt(MAP_SIZE / cellSize);
        Vector2?[,] grid = new Vector2?[gridSize, gridSize];

        List<Vector2> points = new List<Vector2>();
        List<Vector2> activeList = new List<Vector2>();

        Vector2 firstPoint = new Vector2(Random.value * MAP_SIZE, Random.value * MAP_SIZE);
        activeList.Add(firstPoint);
        points.Add(firstPoint);
        grid[(int)(firstPoint.x / cellSize), (int)(firstPoint.y / cellSize)] = firstPoint;

        const int k = 30;

        while (activeList.Count > 0)
        {
            int randomIndex = Random.Range(0, activeList.Count);
            Vector2 currentPoint = activeList[randomIndex];
            bool foundNewPoint = false;

            for (int i = 0; i < k; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                float distance = Random.Range(MIN_RADIUS, 2 * MIN_RADIUS);
                Vector2 candidate = currentPoint + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                if (IsValid(candidate, MAP_SIZE, MIN_RADIUS, cellSize, grid))
                {
                    points.Add(candidate);
                    activeList.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = candidate;
                    foundNewPoint = true;
                    break;
                }
            }

            if (!foundNewPoint)
            {
                activeList.RemoveAt(randomIndex);
            }
        }

        foreach (Vector2 point in points)
        {
            Vector2 position = point - Vector2.one * (MAP_SIZE / 2);
            Ore orePrefab = oreShapes[Random.Range(0, oreShapes.Length)];
            OreInfo oreInfo = oreInfos[Random.Range(0, oreInfos.Length)];
            Ore ore = Instantiate(orePrefab, (Vector3)position, Quaternion.identity, transform);
            ore.Initialize(Random.Range(1, 10), oreInfo.oreItem, oreInfo.color);
        }
    }
}
