using System;

namespace SideScrollConsole
{
    class Enemy : GameObject
    {
        DateTime LastMoveTimeStamp;
        TimeSpan deltaTime;
        double moveRateInMilliSeconds = 1000;

        public Enemy(Vector2 position) : base(position)
        {
            LastMoveTimeStamp = DateTime.Now;
            color = ConsoleColor.Red;
            health = 1;
            displayCharacter = ")*(";
        }

        public void MoveTo(Vector2 targetPosition)
        {
            deltaTime = DateTime.Now - LastMoveTimeStamp;

            if (deltaTime.TotalMilliseconds >= moveRateInMilliSeconds)
            {
                position.x = targetPosition.x;
                position.y = targetPosition.y;
                LastMoveTimeStamp = DateTime.Now;
            }
        }
    }
}