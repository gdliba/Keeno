
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
            Rectangle startButtonRect = new Rectangle(_screenWidth / 2 - _buttonWidth, _screenHeight / 2 - _buttonHeight, _buttonWidth, _buttonHeight);
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
            Rectangle continueButtonRect = new Rectangle(_screenWidth/2- _buttonWidth, _screenHeight/2- _buttonHeight, _buttonWidth, _buttonHeight);
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
