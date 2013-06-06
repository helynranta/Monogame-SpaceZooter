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
        private Matrix world, view, projection;
        public Boolean isPressed = false;
        public int touchID = -1;
        public float speed = 1f;
        Joystick joystick;
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

                    //effect.EnableDefaultLighting();
                    Matrix pos = Matrix.CreateTranslation(position);
                    effect.TextureEnabled = true;
                    effect.Texture = texture;
                    effect.World = pos;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

      
    }
}
