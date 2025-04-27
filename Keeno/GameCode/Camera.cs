using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace Keeno
{
    struct Camera
    {
        public Vector2 Position;
        public float Zoom;

        public Matrix getCam()
        {
            Matrix temp;
            temp = Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0));
            temp *= Matrix.CreateScale(Zoom);
            return temp;
        }
    }
}
