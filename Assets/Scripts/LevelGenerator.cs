using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject collectiblePrefab;
    public GameObject obstaclePrefab;
    public GameObject movingObstaclePrefab;

    [Header("Configuraci�n por Nivel")]
    public int levelNumber = 1;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Configuraci�n basada en el nivel
        int numCollectibles = 5 + (levelNumber * 2); // M�s coleccionables en niveles avanzados
        int numObstacles = levelNumber * 3; // M�s obst�culos en niveles avanzados

        // Generar coleccionables
        for (int i = 0; i < numCollectibles; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-20f, 20f),
                0.5f,
                Random.Range(-20f, 20f)
            );

            Instantiate(collectiblePrefab, randomPos, Quaternion.identity);
        }

        // Generar obst�culos
        for (int i = 0; i < numObstacles; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-18f, 18f),
                1f,
                Random.Range(-18f, 18f)
            );

            GameObject prefabToUse = (levelNumber > 3 && Random.value > 0.5f)
                ? movingObstaclePrefab
                : obstaclePrefab;

            if (prefabToUse != null)
            {
                Instantiate(prefabToUse, randomPos, Quaternion.identity);
            }
        }
    }
}