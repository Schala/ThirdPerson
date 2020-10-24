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
