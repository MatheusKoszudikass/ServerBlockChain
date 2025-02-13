public static class ConsoleHelp
{
    public static void WriteSuccess(string message)
    {
        WriteInColor("SUCESSO", message, ConsoleColor.Green);
    }

    public static void WriteError(string message)
    {
        WriteInColor("ERRO", message, ConsoleColor.Red);
    }

    public static void WriteWarning(string message)
    {
        WriteInColor("AVISO", message, ConsoleColor.Yellow);
    }

    private static void WriteInColor(string typeInfo, string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"{typeInfo}: {message}");
        Console.ResetColor();
    }
}
