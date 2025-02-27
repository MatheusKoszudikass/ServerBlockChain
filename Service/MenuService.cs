using System;
using System.Collections.Generic;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service;
public class MenuDisplayService : IMenuDisplayService
{
    private readonly Dictionary<int, Action> _menuActions = [];
    public int TotalClientConnected { get; set; }
    private int Index = 0;
    private int CurrentPage = 0;
    private int TotalPages = 0;
    private string[] OptionsList = [];
    private object[] ObjectList = [];

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

    public void ExecuteOption(int selectedOption)
    {
        if (_menuActions.TryGetValue(selectedOption, out var action))
        {
            action.Invoke();
        }
        else
        {
            ConsoleHelp.WriteWarning(TypeHelp.Warning, "Invalid option selected.");
        }
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

    public void DisplayMenu(TypeHelp type, string[] array, int selectedIndex)
    {
        this.Index = selectedIndex;
        if (array.Length == 0)
        {
            Console.WriteLine("Item was not found");
            return;
        }

        if (selectedIndex >= 0) Console.Clear();

        Console.WriteLine($"Page {this.CurrentPage + 1} of {this.TotalPages}");
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

    public void DisplayMenu(TypeHelp type, string[] options, object[] objs, int selectedIndex)
    {
        this.Index = selectedIndex;
        if (objs.Length != 0)
        {
            if (selectedIndex >= 0) Console.Clear();

            string value = 
            $"Page {this.CurrentPage + 1} of {this.TotalPages}  Total Client connected: {TotalClientConnected}";
            ConsoleHelp.WriteWarning(TypeHelp.Menu, $"{value} \n");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    ConsoleHelp.WriteSuccess(type, $"> {i + 1}. {options[i]}");
                    continue;
                }

                ConsoleHelp.WriteMenu($"{i + 1}. {options[i]}");
            }
        }
        else
        {
            Console.WriteLine("Item was not found");
            return;
        }
    }

    public void SelectedView(TypeHelp type, string[] options)
    {
        int index = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
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

            DisplayMenu(TypeHelp.Menu);
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

        } while (key != ConsoleKey.Enter);
    }

    public object? SelectedOption(TypeHelp type, string[] options, object[] objs)
    {
        var objsFiltered = FilterList(objs, options);

        this.Index = 0;
        ConsoleKey key;

        if (objsFiltered.Length == 0) return objs;

        try
        {
            do
            {
                var objectList = PageList(type, options, objsFiltered);

                if (Console.KeyAvailable)
                {
                    break;
                }

                key = Console.ReadKey(true).Key;

                this.Index = SelectArrowDisplay(key, objectList, this.Index);

            } while (key != ConsoleKey.Enter);
            this.CurrentPage = 0;
            this.TotalPages = 0;

            if (_menuActions.ContainsKey(this.Index)) ExecuteOption(this.Index);

            return objsFiltered[this.Index];
        }
        catch (Exception ex)
        {
            throw new Exception($"Error when selecting option:{ex.Message}");
        }
    }

    //Needs improvement as the system is evolved
    private static object[] FilterList(object[] objs, string[] options, string searchTerm = "")
    {
        if (string.IsNullOrEmpty(searchTerm) || objs.Length > 0)
        {
            var filteredArray = objs
                .Where(item => item.ToString()?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true)
                .ToArray();

            Console.WriteLine(filteredArray.Length);
            return filteredArray;
        }
        return objs;
    }

    private object[] PageList(TypeHelp type, string[] options, object[] objs)
    {
        int itemsPerPage = 10;
        this.TotalPages = (int)Math.Ceiling((double)objs.Length / itemsPerPage);

        int startIndex = CurrentPage * itemsPerPage;

        if (objs.Length >= itemsPerPage)
        {
            this.ObjectList = [.. objs.Skip(startIndex).Take(itemsPerPage)];
            this.OptionsList = [.. options.Skip(startIndex).Take(itemsPerPage)];

            this.Index = Math.Min(this.Index, this.ObjectList.Length - 1);
            DisplayMenu(type, this.OptionsList, this.ObjectList, this.Index);
            return this.ObjectList;
        }

        Console.Clear();
        this.Index = Math.Min(this.Index, objs.Length - 1);
        DisplayMenu(type, options, objs, this.Index);
        return objs;
    }

    public int SelectedIndex()
    {
        if (int.TryParse(Console.ReadLine(), out int result))
        {
            return result;
        }

        return 0;
    }

    private int SelectArrowDisplay(ConsoleKey key, Object[] objs, int selectedIndex)
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
                case ConsoleKey.LeftArrow:
                    if (this.CurrentPage > 0)
                    {
                        Console.Clear();
                        CurrentPage--;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (this.CurrentPage < this.TotalPages - 1)
                    {
                        Console.Clear();
                        CurrentPage++;
                    }
                    break;
                case ConsoleKey.F:
                    if (objs.Length > 0)
                    {
                        ConsoleHelp.WriteMenu("Enter filter text: ");
                        string newFilter = Console.ReadLine() ?? "";
                        // FilterList(objs, options, newFilter);
                    }
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