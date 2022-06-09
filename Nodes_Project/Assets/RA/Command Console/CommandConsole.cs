using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RA.CommandConsole
{
    public class CommandConsole : MonoBehaviour
    {
        private static bool showConsole;
        private static bool showHelp;
        private string input;
        private Vector2 scroll;
        private List<object> commandList;

        private void Awake()
        {
            // ESTO ES SOLO TESTEO LUEGO HAY QUE IMPLEMENTARLO DE UNA MANERA MAS MODULAR
            //
            // sugerencia 1: los comandos se le puedan pasar funciones como
            // si fueran eventos atraves de la interfaz de unity.
            //
            // sugerencia 2: que a las clases se les pueda marcar algunas funciones o
            // agregar algunas funciones que esto las pueda recoger asi esos metodos simepre
            // estaran disponibles desde el momento que son creadas y se compila el codigo.

            var addMoney = new DebugCommand<int>("add_money", "Add the amount of money", "add_money", (X) =>
            {
                Debug.Log("add money: " + X); // implementar en codigo correspondiente
            });

            var nextDay = new DebugCommand("next_day", "*****", "next_day", () =>
            {
                Debug.Log("next day"); // implementar en codigo correspondiente
            });

            var spawnNode = new DebugCommand<string>("spawn_node", "*****", "spawn_node", (X) =>
            {
                Debug.Log("Spawn: " + X); // implementar en codigo correspondiente
            });

            var destroyAll = new DebugCommand("destroy_all", "*****", "destroy_all", () =>
            {
                Debug.Log("Destroy all"); // implementar en codigo correspondiente
            });

            var getReward = new DebugCommand("get_reward","*****","get_reward",() =>
            {
                Debug.Log("Get Reward"); // implementar en codigo correspondiente
            });

            var help = new DebugCommand("help", "show a list of commands", "help", () => 
            {
                showHelp = true;
            });

            commandList = new List<object>
            {
                addMoney,
                nextDay,
                spawnNode,
                destroyAll,
                help
            };
            commandList = CollectMetohdsFromScripts();
        }

        /// <summary>
        /// Toggles the state of the debug console between active and inactive.
        /// </summary>
        public void ToggleConsole()
        {
            showConsole = !showConsole;
        }

        /// <summary>
        /// Toggles the state of the debug console between active and inactive.
        /// </summary>
        public void ToggleConsole(InputValue value)
        {
            showConsole = !showConsole;
        }

        /// <summary>
        /// Executes what is delivered by text and resets it.
        /// </summary>
        public void OnRetun(InputValue value)
        {
            if(showConsole)
            {
                HandleInput();
                input = "";
            }
        }

        /// <summary>
        /// Executes what is delivered by text and resets it.
        /// </summary>
        public void OnRetun()
        {
            if (showConsole)
            {
                HandleInput();
                input = "";
            }
        }

        private void OnGUI()
        {
            if (!showConsole) return;

            float y = 0f;

            if(showHelp)
            {
                GUI.Box(new Rect(0, y, Screen.width, 100), "");
                Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);
                scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), scroll, viewport);

                for (int i = 0; i < commandList.Count; i++)
                {
                    DebugCommandBase command = commandList[i] as DebugCommandBase;
                    string label = $"{command.CommandFormat} - {command.CommandDescription}";
                    Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                    GUI.Label(labelRect,label);
                }
                GUI.EndScrollView();
                y += 100;
            }

            GUI.Box(new Rect(0, y, Screen.width, 30), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
        }

        /// <summary>
        /// Takes the text delivered by the input, divides it and identifies it,
        /// finally executes the code associated with the command.
        /// </summary>
        private void HandleInput()
        {
            string[] properties = input.Split(' ');

            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase commandBase = commandList[i] as DebugCommandBase;
                if (input.Contains(commandBase.CommandId))
                {
                    if (commandList[i] as DebugCommand != null)
                    {
                        (commandList[i] as DebugCommand).Invoke();
                    }
                    else if ((commandList[i] as DebugCommand<int>) != null)
                    {
                        (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[i]));
                    }
                }
            }
        }

        //WIP
        public List<object> CollectMetohdsFromScripts() // poner un nombre decente!
        {
            List<object> commands = new List<object>();

            var methods = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .SelectMany(t => t.GetMethods())
                        .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
                        .ToArray();

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttributes(typeof(CommandAttribute),false)[0] as CommandAttribute;
                var action = (Action)method.CreateDelegate(typeof(Action));
                commands.Add(new DebugCommand(attribute.Id, attribute.Description, attribute.Format,action));
            }
            return commands;
        }

        // TEST de trabajar con headers y commandos, dejar si funciona!
        [Command("help", "show a list of commands.", "help")]
        public static void Help(string s)
        {
            showHelp = true;
        }
    }


}