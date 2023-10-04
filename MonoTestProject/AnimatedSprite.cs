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
    private int numFrames;
    private int frameIndex; // share with durationIndex since numFrames == durations.Length
    private bool isLooping;

    private float fps = 0; // if duration is constant throughout animation

    //float durationIndex = 0;
    private float duration = 0;
    private float[] durations; // if duration is variable
    private float totalDuration = 0;

    private float currentDuration = 0;

    private int[] sequence; // sequence of frame index, elements must be between 0 and numframes-1 inclusive
    private int sequenceIndex = 0;

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
        sourceRect.X = frameIndex * sourceRect.Width;
        
        if (durations != null) AnimateVariableDuration();
        else if (fps > 0f) AnimateFixedDuration();
    }

    private void AnimateFixedDuration()
    {
        if (currentDuration > duration)
        {
            currentDuration = 0;
            ++frameIndex;
        }
        if (frameIndex > numFrames - 1)
        {
            if (isLooping) frameIndex = 0;
            else frameIndex = numFrames - 1;
        }
    }

    private void AnimateVariableDuration()
    {
        if (currentDuration > durations[frameIndex])
        {
            currentDuration = 0;
            ++frameIndex;
        }
        if (frameIndex > numFrames - 1)
        {
            if (isLooping) frameIndex = 0;
            else frameIndex = numFrames - 1;
        }
    }
}
