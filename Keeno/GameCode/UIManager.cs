
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Keeno
{
    class UIManager
    {
        private Dictionary<string, Button> buttons;
        private Panel _startPanel, _pausePanel;
        public Action OnStartPressed, OnContinuePressed, OnRestartPressed, OnMainMenuPressed, OnExitPressed;

        private static int _screenWidth;
        private static int _screenHeight;
        private static int _buttonWidth;
        private static int _buttonHeight;

        public UIManager()
        {
           buttons = new Dictionary<string, Button>();
            _screenWidth = Globals.ScreenWidth;
            _screenHeight = Globals.ScreenHeight;
            _buttonWidth = 200;
            _buttonHeight = 40;
        }

        public void Load()
        {

            // Start Button
            Rectangle startButtonRect = new Rectangle(_screenWidth / 2 - _buttonWidth/2, _screenHeight / 2 - _buttonHeight, _buttonWidth, _buttonHeight);
            var startButton = new Button(startButtonRect, Globals.StartScreenButtons[0]);
            startButton.OnClick += () => OnStartPressed?.Invoke();
            buttons.Add(Globals.StartScreenButtons[0], startButton);

            // Exit Button
            Rectangle exitButtonRect = new Rectangle(startButtonRect.X, startButtonRect.Y + 2 * _buttonHeight, _buttonWidth, _buttonHeight);
            var exitButton = new Button(exitButtonRect, Globals.StartScreenButtons[1]);
            exitButton.OnClick += () => OnExitPressed?.Invoke();
            buttons.Add(Globals.StartScreenButtons[1], exitButton);

            #region Buttons
            // Continue Button
            Rectangle continueButtonRect = new Rectangle(_screenWidth/2- _buttonWidth/2, _screenHeight/2- _buttonHeight, _buttonWidth, _buttonHeight);
            var continueButton = new Button(continueButtonRect, Globals.PauseScreenButtons[0]);
            continueButton.OnClick += () => OnContinuePressed?.Invoke();
            buttons.Add(Globals.PauseScreenButtons[0], continueButton);

            // Restart Button
            Rectangle restartButtonRect = new Rectangle(continueButtonRect.X, continueButtonRect.Y + 2*_buttonHeight, _buttonWidth, _buttonHeight);
            var restartButton = new Button(restartButtonRect, Globals.PauseScreenButtons[1]);
            restartButton.OnClick += () => OnRestartPressed?.Invoke();
            buttons.Add(Globals.PauseScreenButtons[1], restartButton);

            // Main Menu Button
            Rectangle mainMenuButtonRect = new Rectangle(restartButtonRect.X, restartButtonRect.Y + 2 * _buttonHeight, _buttonWidth, _buttonHeight);
            var mainMenuButton = new Button(mainMenuButtonRect, Globals.PauseScreenButtons[2]);
            mainMenuButton.OnClick += () => OnMainMenuPressed?.Invoke();
            buttons.Add(Globals.PauseScreenButtons[2], mainMenuButton);

            // Quit Button
            Rectangle quitButtonRect = new Rectangle(mainMenuButtonRect.X, mainMenuButtonRect.Y + 2 * _buttonHeight, _buttonWidth, _buttonHeight);
            var quitButton = new Button(quitButtonRect, Globals.PauseScreenButtons[3]);
            quitButton.OnClick += () => OnExitPressed?.Invoke();
            buttons.Add(Globals.PauseScreenButtons[3], quitButton);
            #endregion

            //Panel
            Rectangle pausePanelPosition = 
                new Rectangle(
                continueButtonRect.Left - 4 * _buttonWidth / 3,
                continueButtonRect.Y - 2 * _buttonHeight,
                3 * _buttonWidth + 2 * _buttonWidth / 3,
                11 * _buttonHeight);
            _pausePanel = new Panel(pausePanelPosition, Color.Black * .9f);

            Rectangle startPanelPosition = 
                new Rectangle(pausePanelPosition.X,pausePanelPosition.Y, pausePanelPosition.Width, 7*_buttonHeight);
            _startPanel = new Panel(startPanelPosition, Color.Black * .9f);

        }
        public void UpdateStart()
        {
            foreach (var button in Globals.StartScreenButtons)
            {
                buttons[button].Update();
            }
        }
        public void UpdatePause()
        {
            foreach (var button in Globals.PauseScreenButtons)
            {
                buttons[button].Update();
            }
        }
        public void DrawStart(SpriteBatch sb)
        {
            //_startPanel.Draw(sb);
            foreach (var button in Globals.StartScreenButtons)
            {
                buttons[button].Draw(sb);
            }
        }
        public void DrawPlaying(SpriteBatch sb, List<Keeno> keenos)
        {
            sb.Draw(Assets.ResourceHUD, new Rectangle(0, 0, Globals.ScreenWidth, Globals.ScreenHeight), Color.White);

            var offsetY = 40;


            // Housing
            Vector2 pos1 = new Vector2(66, Globals.ScreenHeight - 220);
            sb.DrawString(Assets.MonogramFont,
                keenos.Count + "/" + ResourceTracker.GetAmount(ResourceType.Housing),
                pos1,
                 Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, .1f);

            // Food
            Vector2 pos2 = new Vector2(pos1.X, pos1.Y+ offsetY);
            sb.DrawString(Assets.MonogramFont,
                ResourceTracker.GetAmount(ResourceType.Food).ToString(),
                pos2,
                 Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, .1f);

            // Wood
            Vector2 pos3 = new Vector2(pos2.X, pos2.Y + offsetY);
            sb.DrawString(Assets.MonogramFont,
                ResourceTracker.GetAmount(ResourceType.Wood).ToString(),
                pos3,
                 Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, .1f);

            // Stone
            Vector2 pos4 = new Vector2(pos3.X, pos3.Y + offsetY);
            sb.DrawString(Assets.MonogramFont,
                ResourceTracker.GetAmount(ResourceType.Stone).ToString(),
                pos4,
                 Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, .1f);

            // Gold
            Vector2 pos5 = new Vector2(pos4.X, pos4.Y + offsetY);
            sb.DrawString(Assets.MonogramFont,
                ResourceTracker.GetAmount(ResourceType.Gold).ToString(),
                pos5,
                 Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, .1f);
        }
        public void DrawPause(SpriteBatch sb)
        {
            _pausePanel.Draw(sb);
            foreach (var button in Globals.PauseScreenButtons)
            {
                buttons[button].Draw(sb);
            }
        }
    }
}
