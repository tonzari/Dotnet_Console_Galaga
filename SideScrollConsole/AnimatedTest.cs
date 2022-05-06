using System;
using System.Collections.Generic;

namespace SideScrollConsole
{
    class AnimatedTest : GameObject, IAnimatable
    {
        public AnimatedTest(Vector2 position) : base(position)
        {
            LastAnimFrameTimeStamp = DateTime.Now;
            displayCharacter = "X";
            color = ConsoleColor.Magenta;
        }

        public DateTime LastAnimFrameTimeStamp { get; set; }
        public TimeSpan DeltaTime { get; set; }
        public double AnimationFrameRate { get => 100; set { } }
        public List<string> Frames { get => new List<string>() { "^_^", "-_-", "-_-", "*_*", "*_*", "^_^" }; set { } }
        public int animIndex { get; set; }

        public void Animate()
        {
            DeltaTime = DateTime.Now - LastAnimFrameTimeStamp;

            if (DeltaTime.TotalMilliseconds >= AnimationFrameRate)
            {
                displayCharacter = Frames[animIndex];

                LastAnimFrameTimeStamp = DateTime.Now;
            }

            animIndex++;

            if (animIndex > Frames.Count-1)
            {
                animIndex = 0;
            }
        }
    }
}