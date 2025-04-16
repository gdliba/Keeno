using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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

    class AnimatedKeeno2D : Animated2D
    {
        Direction _direction;

        public AnimatedKeeno2D(Texture2D spriteSheet, int fps, Rectangle rect)
            : base(spriteSheet, fps, rect)
        {
            _direction = Direction.West;
        }

    }
}
