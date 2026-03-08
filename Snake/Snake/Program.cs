Console.CursorVisible = false;
SnakeGame game = new SnakeGame(20, 40);
game.Run();

public class SnakeGame
{
    public Map Map { get; }
    public Snake Snake { get; }
    public FoodSpawner FoodSpawner { get; }
    public InputHandler InputHandler { get; }
    public ConsoleRenderer ConsoleRenderer { get; }
    public Direction Direction { get; set; }
    public  Location Food { get; set; }
    private int score = 0;

    public SnakeGame(int row, int column)
    {
        Map = new Map(row, column);
        Snake = new(new Location(column / 2, row / 2));
        FoodSpawner = new FoodSpawner();
        InputHandler = new InputHandler();
        ConsoleRenderer = new ConsoleRenderer(Map);
        Food = FoodSpawner.Spawn(Map, Snake);
    }

    public void Run()
    {
        while (true)
        {
            Direction = InputHandler.Input(Direction);
            bool grow = Snake.Head == Food;
            Snake.Movement(Direction, grow);
            if (grow)
            {
                Console.Beep(900, 120);
                Food = FoodSpawner.Spawn(Map, Snake);
                score++;
            }

            if (!Map.IsInside(Snake.Head))
                break;

            if (Snake.Body.Skip(1).Contains(Snake.Head))
                break;

            ConsoleRenderer.Display(Food, Snake, score);

            Thread.Sleep(150);
        }
        Console.Clear();
        Console.Beep(600, 150);
        Console.Beep(500, 150);
        Console.Beep(400, 200);
        Console.Beep(300, 400);
        Console.WriteLine("Game Over");
        Console.WriteLine("Your final score: " + score);
    }
}


public class Snake
{
    public List<Location> Body { get; } = new();

    public Snake(Location body) => Body.Add(body);

    public Location Head => Body[0];

    public void Movement(Direction direction, bool grow)
    {
        Location newHead = direction switch
        {
            Direction.Left => new Location(Head.X - 1, Head.Y),
            Direction.Right => new Location(Head.X + 1, Head.Y),
            Direction.Down => new Location(Head.X, Head.Y + 1),
            Direction.Up => new Location(Head.X, Head.Y - 1)
        };

        Body.Insert(0, newHead);
        if (!grow)
            Body.RemoveAt(Body.Count - 1);
    }
}

public class FoodSpawner
{
    Random random = new Random();

    public Location Spawn(Map map, Snake snake)
    {
        while (true)
        {
            int y = random.Next(map.Row);
            int x = random.Next(map.Column);
            Location location = new Location(x, y);
            if (!snake.Body.Contains(location))
                return location;
        }
    }
}

public class ConsoleRenderer
{
    private Map Map { get; }

    public ConsoleRenderer(Map map) => Map = map;

    public void Display(Location food, Snake snake, int score)
    {
        Console.SetCursorPosition(0, 0);
        for(int y = 0; y < Map.Row; y++)
        {
            for(int x = 0; x < Map.Column; x++)
            {
                Location position = new Location(x, y);
                if (position == snake.Head)Console.Write("O");
                else if (snake.Body.Contains(position)) Console.Write("A");
                else if(position == food) Console.Write("A");
                else Console.Write(".");
            }
            Console.WriteLine();
        }
        Console.WriteLine("=== SNAKE GAME ===");
        Console.WriteLine("Score: " + score);
    }
}

public class InputHandler
{
    public Direction Input(Direction direction)
    {
        if (!Console.KeyAvailable) return direction;
        ConsoleKey key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.A && direction != Direction.Right) direction = Direction.Left;
        else if (key == ConsoleKey.D && direction != Direction.Left) direction = Direction.Right;
        else if (key == ConsoleKey.S && direction != Direction.Up) direction = Direction.Down;
        else if (key == ConsoleKey.W && direction != Direction.Down) direction = Direction.Up;

        return direction;
    }
}

public record Location(int X, int Y);

public class Map
{
    public int Row { get; }
    public int Column { get; }

    public Map(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public bool IsInside(Location location) =>
        location.X >= 0 &&
        location.Y >= 0 &&
        location.X < Column &&
        location.Y < Row;
}

public enum Direction { Left, Right, Up, Down }