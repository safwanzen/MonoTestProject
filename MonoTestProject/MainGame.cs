using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MonoTestProject;

public class MainGame : Game
{
    public static int ScreenWidth, ScreenHeight = 0;
    public static float GravityAcceleration = 50f;

    Texture2D ballTexture;
    Vector2 ballPosition;
    Vector2 ballSpeed;
    float ballRotation;

    Texture2D handTexture;
    Texture2D particleTexture;
    public static Texture2D ParticleTrailTexture;

    Character character;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private List<Particle> _particles = new();

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
        ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2);
        ballSpeed = new Vector2();
        character = new Character()
        {
            Position = new Vector2(ScreenWidth / 2, ScreenHeight / 2),
            Speed = new Vector2(300f, 0f),
        };
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        handTexture = Content.Load<Texture2D>("hand");
        character.Texture = handTexture;
        ballTexture = handTexture;//Content.Load<Texture2D>("ball");
        

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

    }

    double particleTimer = 0;
    Random random = new Random();

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        //var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                
        particleTimer += gameTime.ElapsedGameTime.TotalSeconds;

        var mousestate = Mouse.GetState(Window);
        var dist = mousestate.Position.ToVector2() - character.Position;
        if (dist.Length() > 0) dist.Normalize();
        character.Rotation = (float)Math.Atan2(dist.Y, dist.X);

        character.Update(gameTime);

        if (mousestate.LeftButton == ButtonState.Pressed && particleTimer > 0.05)
        {
            particleTimer = 0;

            var randAngle = character.Rotation + (float)random.NextDouble() * 0.3 - 0.15;
            var dirFluctuation = new Vector2((float)Math.Cos(randAngle), (float)Math.Sin(randAngle));
            var newDirection = dist + dirFluctuation;
            newDirection.Normalize();

            _particles.Add(new Particle(direction: newDirection * 18, rotation: character.Rotation) { 
                Texture = particleTexture, 
                Position = character.Position /*+ new Vector2(random.Next(20) - 10, random.Next(20) - 10) */});
        }

        foreach (var particle in _particles)
        {
            particle.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        character.Draw(_spriteBatch);

        foreach (var particle in _particles)
        {
            particle.Draw(_spriteBatch);
        }

        //_spriteBatch.Draw(ballTexture, ballPosition, null, Color.White, ballRotation + MathHelper.PiOver2,
        //    new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}