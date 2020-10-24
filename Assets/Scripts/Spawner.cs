using UnityEngine;

public class Spawner : ConsoleReadyBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] nearbyWaypoints;
    public float interval = 45f;
    public int limit = 3;
    public bool active = true;
    float intervalDelta;
    int currentlySpawned = 0;
    static readonly System.Random random = new System.Random();

    void Start()
    {
        Debug.Assert(enemyPrefabs != null);
        intervalDelta = interval;
    }

    void Update()
    {
        if (!active || currentlySpawned >= limit) return;

        if (intervalDelta >= interval)
        {
            EnemyController enemy = Instantiate(enemyPrefabs[random.Next(enemyPrefabs.Length)], transform.position, Quaternion.identity).GetComponent<EnemyController>();
            enemy.spawnerCallback = OnDespawn;
            enemy.spawner = this;
            currentlySpawned++;
            intervalDelta = 0f;
            GameConsole.AddMessage($"<color=#FFFF00>{ConsoleString()} spawned {enemy.ConsoleString()}</color>");
        }

        if (intervalDelta < interval) intervalDelta += Time.deltaTime;
    }

    void OnDespawn() => currentlySpawned--;

    private void OnTriggerEnter(Collider other)
	{
        if (!other.gameObject.CompareTag(GameManager.PLAYER)) return;
        active = false;
	}

	private void OnTriggerExit(Collider other)
	{
        if (!other.gameObject.CompareTag(GameManager.PLAYER)) return;
        active = true;
    }
}
