using System;
using System.Collections.Generic;

namespace SideScrollConsole
{
    public class GameObject
    {
        public Vector2 position;
        public bool isActive;
        public string displayCharacter = "";
        public ConsoleColor color;
        public int health = 1;

        public static List<GameObject> AllObjects = new List<GameObject>();

        public GameObject(Vector2 position)
        {
            color = ConsoleColor.Blue;
            this.position = position;
            isActive = true;
            AllObjects.Add(this);
        }

        public int GetPosX()
        {
            return (int)Math.Round(this.position.x);
        }

        public int GetPosY()
        {
            return (int)Math.Round(this.position.y);
        }
    }
}