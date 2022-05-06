using System;
using System.Collections.Generic;

namespace SideScrollConsole
{
    class AnimatedTest : GameObject, IAnimatable
    {
        public AnimatedTest(Vector2 position, List<string> animationFrames) : base(position)
        {
            LastAnimFrameTimeStamp = DateTime.Now;
            displayCharacter = "X";
            color = ConsoleColor.Magenta;
            Frames = animationFrames;
        }

        public DateTime LastAnimFrameTimeStamp { get; set; }
        public TimeSpan DeltaTime { get; set; }
        public double AnimationFrameRate { get => 100; set { } }
        public int animIndex { get; set; }

        public List<string> Frames { get; set; }

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