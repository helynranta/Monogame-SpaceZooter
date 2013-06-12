using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpaceShooter;

namespace SpaceShooter
{
    //JOYSTICK
    public class Joystick
    {
        public Texture2D jsTexture;
        public Vector2 anchorPos;
        public Vector2 position;
        public Vector2 normal;
        public Vector2 dir;
        public int touchID;
        public Boolean isPressed = false;


        public void Initialize(float sh, float sw)
        {
            position = new Vector2(sw - 200, sh - 200);
        }
        //draw it... -.-
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(jsTexture, new Vector2(position.X-50, position.Y-50), Color.White);
            _spriteBatch.End();

        }
    }
}
