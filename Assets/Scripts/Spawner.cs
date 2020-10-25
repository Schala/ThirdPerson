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

public class Spawner : ConsoleReadyBehaviour
{
    public GameObject[] enemyPrefabs;
    public float interval = 10f;
    public int limit = 50;
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
                EnemyController enemy = Instantiate(enemyPrefabs[GameManager.random.Next(enemyPrefabs.Length)], RandomPoint(), Quaternion.identity).GetComponent<EnemyController>();
                enemy.spawner = this;
                enemy.spawnerCallback = OnDespawn;
                currentlySpawned++;
                intervalDelta = 0f;
                GameConsole.AddMessage($"<color=#FFFF00>{ConsoleString()} spawned {enemy.ConsoleString()} at {FindObjectOfType<Terrain>().transform.TransformPoint(enemy.transform.position)}</color>");
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
            y = boundary.bounds.min.y,
            z = Random.Range(boundary.bounds.min.z, boundary.bounds.max.z),
        };
    }

	private void OnTriggerExit(Collider other)
	{
        if (!other.gameObject.CompareTag(GameManager.ENEMY)) return;
        Destroy(other.gameObject);
        GameConsole.AddMessage($"<color=#FF0000>{other.gameObject.GetComponent<EnemyController>().ConsoleString()} destroyed for exiting map boundaries.</color>");
    }
}
