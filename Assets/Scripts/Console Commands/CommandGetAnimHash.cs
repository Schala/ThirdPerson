using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandGetAnimHash : ConsoleCommand
{
	public override string description { get; protected set; }

	public CommandGetAnimHash()
	{
		description = "Returns the tag hash for the specified game object's animation state, or player's animation state if parameter omitted";
	}

	public override void Run(string[] args)
	{
		if (args.Length > 2)
		{
			GameConsole.AddMessage($"{GameConsole.WRONG_NUMBER_PARAMETERS}: {args.Length - 1}");
		}

		Animator animator = null;

		if (args.Length == 1)
		{
			if (GameConsole.selection == null) return;
			animator = GameConsole.selection.GetComponent<Animator>();
		}
		else
		{
			int id = 0;

			try
			{
				id = int.Parse(args[1]);
				GameObject obj = UnityEngine.Object.FindObjectOfType<GameManager>().objects[id];
				animator = obj.GetComponent<Animator>();
			}
			catch (FormatException)
			{
				GameConsole.AddMessage(GameConsole.INVALID_PARAMETERS);
				return;
			}
			catch (KeyNotFoundException)
			{
				GameConsole.AddMessage($"<color=#FF0000>No object with ID {id:X8} exists in the scene.</color>");
				return;
			}
			catch (Exception e)
			{
				GameConsole.AddMessage($"<color=#FF0000>{e.Message}</color>");
				return;
			}
		}

		if (animator == null)
		{
			GameConsole.AddMessage("<color=#FF0000>Animator component not found on object specified</color>");
			return;
		}

		for (int i = 0; i < animator.layerCount; i++)
			GameConsole.AddMessage($"{animator.GetCurrentAnimatorStateInfo(i).tagHash}");
	}
}
