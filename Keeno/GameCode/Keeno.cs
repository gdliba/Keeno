using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Keeno
{

    class Keeno : AnimatedKeeno2D
    {
        public Keeno(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base (spriteSheet, fps, rect, pixel)
        {

        }

    }
    class Player : MobileSwarmPoint
    {
        public Player(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base(spriteSheet, fps, rect, pixel)
        {
            _moveSpeed = 200;
        }
        //public void updateme(GameTime gt, KeyboardState kb)
        //{
            
        //    if (Globals.MoveUP)       // move NORTH
        //        MoveMe(Direction.North);
        //    if (Globals.MoveLEFT)      // move WEST
        //        MoveMe(Direction.West);
        //    if (Globals.MoveRIGHT)      // move EAST
        //        MoveMe(Direction.East);
        //    if (Globals.MoveDOWN)      // move SOUTH
        //        MoveMe(Direction.South);
        //    if (Globals.NoMovementKeysPressed)
        //        MoveMe(Direction.None);     // DONT MOVE

        //    base.updateme(gt);
        //}
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
