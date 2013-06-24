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
        public float health;
        private Vector2 position;
        private Texture2D texture;
        private Game1 game;
        
        public HealthBar(float startHealth, Vector2 pos, Game1 g, Texture2D tex)
        {
            health = startHealth;
            position = pos;
            game = g;
            texture = tex;
        }
        public void Update(float h)
        {
            if (health > h){
                health--;
                game.combo = 0;
            }
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();
            for(float h = 0; h < health; h++)
            {
                Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width / 2, texture.Height / 2 + (int)(0.5 * h));
                Vector2 origin = new Vector2(texture.Width/2,texture.Height/2);
                
                _spriteBatch.Draw(texture, new Vector2(position.X+texture.Width/2*h, position.Y-(int)(0.5*h)), sourceRectangle, Color.White);
            }
            _spriteBatch.End();
        }
    }
}
