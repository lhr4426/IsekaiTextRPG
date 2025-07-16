public class Program
{
    static void Main(string[] args)
    {
        GameManager gameManager = new GameManager();
        gameManager.GameStart();
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
    }
}

