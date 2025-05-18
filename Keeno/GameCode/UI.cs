using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Keeno
{
    class Panel : StaticGraphic
    {
        private Color _tint;
        private Texture2D _borderTxr;
        public Panel(Rectangle position, Color Tint)
            : base(position, null)
        {
            _tint = Tint;
            _txr = Assets.UIPanelTxr;
            _borderTxr = Assets.UIPanelBorderTxr;
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(_txr, _rect, _tint);
            sb.Draw(_borderTxr, _rect, Color.White);
        }
    }

    enum ButtonState
    {
        Neutral,
        Hovered,
        Pressed
    }
    class Button : StaticGraphic
    {
        public event Action OnClick; // Event that will notify when Button is clicked
        private SpriteFont _font;
        private ButtonState _state;
        private SoundEffect _hoverSfx, _pressedSfx;
        private bool _hoverSoundHasPlayed, _pressedSoundHasPlayed;
        private string _text;
        private ButtonHiglight _buttonHiglight;

        private Color _fontColour;
        private float _flashingFontTimer, _flashingFontTimerReset;

        public Button(Rectangle position, string text) 
            : base(position, null)
        {
            _rect = position;
            _state = ButtonState.Neutral;
            _font = Assets.MonogramFont;
            _text = text;
            _buttonHiglight = new ButtonHiglight(position);

            _flashingFontTimer = 0f;
            _flashingFontTimerReset = .1f;

            _fontColour = Color.Gray;
        }
        public void Update()
        {

            // apply the appropriate effect
            if (_flashingFontTimer > 0)
            {
                _flashingFontTimer -= Globals.DeltaTime;
                _fontColour = Color.White;
            }
            else
            {
                _fontColour = Color.Gray;
            }




            switch (_state)
            {
                case ButtonState.Neutral:
                    _hoverSoundHasPlayed = false;
                    NeutralLogic(Globals.MousePosition);
                    break;
                case ButtonState.Hovered:
                    _pressedSoundHasPlayed = false;
                    HoverLogic(Globals.MousePosition);
                    break;
                case ButtonState.Pressed:
                    PressedLogic(Globals.MousePosition);
                    break;
            }
        }
        public void NeutralLogic(Point mousepos)
        {
            if (_rect.Contains(mousepos))
                _state = ButtonState.Hovered;
        }
        public void HoverLogic(Point mousepos)
        {
            _flashingFontTimer = _flashingFontTimerReset;

            if (!_hoverSoundHasPlayed)
            {
                //_hoverSfx.Play();
                _hoverSoundHasPlayed = true;
            }

            if (!_rect.Contains(mousepos))
            {
                _state = ButtonState.Neutral;
                return;
            }

            else if (Globals.LeftClick)
                DoPressed();
            
        }
        public void DoPressed()
        {
            OnClick.Invoke();
            _state = ButtonState.Pressed;
            _flashingFontTimer = _flashingFontTimerReset;
        }

        public void PressedLogic(Point mousepos)
        {
            if (!_pressedSoundHasPlayed)
            {
                //_pressedSfx.Play();
                _pressedSoundHasPlayed = true;
            }
            if (!_rect.Contains(mousepos))
            {
                _state = ButtonState.Neutral;
                return;
            }
            if (!Globals.LeftClick)
            {
                _state = ButtonState.Hovered;
            }
        }
        /// <summary>
        /// The draw method makes slight tweaks to the colour/position of the button.
        /// It also uses the given Font and String given to it in the constructor
        /// to measure the String and centre it.
        /// The downside to this, currently, is that if a very long string were to
        /// be added to the button, then it would exceed the confines of the button,
        /// rather than word wrap.
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            // Set up string measurements to draw text on the Button
            Vector2 textlength = _font.MeasureString(_text);
            var halfButtonX = _rect.X + _rect.Width / 2;
            var halfButtonY = _rect.Y + _rect.Height / 2;

            switch (_state)
            {
                case ButtonState.Neutral:
                    sb.DrawString(_font, _text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2), _fontColour);
                    break;
                case ButtonState.Hovered:
                    sb.DrawString(_font, _text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2), _fontColour);
                    _buttonHiglight.Draw(sb);
                    break;
                case ButtonState.Pressed:
                    sb.DrawString(_font, _text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2), _fontColour);
                    _buttonHiglight.Draw(sb);
                    break;
            }
            //sb.Draw(Assets.DebugPixelTxr, _rect, Color.White*.5f);

        }
    }
    class ButtonHiglight : StaticGraphic
    {
        private Rectangle _buttonPosition;
        public ButtonHiglight(Rectangle buttonPosition)
            : base(buttonPosition, null)
        {
            _buttonPosition = buttonPosition;
            _txr = Assets.UIHighlightTxr;
            _rect = _txr.Bounds;
        }
        public override void Draw(SpriteBatch sb)
        {
            int bufferSpace = 5;
            Rectangle firstHighlighRect = new Rectangle(_buttonPosition.Left - _rect.Width - bufferSpace, _buttonPosition.Y + _buttonPosition.Height / 4, _rect.Width, _buttonPosition.Height / 2);
            Rectangle SecondHighlighRect = new Rectangle(_buttonPosition.Right + bufferSpace, _buttonPosition.Y + _buttonPosition.Height / 4, _rect.Width, _buttonPosition.Height / 2);


            sb.Draw(_txr, firstHighlighRect, null, Color.White, 0f, Vector2.Zero,SpriteEffects.None, Globals.UIHighlightLD);
            sb.Draw(_txr, SecondHighlighRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, Globals.UIHighlightLD);
        }
    }
}
