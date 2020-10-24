﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public const string HORIZONTAL = "Horizontal";
	public const string VERTICAL = "Vertical";
	public const string FIRE_1 = "Fire1";
	public const string JUMP = "Jump";

	public const string ENEMY = "Enemy";
	public const string PLAYER = "Player";
	public const string SPAWNER = "Spawner";
	public const string WAYPOINT = "Waypoint";

	public const string ANNOUNCER = "Announcer";

	public static readonly int ATTACK_HASH = Animator.StringToHash("Attack");
	public static readonly int ATTACK_VARIANT_HASH = Animator.StringToHash("Attack Variant");
	public static readonly int DEAD_HASH = Animator.StringToHash("Dead");
	public static readonly int FIRING_HASH = Animator.StringToHash("Firing");
	public static readonly int HURT_HASH = Animator.StringToHash("Hurt");
	public static readonly int JUMP_HASH = Animator.StringToHash(JUMP);
	public static readonly int SPEED_HASH = Animator.StringToHash("Speed");

	public Dictionary<int, GameObject> objects;
	public AudioClip[] tracks;
	public AudioClip[] announcerClips;
	public Text dialogue;
	AudioSource audioSource;
	int trackIndex = 0;

	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
		objects = new Dictionary<int, GameObject>();
	}

	void Start()
	{
		// Unity doesn't have a "find object by ID" function, so register every object in the scene in our dictionary
		GameObject[] objs = FindObjectsOfType<GameObject>();
		for (int i = 0; i < objs.Length; i++) objects.Add(objs[i].GetInstanceID(), objs[i]);

		audioSource = GetComponent<AudioSource>();
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		if (!audioSource.isPlaying)
		{
			audioSource.clip = tracks[trackIndex++];
			audioSource.Play();
			trackIndex %= tracks.Length;
		}
	}
}
