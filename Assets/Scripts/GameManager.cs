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
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static readonly int ATTACK_HASH = Animator.StringToHash("Attack");
	public static readonly int ATTACK_VARIANT_HASH = Animator.StringToHash("Attack Variant");
	public static readonly int DEAD_HASH = Animator.StringToHash("Dead");
	public static readonly int FIRING_HASH = Animator.StringToHash("Firing");
	public static readonly int HURT_HASH = Animator.StringToHash("Hurt");
	public static readonly int HURT_VARIANT_HASH = Animator.StringToHash("Hurt Variant");
	public static readonly int JUMP_HASH = Animator.StringToHash("Jump");
	public static readonly int SPEED_HASH = Animator.StringToHash("Speed");
	public static readonly int TEXT_FADE_HASH = Animator.StringToHash("Text Fade");
	public static readonly System.Random random = new System.Random();
	public static GameManager instance { get; private set; }

	public Dictionary<int, GameObject> objects;

	[Header("Audio")]
	public AudioClip[] tracks;
	public AudioClip[] announcerClips;

	[Header("UI")]
	public TMP_Text scoreText;
	public TMP_Text waveText;

	[Header("Gameplay")]
	public int scorePerEnemy = 50;
	public float waveClearInterval = 5f;

	AudioSource audioSource;
	AudioSource announcerAudio;
	Animator waveTextAnim;
	WaveSpawner[] spawners;
	int score = 0;
	int trackIndex = 0;
	int currentWave = 0;
	bool wavesClear = false;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		announcerAudio = GameObject.Find("Announcer").GetComponent<AudioSource>();
		objects = new Dictionary<int, GameObject>();
		instance = this;
		spawners = FindObjectsOfType<WaveSpawner>();
		waveTextAnim = waveText.GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
		Cursor.lockState = CursorLockMode.Locked;

		// Unity doesn't have a "find object by ID" function, so register every object in the scene in our dictionary
		GameObject[] objs = FindObjectsOfType<GameObject>();
		for (int i = 0; i < objs.Length; i++) objects.Add(objs[i].GetInstanceID(), objs[i]);
	}

	void Start()
	{
		score = 0;
		waveText.text = string.Empty;
	}

	void Update()
	{
		if (!wavesClear) StartCoroutine(CheckWaveClear());

		if (!audioSource.isPlaying)
		{
			audioSource.clip = tracks[trackIndex++];
			audioSource.Play();
			trackIndex %= tracks.Length;
		}
	}

	public void AnnounceInternal(AudioClip clip)
	{
		if (announcerAudio.isPlaying) announcerAudio.Stop();
		announcerAudio.clip = clip;
		announcerAudio.Play();
	}

	IEnumerator CheckWaveClear()
	{
		wavesClear = true;
		for (int i = 0; i < spawners.Length; i++)
			if (!spawners[i].cleared) wavesClear = false;
		if (wavesClear) yield return SpawnNextWave();
		else yield return new WaitForEndOfFrame();
	}

	IEnumerator SpawnNextWave()
	{
		if (currentWave > 0)
		{
			waveTextAnim.Play(TEXT_FADE_HASH, -1, 0f);
			waveText.text = $"Wave {currentWave++} cleared!";
		}
		yield return new WaitForSeconds(waveClearInterval);
		wavesClear = false;
		waveTextAnim.Play(TEXT_FADE_HASH, -1, 0f);
		waveText.text = "Next wave!";

		for (int i = 0; i < spawners.Length; i++)
			spawners[i].SpawnWave();
	}

	public static void Announce(AudioClip clip) => instance.AnnounceInternal(clip);
	public static void AddToScore(int points)
	{
		instance.score += points;
		instance.scoreText.text = instance.score.ToString();
	}
}
