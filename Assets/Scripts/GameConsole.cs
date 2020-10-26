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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ConsoleCommand
{
    public abstract string description { get; protected set; }

    public abstract void Run(string[] args);
}

public class GameConsole : MonoBehaviour
{
    public const string COMMAND_NOT_RECOGNISED = "<color=#FF0000>Command not recognised</color>";
    public const string INVALID_PARAMETERS = "<color=#FF0000>Invalid parameters</color>";
    public const string WRONG_NUMBER_PARAMETERS = "<color=#FF0000>Wrong number of parameters</color>";

    public static GameConsole instance { get; private set; }
    public static Dictionary<string, ConsoleCommand> commands { get; private set; }
    public static GameObject selection { get; private set; }
    public static GameObject player { get; set; }
    public GameObject console;
    public GameObject contentPane;
    public TMP_InputField consoleInput;
    public TMP_Text selectionText;
    public LayerMask layerMask;
    public int entryLimit = 256;
    List<TMP_Text> entries;
    TMP_Text entryPrefab;

	private void Awake()
	{
        if (instance != null) return;
        instance = this;

		commands = new Dictionary<string, ConsoleCommand>(StringComparer.OrdinalIgnoreCase)
		{
			{ "KillAll", new CommandKillAll() },
            { "ShowColliders", new CommandShowColliders() },
            { "ToggleAI", new CommandToggleAI() },
            { "GetAnimHash", new CommandGetAnimHash() },
            { "GetPos", new CommandGetPos() }
		};
	}

    void Start()
    {
        console.SetActive(false);
        selection = null;
        entryPrefab = Resources.Load<TMP_Text>("Prefabs/Console Entry");
        entries = new List<TMP_Text>();
    }

    private void OnEnable() => Application.logMessageReceived += OnLogMessage;
    private void OnDisable() => Application.logMessageReceived -= OnLogMessage;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            console.SetActive(!console.activeInHierarchy);
            selectionText.enabled = console.activeInHierarchy;
        }

        Cursor.lockState = console.activeInHierarchy ? CursorLockMode.Confined : CursorLockMode.Locked;
        Time.timeScale = console.activeInHierarchy ? 0f : 1f;

        if (console.activeInHierarchy)
        {
            consoleInput.Select();
            consoleInput.ActivateInputField();

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    selection = hit.collider.gameObject;
                    selectionText.text = $"\"{selection.name}\" ({selection.GetInstanceID():X8})";
                    AddMessage($"<color=#00FF00>Selected {selectionText.text}</color>");
                }
                else
                {
                    selection = null;
                    selectionText.text = string.Empty;
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) && consoleInput.text != string.Empty)
            {
                AddMessage(consoleInput.text);
                ParseInput(consoleInput.text);
                consoleInput.text = string.Empty;
                consoleInput.Select();
                consoleInput.ActivateInputField();
            }
        }
	}

    void AddMessageInternal(string message)
    {
        TMP_Text entry = Instantiate(entryPrefab);
        entries.Add(entry);
        entry.transform.SetParent(contentPane.transform);
        entry.text = message;
        if (entries.Count > entryLimit) entries.RemoveAt(0);
    }

    public static void AddMessage(string message) => instance.AddMessageInternal(message);

    void ParseInput(string input)
    {
        string[] splitInput = input.Split(null);

        if (splitInput.Length == 0 || splitInput == null)
        {
            AddMessage(COMMAND_NOT_RECOGNISED);
            consoleInput.Select();
            return;
        }

        if (!commands.ContainsKey(splitInput[0]))
        {
            AddMessage(COMMAND_NOT_RECOGNISED);
            consoleInput.Select();
            return;
        }

        commands[splitInput[0]].Run(splitInput);
        consoleInput.Select();
    }

    void OnLogMessage(string logMessage, string stackTrace, LogType type)
    {
        string typeColor = string.Empty;

        switch (type)
        {
            case LogType.Warning: typeColor = "<color=#FFFF00>"; break;
            case LogType.Assert:
            case LogType.Error:
            case LogType.Exception: typeColor = "<color=#FF0000>"; break;
            default: break;
        }

        AddMessage($"{typeColor}{logMessage}\n{stackTrace}{(string.IsNullOrEmpty(typeColor) ? "" : "</color>")}");
    }
}
