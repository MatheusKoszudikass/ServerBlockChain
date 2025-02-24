using System;
using System.Collections.Generic;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class MenuDisplayService : IMenuDisplayService
    {
        private readonly Dictionary<int, Action> _menuActions = [];

        public void RegisterOption(int option, Action action)
        {
            if (!_menuActions.ContainsKey(option))
            {
                _menuActions.Add(option, action);
            }
        }

        public void DeleteOption()
        {
            _menuActions.Clear();
        }

        public void DisplayMenu(TypeHelp type) => ConsoleHelp.WriteMenu("Press any key to return...");

        public void DisplayMenu(TypeHelp type, object[] array)
        {
            Console.Clear();

            if (array.Length == 0)
            {
                Console.WriteLine("Item was not found");
                
                return;
            }

            for (int i = 0; i < array.Length; i++)
            {
                ConsoleHelp.WriteMenu($"{i + 1}. {array[i]}");
            }
        }

        public void DisplayMenu(TypeHelp type, object[] array, int selectedIndex)
        {
            if (selectedIndex >= 0) Console.Clear();

            for (int i = 0; i < array.Length; i++)
            {
                if (i == selectedIndex)
                {
                    ConsoleHelp.WriteSuccess(type, $"> {i + 1}. {array[i]}");
                    continue;
                }

                ConsoleHelp.WriteMenu($"{i + 1}. {array[i]}");
            }
        }

        public void ExecuteOption(int selectedOption)
        {
            if (_menuActions.TryGetValue(selectedOption, out var action))
            {
                action.Invoke();
            }
            else
            {
                ConsoleHelp.WriteWarning("Invalid option selected.");
            }
        }

        public void SelectedView(TypeHelp type, string[] options)
        {
            int index = 0;
            ConsoleKey key;

            do
            {

                DisplayMenu(type, options);

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == index)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        index = (index == 0) ? options.Length - 1 : index - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        index = (index == options.Length - 1) ? 0 : index + 1;
                        break;
                }

            } while (key != ConsoleKey.Escape); // Sai do loop ao pressionar ESC
        }


        public object SelectedOption(TypeHelp type, object[] array, object[] objs)
        {
            int selectedIndex = 0;
            ConsoleKey key;

            try
            {
                do
                {
                    DisplayMenu(type, array, selectedIndex);

                    if (Console.KeyAvailable)
                    {
                        break;
                    }

                    key = Console.ReadKey(true).Key;

                    selectedIndex = SelectArrowDisplay(key, objs, selectedIndex);

                } while (key != ConsoleKey.Enter);

                ExecuteOption(selectedIndex);
                return selectedIndex;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when selecting option:{ex.Message}");
            }
        }

        public int SelectedIndex()
        {
            if (int.TryParse(Console.ReadLine(), out int result))
            {
                return result;
            }

            return 0;
        }

        private static int SelectArrowDisplay(ConsoleKey key, Object[] objs, int selectedIndex)
        {
            try
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex == 0) ? objs.Length - 1 : selectedIndex - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex == objs.Length - 1) ? 0 : selectedIndex + 1;
                        break;
                }

                return selectedIndex;
            }
            catch (Exception)
            {
                throw new Exception("Invalid option selected.");
            }
        }
    }
}