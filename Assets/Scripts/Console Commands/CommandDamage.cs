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

public class CommandDamage : ConsoleCommand
{
	public override string description { get; protected set; }

	public CommandDamage()
	{
		description = "Damage the player";
	}

	public override void Run(string[] args)
	{
		if (args.Length != 2)
		{
			GameConsole.AddMessage($"<color=#FF0000>Wrong number of parameters</color>: {args.Length - 1}");
			return;
		}

		int amount = 0;

		try
		{
			amount = int.Parse(args[1]);
		}
		catch (FormatException)
		{
			GameConsole.AddMessage("<color=#FF0000>Invalid parameters</color>");
			return;
		}
		catch (Exception e)
		{
			GameConsole.AddMessage($"<color=#FF0000>{e.Message}</color>");
			return;
		}

		GameConsole.player.GetComponent<PlayerController>().Damage(amount);
		GameConsole.AddMessage("BONK!");
	}
}
