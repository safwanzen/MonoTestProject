using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MonoTestProject;

public class MainGame : Game
{
    public static int ScreenWidth, ScreenHeight = 0;
    public static float GravityAcceleration = 50f;

    Texture2D ballTexture;
    Vector2 ballPosition;
    float ballSpeed;

    Texture2D handTexture;

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
        ballSpeed = 1000f;
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
        character.Texture = Content.Load<Texture2D>("ball");
        ballTexture = Content.Load<Texture2D>("ball");
        handTexture = Content.Load<Texture2D>("hand");
    }

    double particleTimer = 0;

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        particleTimer += gameTime.ElapsedGameTime.TotalSeconds;

        var mousestate = Mouse.GetState();
        
        if (mousestate.LeftButton == ButtonState.Pressed && particleTimer > 0.01)
        {
            particleTimer = 0;
            _particles.Add(new Particle() { Texture = handTexture, Position = mousestate.Position.ToVector2() });
        }

        foreach (var particle in _particles)
        {
            particle.Update(gameTime);
        }

        //character.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        foreach (var particle in _particles)
        {
            particle.Draw(_spriteBatch);
        }
        //character.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}