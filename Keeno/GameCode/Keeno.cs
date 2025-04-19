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
            _drawBounds = true;
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
        public Player(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base(spriteSheet, fps, rect, pixel)
        {
            _moveSpeed = Globals.PlayerMovementSpeed;
            //_rect = new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height / 2);
            //_testRectangle = _rect;
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
    }
}
