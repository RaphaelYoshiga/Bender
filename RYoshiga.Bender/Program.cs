using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Solution
{
    static void Main(string[] args)
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int L = int.Parse(inputs[0]);
        int C = int.Parse(inputs[1]);

        var map = new Map(L, C);
        for (int i = 0; i < L; i++)
        {
            var row = Console.ReadLine();
            Console.Error.WriteLine(row);
            map.SetRow(row, i);
        }

        var bender = new Bender(map);
        try
        {
            var answers = bender.GetAnswer();
            foreach (var answer in answers)
                Console.WriteLine(answer);
        }
        catch (LoopException e)
        {
            Console.WriteLine("LOOP");
        }

    }
}

public class Map
{
    private readonly char[,] _map;
    private readonly int _lines;
    private readonly int _columns;
    private int _brokenWalls = 0;

    public Map(int lines, int columns)
    {
        _columns = columns;
        _lines = lines;
        _map = new char[lines, columns];
    }

    public void SetRow(string row, in int line)
    {
        for (int i = 0; i < row.Length; i++)
            _map[line, i] = row[i];
    }

    public char Get(int line, int column)
    {
        if (line < 0 || line >= _lines)
            return '#';

        if (column < 0 || column >= _columns)
            return '#';

        return _map[line, column];
    }

    public Point StartingPoint()
    {
        for (int line = 0; line < _lines; line++)
        {
            for (int column = 0; column < _columns; column++)
            {
                var character = Get(line, column);
                if (character == '@')
                    return new Point(line, column);
            }

        }

        return new Point();
    }

    public int Get(Point position)
    {
        return Get(position.Line, position.Columns);
    }

    public void BreakWall(Point position)
    {
        _map[position.Line, position.Columns] = ' ';
        _brokenWalls++;
    }

    public Point OtherPortal(Point position)
    {
        for (int line = 0; line < _lines; line++)
        {
            for (int column = 0; column < _columns; column++)
            {
                var character = Get(line, column);
                if (character == 'T' && !position.Equals(new Point(line, column)))
                    return new Point(line, column);
            }
        }
        throw new InvalidOperationException("Can't find exit portal");
    }

    public int BrokenWalls => _brokenWalls;
}

public class Bender
{
    private readonly Map _map;
    private Point _position;
    private readonly List<string> _answers = new List<string>();
    private readonly HashSet<Step> _hashSet = new HashSet<Step>();
    private int _blockMove;
    private bool _breakerMode;
    private bool _circuit = true;

    public Bender(Map map)
    {
        _map = map;

        _position = _map.StartingPoint();

        CurrentDirection = Direction.South;
    }

    public Direction CurrentDirection { get; set; }

    public IEnumerable<string> GetAnswer()
    {
        _blockMove = 0;
        while (true)
        {
            var nextPosition = GetNextPosition();
            var nextChar = _map.Get(nextPosition);

            if (nextChar == '#' || (nextChar == 'X' && !_breakerMode))
            {
                SetNewDirection();

                continue;
            }

            Console.Error.WriteLine($"{nextPosition.Line}, {nextPosition.Columns} - Going: {CurrentDirection.ToString()}  breaker: {_breakerMode} circuit: {_circuit}");

            if (nextChar == '$')
            {
                _answers.Add(CurrentDirection.ToString().ToUpper());
                break;
            }

            HandleWalkableSpot(nextPosition, nextChar);

            _blockMove = 0;
        }
        return _answers;
    }

    private void SetNewDirection()
    {
        if (_circuit)
        {
            if (_blockMove >= 4)
            {
                throw new ArgumentOutOfRangeException();
            }

            CurrentDirection = (Direction)_blockMove;
            _blockMove++;

            return;
        }

        if (_blockMove >= 4)
        {
            throw new ArgumentOutOfRangeException();
        }

        CurrentDirection = (Direction)3 - _blockMove;
        _blockMove++;
    }

    private void HandleWalkableSpot(Point nextPosition, int nextChar)
    {
        _answers.Add(CurrentDirection.ToString().ToUpper());
        _position = nextPosition;

        CurrentDirection = nextChar switch
        {
            'S' => Direction.South,
            'W' => Direction.West,
            'E' => Direction.East,
            'N' => Direction.North,
            _ => CurrentDirection
        };

        if (nextChar == 'B')
        {
            this.DrinkBeer();
        }

        if (nextChar == 'I')
        {
            this.InvertCircuit();
        }

        if (nextChar == 'X')
        {
            _map.BreakWall(nextPosition);
        }

        if (nextChar == 'T')
        {
            _position = _map.OtherPortal(_position);
        }


        var step = new Step(nextPosition, CurrentDirection, _circuit, _breakerMode, _map.BrokenWalls);
        if (_hashSet.Contains(step))
        {
            Console.Error.WriteLine("Already have been here");
            throw new LoopException();

        }

        _hashSet.Add(step);
    }

    private void InvertCircuit()
    {
        _circuit = !_circuit;
    }

    private void DrinkBeer()
    {
        _breakerMode = !_breakerMode;
    }

    private Point GetNextPosition()
    {
        return CurrentDirection switch
        {
            Direction.South => new Point(_position.Line + 1, _position.Columns),
            Direction.East => new Point(_position.Line, _position.Columns + 1),
            Direction.North => new Point(_position.Line - 1, _position.Columns),
            Direction.West => new Point(_position.Line, _position.Columns - 1),
            Direction.None => new Point(_position.Line, _position.Columns + 1),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

internal class LoopException : Exception
{
}

internal struct Step
{
    public Point NextPosition { get; }
    public Direction CurrentDirection { get; }
    public bool Circuit { get; }
    public bool BreakerMode { get; }
    public int BrokenWalls { get; }

    public Step(Point nextPosition, Direction currentDirection, in bool circuit, in bool breakerMode,
        int brokenWalls)
    {
        NextPosition = nextPosition;
        CurrentDirection = currentDirection;
        Circuit = circuit;
        BreakerMode = breakerMode;
        BrokenWalls = brokenWalls;
    }
}

public struct Point
{
    public int Line { get; }
    public int Columns { get; }

    public Point(int line, int columns)
    {
        Line = line;
        Columns = columns;
    }
}

public enum Direction
{
    South,
    East,
    North,
    West,
    None
}