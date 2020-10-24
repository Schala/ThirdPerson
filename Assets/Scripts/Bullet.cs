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

public class Bullet : MonoBehaviour
{
	public int damage = 1;
	static bool soundExplained = false;
	GameManager game;

	private void Start() => game = FindObjectOfType<GameManager>();

	void OnTriggerEnter(Collider collider)
	{
		if (!collider.gameObject.CompareTag(GameManager.ENEMY)) return;
		collider.gameObject.GetComponent<EnemyController>().Damage(damage);

		if (!soundExplained)
		{
			AudioSource announcerAudio = GameObject.Find(GameManager.ANNOUNCER).GetComponent<AudioSource>();
			announcerAudio.clip = game.announcerClips[0];
			announcerAudio.Play();
			game.dialogue.text = "At first, the sound merely paralyses...\nAfter that... your mind will feel as if it's seeping out of your ears.";
			soundExplained = true;
		}
	}
}
