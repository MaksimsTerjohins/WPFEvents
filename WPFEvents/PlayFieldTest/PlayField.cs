using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayFieldTest
{
    class Field
    {

        static string[] map = new string[]
        {
            "+------+",
            "|      |",
            "|A X   |",
            "|XXX   |",
            "|   X  |",
            "| B    |",
            "|      |",
            "+------+",
        };

        static int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

        static List<Cell> getWalkableAdjacentCells(int x, int y, string[] map)
        {
            var proposedCells = new List<Cell>()
            {
                new Cell() {X = x, Y = y - 1},
                new Cell() {X = x, Y = y + 1},
                new Cell() {X = x - 1, Y = y},
                new Cell() {X = x + 1, Y = y}
            };
            return proposedCells.Where(l => map[l.Y][l.X] == ' ' || map[l.Y][l.X] == 'B').ToList();
        }

        static void Main(string[] args)
        {
            Cell current = null;
            var start  = new Cell(1,1);
            var finish = new Cell(6,4);
            var openCells = new List<Cell>();
            var closedCells = new List<Cell>();
            int g = 0;

            openCells.Add(start);
            start.G = 0;
            start.F = start.G + start.calcH(start.X, start.Y, finish.X, finish.Y);
            while (openCells.Count > 0)
            {
                var lowest = openCells.Min(l => l.F);
                current = openCells.First(l => l.F == lowest);
                if (current.X != finish.X && current.Y != finish.Y)
                {
                    closedCells.Add(new Cell(current.X,current.Y));
                    openCells.Remove(current);
                    List<Cell> NeighborCells = getWalkableAdjacentCells(current.X, current.Y, map).ToList();
                    foreach (var cell in NeighborCells)
                    {
                        if (!closedCells.Contains(cell))
                        {
                            int tempG = current.G + 1; //СВОЕВОЛЬНОСТЬ - current.G + dist(cur - neighbor)
                            if (!openCells.Contains(cell) | tempG < cell.G)
                            {
                                cell.X = current.X; // ???
                                cell.Y = current.Y; // ???
                                cell.G = tempG;
                                cell.F = cell.G + cell.calcH(cell.X, cell.Y, finish.X, finish.Y);
                            }
                            //openCells.Add(cell);
                            //Console.WriteLine("Added NeighborCell to OpenCells: " + cell);
                        }
                    }

                }
               
            }
        }
    }
}
