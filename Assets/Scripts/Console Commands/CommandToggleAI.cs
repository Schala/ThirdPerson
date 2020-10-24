using UnityEngine;

public class CommandToggleAI : ConsoleCommand
{
	public override string description { get; protected set; }

	public CommandToggleAI()
	{
		description = "Toggles enemy AI on or off, optionally with instance ID provided";
	}

	public override void Run(string[] args)
	{
		if (args.Length > 2)
		{
			GameConsole.AddMessage(GameConsole.WRONG_NUMBER_PARAMETERS);
			return;
		}

		if (args.Length == 1)
		{
			GameObject[] entities = Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < entities.Length; i++)
				if (entities[i].CompareTag(GameManager.ENEMY))
					entities[i].GetComponent<EnemyController>().ToggleAI();
		}
	}
}
