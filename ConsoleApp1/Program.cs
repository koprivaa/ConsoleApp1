using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Snake;
//Test
class Game
{
    private const int WindowHeight = 16;
    private const int WindowWidth = 32;
    private const int InitialScore = 5;
    private const int FrameDuration = 500;

    private readonly Board _board;
    private readonly Snake _snake;
    private Pixel _berry;
    private int _score;
    private readonly Random _random = new();

    public Game()
    {
        Console.SetWindowSize(WindowWidth, WindowHeight);
        _board = new Board(WindowWidth, WindowHeight);
        _snake = new Snake(WindowWidth / 2, WindowHeight / 2);
        _berry = GenerateRandomBerry();
        _score = InitialScore;
    }

    public void Run()
    {
        while (!_snake.HasCollision(WindowWidth, WindowHeight))
        {
            Console.Clear();
            _board.Draw();
            UpdateGame();
            DrawGame();
            WaitForNextFrame();
        }
        DisplayGameOverMessage();
    }

    private void UpdateGame()
    {
        if (_snake.Head.Equals(_berry))
        {
            _score++;
            _berry = GenerateRandomBerry();
        }
        _snake.Move();
    }

    private void DrawGame()
    {
        _snake.Draw();
        DrawPixel(_berry);
    }

    private Pixel GenerateRandomBerry() =>
        new(_random.Next(1, WindowWidth - 2), _random.Next(1, WindowHeight - 2), ConsoleColor.Cyan);

    private void WaitForNextFrame()
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.ElapsedMilliseconds <= FrameDuration)
            _snake.Direction = InputHandler.ReadDirection(_snake.Direction);
    }

    private void DisplayGameOverMessage()
    {
        Console.SetCursorPosition(WindowWidth / 5, WindowHeight / 2);
        Console.WriteLine($"Game over, Score: {_score - InitialScore}");
        Console.ReadKey();
    }

    private static void DrawPixel(Pixel pixel)
    {
        Console.SetCursorPosition(pixel.X, pixel.Y);
        Console.ForegroundColor = pixel.Color;
        Console.Write("■");
    }
}

class Snake
{
    public Pixel Head { get; private set; }
    public Direction Direction { get; set; } = Direction.Right;
    private readonly List<Pixel> _body = new();

    public Snake(int startX, int startY)
    {
        Head = new Pixel(startX, startY, ConsoleColor.Red);
    }

    public void Move()
    {
        _body.Add(Head);
        Head = Direction switch
        {
            Direction.Up => Head with { Y = Head.Y - 1 },
            Direction.Down => Head with { Y = Head.Y + 1 },
            Direction.Left => Head with { X = Head.X - 1 },
            Direction.Right => Head with { X = Head.X + 1 },
            _ => Head
        };
        if (_body.Count > 5) _body.RemoveAt(0);
    }

    public bool HasCollision(int width, int height) =>
        Head.X == 0 || Head.X == width - 1 ||
        Head.Y == 0 || Head.Y == height - 1 ||
        _body.Exists(segment => segment.Equals(Head));

    public void Draw()
    {
        foreach (var segment in _body) DrawPixel(segment);
        DrawPixel(Head);
    }

    private static void DrawPixel(Pixel pixel)
    {
        Console.SetCursorPosition(pixel.X, pixel.Y);
        Console.ForegroundColor = pixel.Color;
        Console.Write("■");
    }
}

class Board
{
    private readonly int _width;
    private readonly int _height;

    public Board(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Draw()
    {
        for (int i = 0; i < _width; i++)
        {
            Console.SetCursorPosition(i, 0);
            Console.Write("■");
            Console.SetCursorPosition(i, _height - 1);
            Console.Write("■");
        }

        for (int i = 0; i < _height; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write("■");
            Console.SetCursorPosition(_width - 1, i);
            Console.Write("■");
        }
    }
}

static class InputHandler
{
    public static Direction ReadDirection(Direction currentDirection)
    {
        if (!Console.KeyAvailable) return currentDirection;

        var key = Console.ReadKey(true).Key;
        return key switch
        {
            ConsoleKey.UpArrow when currentDirection != Direction.Down => Direction.Up,
            ConsoleKey.DownArrow when currentDirection != Direction.Up => Direction.Down,
            ConsoleKey.LeftArrow when currentDirection != Direction.Right => Direction.Left,
            ConsoleKey.RightArrow when currentDirection != Direction.Left => Direction.Right,
            _ => currentDirection
        };
    }
}

readonly record struct Pixel(int X, int Y, ConsoleColor Color);

enum Direction
{
    Up,
    Down,
    Right,
    Left
}

class Program
{
    static void Main() => new Game().Run();
}
