using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Keeno
{

    class Keeno : AnimatedKeeno2D
    {
        private float _moveTimer;
        private float _moveTimerReset;

        public Keeno(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base (spriteSheet, fps, rect, pixel)
        {
            _moveTimer = 3;
            _moveTimerReset = _moveTimer;
            _moveSpeed = Globals.KeenoMovementSpeed;
            _drawBounds = false;
        }

        public override void updateme(GameTime gt)
        {
            //MoveMe(MoveInThisDirection(gt));

            base.updateme(gt);
        }
        public Vector2 MoveInThisDirection(GameTime gt)
        {

            if (_moveTimer >= 0)
            {
                _moveTimer -= (float)gt.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                TryToMove();
                _moveTimer = _moveTimerReset;
            }
            return _direction;
        }
        public void TryToMove()
        {
            _direction = new Vector2(Game1.RNG.Next(-1, 2), Game1.RNG.Next(-1, 2));
        }
        public float DistanceTo(Vector2 destination)
        {
            return (destination - _position).Length();
        }
        public void Selected()
        {
            _isSelected = true;
        }
    }
    class Player : MobileSwarmPoint
    {

        private Rectangle _interactionRange;
        public Rectangle InteractionRange { get { return _interactionRange; } }
        

        public Player(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base(spriteSheet, fps, rect, pixel)
        {
            _moveSpeed = Globals.PlayerMovementSpeed;
            _drawBounds = false;
            _interactionRange = new Rectangle((int)_position.X - rect.Width, (int)_position.Y - rect.Height, rect.Width * 3, rect.Height * 3);
        }
        public override void updateme(GameTime gt)
        {
            base.updateme(gt);
        }

        public Rectangle HandleInput()
        {
             _direction = Vector2.Zero;

            if (Globals.MoveUP) _direction.Y -= 1;
            if (Globals.MoveDOWN) _direction.Y += 1;
            if (Globals.MoveLEFT) _direction.X -= 1;
            if (Globals.MoveRIGHT) _direction.X += 1;

            return _targetDestinationBounds;
        }
        public override void drawme(SpriteBatch sb)
        {
            // Make sure the rectangle moves and is drawn in the right position
            _interactionRange.X = (int)_position.X - _rect.Width;
            _interactionRange.Y = (int)_position.Y - _rect.Height;

            // Make sure the rectangle moves and is drawn in the right position
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;


            // determine when to flip the sprite (making it look to the RIGHT)
            var flip = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // Draw Player
            sb.Draw(_txr, _rect, _srcRect, _tint, 0f,
                    Vector2.Zero, flip, 0f);

            // Draw test pixel
            if (_drawBounds)
            {
                sb.Draw(_testPixel, _interactionRange, Color.Red * .75f);               // Draw interactionRange
                sb.Draw(_testPixel, Bounds, Color.Blue * .7f);                          // Draw Player Bounds
                sb.Draw(_testPixel, _targetDestinationBounds, Color.White * .75f);      // Draw _targetDestinationBound
                sb.Draw(_testPixel, new Vector2(Position.X, Position.Y), Color.Black);  // Draw Player Position
            }
            //base.drawme(sb);
        }
    }
}
