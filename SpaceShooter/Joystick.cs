using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpaceShooter;
//<summary>
//joystick class
//TODO handle joystick touches here, as a listener or something.
//</summary>
namespace SpaceShooter
{
    //JOYSTICK
    public class Joystick
    {
        public Game1 game;
        public Texture2D jsTexture;
        public Texture2D jsBackgroundTex;
        public Vector2 anchorPos;
        public Vector2 position;
        public Vector2 normal;
        public Vector2 dir;
        public int touchID;
        public Boolean isPressed = false;


        public void Initialize(Game1 g, Vector2 pos)
        {
            game = g;
            position = pos;
            anchorPos = pos;
        }
        //draw it... -.-
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(game.jsBackgroundTex, new Vector2(anchorPos.X - 100, anchorPos.Y - 100), Color.White);
            _spriteBatch.Draw(game.jsTexture, new Vector2(position.X-50, position.Y-50), Color.White);
        }
    }
}
