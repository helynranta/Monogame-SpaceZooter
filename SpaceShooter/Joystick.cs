using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpaceShooter;

namespace SpaceShooter
{
    class Joystick
    {
        public Texture2D jsTexture;
        public Vector2 anchorPos;
        public Vector2 position;
        public Vector2 normaali;
        public Vector2 dir;
        public int touchID;
        public Boolean isPressed = false;
        float SCREEN_WIDTH, SCREEN_HEIGHT;


        public void Initialize(float sh, float sw)
        {
            position = new Vector2(sw - 200, sh - 200);
        }
        public void Update(float sh, float sw, Vector2 mousePosition)
        {
            SCREEN_HEIGHT = sh;
            SCREEN_WIDTH = sw;
            anchorPos = new Vector2(SCREEN_WIDTH - 200, SCREEN_HEIGHT - 200);
            normaali = Vector2.Normalize(Vector2.Subtract(anchorPos,mousePosition));
            if (anchorPos != position)
                dir = Vector2.Normalize(Vector2.Subtract(anchorPos, position));
            else
                dir = Vector2.Zero;

        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(jsTexture, new Vector2(position.X-50, position.Y-50), Color.White);
            _spriteBatch.End();

        }
    }
}
