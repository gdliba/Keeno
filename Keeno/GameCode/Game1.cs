using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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
        // RNG
        public static readonly Random RNG = new Random();


        #region VARIABLES

        // Game State
        private GameState currentGameState;

        // Debug Pixel
        private Texture2D debugPixel;

        // TESTS
        private StaticSwarmPoint testSwarmPoint;
        private MobileSwarmPoint testMobileSwarmPoint;

        // Keyboard
        KeyboardState kb_curr;

        // Player
        Player testPlayer;

        // Keeno
        List<Keeno> keenos;
        Texture2D keenoTexture;

        // Fonts
        SpriteFont debugFont;

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
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();


            currentGameState = GameState.Start;

            #region List Initialisations
            keenos = new List<Keeno>();
            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            debugPixel = Content.Load<Texture2D>("Pixel");

            //testSwarmPoint = new StaticSwarmPoint(Content.Load<Texture2D>("SpriteSheets\\color_t")
            //    , 2, 19, 16, 16, new Rectangle(100, 100, 16, 16), debugPixel);

            //testMobileSwarmPoint = new MobileSwarmPoint(Content.Load<Texture2D>("Characters\\Keeno"), 3, new Rectangle(200, 200, 16, 16), debugPixel);

            testPlayer = new Player(Content.Load<Texture2D>("Characters\\Keeno"), 3, new Rectangle(200, 200, 16, 16), debugPixel);

            // Keeno
            keenoTexture = Content.Load<Texture2D>("Characters\\Keeno");

            // Fonts
#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts\\debugFont");
#endif        
        }

        protected override void Update(GameTime gt)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            kb_curr = Keyboard.GetState();

            // GameState Switch
            switch (currentGameState)
            {
                case GameState.Start:
                    StartUpdate(gt, kb_curr);
                    break;
                case GameState.Playing:
                    PlayingUpdate();
                    break;
                case GameState.GameOver:
                    GameOverUpdate();
                    break;
            }
            base.Update(gt);
        }

        protected override void Draw(GameTime gt)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            switch (currentGameState)
            {
                case GameState.Start:
                    StartDraw(gt);
                    break;
                case GameState.Playing:
                    PlayingDraw();
                    break;
                case GameState.GameOver:
                    GameOverDraw();
                    break;
            }

            _spriteBatch.End();


            base.Draw(gt);
        }
        #region STATE UPDATES
        private void StartUpdate(GameTime gt, KeyboardState kb)
        {
            //testMobileSwarmPoint.updateme(gt);
            testPlayer.updateme(gt,kb);

            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                int x = RNG.Next(100, 501);
                int y = RNG.Next(100, 501);

                var newKeeno = new Keeno(keenoTexture,3,new Rectangle(x,y,16,16),debugPixel);
                keenos.Add(newKeeno);
            }




            // Keeno
            foreach (var keeno in keenos)
            {
                keeno.updateme(gt,kb);
            }

        }

        private void PlayingUpdate()
        {

        }

        private void GameOverUpdate()
        {

        }
        #endregion
        #region STATE DRAWS
        private void StartDraw(GameTime gt)
        {
            //testSwarmPoint.drawme(_spriteBatch);
            //testMobileSwarmPoint.drawme(_spriteBatch);
            testPlayer.drawme(_spriteBatch);

            // Keenos
            foreach (var keeno in keenos)
            {
                keeno.drawme(_spriteBatch);
            }
#if DEBUG
            _spriteBatch.DrawString(debugFont, _graphics.PreferredBackBufferWidth + "x " + _graphics.PreferredBackBufferHeight
                + "\nKeenos: " + keenos.Count,
                new Vector2(10, 10), Color.White);
#endif
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
