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
 
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
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
	public static readonly int HURT_VARIANT_HASH = Animator.StringToHash("Hurt Variant");
	public static readonly int JUMP_HASH = Animator.StringToHash(JUMP);
	public static readonly int SPEED_HASH = Animator.StringToHash("Speed");
	public static readonly System.Random random = new System.Random();
	public static GameManager instance { get; private set; }

	public Dictionary<int, GameObject> objects;
	public AudioClip[] tracks;
	public AudioClip[] announcerClips;
	public Text dialogue;
	AudioSource audioSource;
	AudioSource announcerAudio;
	int trackIndex = 0;

	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
		announcerAudio = GameObject.Find(GameManager.ANNOUNCER).GetComponent<AudioSource>();
		objects = new Dictionary<int, GameObject>();
		instance = this;
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

	public void AnnounceInternal(AudioClip clip)
	{
		if (announcerAudio.isPlaying) announcerAudio.Stop();
		announcerAudio.clip = clip;
		announcerAudio.Play();
	}

	public static void Announce(AudioClip clip) => instance.AnnounceInternal(clip);

}
