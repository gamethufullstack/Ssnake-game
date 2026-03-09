Console.CursorVisible = false;
SnakeGame game = new SnakeGame(20, 40);
game.Run();

public class SnakeGame
{
    public Map Map { get; }
    public Snake Snake { get; }
    public FoodSpawner FoodSpawner { get; }
    public Direction Direction { get; set; }
    public Location Food { get; set; }
    public int Score { get; set; }

    public SnakeGame(int x, int y)
    {
        Map = new Map(x, y);
        Snake = new Snake(new Location(x / 2, y / 2));
        FoodSpawner = new FoodSpawner();
        Food = FoodSpawner.Spawn(Map, Snake);
    }

    public void Run()
    {
        while (true)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("==== SNAKE GAME ===");
            Console.WriteLine("Score: " + Score);
            Direction = Snake.InputHandler(Direction);
            bool grow = Snake.Head == Food;
            Snake.Movement(Direction, grow);

            if (grow)
            {
                Console.Beep(900, 120);
                Food = FoodSpawner.Spawn(Map, Snake);
                Score++;
            }

            if (!Map.IsOnMap(Snake.Head)) break;
            if (Snake.Body.Skip(1).Contains(Snake.Head)) break;
            Thread.Sleep(150);
            Display(Food, Snake);
        }

        Console.Clear();
        Console.Beep(600, 150);
        Console.Beep(500, 150);
        Console.Beep(400, 200);
        Console.Beep(300, 400);
        Console.WriteLine("Game Over");
    }

    public void Display(Location food, Snake snake)
    {
        Console.SetCursorPosition(0, 3);
        for (int y = 0; y < Map.Row; y++)
        {
            for(int x = 0; x < Map.Column; x++)
            {
                Location position = new Location(x, y);
                
                if(position == snake.Head) Console.Write("O");
                else if (snake.Body.Contains(position)) Console.Write("A");
                else if(position == food) Console.Write("A");
                else Console.Write(".");
            }
            Console.WriteLine();
        }
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

        if (!grow) Body.RemoveAt(Body.Count - 1);
    }

    public Direction InputHandler(Direction direction)
    {
        if (!Console.KeyAvailable) return direction;

        ConsoleKey key = Console.ReadKey(true).Key;

        if ((key == ConsoleKey.A || key == ConsoleKey.LeftArrow) && direction != Direction.Right) direction =  Direction.Left;
        if ((key == ConsoleKey.D || key == ConsoleKey.RightArrow) && direction != Direction.Left) direction = Direction.Right;
        if ((key == ConsoleKey.S || key == ConsoleKey.DownArrow) && direction != Direction.Up) direction = Direction.Down;
        if ((key == ConsoleKey.W || key == ConsoleKey.UpArrow) && direction != Direction.Down) direction = Direction.Up;

        return direction;
    }
}

public class FoodSpawner
{
    public Random Random { get;} =  new Random();

    public Location Spawn(Map map, Snake snake)
    {
        while (true)
        {
            int x = Random.Next(map.Column);
            int y = Random.Next(map.Row);

            Location location = new Location(x, y);

            if (!snake.Body.Contains(location)) 
                return location;
        }
    }
}

public class Map
{
    public int Row { get; set; }
    public int Column { get; set; }

    public Map(int row, int column)
    {
        Row = row;
        Column = column;

    }

    public bool IsOnMap(Location location) =>
         location.X >= 0 &&
        location.X < Column &&
        location.Y >= 0 &&
        location.Y < Row;
}

public record Location(int X, int Y);

public enum Direction { Up, Down, Right, Left }
