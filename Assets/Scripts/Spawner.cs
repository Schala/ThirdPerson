/*
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street - Fifth Floor, Boston, MA 02110-1301, USA.
 */
 
using UnityEngine;
using UnityEngine.AI;

public class Spawner : ConsoleReadyBehaviour
{
    public GameObject[] enemyPrefabs;
    public AudioClip spawnAudio;
    public float interval = 10f;
    public int limit = 50;
    public float navMeshCheckDistance = 100f;
    public float enemySpeed = 0.5f;
    public bool active = true;
    Collider boundary;
    float intervalDelta;
    int currentlySpawned = 0;

    void Start()
    {
        Debug.Assert(enemyPrefabs != null);
        boundary = GetComponent<Collider>();
        intervalDelta = interval;
    }

    void Update()
    {
        if (!active || currentlySpawned >= limit) return;

        if (intervalDelta >= interval)
        {
            for (int i = currentlySpawned; i <= limit; i++)
            {
                Vector3 point = RandomPoint();
                GameObject enemyObject = Instantiate(enemyPrefabs[GameManager.random.Next(enemyPrefabs.Length)], point, Quaternion.identity);
                EnemyController enemy = enemyObject.GetComponent<EnemyController>();

                if (NavMesh.SamplePosition(point, out NavMeshHit hit, navMeshCheckDistance, NavMesh.AllAreas))
                {
                    enemyObject.transform.position = hit.position;
                    enemy.agent = enemyObject.AddComponent<NavMeshAgent>();
                    enemy.agent.speed = enemySpeed;
                }
                else
                {
                    GameConsole.AddMessage($"<color=#FF0000>{enemy.ConsoleString()} destroyed for null NavAgent</color>");
                    Destroy(enemyObject);
                    break;
                }

                enemy.spawner = this;
                enemy.spawnerCallback = OnDespawn;
                currentlySpawned++;
                intervalDelta = 0f;
                GameConsole.AddMessage($"<color=#FFFF00>{ConsoleString()} spawned {enemy.ConsoleString()} at {FindObjectOfType<Terrain>().transform.TransformPoint(enemy.transform.position)}</color>");
                AudioSource.PlayClipAtPoint(spawnAudio, enemyObject.transform.position);
            }
        }

        if (intervalDelta < interval) intervalDelta += Time.deltaTime;
    }

    void OnDespawn() => currentlySpawned--;

    public Vector3 RandomPoint()
    {
        return new Vector3
        {
            x = Random.Range(boundary.bounds.min.x, boundary.bounds.max.x),
            y = boundary.bounds.max.y,
            z = Random.Range(boundary.bounds.min.z, boundary.bounds.max.z),
        };
    }

	private void OnTriggerExit(Collider other)
	{
        if (!other.gameObject.CompareTag(GameManager.ENEMY)) return;
        GameConsole.AddMessage($"<color=#FF0000>{other.gameObject.GetComponent<EnemyController>().ConsoleString()} destroyed for exiting map boundaries.</color>");
        Destroy(other.gameObject);
    }
}
