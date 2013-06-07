using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceShooter;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Windows.Devices.Input;


namespace SpaceShooter
{
    class Player : Game
    {
        #region variables
        public Model model;
        public Texture2D texture;
        public Vector3 position;
        public Vector3 mouseInWorld = new Vector3();
        public float SCREEN_WIDTH, SCREEN_HEIGHT;
        private Matrix world, view, projection, RotationMatrix;
        public Boolean isPressed = false;
        public int touchID = -1;
        public float speed = 1f;
        Joystick joystick;
        public float turnSpeed = 0.75f;
        float angle = 0.0f;
        public Vector3 aimSpot;
        #endregion

        public void Update(float sw, float sh, Matrix w, Matrix v, Matrix p, Vector3 msw, Joystick js)
        {
            joystick = js;
            SCREEN_WIDTH = sw;
            SCREEN_HEIGHT = sh;
            world = w;
            view = v;
            projection = p;
            mouseInWorld = msw;
            speed =  Vector2.Subtract(joystick.anchorPos, joystick.position).Length()/100;
            angle = LookAt(position, aimSpot, angle, turnSpeed);
        }

        protected float LookAt(Vector3 position, Vector3 aimSpot, float currentAngle, float turnSpeed)
        {
            float x = aimSpot.X - position.X;
            float y = aimSpot.Y - position.Y;
            float desiredAngle = (float)Math.Atan2(y, x);
            float difference = WrapAngle(desiredAngle - currentAngle);
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
            return WrapAngle(currentAngle + difference);

        }

        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }


        public void Draw(SpriteBatch _spriteBatch, SpriteFont font)
        {
            _spriteBatch.Begin();
            _spriteBatch.End();
            DrawModel(texture);
        }
       
        public void DrawModel(Texture2D texture)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    RotationMatrix = RotationMatrix * Matrix.CreateFromAxisAngle(RotationMatrix.Up, MathHelper.ToRadians(1.0f));
                    Matrix[] absoluteBoneTransform = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(absoluteBoneTransform);
                    
                    effect.TextureEnabled = true;
                    effect.Texture = texture;
                    effect.World = Matrix.CreateRotationZ(angle + MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

      
    }
}
