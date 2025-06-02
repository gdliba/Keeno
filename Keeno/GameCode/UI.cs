using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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

            _hoverSfx = Assets.ButtonHoverSFX;
            _pressedSfx = Assets.ButtonPressSFX;

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
                _hoverSfx.Play();
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
            _pressedSfx.Play();

            OnClick.Invoke();
            _state = ButtonState.Pressed;
            _flashingFontTimer = _flashingFontTimerReset;
        }

        public void PressedLogic(Point mousepos)
        {
            //if (!_pressedSoundHasPlayed)
            //{
            //    _pressedSfx.Play();
            //    _pressedSoundHasPlayed = true;
            //}
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
    class TypewriterText
    {
        private Vector2 _position;

        public string RawText { get; private set; }
        private string _visibleText;

        private List<(string text, Color color)> _segments = new();

        private int _charIndex;

        private float _charDelay;
        private float _timer;


        private bool _isActive;
        public bool IsActive { get { return _isActive; } }

        public TypewriterText(Vector2 position, string text)
        {
            _timer = 0f;
            _charIndex = 0;
            _isActive = false;
            _position = position;
            _charDelay = .06f;
            _visibleText = "";
            RawText = text;
            ParseTextWithColours(text);
        }

        public void SetActive()
        {
            _timer = 0f;
            _charIndex = 0;
            _visibleText = "";
            _isActive = true;
        }

        public void Reset()
        {
            _timer = 0f;
            _charIndex = 0;
            _visibleText = "";
            _isActive = false;
        }

        public void Update( )
        {
            if (!_isActive) return;

            _timer += Globals.DeltaTime;
            while (_charIndex < GetTotalCharCount() && _timer >= _charDelay)
            {
                _timer -= _charDelay;
                _charIndex++;
                _visibleText = GetVisibleText(_charIndex);



                SoundEffectInstance instance = Assets.TypingSFX.CreateInstance();

                // Randomize pitch
                instance.Pitch = (float)(Globals.RNG.NextDouble() * .5f);

                // Randomize volume
                instance.Volume = (float)(Globals.RNG.NextDouble() * .2f);

                instance.Play();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            if (!_isActive || string.IsNullOrEmpty(_visibleText)) return;

            SpriteFont font = Assets.MonogramFont;
            Vector2 drawPos = _position;
            int charsDrawn = 0;

            foreach (var (text, color) in _segments)
            {
                for (int i = 0; i < text.Length && charsDrawn < _charIndex; i++)
                {
                    string c = text[i].ToString();
                    spriteBatch.DrawString(font, c, drawPos, color);
                    drawPos.X += font.MeasureString(c).X;
                    charsDrawn++;
                }

                if (charsDrawn >= _charIndex) break;
            }
        }

        private void ParseTextWithColours(string input)
        {
            _segments.Clear();

            Color currentColour = Color.White;
            string current = "";
            bool inTag = false;
            string tag = "";

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '<')
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        _segments.Add((current, currentColour));
                        current = "";
                    }
                    inTag = true;
                    tag = "";
                }
                else if (input[i] == '>' && inTag)
                {
                    inTag = false;
                    if (tag.StartsWith("/"))
                        currentColour = Color.White;
                    else
                        currentColour = ParseColour(tag);
                }
                else if (inTag)
                {
                    tag += input[i];
                }
                else
                {
                    current += input[i];
                }
            }

            if (!string.IsNullOrEmpty(current))
                _segments.Add((current, currentColour));
        }

        private int GetTotalCharCount() => _segments.Sum(s => s.text.Length);

        private string GetVisibleText(int count)
        {
            string result = "";
            int chars = 0;
            foreach (var segs in _segments)
            {
                if (chars + segs.text.Length <= count)
                {
                    result += segs.text;
                    chars += segs.text.Length;
                }
                else
                {
                    result += segs.text.Substring(0, count - chars);
                    break;
                }
            }
            return result;
        }

        private Color ParseColour(string tag)
        {
            return tag.ToLower() switch
            {
                "r" => Color.Red,
                "g" => Color.Green,
                "b" => Color.Blue,
                "y" => Color.Yellow,
                "w" => Color.White,
                _ => Color.White
            };
        }
    }
}
