using System;

namespace SideScrollConsole
{
    class Trajectory : GameObject
    {
        public Trajectory(Vector2 position) : base(position)
        {
            color = ConsoleColor.Yellow;
            health = 6;
            displayCharacter = " ^ ";
        }
    }
}