using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter
{
    public class HealthBar
    {
        public float Health;
        private Vector2 position;
        private Texture2D texture;
        private Game1 game;
        
        public HealthBar(float startHealth, Vector2 pos, Game1 g)
        {
            Health = startHealth;
            position = pos;
            game = g;
        }
        public void Update()
        {

        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            for(float h = 0; h < Health; h++)
            {
                SpriteEffects e = new SpriteEffects();
                Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                Vector2 origin = new Vector2(texture.Width/2,texture.Height/2);
                _spriteBatch.Begin();
                //_spriteBatch.Draw(texture, new Vector2(position.X - texture.Width+h), position.Y, sourceRectangle, 0f, origin,h, e, 0);
                _spriteBatch.End();
            }
        }
    }
}
