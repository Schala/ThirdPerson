using System;
using UnityEngine;

public class CommandShowColliders : ConsoleCommand
{
	public override string description { get; protected set; }

	public CommandShowColliders()
	{
		description = "Shows or hides object colliders and trigger volumes (0 = false, 1 = true)";
	}

	public override void Run(string[] args)
	{
		if (args.Length != 2)
		{
			GameConsole.AddMessage($"{GameConsole.WRONG_NUMBER_PARAMETERS}: {args.Length - 1}");
			return;
		}

		bool state;
		try
		{
			state = int.Parse(args[1]) != 0;
		}
		catch (FormatException)
		{
			GameConsole.AddMessage(GameConsole.INVALID_PARAMETERS);
			return;
		}

		GameObject[] objects = UnityEngine.Object.FindObjectsOfType<GameObject>();
		for (int i = 0; i < objects.Length; i++)
			if (objects[i].CompareTag(GameManager.SPAWNER) || objects[i].CompareTag(GameManager.WAYPOINT))
				objects[i].GetComponent<MeshRenderer>().enabled = state;
	}
}
