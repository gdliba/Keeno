using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Keeno
{
    public enum GameState
    {
        Start,
        Playing,
        GameOver
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private StaticSwarmPoint testSwarmPoint;

        #region VARIABLES

        // Game State
        private GameState currentGameState;

        // Debug Pixel
        private Texture2D debugPixel;

        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "Keeno";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Match the resolution to the current display
            _graphics.PreferredBackBufferWidth = 
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = 
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            // Set screen to Fullscreen
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();


            currentGameState = GameState.Start;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            debugPixel = Content.Load<Texture2D>("Pixel");

            testSwarmPoint = new StaticSwarmPoint(Content.Load<Texture2D>("SpriteSheets\\mono")
                , 2, 19, 16, 16, new Rectangle(100, 100, 16, 16), debugPixel);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // GameState Switch
            switch (currentGameState)
            {
                case GameState.Start:
                    StartUpdate();
                    break;
                case GameState.Playing:
                    PlayingUpdate();
                    break;
                case GameState.GameOver:
                    GameOverUpdate();
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            switch (currentGameState)
            {
                case GameState.Start:
                    StartDraw();
                    break;
                case GameState.Playing:
                    PlayingDraw();
                    break;
                case GameState.GameOver:
                    GameOverDraw();
                    break;
            }

            _spriteBatch.End();


            base.Draw(gameTime);
        }
        #region STATE UPDATES
        private void StartUpdate()
        {

        }

        private void PlayingUpdate()
        {

        }

        private void GameOverUpdate()
        {

        }
        #endregion
        #region STATE DRAWS
        private void StartDraw()
        {
            testSwarmPoint.drawme(_spriteBatch);
        }

        private void PlayingDraw()
        {

        }

        private void GameOverDraw()
        {

        }
        #endregion
    }
}
