using System;
using System.Collections.Generic;
using System.Linq;

namespace SideScrollConsole
{
    public class Cell
    {
        public static int counter = 0;
        public int id;
        public int x;
        public int y;
        public string defaultDisplayChars = " . ";
        public string displayCharacters = "";
        public int animIndex = 0;
        public ConsoleColor color;
        public static List<Cell> cells = new List<Cell>();

        public Cell(int x, int y)
        {
            counter++;
            this.id = counter;
            this.x = x;
            this.y = y;
            color = ConsoleColor.Blue;
            cells.Add(this);
        }

        public List<GameObject> GetOccupants(List<GameObject> gameObjects)
        {
            return gameObjects.Where(go => go.GetPosX() == x && go.GetPosY() == y).ToList();
        }

        public GameObject GetStrongestOccupant(List<GameObject> gameObjects)
        {
            List<GameObject> occupants = GetOccupants(gameObjects);
            GameObject winner = occupants.OrderByDescending(x => x.health).First();

            return winner;
        }
    }
}