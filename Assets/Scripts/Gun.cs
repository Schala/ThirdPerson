﻿using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
	public ParticleSystem particles;
	public GameObject particlesPivot;
	public GameObject bulletPrefab;
	public AudioClip[] audioClips;
	public Animator playerAnimator;
	public float bulletSpeed = 5000f;
	public float fireRate = 0.1f;
	public int bulletLifetime = 3;
	public Ray ray { get; private set; }
	AudioSource audioSource;
	float firingLength = 0;
	bool firing = false;

	void Start() => audioSource = GetComponent<AudioSource>();

	void FixedUpdate()
	{
		if (Input.GetAxis(GameManager.FIRE_1) == 1f)
		{
			if (!firing)
			{
				firing = true;
				StartCoroutine(Fire());
			}
			playerAnimator.SetBool(GameManager.FIRING_HASH, true);
		}
		else
			playerAnimator.SetBool(GameManager.FIRING_HASH, false);
	}

	void Update()
	{
		if (Input.GetAxis(GameManager.FIRE_1) == 1f)
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay(ray.origin, ray.direction * bulletSpeed, Color.red, 2f);

			// Rotate the particle system toward the reticle
			particlesPivot.transform.rotation = Quaternion.LookRotation(ray.direction);

			if (!particles.isPlaying) particles.Play();

			// gun audio start
			if (firingLength <= 0)
			{
				audioSource.clip = audioClips[0];
				audioSource.Play();
			}

			// player is holding the trigger longer than the start audio time
			if (firingLength >= audioClips[0].length && !(audioSource.isPlaying))
			{
				audioSource.clip = audioClips[1];
				audioSource.Play();
			}

			firingLength += Time.deltaTime;
		}
		else
		{
			if (particles.isPlaying)
			{
				particles.Stop();
				particles.Clear();
			}

			// play upon trigger release
			if (firingLength > 0)
			{
				audioSource.Stop();
				audioSource.clip = audioClips[2];
				audioSource.Play();
				firingLength = 0;
			}
		}
	}

	IEnumerator Fire()
	{
		// Fire toward the reticle
		GameObject bullet = Instantiate(bulletPrefab, ray.origin, Quaternion.identity) as GameObject;
		bullet.GetComponent<Rigidbody>().AddForce(ray.direction * bulletSpeed, ForceMode.Impulse);
		yield return new WaitForSeconds(fireRate);
		firing = false;
		Destroy(bullet, bulletLifetime);
	}
}
