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
            base.updateme(gt);
        }
        public void TryToMove(GameTime gt)
        {
            _direction = Vector2.Zero;

            if (_moveTimer >= 0)
            {
                _moveTimer -= (float)gt.ElapsedGameTime.TotalSeconds;

            }
            else
            {
                _direction = new Vector2(Game1.RNG.Next(-1, 2), Game1.RNG.Next(-1, 2));
                MoveMe(_direction);
                _moveTimer = _moveTimerReset;
            }
        }
    }
    class Player : MobileSwarmPoint
    {

        private Rectangle _interactionRange;
        public Rectangle InteractionRange { get { return _interactionRange; } }
        public Vector2 Position { get { return new(_position.X + _rect.Width / 2, _position.Y + _rect.Height / 2); } }

        public Player(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base(spriteSheet, fps, rect, pixel)
        {
            _moveSpeed = Globals.PlayerMovementSpeed;
            _drawBounds = true;
            _interactionRange = new Rectangle((int)_position.X - rect.Width, (int)_position.Y - rect.Height, rect.Width * 3, rect.Height * 3);
        }
        public override void updateme(GameTime gt)
        {
            //// Update the player's "would be" bounds in relation to
            //// the direction they are moving in
            //_targetDestinationBounds = new Rectangle(
            //    _rect.X + _rect.Width / 4 + (int)_direction.X * 5,
            //    _rect.Y + _rect.Height / 4 + (int)_direction.Y * 5,
            //    2 * _rect.Width / 3,
            //    2 * _rect.Height / 3);

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

            //sb.Draw(_testPixel, _interactionRange, Color.Red*.75f);

            base.drawme(sb);
            //sb.Draw(_testPixel, new Vector2(Position.X,Position.Y), Color.Blue);
        }
    }
}
