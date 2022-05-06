using System;
using System.Collections.Generic;

namespace SideScrollConsole
{
    public interface IAnimatable
    {
        public DateTime LastAnimFrameTimeStamp { get; set; }
        public TimeSpan DeltaTime { get; set; }
        public double AnimationFrameRate { get; set; }
        public List<string> Frames { get; set; }
        public int animIndex { get; set; }

        public void Animate();

    }
}