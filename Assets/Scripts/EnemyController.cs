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
 
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : ConsoleReadyBehaviour
{
	public AudioClip deathAudio;
	public Action spawnerCallback;
	public Spawner spawner;
	public GameObject castOrigin;
	public LayerMask layerMask;
	public int health = 10;
	public float corpseDespawnDelay = 5f;
	public float deathEffectLifetime = 3f;
	public float attackInterval = 1f;
	public float attackDistance = 0.75f;
	Animator animator;
	AudioSource audioSource;
	AudioClip attackAudio;
	public NavMeshAgent agent { get; private set; }
	public GameObject aggroTarget { get; private set; }
	//CharacterController controller;
	ParticleSystem deathEffect;
	float speed;
	bool exiting = false;
	bool ignoreWaypointCollision = false;
	public bool attacking { get; set; }
	static readonly System.Random random = new System.Random();

	void Start()
    {
        animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
		agent = GetComponent<NavMeshAgent>();
		speed = agent.speed;
		GetComponentInChildren<AggroTrigger>().aggroCallback = OnAggro;
		aggroTarget = null;
		deathEffect = Resources.Load<ParticleSystem>("Prefabs/Death Effect");
		attackAudio = Resources.Load<AudioClip>("Audio/hit");
		//controller = GetComponent<CharacterController>();
		attacking = false;

		agent.SetDestination(spawner.nearbyWaypoints[random.Next(spawner.nearbyWaypoints.Length - 1)].transform.position);
		animator.SetFloat(GameManager.SPEED_HASH, agent.speed);
    }

	void Update()
    {
        if (health <= 0)
		{
			animator.SetBool(GameManager.DEAD_HASH, true);
			agent.enabled = false;
			aggroTarget = null;
			if (audioSource.isPlaying) audioSource.pitch = UnityEngine.Random.value * random.Next(1, 5);
			Destroy(gameObject, corpseDespawnDelay);
		}

		/*if (animator.GetCurrentAnimatorStateInfo(2).tagHash != 0) // agonising in place
			agent.enabled = false;
		else
			agent.enabled = true;*/

		if (!audioSource.isPlaying) StartCoroutine(Sing());
	}

	public IEnumerator Attack()
	{
		animator.SetInteger(GameManager.ATTACK_VARIANT_HASH, random.Next(0, 3));
		animator.SetTrigger(GameManager.ATTACK_HASH);
		yield return new WaitForSeconds(attackInterval);
		attacking = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!agent.enabled) return;

		if (!ignoreWaypointCollision && other.gameObject.CompareTag(GameManager.WAYPOINT))
		{
			FindNextWaypoint(other.gameObject);
			ignoreWaypointCollision = true; // so we don't set the destination every update, since we're colliding currently
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!agent.enabled) return;

		if (other.gameObject.CompareTag(GameManager.WAYPOINT))
			ignoreWaypointCollision = false;
	}

	public void PlayHit() => AudioSource.PlayClipAtPoint(attackAudio, transform.position);

	void FindNextWaypoint(GameObject currentWaypoint)
	{
		int nextWaypoint = random.Next(spawner.nearbyWaypoints.Length - 1);

		while (spawner.nearbyWaypoints[nextWaypoint].GetInstanceID() == currentWaypoint.GetInstanceID())
			nextWaypoint = random.Next(spawner.nearbyWaypoints.Length - 1);

		agent.SetDestination(spawner.nearbyWaypoints[nextWaypoint].transform.position);
	}

	public void Damage(int damage)
	{
		health -= damage;
		animator.SetTrigger(GameManager.HURT_HASH);
	}

	public void ToggleAI() => agent.enabled = !agent.enabled;

	IEnumerator Sing()
	{
		audioSource.pitch = Mathf.Clamp(UnityEngine.Random.value, 0.25f, 0.5f);
		audioSource.Play();
		yield return new WaitForSeconds(audioSource.clip.length);
	}

	private void OnDestroy()
	{
		if (exiting) return;
		spawnerCallback();
		AudioSource.PlayClipAtPoint(deathAudio, transform.position);
		var deathEffectInstance = Instantiate(deathEffect, transform.position, Quaternion.identity);
		Destroy(deathEffectInstance, deathEffectLifetime);
		GameConsole.AddMessage($"{ConsoleString()} has been killed.");
	}

	private void OnApplicationQuit() => exiting = true;

	void OnAggro(Collider other, bool entering)
	{
		if (entering && other.gameObject.CompareTag(GameManager.PLAYER))
		{
			aggroTarget = other.gameObject;
			ignoreWaypointCollision = true;
			GameConsole.AddMessage($"<color=#FF0000>{ConsoleString()} has spotted \"{other.gameObject.name}\" ({other.gameObject.GetInstanceID():X8}) at a distance of {Vector3.Distance(other.gameObject.transform.position, gameObject.transform.position)}</color>");
		}
		if (!entering && other.gameObject.CompareTag(GameManager.PLAYER))
		{
			aggroTarget = null;
			ignoreWaypointCollision = false;
			FindNextWaypoint(other.gameObject);
			GameConsole.AddMessage($"<color=#00FF00>{ConsoleString()} has lost \"{other.gameObject.name}\" ({other.gameObject.GetInstanceID():X8})</color>");
		}
	}
}
