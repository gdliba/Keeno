using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace Keeno
{
    /// <summary>
    /// Struct creates a camera that will follow the player
    /// </summary>
    struct Camera
    {
        public Vector2 Position;
        public float Zoom;

        /// <summary>
        /// Creates a metrix that will be used by the draw method in Game1 to follow the player
        /// </summary>
        /// <returns></returns>
        public Matrix getCam()
        {
            Matrix temp;
            temp = Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0));
            temp *= Matrix.CreateScale(Zoom);
            return temp;
        }
    }
}
