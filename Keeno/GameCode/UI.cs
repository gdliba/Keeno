using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Keeno
{
    /// <summary>
    /// The pause menu background panel.
    /// </summary>
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
    /// <summary>
    /// All buttons are made through this class.
    /// </summary>
    class Button : StaticGraphic
    {
        // Event that will notify when Button is clicked
        public event Action OnClick; 

        private ButtonState _state;
        private SoundEffect _hoverSfx, _pressedSfx;
        private bool _hoverSoundHasPlayed, _pressedSoundHasPlayed;

        private string _text;
        private ButtonHiglight _buttonHiglight;

        private SpriteFont _font;
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
        /// <summary>
        /// Updates button based on state. Counts town the timer in charge
        /// of the text illumination when the mouse has hovered over the button.
        /// </summary>
        public void Update()
        {
            // apply the appropriate effect
            if (_flashingFontTimer > 0)
            {
                _flashingFontTimer -= Globals.DeltaTime;
                _fontColour = Color.White;
            }
            else
                _fontColour = Color.Gray;

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

        #region Basic Button Logic

        /// <summary>
        /// Method called when the button is not being interacted with.
        /// </summary>
        public void NeutralLogic(Point mousepos)
        {
            if (_rect.Contains(mousepos))
                _state = ButtonState.Hovered;
        }
        /// <summary>
        /// Method called when the mouse is hovering over the button.
        /// Also checks if the button has been pressed.
        /// If that happens, it calls the appropriate method.
        /// </summary>
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
        /// <summary>
        /// Method called when the button is pressed.
        /// Fires the OnClick event.
        /// </summary>
        public void DoPressed()
        {
            _pressedSfx.Play();
            OnClick.Invoke();
            _state = ButtonState.Pressed;
            _flashingFontTimer = _flashingFontTimerReset;
        }
        /// <summary>
        /// Method in charge of switching state back to neutral or hovered.
        /// </summary>
        /// <param name="mousepos"></param>
        public void PressedLogic(Point mousepos)
        {
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
        #endregion

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
        }
    }
    /// <summary>
    /// For stylistic purposes, I've chosen to have 2 highlights appear when a Button
    /// is being selected: one on the left and one on the right of the button.
    /// This class creates those higlights.
    /// The class is self sufficient and only needs the Button Rect.
    /// </summary>
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
        /// <summary>
        /// Draw one highlight on the left hand side of the Button and one on the right.
        /// </summary>
        public override void Draw(SpriteBatch sb)
        {
            int bufferSpace = 5;
            Rectangle firstHighlighRect = new Rectangle(_buttonPosition.Left - _rect.Width - bufferSpace, _buttonPosition.Y + _buttonPosition.Height / 4, _rect.Width, _buttonPosition.Height / 2);
            Rectangle SecondHighlighRect = new Rectangle(_buttonPosition.Right + bufferSpace, _buttonPosition.Y + _buttonPosition.Height / 4, _rect.Width, _buttonPosition.Height / 2);


            sb.Draw(_txr, firstHighlighRect, null, Color.White, 0f, Vector2.Zero,SpriteEffects.None, Globals.UIHighlightLD);
            sb.Draw(_txr, SecondHighlighRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, Globals.UIHighlightLD);
        }
    }
    /// <summary>
    /// This class displays a string on screen one character at a time, given a delay.
    /// It recognises when Tags (ex. <y>YELLOW TEXT</y>)are added to change the colour of the string.
    /// It also knows to ignore tags as strings and thus only displaying the "Visible Text"
    /// on screen and only delaying said visible text characters, indipendantly of the tags.
    /// </summary>
    class TypewriterText
    {
        public string RawText { get; private set; }
        private string _visibleText;

        private Vector2 _position;

        private int _charIndex;
        private float _charDelay;
        private float _timer;

        // Stores pairs of strings with their associated colours
        private List<(string text, Color colour)> _segments = new();

        public bool IsActive { get { return _isActive; } }
        private bool _isActive;

        /// <summary>
        /// Constructor. Takes in a position and a "raw" string.
        /// Sets all the default values.
        /// </summary>
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
        /// <summary>
        /// Method that toggles "_isActive" bool and resets default values
        /// for tracking chars.
        /// </summary>
        public void SetActive()
        {
            _timer = 0f;
            _charIndex = 0;
            _visibleText = "";
            _isActive = true;
        }
        /// <summary>
        /// Method that toggles "_isActive" bool back to false 
        /// and resets default values for tracking chars.
        /// </summary>
        public void Reset()
        {
            _timer = 0f;
            _charIndex = 0;
            _visibleText = "";
            _isActive = false;
        }

        /// <summary>
        /// Most of the work is done when Processing the raw string.
        /// Update just counts down and displays a char when the condition is met.
        /// Plays the sound every time a char is to be displayed.
        /// </summary>
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

        /// <summary>
        /// Draws chars on screen at the 
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            // To avoid crashes.
            if (!_isActive || string.IsNullOrEmpty(_visibleText)) return;

            SpriteFont font = Assets.MonogramFont;
            Vector2 drawPos = _position;
            int charsDrawn = 0;

            // Loop through the segments of text (pairs of strings and colours)
            foreach (var (text, colour) in _segments)
            {
                // for each char in the segment of text
                for (int i = 0; i < text.Length && charsDrawn < _charIndex; i++)
                {
                    string character = text[i].ToString();
                    sb.DrawString(font, character, drawPos, colour);
                    // Move the position the next char is to be drawn by the width of a char.
                    drawPos.X += font.MeasureString(character).X;
                    charsDrawn++;
                }
                if (charsDrawn >= _charIndex) break;
            }
        }

        /// <summary>
        /// Method in charge of processing the text and assigning Colour
        /// given the tag in the raw text.
        /// </summary>
        private void ParseTextWithColours(string input)
        {
            _segments.Clear();

            Color currentColour = Color.White;
            string current = "";
            bool inTag = false;
            string tag = "";
            // Perform a linear search for the tags.
            for (int i = 0; i < input.Length; i++)
            {
                // if the tag start is found
                if (input[i] == '<')
                {
                    // Reset the "current" text in case it's not empty.
                    if (!string.IsNullOrEmpty(current))
                    {
                        _segments.Add((current, currentColour));
                        current = "";
                    }
                    // Let it be known that you're in a tag.
                    inTag = true;
                    // Start tracking what is inside the tag.
                    tag = "";
                }
                // if the tag is closed
                else if (input[i] == '>' && inTag)
                {
                    // Let it be known that the tag has been processed.
                    inTag = false;
                    // if this tag starts with a "/" it means it's the closing tag.
                    if (tag.StartsWith("/"))
                        currentColour = Color.White;
                    // if you're inside the tag,
                    // process what colour was named in the tag.
                    else
                        currentColour = ParseColour(tag);
                }
                // Track what the text inside the tag is.
                else if (inTag)
                {
                    tag += input[i];
                }
                // Track what the rest of the text is.
                else
                {
                    current += input[i];
                }
            }
            // To avoid crashes.
            if (!string.IsNullOrEmpty(current))
                // Add the text and colour pair to the list of segments.
                _segments.Add((current, currentColour));
        }

        /// <summary>
        /// Method used in Update() to track if all the chars in the segment have
        /// been displayed.
        /// Extracted the method to keep update cleaner.
        /// </summary>
        /// <returns>
        ///             The number of chars in the segement. </returns>
        private int GetTotalCharCount() => _segments.Sum(s => s.text.Length);

        /// <summary>
        /// Method that builds and returns the substring of visible chars up ro a specified count.
        /// Iterates through each segment (which has already been processed, thus does not contain tags)
        /// and adds characters to the string until the total is equal to the count.
        /// </summary>
        /// <param name="count">    Number of visible chars in the string.  </param>
        /// <returns>   
        ///                         The string of visible characters.       </returns>
        private string GetVisibleText(int count)
        {
            string result = "";
            int chars = 0;

            // Loop through each segment
            foreach (var seg in _segments)
            {
                // if adding the segment doesn't exceed the count
                if (chars + seg.text.Length <= count)
                {
                    // Add the segment to the resulting string.
                    result += seg.text;
                    // increase char total.
                    chars += seg.text.Length;
                }
                // else if adding the segment exceeds the count
                else
                {
                    // Calculate how many chars are needed from said segment.
                    int remainder = count - chars;
                    // Only add the first remainder chars of the segment
                    result += seg.text.Substring(0, remainder);
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// Method that processed the information inside 
        /// the tags and returns the corresponding colour.
        /// </summary>
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

            //switch (tag.ToLower())
            //{
            //    case "r":
            //        return Color.Red;
            //    case "g":
            //        return Color.Green;
            //    case "b":
            //        return Color.Blue;
            //    case "y":
            //        return Color.Yellow;
            //    case "w":
            //        return Color.White;
            //    default:
            //        return Color.White;
            //}
        }
    }
}
