﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitsOfWork
{
    public class Program
    {
        public static void Main()
        {
            var game = new Game();
            var finalResult = new StringBuilder();

            var line = Console.ReadLine();
            while (line != "end")
            {
                var command = Command.Parse(line);

                switch (command.Name)
                {
                    case "add":
                        var unit = Unit.Parse(command.Arguments);
                        var added = game.Add(unit);
                        if (added)
                        {
                            finalResult.AppendLine(string.Format("SUCCESS: {0} added!", unit.Name));
                        }
                        else
                        {
                            finalResult.AppendLine(string.Format("FAIL: {0} already exists!", unit.Name));
                        }

                        break;

                    case "remove":
                        var name = command.Arguments[0];
                        var removed = game.Remove(name);
                        if (removed)
                        {
                            finalResult.AppendLine(string.Format("SUCCESS: {0} removed!", name));
                        }
                        else
                        {
                            finalResult.AppendLine(string.Format("FAIL: {0} could not be found!", name));
                        }

                        break;

                    case "find":
                        var type = command.Arguments[0];
                        var found = game.FindByType(type);
                        finalResult.AppendLine(string.Format("RESULT: {0}", string.Join(", ", found)));

                        break;

                    case "power":
                        var mostPowerful = game.MostPowerful(int.Parse(command.Arguments[0]));
                        finalResult.AppendLine(string.Format("RESULT: {0}", string.Join(", ", mostPowerful)));

                        break;
                    default:
                        throw new InvalidOperationException();
                }

                line = Console.ReadLine();
            }

            Console.WriteLine(finalResult.ToString().Trim());
        }
    }

    public class Unit : IComparable<Unit>
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Attack { get; set; }

        public static Unit Parse(IList<string> unitParts)
        {
            return new Unit
            {
                Name = unitParts[0],
                Type = unitParts[1],
                Attack = int.Parse(unitParts[2])
            };
        }

        public int CompareTo(Unit other)
        {
            var result = this.Attack.CompareTo(other.Attack) * -1;
            if (result == 0)
            {
                result = this.Name.CompareTo(other.Name);
            }

            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]({2})", this.Name, this.Type, this.Attack);
        }
    }

    public class Command
    {
        public string Name { get; set; }

        public IList<string> Arguments { get; set; }

        public static Command Parse(string commandAsString)
        {
            var commandParts = commandAsString.Split(' ');

            var name = commandParts[0];

            var newListOfStrings = new List<string>();
            for (int i = 1; i < commandParts.Length; i++)
            {
                newListOfStrings.Add(commandParts[i]);
            }

            return new Command
            {
                Name = name,
                Arguments = newListOfStrings
            };
        }
    }

    public class Game
    {
        private List<Unit> units;

        public Game()
        {
            this.units = new List<Unit>();
        }

        public bool Add(Unit unit)
        {
            if (this.units.Any(u => u.Name == unit.Name))
            {
                return false;
            }
            
            this.units.Add(unit);

            return true;
        }

        public IEnumerable<Unit> FindByType(string type)
        {
            return this.units
                .Where(u => u.Type == type)
                .OrderByDescending(u => u.Attack)
                .ThenBy(u => u.Type)
                .Take(10);
        }

        public IEnumerable<Unit> MostPowerful(int numberOfUnits)
        {
            return this.units
                .OrderByDescending(u => u.Attack)
                .Take(numberOfUnits);
        }

        public bool Remove(string name)
        {
            if (!this.units.Any(u => u.Name == name))
            {
                return false;
            }

            this.units.RemoveAll(u => u.Name == name);

            return true;
        }
    }
}
