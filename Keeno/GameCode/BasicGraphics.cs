using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static System.Net.Mime.MediaTypeNames;

namespace Keeno
{
    public enum Direction
    {
        North,
        West,
        East,
        South
    }
    class StaticGraphic
    {
        protected Rectangle _rect;
        protected Texture2D _txr;
        protected Rectangle? _staticSrcRect;

        public StaticGraphic(Rectangle rectPosition, Texture2D txrImage)
        {
            _rect = rectPosition;
            _txr = txrImage;
            _staticSrcRect = null;
        }

        public StaticGraphic(Texture2D txrImage, int xPos, int yPos, int width, int height)
            : this(new Rectangle(xPos, yPos, width, height), txrImage)
        {
        }

        /// <summary>
        /// This method takes in a SpriteSheet so that I can use it rather than exporting
        /// individual graphics from the spritesheet externally
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="sourceRect"></param>
        /// <param name="destinationRect"></param>
        public StaticGraphic(Texture2D spriteSheet, int xTile, int yTile, int tileWidth, int tileHeight, Rectangle destinationRect)
        {
            _txr = spriteSheet;
            _staticSrcRect = new Rectangle(xTile * tileWidth, yTile * tileHeight, tileWidth, tileHeight);
            _rect = destinationRect;
        }

        public virtual void drawme(SpriteBatch sBatch)
        {
            sBatch.Draw(_txr, _rect, _staticSrcRect, Color.White);
        }
    }

    class MotionGraphic : StaticGraphic
    {
        protected Vector2 _position;
        protected Vector2 _velocity;

        public MotionGraphic(Rectangle rect, Texture2D txr)
            : base(rect, txr)
        {
            _position = new Vector2(rect.X, rect.Y);
            _velocity = Vector2.Zero;
        }

        public override void drawme(SpriteBatch sBatch)
        {
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;

            sBatch.Draw(_txr, _rect, Color.White);
        }
    }

    class Animated2D : MotionGraphic
    {
        protected Rectangle _srcRect;
        protected float _updateTrigger;
        protected int _framesPerSecond;

        public Animated2D(Texture2D spriteSheet, int fps, Rectangle rect)
            : base(rect, spriteSheet)
        {
            _srcRect = new Rectangle(0, 0, rect.Width, rect.Height);
            _updateTrigger = 0;
            _framesPerSecond = fps;

            _position = new Vector2(rect.X, rect.Y);
            _velocity = Vector2.Zero;
        }

        public virtual void updateme(GameTime gt)
        {
            _updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * _framesPerSecond;

            if (_updateTrigger >= 1)
            {
                _updateTrigger = 0;
                _srcRect.X += _srcRect.Width;
                if (_srcRect.X == _txr.Width)
                    _srcRect.X = 0;
            }

            _position = _position + _velocity;
        }

        public override void drawme(SpriteBatch sBatch)
        {
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;

            sBatch.Draw(_txr, _rect, _srcRect, Color.White);
        }
    }
    /// <summary>
    /// Class made to help with making both the Keeno and the player inherit
    /// their walk animation and relevant logic shared without making the Keeno
    /// an odd child of the player, by not also making them a MobileSpawnPoint
    /// </summary>
    class AnimatedKeeno2D : Animated2D
    {
        protected Direction _direction;
        protected bool _isWalking;
        protected int _defaultFps;
        protected int _idleFPS;

        public AnimatedKeeno2D(Texture2D spriteSheet, int fps, Rectangle rect)
            : base(spriteSheet, fps, rect)
        {
            _direction = Direction.West;
            _srcRect = new Rectangle(32, 0, rect.Width, rect.Height);
            _defaultFps = fps;
            _idleFPS = 2;

            _isWalking = false;
        }
        public override void updateme(GameTime gt)
        {
            _updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * _framesPerSecond;
            /// Frame 1 is death frame

            // if iswalking (direction will tell drawme to flip)
            if (_isWalking)
            {
                // Set FPS to what is stated in the constructor
                _framesPerSecond = _defaultFps;
                // cycle between frames 3 and 4
                if (_updateTrigger >= 1)
                {
                    _updateTrigger = 0;
                    _srcRect.X += _srcRect.Width;
                    if (_srcRect.X == _txr.Width)
                        _srcRect.X = _srcRect.Width*2;
                }
            }
            // else 
            else
            {
                // change the fps to slow down the idle animation
                _framesPerSecond = _idleFPS;
                // cycle between frames 2 and 3 (idle animation)
                if (_updateTrigger >= 1)
                {
                    _updateTrigger = 0;
                    _srcRect.X += _srcRect.Width;
                    if (_srcRect.X == _txr.Width - _srcRect.Width) 
                        _srcRect.X = _srcRect.Width;
                }
            }

            _position += _velocity;
        }
        public override void drawme(SpriteBatch sBatch)
        {
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;

            if (_direction == Direction.East)
                sBatch.Draw(_txr, _rect, _srcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
            else
                sBatch.Draw(_txr, _rect, _srcRect, Color.White);




        }

    }
}
