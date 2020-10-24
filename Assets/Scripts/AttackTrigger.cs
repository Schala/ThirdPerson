using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
	EnemyController enemy;

	private void Start() => enemy = GetComponentInParent<EnemyController>();

	private void FixedUpdate()
	{
		if (enemy.aggroTarget != null && enemy.agent.enabled)
		{
			GameObject target = enemy.aggroTarget.GetComponent<PlayerController>().castTarget;
			Vector3 playerDirection = Vector3.Normalize(target.transform.position - enemy.castOrigin.transform.position) * enemy.attackDistance;

			// check if in reach of attack
			if (Physics.Raycast(enemy.castOrigin.transform.position, playerDirection, out _, enemy.attackDistance, enemy.layerMask))
			{
				Debug.DrawRay(enemy.castOrigin.transform.position, playerDirection, Color.green);
				enemy.agent.isStopped = true;
				enemy.attacking = true;
				StartCoroutine(enemy.Attack());
			}
			else
			{
				Debug.DrawRay(enemy.castOrigin.transform.position, playerDirection, Color.red);
				enemy.agent.isStopped = false;
				enemy.agent.SetDestination(enemy.aggroTarget.transform.position);
			}
		}
	}
}
