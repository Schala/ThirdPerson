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

public class WaveSpawner : ConsoleReadyBehaviour
{
    [Header("Behavior")]
    public GameObject[] enemyPrefabs;
    public Collider mapBoundary;
    public int maxLimit = 10;
    public float navMeshCheckDistance = 100f;

    [Header("Spawn Info")]
    public float enemySpeed = 0.5f;

    [Header("Misc.")]
    public AudioClip spawnAudio;

    int currentlySpawned = 0;
    int currentLimit = 1;
    public bool cleared { get; private set; }

    void Awake()
    {
        cleared = true;
    }

    public void SpawnWave()
    {
        cleared = false;
        if (currentLimit > maxLimit) currentLimit = maxLimit;

        for (int i = currentlySpawned; i <= currentLimit; i++)
        {
            GameObject enemyObject = Instantiate(enemyPrefabs[GameManager.random.Next(enemyPrefabs.Length)], transform.position, Quaternion.identity);
            var enemy = enemyObject.GetComponent<EnemyController>();

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshCheckDistance, NavMesh.AllAreas))
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
            GameConsole.AddMessage($"<color=#FFFF00>{ConsoleString()} spawned {enemy.ConsoleString()} at {FindObjectOfType<Terrain>().transform.TransformPoint(enemy.transform.position)}</color>");
            AudioSource.PlayClipAtPoint(spawnAudio, enemyObject.transform.position);
        }

        currentLimit++;
        enemySpeed += 0.5f;
    }

	private void OnDespawn()
	{
        if (--currentlySpawned <= 0) cleared = true;
	}

	public Vector3 RandomPoint()
    {
        return new Vector3
        {
            x = Random.Range(mapBoundary.bounds.min.x, mapBoundary.bounds.max.x),
            y = mapBoundary.bounds.max.y,
            z = Random.Range(mapBoundary.bounds.min.z, mapBoundary.bounds.max.z),
        };
    }
}
