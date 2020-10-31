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

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : ConsoleReadyBehaviour
{
	[Header("Positioning")]
	public GameObject castTarget;
	public Transform spawnPoint;

	[Header("Movement Physics")]
	public float turnSpeed = 10f;
	public float jumpStrength = 1f;
	public float moveSpeedMultiplier = 2f;
	public float gravityScale = -3f;
	public float groundDistanceFactor = 0.25f;

	[Header("Behavior")]
	public int maxHealth = 100;

	Animator animator;
	Quaternion freeRotation;
	Vector3 controllerVelocity;
	Vector3 input;
	CharacterController controller;
	HealthBar healthBar;
	GameObject damageText;
	float moveSpeed = 0f;
	//float direction = 0f; // future use if we get directional animations
	float velocity;
	int health;
	bool grounded = false;
	public bool dead { get; private set; }

	void Awake()
	{
		animator = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
		GameConsole.player = gameObject;
		healthBar = GetComponent<HealthBar>();
		damageText = Resources.Load<GameObject>("Prefabs/Damage Text");
	}

	private void Start()
	{
		dead = false;
		health = maxHealth;
		healthBar.maxValue = maxHealth;
		healthBar.current = maxHealth;
	}

	void Rotate()
	{
		var forward = Camera.main.transform.TransformDirection(Vector3.forward);
		forward.y = 0f;
		if (input.z < 0f) forward = -forward;
		var targetDirection = input.x * Camera.main.transform.TransformDirection(Vector3.right) + input.z * forward;

		bool movement = input != Vector3.zero && targetDirection.magnitude > 0.1f;

		if (movement || Input.GetAxis("Fire1") == 1f)
		{
			if (movement)
			{
				var lookDirection = targetDirection.normalized;
				freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
			}
			else // fire button
				freeRotation = Quaternion.LookRotation(Camera.main.transform.forward, transform.up);

			float rotationDifference = freeRotation.eulerAngles.y - transform.eulerAngles.y;
			float eulerY = transform.eulerAngles.y;

			if (rotationDifference < 0f || rotationDifference > 0f) eulerY = freeRotation.eulerAngles.y;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0f, eulerY, 0f)), turnSpeed * Time.deltaTime);
		}
	}

	private void FixedUpdate()
	{
		if (Physics.Raycast(transform.position, -Vector3.up, out _, groundDistanceFactor))
			grounded = true;
		else grounded = false;
	}

	void Move()
	{
		controllerVelocity = controller.velocity;

		moveSpeed = Mathf.Clamp(/*input.x + */input.z, -1f, 1f);
		moveSpeed = Mathf.SmoothDamp(animator.GetFloat(GameManager.SPEED_HASH), moveSpeed, ref velocity, 0.1f);
		animator.SetFloat(GameManager.SPEED_HASH, moveSpeed);
		input.z = Mathf.Abs(input.z); // no moonwalking!
		controller.Move(freeRotation * input * moveSpeed * moveSpeedMultiplier * Time.deltaTime); // apply movement

		if (grounded && Input.GetMouseButtonDown(2)) animator.SetTrigger(GameManager.JUMP_HASH);

		if (!grounded) controllerVelocity.y += Physics.gravity.y * Time.deltaTime;
		controller.Move(controllerVelocity * Time.deltaTime); // apply the gravity
	}

	void Update()
	{
		if (dead) return;
		input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

		Rotate();
		Move();
	}

	public void Jump()
	{
		controllerVelocity.y += Mathf.Sqrt(jumpStrength * gravityScale * Physics.gravity.y);
		controller.Move(controllerVelocity * Time.deltaTime);
	}

	public void Damage(int amount)
	{
		health -= amount;
		if (health > 0) healthBar.current -= amount;
		else
		{
			healthBar.current = 0;
			dead = true;
			GameManager.Announce(GameManager.instance.announcerClips[6]);
			animator.SetBool(GameManager.DEAD_HASH, true);
			StartCoroutine(Restart());
		}

		GameObject dtext = Instantiate(damageText, (Vector3.up * 2.125f) + transform.position, Quaternion.identity);
		dtext.GetComponentInChildren<TMP_Text>().text = amount.ToString();

		animator.SetInteger(GameManager.HURT_VARIANT_HASH, GameManager.random.Next(0, 2));
		animator.SetTrigger(GameManager.HURT_HASH);
	}

	IEnumerator Restart()
	{
		yield return new WaitForSeconds(5f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
