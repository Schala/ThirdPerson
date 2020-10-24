using UnityEngine;

public class CommandKillAll : ConsoleCommand
{
	public override string description { get; protected set; }

	public CommandKillAll()
	{
		description = "Kill all currently spawned enemies";
	}

	public override void Run(string[] args)
	{
		if (args.Length > 1)
		{
			GameConsole.AddMessage(GameConsole.WRONG_NUMBER_PARAMETERS);
			return;
		}

		GameObject[] entities = Object.FindObjectsOfType<GameObject>();
		GameConsole.AddMessage("Execute Order 66!");

		for (int i = 0; i < entities.Length; i++)
			if (entities[i].CompareTag(GameManager.ENEMY))
				Object.Destroy(entities[i]);
	}
}
