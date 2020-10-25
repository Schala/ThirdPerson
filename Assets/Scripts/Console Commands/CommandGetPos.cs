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

public class CommandGetPos : ConsoleCommand
{
	public override string description { get; protected set; }

	public CommandGetPos()
	{
		description = "Get the selected object's world space position, if not specified";
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
			if (GameConsole.selection == null) return;
			GameConsole.AddMessage(GameConsole.selection.transform.position.ToString());
		}
		else
		{
			int id = 0;

			try
			{
				id = int.Parse(args[1]);
				GameConsole.AddMessage(UnityEngine.Object.FindObjectOfType<GameManager>().objects[id].transform.position.ToString());
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
	}
}
