﻿using System;
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
        var answers = bender.GetAnswer();
        foreach (var answer in answers)
            Console.WriteLine(answer);
    }
}

public class Map
{
    private readonly char[,] _map;
    private readonly int _lines;
    private readonly int _columns;

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
}

public class Bender
{
    private readonly Map _map;
    private Point _position;
    private List<string> _answers = new List<string>();
    private HashSet<Point> _hashSet = new HashSet<Point>();
    public Bender(Map map)
    {
        _map = map;

        _position = _map.StartingPoint();

        CurrentDirection = Direction.South;
    }

    public Direction CurrentDirection { get; set; }

    public IEnumerable<string> GetAnswer()
    {
        int blockMove = 0;
        while (true)
        {
            var nextPosition = GetNextPosition();
            Console.Error.WriteLine("" + nextPosition.Line + ", " + nextPosition.Columns);
            var nextChar = _map.Get(nextPosition);

            if (nextChar == 'X' || nextChar == '#')
            {
                if (CurrentDirection != Direction.West)
                {
                    CurrentDirection = (Direction)blockMove;
                    blockMove++;
                }
                else
                {
                    break;
                }

                continue;
            }
            else if (nextChar == '$')
            {
                _answers.Add(CurrentDirection.ToString().ToUpper());
                break;
            }
            else
            {
                _hashSet.Add(nextPosition);
                _answers.Add(CurrentDirection.ToString().ToUpper());

                _position = nextPosition;
            }

            blockMove = 0;
        }
        return _answers;
    }

    private Point GetNextPosition()
    {
        return CurrentDirection switch
        {
            Direction.South => new Point(_position.Line + 1, _position.Columns),
            Direction.East => new Point(_position.Line, _position.Columns + 1),
            Direction.North => new Point(_position.Line - 1, _position.Columns),
            Direction.West => new Point(_position.Line, _position.Columns + 1),
            Direction.None => new Point(_position.Line, _position.Columns + 1),
            _ => throw new ArgumentOutOfRangeException()
        };
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