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
