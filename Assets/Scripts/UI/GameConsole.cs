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
using UnityEngine.SceneManagement;

public abstract class ConsoleCommand
{
    public abstract string description { get; protected set; }

    public abstract void Run(string[] args);
}

public class GameConsole : MonoBehaviour
{
    public static GameConsole instance { get; private set; }
    public static Dictionary<string, ConsoleCommand> commands { get; private set; }
    public static GameObject selection { get; private set; }
    public static GameObject player { get; set; }

    [Header("UI")]
    public GameObject console;
    public GameObject contentPane;
    public TMP_InputField consoleInput;
    public TMP_Text selectionText;

    [Header("Behavior")]
    public LayerMask layerMask;
    public int entryLimit = 256;

    List<TMP_Text> entries;
    List<string> commandCache;
    TMP_Text entryPrefab;
    int cacheIndex = 0;
    static bool exiting = false;

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
            { "GetPos", new CommandGetPos() },
            { "Damage", new CommandDamage() }
		};
	}

    void Start()
    {
        console.SetActive(false);
        selection = null;
        entryPrefab = Resources.Load<TMP_Text>("Prefabs/Console Entry");
        entries = new List<TMP_Text>();
        commandCache = new List<string>();
    }

    private void OnEnable()
    {
        Application.logMessageReceived += OnLogMessage;
        SceneManager.sceneUnloaded += OnSceneUnload;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= OnLogMessage;
        SceneManager.sceneUnloaded -= OnSceneUnload;
    }

    private void OnSceneUnload<Scene>(Scene scene) => exiting = true;

	void Update()
    {
        if (exiting) return;

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
                commandCache.Add(consoleInput.text);
                cacheIndex = commandCache.Count - 1;
                AddMessage(consoleInput.text);
                ParseInput(consoleInput.text);
                consoleInput.text = string.Empty;
                consoleInput.Select();
                consoleInput.ActivateInputField();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && commandCache.Count > cacheIndex && cacheIndex >= 0)
            {
                consoleInput.text = commandCache[cacheIndex--];
                consoleInput.Select();
                consoleInput.ActivateInputField();
                if (cacheIndex < 0) cacheIndex = 0;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && cacheIndex < commandCache.Count)
            {
                consoleInput.text = commandCache[cacheIndex++];
                consoleInput.Select();
                consoleInput.ActivateInputField();
                if (cacheIndex >= commandCache.Count) cacheIndex = commandCache.Count - 1;
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

    public static void AddMessage(string message)
    {
        if (exiting) return;
        instance.AddMessageInternal(message);
    }

    void ParseInput(string input)
    {
        string[] splitInput = input.Split(null);

        if (splitInput.Length == 0 || splitInput == null)
        {
            AddMessage("<color=#FF0000>Command not recognised</color>");
            consoleInput.Select();
            return;
        }

        if (!commands.ContainsKey(splitInput[0]))
        {
            AddMessage("<color=#FF0000>Command not recognised</color>");
            consoleInput.Select();
            return;
        }

        commands[splitInput[0]].Run(splitInput);
        consoleInput.Select();
    }

    void OnLogMessage(string logMessage, string stackTrace, LogType type)
    {
        if (exiting) return;

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
