using System;

namespace SideScrollConsole
{
    class FallingObstacle : GameObject
    {
        public FallingObstacle(Vector2 position) : base(position)
        {
            color = ConsoleColor.White;
            health = 5;
            displayCharacter = " v ";
        }
    }
}