using System;
using System.Collections.Generic;

namespace SideScrollConsole
{
    class Player : GameObject
    {
        public List<Trajectory> trajectories = new List<Trajectory>();

        public Player(Vector2 position) : base(position)
        {
            color = ConsoleColor.Cyan;
            health = 4;
            displayCharacter = "\\o/";
        }

        public void MoveTo(Vector2 targetPosition)
        {
            position.x = targetPosition.x;
            position.y = targetPosition.y;
        }

        public void Shoot(Vector2 targetPosition)
        {
            Trajectory trajectory = new Trajectory(new Vector2(this.position.x, this.position.y));
            trajectories.Add(trajectory);
        }
    }
}