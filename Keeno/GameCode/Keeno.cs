using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Keeno
{

    class Keeno : AnimatedKeeno2D
    {
        private float _moveTimer;
        private float _moveTimerReset;
        private Vector2 _moveDir;

        public Keeno(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base (spriteSheet, fps, rect, pixel)
        {
            _moveTimer = 3;
            _moveTimerReset = _moveTimer;
            _moveSpeed = Globals.KeenoMovementSpeed;
            _drawBounds = false;
        }

        public void updateme(GameTime gt, KeyboardState kb)
        {
             _moveDir = Vector2.Zero;

            if (_moveTimer >= 0)
            {
                _moveTimer -= (float)gt.ElapsedGameTime.TotalSeconds;

            }
            else
            {
                _moveDir = new Vector2(Game1.RNG.Next(-1, 2), Game1.RNG.Next(-1, 2));
                MoveMe(_moveDir);
                _moveTimer = _moveTimerReset;
            }

            base.updateme(gt);
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
        public void updateme(GameTime gt, KeyboardState kb)
        {
            Vector2 moveDir = Vector2.Zero;

            if (Globals.MoveUP) moveDir.Y -= 1;
            if (Globals.MoveDOWN) moveDir.Y += 1;
            if (Globals.MoveLEFT) moveDir.X -= 1;
            if (Globals.MoveRIGHT) moveDir.X += 1;

            MoveMe(moveDir); // Call the base class method with the final direction

            base.updateme(gt);
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
