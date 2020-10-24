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
