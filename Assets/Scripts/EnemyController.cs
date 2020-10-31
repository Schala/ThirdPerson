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
using UnityEngine.SceneManagement;

public class EnemyController : ConsoleReadyBehaviour
{
	[Header("Behavior")]
	public LayerMask layerMask;
	public float corpseDespawnDelay = 5f;
	public float attackInterval = 1f;
	public float attackDistance = 0.75f;
	public int maxHealth = 10;

	[Header("Positioning")]
	public Action spawnerCallback;
	public WaveSpawner spawner;
	public GameObject castOrigin;

	[Header("Aesthetic")]
	public GameObject hud;

	Animator animator;
	AudioSource audioSource;
	AudioClip attackAudio;
	HealthBar healthBar;

	public NavMeshAgent agent { get; set; }
	public GameObject aggroTarget { get; private set; }
	public CharacterController controller { get; private set; }
	public bool attacking { get; set; }
	ParticleSystem deathEffect;
	Vector3 nextWaypoint;
	//float speed; // future plans, dynamic movement speed?
	int health;
	bool exiting = false;
	bool announceDeath = false;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
		//GetComponentInChildren<AggroTrigger>().aggroCallback = OnAggro;
		//aggroTarget = null;
		deathEffect = Resources.Load<ParticleSystem>("Prefabs/Death Effect");
		attackAudio = Resources.Load<AudioClip>("Audio/hit");
		controller = GetComponent<CharacterController>();
		healthBar = GetComponent<HealthBar>();
		hud.SetActive(false);
		health = maxHealth;
		healthBar.maxValue = maxHealth;
		healthBar.current = maxHealth;
		attacking = false;
	}

	void Start()
    {
		aggroTarget = GameConsole.player;
		//nextWaypoint = spawner.RandomPoint();
		animator.SetFloat(GameManager.SPEED_HASH, agent.speed);
	}

	void Update()
    {
		if (aggroTarget != null && aggroTarget.GetComponent<PlayerController>().dead) aggroTarget = null;
		if (health <= 0)
		{
			animator.SetBool(GameManager.DEAD_HASH, true);
			agent.enabled = false;
			aggroTarget = null;
			if (audioSource.isPlaying) audioSource.pitch = UnityEngine.Random.value * GameManager.random.Next(1, 5);
			if (GameManager.random.Next(2) == 1 && !announceDeath)
				GameManager.Announce(GameManager.instance.announcerClips[GameManager.random.Next(2, 5)]);
			announceDeath = true;
			StartCoroutine(Death());
		}

		if (aggroTarget == null && transform.position.x > (nextWaypoint.x - 1) && transform.position.y > (nextWaypoint.y - 1) &&
			transform.position.x < (nextWaypoint.x + 1) && transform.position.y < (nextWaypoint.y + 1))
		{
			nextWaypoint = spawner.RandomPoint();
			agent.SetDestination(nextWaypoint);
		}

		/*if (animator.GetCurrentAnimatorStateInfo(2).tagHash != 0) // agonising in place
			agent.enabled = false;
		else
			agent.enabled = true;*/

		if (!audioSource.isPlaying) StartCoroutine(Sing());
	}

	public IEnumerator Attack()
	{
		animator.SetInteger(GameManager.ATTACK_VARIANT_HASH, GameManager.random.Next(0, 3));
		animator.SetTrigger(GameManager.ATTACK_HASH);
		yield return new WaitForSeconds(attackInterval);
		attacking = false;
	}

	public void PlayHit()
	{
		if (!aggroTarget) return;

		AudioSource.PlayClipAtPoint(attackAudio, transform.position);
		var player = aggroTarget.GetComponent<PlayerController>();

		player.Damage(GameManager.random.Next(2, 6));
	}

	public void Damage(int damage)
	{
		health -= damage;
		hud.SetActive(health < maxHealth);
		if (health > 0) healthBar.current -= damage;
		else
		{
			healthBar.current = 0;
			if (!announceDeath)
			{
				GameManager.AddToScore(GameManager.instance.scorePerEnemy);
				GameConsole.AddMessage($"{ConsoleString()} has been killed.");
			}
		}

		animator.SetTrigger(GameManager.HURT_HASH);
	}

	public void ToggleAI() => agent.enabled = !agent.enabled;

	IEnumerator Sing()
	{
		audioSource.pitch = Mathf.Clamp(UnityEngine.Random.value, 0.25f, 0.5f);
		audioSource.Play();
		yield return new WaitForSeconds(audioSource.clip.length);
	}

	IEnumerator Death()
	{
		yield return new WaitForSeconds(corpseDespawnDelay);
		Instantiate(deathEffect, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

	private void OnDestroy()
	{
		if (exiting) return;
		spawnerCallback();
	}

	private void OnApplicationQuit() => exiting = true;

	/*void OnAggro(Collider other, bool entering)
	{
		if (entering && other.gameObject.CompareTag("Player"))
		{
			aggroTarget = other.gameObject;
			GameConsole.AddMessage($"<color=#FF0000>{ConsoleString()} has spotted \"{other.gameObject.name}\" ({other.gameObject.GetInstanceID():X8}) at a distance of {Vector3.Distance(other.gameObject.transform.position, gameObject.transform.position)}</color>");
		}
		if (!entering && other.gameObject.CompareTag("Player"))
		{
			aggroTarget = null;
			GameConsole.AddMessage($"<color=#00FF00>{ConsoleString()} has lost \"{other.gameObject.name}\" ({other.gameObject.GetInstanceID():X8})</color>");
		}
	}*/
}
