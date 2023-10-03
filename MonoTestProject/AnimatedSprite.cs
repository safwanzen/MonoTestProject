using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

internal class AnimatedSprite : Sprite
{
    int numFrames;
    int frameIndex;
    bool isLooping;

    float fps; // if duration is constant throughout animation
    float duration = 0;
    float[] durations; // if duration is variable
    float totalDuration = 0;

    float currentDuration = 0;

    int[] sequence; // sequence of frame index, elements must be between 0 and numframes-1 inclusive
    int sequenceIndex = 0;

    public AnimatedSprite(Texture2D texture, Rectangle sourceRect, Vector2 origin, int numFrames, float fps, bool isLooping = false, int[] sequence = null)
        : base(texture, sourceRect, origin)
    {
        this.numFrames = numFrames;
        this.fps = fps;
        this.sequence = sequence;
        this.isLooping = isLooping;
        totalDuration = fps;
        duration = 1f / fps;
    }

    public AnimatedSprite(Texture2D texture, Rectangle sourceRect, Vector2 origin, int numFrames, float[] durations, bool isLooping = false, int[] sequence = null)
        : base(texture, sourceRect, origin)
    {
        this.numFrames = numFrames;
        this.durations = durations;
        this.sequence = sequence;
        this.isLooping = isLooping;
        totalDuration = fps;
    }

    public override void Update(float deltaTime)
    {
        currentDuration += deltaTime;
        if (fps > 0f)
        {
            sourceRect.X = frameIndex * sourceRect.Width;
            if (currentDuration > duration)
            {
                currentDuration = 0;
                ++frameIndex;
            }
            if (isLooping && frameIndex > numFrames - 1) { frameIndex = 0; }
            if (frameIndex > numFrames - 1) { frameIndex = numFrames - 1; }
        }
    }
}
