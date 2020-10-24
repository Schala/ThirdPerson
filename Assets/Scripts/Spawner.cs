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
