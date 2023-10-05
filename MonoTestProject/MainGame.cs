using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MonoTestProject;

public class MainGame : Game
{
    public static int ScreenWidth, ScreenHeight = 0;
    public static float GravityAcceleration = 50f;

    Texture2D ballTexture;
    Vector2 ballPosition;
    Vector2 ballSpeed;
    float ballRotation;

    public static Texture2D handTexture;
    public static Texture2D particleTexture;
    public static Texture2D ParticleTrailTexture;
    public static Texture2D BulletTextureMedium;
    public static Texture2D BulletTextureLarge;
    public static Texture2D BulletTextureXLarge;
    public static Texture2D BulletSheet;
    public Texture2D CaveStoryCharSheet;

    public static SoundEffect BulletHitSound;
    public static SoundEffect BulletFireSound;
    public static List<SoundEffect> Sounds = new();

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static List<Bullet> Bullets = new();
    public static List<Enemy> Enemies = new();
    public static List<Entity> Entities = new();

    public static ButtonState PrevLMBState = ButtonState.Released;
    public static ButtonState PrevRMBState = ButtonState.Released;

    public static SpriteFont Font;

    public static World World = new();

    PlatformerCharacter character;
    
    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        ScreenWidth = _graphics.PreferredBackBufferWidth;
        ScreenHeight = _graphics.PreferredBackBufferHeight;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        Console.WriteLine("initialize called");
        base.Initialize();
        SoundEffect.MasterVolume = .0f;
    }

    protected override void LoadContent()
    {
        Console.WriteLine("loadcontent called");
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        Font = Content.Load<SpriteFont>("TextFont");
        handTexture = Content.Load<Texture2D>("hand");
        ballTexture = handTexture; //Content.Load<Texture2D>("ball");        
        BulletTextureMedium = Content.Load<Texture2D>("ms_bullet_round_medium");
        BulletTextureLarge = Content.Load<Texture2D>("ms_bullet_round_large");
        BulletTextureXLarge = Content.Load<Texture2D>("ms_bullet_round_xlarge");
        BulletSheet = Content.Load<Texture2D>("ms_bullet_16x48");
        CaveStoryCharSheet = Content.Load<Texture2D>("cave-story-wii-sprite-sheet-transparent");

        BulletHitSound = Content.Load<SoundEffect>("Audio/MMX3_SE_00044");
        BulletFireSound = Content.Load<SoundEffect>("Audio/ST01_00_00002");

        Sounds.Add(Content.Load<SoundEffect>("Audio/MMX3_SE_00044"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/PL00_U_00027"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/PL02_U_00020"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/ST01_00_00002"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/ST01_00_00003"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/PL01_U_00024"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/PL00_U_00016"));

        var data = new Color[handTexture.Width * handTexture.Height];
        handTexture.GetData(data);
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].A == 0) continue;
            data[i] = Color.Orange;
        }

        particleTexture = new Texture2D(GraphicsDevice, handTexture.Width, handTexture.Height);
        particleTexture.SetData(data);

        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].A == 0) continue;
            data[i] = Color.AliceBlue;
        }
        ParticleTrailTexture = new Texture2D(GraphicsDevice, handTexture.Width, handTexture.Height);
        ParticleTrailTexture.SetData(data);

        // init objects
        ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2);
        ballSpeed = new Vector2();

        int sqw = 32;
        character = new PlatformerCharacter()
        {
            Position = new Vector2(ScreenWidth / 2, ScreenHeight / 2),
            runningSprite = new AnimatedSprite(
                CaveStoryCharSheet,
                new Rectangle(0 * sqw, 1 * sqw, sqw, sqw),
                new Vector2(sqw / 2, sqw / 2),
                3, 
                durations: new float[] { .15f, .083f, .15f, .083f },
                true, 
                sequence: new int[] { 1, 0, 2, 0 }),
            standingSprite = new Sprite(
                CaveStoryCharSheet,
                new Rectangle(0 * sqw, 1 * sqw, sqw, sqw),
                new Vector2(sqw / 2, sqw / 2)
                ),
        };

        //enemy = new Enemy(new Vector2(500, 200));
        //Entities.Add(enemy);
    }

    Random random = new Random();
    
    private float xoff = 0, yoff = 0;
    private float xscale = 1, yscale = 1;
    private float xscreencenter, yscreencenter;
    Vector2 lastMousePosition;
    private int lastScrollWheel;

    protected override void Update(GameTime gameTime)
    {
        InputManager.BeginFrame();
        //Console.WriteLine("update \t {0}", gameTime.TotalGameTime);
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        var mousestate = Mouse.GetState(Window);

        character.Update(deltaTime);
        
        foreach (var enemy in Enemies)
        {
            enemy.Update(deltaTime);
        }
        
        //if (PrevRMBState == ButtonState.Released && mousestate.RightButton == ButtonState.Pressed)
        if (InputManager.IsPressed(MouseButtons.RightButton))
        {
            //for (int i = 0; i < 5000; i++) Enemies.Add(new Enemy(mousestate.Position.ToVector2()));
            var e = new Enemy(World.ScreenToWorld(mousestate.Position.ToVector2()));
            Console.WriteLine("Enemy added {0}", e.GetHashCode());
            Enemies.Add(e);
        }

        var currmouseposition = mousestate.Position.ToVector2();

        if (InputManager.IsDown(MouseButtons.LeftButton) 
            && currmouseposition.X < ScreenWidth && currmouseposition.Y < ScreenHeight 
            && currmouseposition.X > 0 && currmouseposition.Y > 0)
        {
            var diff = currmouseposition - lastMousePosition;
            xoff += diff.X / xscale;
            yoff += diff.Y / yscale;
            World.SetOffset(xoff, yoff);
        }

        var oldmouseworldcoord = World.ScreenToWorld(currmouseposition);

        if (lastScrollWheel - mousestate.ScrollWheelValue > 0)
        {
            xscale *= 1.1f;
            yscale *= 1.1f;
            World.scaleX = xscale;
            World.scaleY = yscale;
        }

        if (lastScrollWheel - mousestate.ScrollWheelValue < 0)
        {
            xscale *= .9f;
            yscale *= .9f;
            World.scaleX = xscale;
            World.scaleY = yscale;
        }

        var newmouseworldcoord = World.ScreenToWorld(currmouseposition);
        var mousediff = newmouseworldcoord - oldmouseworldcoord;
        xoff += mousediff.X;
        yoff += mousediff.Y;
        World.SetOffset(xoff, yoff);

        lastMousePosition = mousestate.Position.ToVector2();
        lastScrollWheel = mousestate.ScrollWheelValue;

        for (int i = 0; i < Entities.Count;)
        {
            if (!Entities[i].IsAlive) Entities.RemoveAt(i);
            else
            {
                Entities[i].Update(deltaTime);
                i++;
            }
        }

        for (int i = 0; i < Bullets.Count;)
        {
            if (Bullets[i].WasHit)
            {
                for (int a = 0; a < 5; a++)
                {
                    var p = new Particle(Bullets[i].Position, (float)(random.NextDouble() * MathHelper.Pi * 2), 0.1f)
                    {
                        Speed = 600
                    };
                    Entities.Add(p);
                }
                Bullets.RemoveAt(i);
            }
            else
            {
                Bullets[i].Update(deltaTime);
                i++;
            }
        }

        // reset mouse
        //PrevLMBState = mousestate.LeftButton;
        //PrevRMBState = mousestate.RightButton;
        InputManager.EndFrame();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //Console.WriteLine("draw \t {0}", gameTime.TotalGameTime);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        character.Draw(_spriteBatch);

        foreach (var entity in Enemies)
        {
            entity.Draw(_spriteBatch);
        }

        foreach(var trail in Entities)
        {
            trail.Draw(_spriteBatch);
        }

        foreach (var particle in Bullets)
        {
            particle.Draw(_spriteBatch);
        }

        //_spriteBatch.Draw(ballTexture, ballPosition, null, Color.White, ballRotation + MathHelper.PiOver2,
        //    new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);

        _spriteBatch.DrawString(Font, "(0, 0)", World.WorldToScreen(0, 0), Color.White, 0f, Vector2.Zero, new Vector2(xscale, yscale), SpriteEffects.None, 0);

        _spriteBatch.DrawString(Font, $"Bullets: {Bullets.Count}", new Vector2(10, 10), Color.Black);
        _spriteBatch.DrawString(Font, $"Enemies: {Enemies.Count}", new Vector2(10, 30), Color.Black);
        _spriteBatch.DrawString(Font, $"SpeedX: {character.Position.X}", new Vector2(10, 50), Color.Black);
        _spriteBatch.DrawString(Font, $"SpeedY: {character.Position.Y}", new Vector2(10, 70), Color.Black);
        //_spriteBatch.DrawString(Font, $"Speed magnitude: {character.speedMagnitude}", new Vector2(10, 90), Color.Black);
        //_spriteBatch.DrawString(Font, $"Direction: {character.direction}", new Vector2(10, 110), Color.Black);
        _spriteBatch.DrawString(Font, $"MouseXY: {lastMousePosition.X} {lastMousePosition.Y}", new Vector2(10, 90), Color.Black);
        //_spriteBatch.DrawString(Font, $"MouseXY: {World.ScreenToWorld)}", new Vector2(10, 110), Color.Black);
        _spriteBatch.DrawString(Font, $"world offset XY: {xoff} {yoff}", new Vector2(10, 130), Color.Black);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}