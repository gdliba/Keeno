using Keeno.GameCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Keeno
{
    public enum GameState
    {
        Start,
        Playing,
        EndOfDay,
        Pause,
        GameOver
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        #region VARIABLES

        // Game State
        private GameState currentGameState;


        // Managers
        TimeManager timeManager;
        UIManager uiManager;
        GameManager gameManager;
        MusicPlayer musicPlayer;
        TextManager textManager;

        // Track DayTime
        float brightness;

        //Map
        Map map;

        //Camera
        Camera camera;

        // Player
        Player player;

        // Keeno
        int keenosThatStarved;
        List<Keeno> keenos;
        List<Keeno> startScreenkeenos, endOfDayScreenKeenos;

        // Game Completed check
        bool gameCompleted;
        #endregion

        public Game1()
        {
            Globals.Graphics = _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "Keeno";
            IsMouseVisible = true;

            // Decided to move the resolution into Globals.
            // In this case I mainly did that so that if I decided to change
            // the resolution of the game for some reason, all the 
            // variables that use the screen width/height would dynamically change too.
            Globals.ChangeResolution(1920, 1080);
        }

        protected override void Initialize()
        {
            currentGameState = GameState.Start;
            camera.Position = Vector2.Zero;
            camera.Zoom = 5;

            #region List Initialisations
            keenos = new List<Keeno>();
            startScreenkeenos = new List<Keeno>();
            endOfDayScreenKeenos = new List<Keeno>();
            keenosThatStarved = 0;
            #endregion

            brightness = 1;
            gameCompleted = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Assets.Load(this.Content);

            textManager = new TextManager(TextState.InGame);

            map = new Map("Content/MapData/MainMap1.csv");
            map.BellRung += () =>
            {
                foreach (var keeno in keenos)
                {
                    keeno.PlayerRangBell();
                }
            };
            map.TownCentreSpawnedKeeno += keeno =>
            {
                keenos.Add(keeno);
                if (keenos.Count == 1)
                    textManager.SetActive("First Keeno");
                if (keenos.Count == 2)
                    textManager.SetActive("Housing");
                if (keenos.Count == 3)
                    textManager.SetActive("Population");
                if (keenos.Count == 4)
                    textManager.SetActive("Hunger2");
                if (keenos.Count == 5)
                    textManager.SetActive("BuildersCabin");
                if (keenos.Count == 6)
                    textManager.SetActive("10 Keeno Challenge");
            };

            player = new Player(Content.Load<Texture2D>("Characters\\Keeno"), 5, new Rectangle(400, 300, 16, 16),
                map, keenos);
            #region Player Events
            // Subscribe to all of the player's Events.
            // They are mostly in charge of showing tutorial Text.
            player.FirstInteraction += () =>
            {
                textManager.SetActive("Resource Interact");
            };
            player.FirstFollower += () =>
            {
                textManager.SetActive("First Follower");
            };
            player.FirstBluePrint += () =>
            {
                textManager.SetActive("Blueprint1");
            };
            player.FirstKeeno += () =>
            {
                textManager.SetActive("Buy Keeno");
            };
            player.FirstStone += () =>
            {
                textManager.SetActive("Houses1");
            };
            #endregion

            timeManager = new TimeManager();
            uiManager = new UIManager();
            uiManager.Load();
            musicPlayer = new MusicPlayer();

            gameManager = new GameManager(map, timeManager, keenos, this.player, textManager);
            #region GameManager Events
            // Subscribe to all of the GameManager Events.
            gameManager.TenKeenoMilestone += () =>
            {
                textManager.SetActive("10 Keeno Milestone");
            };
            gameManager.TwentyFiveKeenoMilestone += () =>
            {
                textManager.SetActive("25 Keeno Milestone");
            };
            gameManager.OneHundredKeenoMilestone += () =>
            {
                textManager.SetActive("100 Keeno Milestone");
            };
            gameManager.OneHundredKeenoMilestoneReset += () =>
            {
                textManager.SetActive("100 Keeno Milestone Reset");
            };
            #endregion

            #region Button Presses
            // Subscribe to all of the Button Press Events.
            uiManager.OnPlayPressed = () =>
            {
                DoPlayButtonPressed();
            };
            uiManager.OnContinuePressed = () =>
            {
                currentGameState = GameState.Playing;
                musicPlayer.ResumeMusic();
            };
            uiManager.OnRestartPressed = () =>
            {
                gameCompleted = false;
                textManager.SwitchToInGame();
                musicPlayer.PlayFirstRain();
                gameManager.ResetAll();
                currentGameState = GameState.Playing;
            };
            uiManager.OnMainMenuPressed = () =>
            {
                currentGameState = GameState.Start;
                startScreenkeenos.Clear();
                endOfDayScreenKeenos.Clear();

            };
            uiManager.OnExitPressed = () =>
            {
                Exit();
            };
            uiManager.OnNextDayPressed = () =>
            {
                DoNextDay();
            };
            #endregion

        }
        /// <summary>
        /// Method Called when the Play Button is Pressed.
        /// </summary>
        private void DoPlayButtonPressed()
        {
            if (gameCompleted)
            {
                gameCompleted = false;
                gameManager.ResetAll();
            }
            musicPlayer.PauseMusic();
            currentGameState = GameState.Playing;
            musicPlayer.PlayFirstRain();
            textManager.Start();
        }

        /// <summary>
        /// Method Called when a new day starts.
        /// </summary>
        private void DoNextDay()
        {
            textManager.SwitchToInGame();
            currentGameState = GameState.Playing;
            endOfDayScreenKeenos.Clear();
        }

        /// <summary>
        /// Main Update Method. In charge of updating everything.
        /// </summary>
        protected override void Update(GameTime gt)
        {
            // Removes dead Keenos, regardless of currentGameState
            for (int i = keenos.Count - 1; i >= 0; i--)
            {
                if (keenos[i].State == KeenoState.Dead)
                    keenos.RemoveAt(i);
            }

            Globals.Update(gt);

            // GameState Switch
            switch (currentGameState)
            {
                case GameState.Start:
                    StartUpdate(gt);
                    break;
                case GameState.Playing:
                    PlayingUpdate(gt);
                    break;
                case GameState.EndOfDay:
                    EndOfDayUpdate(gt);
                    break;
                case GameState.Pause:
                    PauseUpdate();
                    break;
                case GameState.GameOver:
                    GameOverUpdate(gt);
                    break;
            }
            base.Update(gt);
        }

        /// <summary>
        /// Main Draw Method. In charge of Drawing Everything.
        /// </summary>
        /// <param name="gt"></param>
        protected override void Draw(GameTime gt)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (currentGameState)
            {
                case GameState.Start:
                    StartDraw(gt);
                    break;
                case GameState.Playing:
                    PlayingDraw();
                    break;
                case GameState.EndOfDay:
                    EndOfDayDraw(gt);
                    break;
                case GameState.Pause:
                    PauseDraw();
                    break;
                case GameState.GameOver:
                    GameOverDraw();
                    break;
            }

            base.Draw(gt);
        }

        #region STATE UPDATES
        // State Specific Draw Methods.

        /// <summary>
        /// Start Screen Update Method.
        /// </summary>
        private void StartUpdate(GameTime gt)
        {
            uiManager.StartUpdate();
            musicPlayer.PlayMainTheme();

            if (Globals.Enter_KeyPress || Globals.E_KeyPress)
            {
                DoPlayButtonPressed();
            }

            // Random Keeno spawn
            foreach (var keeno in startScreenkeenos)
            {
                keeno.Update(gt);
            }
            for (int i = 0; i <= 500;)
            {
                if(startScreenkeenos.Count>=500)
                    break;
                int x = Globals.RNG.Next(0, Globals.ScreenWidth);
                int y = Globals.RNG.Next(0, Globals.ScreenHeight);

                var newKeeno = new Keeno(Assets.KeenoTxr, 5, new Rectangle(x, y, 16, 16), null, true);
                startScreenkeenos.Add(newKeeno);
                break;
            }
        }

        /// <summary>
        /// EndOfDay Screen Update Method.
        /// </summary>
        private void EndOfDayUpdate(GameTime gt)
        {
            textManager.Update();
            uiManager.EndOfDayUpdate();

            if (Globals.Enter_KeyPress || Globals.E_KeyPress)
            {
                DoNextDay();
            }

            // Random Keeno spawn (1 Keeno per alive Keeno in Game)
            foreach (var keeno in endOfDayScreenKeenos)
            {
                keeno.Update(gt);
            }
            for (int i = 0; i <= keenos.Count;)
            {
                if (endOfDayScreenKeenos.Count == keenos.Count)
                    break;
                int x = Globals.RNG.Next(Globals.ScreenWidth/4, 3*Globals.ScreenWidth/4);
                int y = Globals.RNG.Next(Globals.ScreenHeight / 4, 3 * Globals.ScreenHeight / 4);

                var newKeeno = new Keeno(Assets.KeenoTxr, 5, new Rectangle(x, y, 16, 16), null, true);
                endOfDayScreenKeenos.Add(newKeeno);
                break;
            }
        }

        /// <summary>
        /// Method Called when the day ends.
        /// Works out if any Keeno starve.
        /// Checks if the game's win condition is fulfilled.
        /// </summary>
        public void GoToNextDay()
        {
            endOfDayScreenKeenos.Clear();
            gameManager.NextDay();

            // Each Keeno should Eat
            foreach (var keeno in keenos)
            {
                ResourceTracker.Spend(ResourceType.Food, 1);
                keeno.StopConstructingSound();
                keeno.StopWorkSound();
            }
            // If you don't have enough food for every Keeno
            if (ResourceTracker.GetAmount(ResourceType.Food) < 0)
            {
                int starvingKeeno = ResourceTracker.GetAmount(ResourceType.Food);
                keenosThatStarved = Math.Abs(starvingKeeno);

                if (keenos.Count > 0)
                    for (int i = 0; i < Math.Abs(starvingKeeno); i++)
                    {
                        ResourceTracker.Spend(ResourceType.Keeno, 1);
                        keenos.RemoveAt(0);
                    }
                // Reset Food to 0 as it doesn't make sense to have negative resources
                ResourceTracker.Add(ResourceType.Food, Math.Abs(starvingKeeno));
            }
            // Win condition
            if (keenos.Count >= 100 && keenosThatStarved == 0)
            {
                currentGameState = GameState.GameOver;
                textManager.SwitchToGameOver();
                return;
            }
            textManager.SwitchToEndOfDay(keenosThatStarved);
            currentGameState = GameState.EndOfDay;
            // Reset this variable to 0
            keenosThatStarved = 0;
        }

        /// <summary>
        /// Playing Screen Update Method.
        /// </summary>
        private void PlayingUpdate(GameTime gt)
        {
            #region Updates
            gameManager.TrackMilestones();
            textManager.Update();
            timeManager.UpdateTime((float)gt.ElapsedGameTime.TotalSeconds);
            map.Update(gt);
            // Keeno
            foreach (var keeno in keenos)
            {
                keeno.Update(gt);
            }
            player.Update(gt);
            #endregion
            #region EndOfDay / Hunger
            // When the day ends
            if (brightness <= .1)
            {
                GoToNextDay();
            }
            #endregion

            // Hides/unhides the Button Prompts and Building Names.
            if (Globals.I_KeyPress)
                Globals.HidePromtsAndNames = !Globals.HidePromtsAndNames;

            // Make sure the Camera Follows the player.
            //camera.Position.X = (-player.Bounds.X + _graphics.PreferredBackBufferWidth / (2 * camera.Zoom));
            //camera.Position.Y = (-player.Bounds.Y + _graphics.PreferredBackBufferHeight / (2 * camera.Zoom));

            camera.Position = new Vector2(
                    -player.RenderPosition.X +
                    _graphics.PreferredBackBufferWidth / (2f * camera.Zoom),

                    -player.RenderPosition.Y +
                    _graphics.PreferredBackBufferHeight / (2f * camera.Zoom)
);
#if DEBUG
            //switch (debugResource)
            //{
            //    case ResourceType.None:
            //        if (Globals.Tab_KeyPress)
            //            debugResource = ResourceType.Food;
            //        break;
            //    case ResourceType.Food:
            //        if (Globals.Tab_KeyPress)
            //            debugResource = ResourceType.Wood;
            //        break;
            //    case ResourceType.Wood:
            //        if (Globals.Tab_KeyPress)
            //            debugResource = ResourceType.Gold;
            //        break;
            //    case ResourceType.Gold:
            //        if (Globals.Tab_KeyPress)
            //            debugResource = ResourceType.Stone;
            //        break;
            //    case ResourceType.Stone:
            //        if (Globals.Tab_KeyPress)
            //            debugResource = ResourceType.Food;
            //        break;
            //}
            ////if (Globals.X_KeyPress)
            ////    ResourceTracker.Reset();
            //if (Globals.UpArrow_KeyPress)
            //    ResourceTracker.Add(debugResource, 10);
            //if (Globals.DownArrow_KeyPress)
            //    ResourceTracker.Add(debugResource, -10);


            //if (Globals.I_KeyPress)
            //{
            //    var newKeeno = new Keeno(Assets.KeenoTxr, 5, new Rectangle(100, 100, 16, 16), Assets.DebugPixelTxr, null, true);
            //    keenos.Add(newKeeno);
            //    ResourceTracker.Add(ResourceType.Keeno, 1);
            //}
#endif
            // Pause
            if (Globals.Escape_KeyPress)
            {
                musicPlayer.PauseMusic();
                currentGameState = GameState.Pause;
                Assets.PauseSFX.Play();

                foreach (var keeno in keenos)
                {
                    keeno.StopConstructingSound();
                    keeno.StopWorkSound();

                }
            }

            // CHEATS
            if (Globals.LeftShift_KeyDown && Globals.LeftControl_KeyDown && Globals.Enter_KeyPress)
                GoToNextDay();
            if (Globals.LeftShift_KeyDown && Globals.F_KeyDown && Globals.UpArrow_KeyPress)
                ResourceTracker.Add(ResourceType.Food, 10);
            if (Globals.LeftShift_KeyDown && Globals.W_KeyDown && Globals.UpArrow_KeyPress)
                ResourceTracker.Add(ResourceType.Wood, 10);
            if (Globals.LeftShift_KeyDown && Globals.S_KeyDown && Globals.UpArrow_KeyPress)
                ResourceTracker.Add(ResourceType.Stone, 10);
            if (Globals.LeftShift_KeyDown && Globals.G_KeyDown && Globals.UpArrow_KeyPress)
                ResourceTracker.Add(ResourceType.Gold, 10);
        }

        /// <summary>
        /// Pause Screen Update Method.
        /// </summary>
        private void PauseUpdate()
        {
            uiManager.PauseUpdate();
            // UnPause
            if (Globals.Escape_KeyPress)
            {
                musicPlayer.ResumeMusic();
                currentGameState = GameState.Playing;
                Assets.ButtonPressSFX.Play();
            }
        }

        /// <summary>
        /// GameOver Screen Update Method.
        /// </summary>
        private void GameOverUpdate(GameTime gt)
        {
            gameCompleted = true;
            textManager.Update();
            uiManager.GameOverUpdate();

            // Random Keeno spawn (1 Keeno per alive Keeno in Game)
            foreach (var keeno in endOfDayScreenKeenos)
            {
                keeno.Update(gt);
            }
            for (int i = 0; i <= keenos.Count;)
            {
                if (endOfDayScreenKeenos.Count == keenos.Count)
                    break;
                int x = Globals.RNG.Next(Globals.ScreenWidth / 4, 3 * Globals.ScreenWidth / 4);
                int y = Globals.RNG.Next(Globals.ScreenHeight / 4, 3 * Globals.ScreenHeight / 4);

                var newKeeno = new Keeno(Assets.KeenoTxr, 5, new Rectangle(x, y, 16, 16), null, true);
                endOfDayScreenKeenos.Add(newKeeno);
                break;
            }
        }
        #endregion

        #region STATE DRAWS

        /// <summary>
        /// Start Screen Draw Method.
        /// </summary>
        private void StartDraw(GameTime gt)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            foreach (var keeno in startScreenkeenos)
            {
                keeno.Draw(_spriteBatch);
            }
            uiManager.StartDraw(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// EndOfDay Screen Draw Method.
        /// </summary>
        private void EndOfDayDraw(GameTime gt)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            foreach (var keeno in endOfDayScreenKeenos)
            {
                keeno.Draw(_spriteBatch);
            }
            uiManager.EndOfDaytDraw(_spriteBatch);
            _spriteBatch.End();
            _spriteBatch.Begin();
            textManager.Draw(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// Playing Screen Draw Method.
        /// Camera is in charge of what is visible on screen.
        /// UI and HUD are drawn over the Camera so they aren't affected by the Night Time Overlay
        /// </summary>
        private void PlayingDraw()
        {
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.getCam());
            //MAP
            map.Draw(_spriteBatch);
            // Keenos
            foreach (var keeno in keenos)
            {
                keeno.Draw(_spriteBatch);
            }
            player.Draw(_spriteBatch);
            #region Night Time Overlay
            // Night Time Overlay
            float t = timeManager.TimeOfDay / timeManager.DayLengthSeconds;
            brightness = 0.5f + 0.5f * (float)Math.Cos(Math.PI * t);
            if (t < 0.75f)
                brightness = 1f;
            else
            {
                float ramp = (t - 0.75f) / 0.25f;
                ramp = MathHelper.Clamp(ramp, 0f, 1f);

                // Fade linearly
                brightness = 1f - ramp;
            }
            int screenwidth = _graphics.PreferredBackBufferWidth;
            int screenHeight = _graphics.PreferredBackBufferHeight;

            Rectangle worldRect = new Rectangle(-screenwidth / 2, -screenHeight / 2, 2 * screenwidth, 2 * screenHeight);
            Color tintColor = Color.Lerp(Color.Black * .75f, Color.Transparent, brightness);

            _spriteBatch.Draw(Assets.DebugPixelTxr, worldRect,
                null, tintColor, 0f, Vector2.Zero, SpriteEffects.None, .099f);
            #endregion
            _spriteBatch.End();

            // Draw the Hud and UI
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            uiManager.PlayingDraw(_spriteBatch, keenos);
            textManager.Draw(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// Pause Screen Draw Method.
        /// </summary>
        private void PauseDraw()
        {
            // Draw everything in the playing screen so that the
            // player is not looking at a blank screen.
            PlayingDraw();
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            uiManager.PauseDraw(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// GameOver Draw Method.
        /// </summary>
        private void GameOverDraw()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            foreach (var keeno in endOfDayScreenKeenos)
            {
                keeno.Draw(_spriteBatch);
            }
            uiManager.GameOverDraw(_spriteBatch);
            _spriteBatch.End();
            _spriteBatch.Begin();
            textManager.Draw(_spriteBatch);
            _spriteBatch.End();
        }
        #endregion
    }
}
