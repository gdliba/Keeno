using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;

namespace Keeno
{
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

            _position += _velocity * (float)gt.ElapsedGameTime.TotalSeconds;
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
        protected bool _isWalking { get { return _velocity.Length() > 0; } }
        protected bool _wasWalking;
        protected int _defaultFps;
        protected int _idleFPS;
        protected float _moveSpeed;
        protected bool _facingRight;
        protected bool _drawBounds;
        public Rectangle _targetDestinationBounds;


        public Rectangle Bounds { get { return _rect; } }

        // test related
        protected Texture2D _testPixel;
        protected Rectangle _testRectangle;

        public AnimatedKeeno2D(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base(spriteSheet, fps, rect)
        {
            _idleFPS = 1;

            _srcRect = new Rectangle(32, 0, rect.Width, rect.Height);
            _defaultFps = fps;

            // test related
            _testPixel = pixel;
            _testRectangle = rect;
            _drawBounds = false;
            _targetDestinationBounds = _rect;
        }
        public override void updateme(GameTime gt)
        {
            _updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * _framesPerSecond;

            // Detect transitions between walking and idle
            if (_isWalking != _wasWalking)  // if they don't match up it means it's transitioning
            {
                _updateTrigger = 0;

                if (_isWalking)
                {
                    // Reset to start of walking frames
                    _srcRect.X = _srcRect.Width * 2;
                }
                else
                {
                    // Reset to start of idle frames
                    _srcRect.X = _srcRect.Width;
                }
            }

            if (_isWalking)
            {
                _framesPerSecond = _defaultFps; // switch FPS to the deault

                if (_updateTrigger >= 1)
                {
                    _updateTrigger = 0;
                    _srcRect.X += _srcRect.Width;

                    // Loop walking frames (3 and 4)
                    if (_srcRect.X >= _srcRect.Width * 4)
                        _srcRect.X = _srcRect.Width * 2;
                }
            }
            else
            {
                _framesPerSecond = _idleFPS;    // switch FPS to _idleFPS (slower animation)

                if (_updateTrigger >= 1)
                {
                    _updateTrigger = 0;
                    _srcRect.X += _srcRect.Width;

                    // Loop idle frames (2 and 3)
                    if (_srcRect.X >= _srcRect.Width * 3)
                        _srcRect.X = _srcRect.Width;
                }
            }

            _wasWalking = _isWalking;           // Update for next frame
            _position += _velocity * (float)gt.ElapsedGameTime.TotalSeconds;
            _rect.Location = _position.ToPoint();

            _testRectangle.Location = _rect.Location;
        }
        public virtual void MoveMe(Vector2 direction)
        {
            if (direction != Vector2.Zero)
            {
                // normalize direction to prevent diagonal movement from being faster
                direction.Normalize();

                _velocity = direction * _moveSpeed;
                // Trying to make collisions work
                _targetDestinationBounds = new Rectangle(_rect.X + (int)direction.X * 5,
                    _rect.Y + (int)direction.Y * 5, _rect.Width, _rect.Height);

                //if moving towards the right
                if (direction.X > 0)
                {
                    _facingRight = true;
                }
                // else if moving towards the left
                else if (direction.X < 0)
                {
                    _facingRight = false;
                }
            }
            else
            {
                _velocity = Vector2.Zero;
                // Trying to make collisions work
                _targetDestinationBounds = new Rectangle(_rect.X + (int)direction.X * 5,
                    _rect.Y + (int)direction.Y * 5, _rect.Width, _rect.Height);
            }
        }
        public override void drawme(SpriteBatch sb)
        {
            // Make sure the rectangle moves and is drawn in the right position
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;

            // Draw test pixel
            if (_drawBounds)
                sb.Draw(_testPixel, Bounds, Color.Blue * 1);
            sb.Draw(_testPixel, _targetDestinationBounds, Color.Red * .75f);


            // determine when to flip the sprite (making it look to the RIGHT)
            var flip = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            // Draw actual sprite
            sb.Draw(_txr, _rect, _srcRect, Color.White, 0f, 
                Vector2.Zero, flip, 0f);



        }

    }
}
