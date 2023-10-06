using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class AnimatedSprite : Sprite
{
    public Action OnAnimationEnd = () => { }; 

    private readonly int numFrames;
    private int frameIndex;
    private readonly bool isLooping;

    private readonly float fps = 0; // if duration is constant throughout animation

    private int durationIndex = 0;
    private float duration = 0;
    private float[] durations; // if duration is variable
    private float totalDuration = 0;

    private float currentDuration = 0;

    private int[] sequence; // sequence of frame index, elements must be between 0 and numframes-1 inclusive
    private int sequenceIndex = 0;

    private bool pairDurationsWithSequence = false;

    public AnimatedSprite(Texture2D texture, Rectangle sourceRect, Vector2 origin, int numFrames, float fps, bool isLooping = false, int[] sequence = null)
        : base(texture, sourceRect, origin)
    {
        this.numFrames = numFrames;
        this.fps = fps;
        this.sequence = sequence;
        this.isLooping = isLooping;
        totalDuration = fps;
        duration = 1f / fps;

        if (sequence != null)
        {
            frameIndex = sequence[0];
        }
    }

    /// <summary>
    /// <paramref name="durations"/> - contains the duration for each frame.
    /// The length of <paramref name="durations"/> must be the same as <paramref name="numFrames"/>
    /// <br></br>
    /// <paramref name="sequence"/> - The animation sequence. The frame numbers contained by the array
    /// must not exceed <paramref name="numFrames"/> - 1. eg: new int[] { 0, 1, 3 } will
    /// play frames 0 followed by frame 1 then frame 3. If <paramref name="isLooping"/> is true
    /// the sequence loops. 
    /// <br></br>
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="sourceRect"></param>
    /// <param name="origin"></param>
    /// <param name="numFrames"></param>
    /// <param name="durations"></param>
    /// <param name="isLooping"></param>
    /// <param name="sequence"></param>
    public AnimatedSprite(Texture2D texture, Rectangle sourceRect, Vector2 origin, int numFrames, float[] durations, bool isLooping = false, int[] sequence = null)
        : base(texture, sourceRect, origin)
    {
        this.numFrames = numFrames;
        this.durations = durations;
        this.sequence = sequence;
        this.isLooping = isLooping;
        totalDuration = fps;

        if (sequence != null)
        {
            frameIndex = sequence[0];
            pairDurationsWithSequence = sequence.Length == durations.Length;
        }
    }

    public override void Update(float deltaTime)
    {
        currentDuration += deltaTime;
        sourceRect.X = frameIndex * sourceRect.Width;
        
        if (durations != null) AnimateVariableDuration();
        else if (fps > 0f) AnimateFixedDuration();
    }

    public void ResetAnimation()
    {
        currentDuration = 0f;
        if (sequence != null) frameIndex = sequence[0];
        else frameIndex = 0;
        sequenceIndex = 0;
        durationIndex = 0;
    }

    private void NextFrame()
    {
        currentDuration = 0;
        
        if (sequence != null)
        {
            ++sequenceIndex;
            
            if (sequenceIndex >= sequence.Length) 
            {
                if (isLooping) sequenceIndex = 0;
                else sequenceIndex = sequence.Length - 1;
                OnAnimationEnd();
            }

            frameIndex = sequence[sequenceIndex];
            if (pairDurationsWithSequence) durationIndex = sequenceIndex;
            else durationIndex = frameIndex;
        }
        else
        {
            ++frameIndex;
            if (frameIndex > numFrames - 1)
            {
                if (isLooping) frameIndex = 0;
                else frameIndex = numFrames - 1;
                OnAnimationEnd();
            }
            durationIndex = frameIndex;
        }
    }

    private void AnimateFixedDuration()
    {
        if (currentDuration > duration)
        {
            NextFrame();
        }
    }

    private void AnimateVariableDuration()
    {
        if (currentDuration > durations[durationIndex])
        {
            NextFrame();
        }        
    }
}
