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
