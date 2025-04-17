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
            _isWalking = false;
            _moveTimer = 3;
            _moveTimerReset = _moveTimer;
            _moveSpeed = 20;
        }

        public void updateme(GameTime gt, KeyboardState kb)
        {
            Vector2 moveDir = Vector2.Zero;

            if (_moveTimer >= 0)
            {
                _moveTimer -= (float)gt.ElapsedGameTime.TotalSeconds;

            }
            else
            {
                moveDir = new Vector2(Game1.RNG.Next(-1, 2), Game1.RNG.Next(-1, 2));
                MoveMe(moveDir);
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
            _moveSpeed = 200;
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
