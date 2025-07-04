using ServerBlockChain.Entities.Enum;

public static class ConsoleHelp
{
    public static void WriteMenu(string message)
    {
        // BackGroundInColor(ConsoleColor.DarkBlue);
        WriteInColor("", message, ConsoleColor.White);
    }
    public static void WriteSuccess(TypeHelp type, string message)
    {
        WriteInColor(type.ToString(), message, ConsoleColor.Green);
    }

    public static void WriteError(TypeHelp type, string message)
    {
        WriteInColor(type.ToString(), message, ConsoleColor.Red);
    }

    public static void WriteWarning(TypeHelp type, string message)
    {
        WriteInColor(type.ToString() , message, ConsoleColor.Yellow);
    }

    private static void WriteInColor(string typeInfo, string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"{typeInfo}: {message}");
        Console.ResetColor();
    }

    private static void BackGroundInColor(ConsoleColor color)
    {
        Console.BackgroundColor = color;
    }
}
