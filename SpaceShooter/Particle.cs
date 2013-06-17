using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//<summary>
//this particle system is based on tutorial here:
//http://rbwhitaker.wikidot.com/2d-particle-engine-1
//</summary>

namespace SpaceShooter
{
    public class Particle
    {
        //instance variables, most pretty self-explanatory
        public Texture2D texture;
        public Vector3 position;
        public Vector3 velocity;
        public float angle;
        public float angularVelocity;
        public Color color;
        public float size;
        public int timeToLive;
        public int parent;
        public float alpha;
        //Constuctor
        public Particle(Texture2D tex, Vector3 pos, Vector3 vel, float ang, float angVel, Color c, float s, int ttl)
        {
            texture = tex;
            position = pos;
            velocity = vel;
            angle = ang;
            angularVelocity = angVel;
            color = c;
            size = s/4;
            timeToLive = ttl;
            alpha = 1f;
        }
        //update function
        public void Update()
        {
            timeToLive--;
            position += velocity;
            angle += angularVelocity;
            alpha -= 0.04f;
        }
        //draw function
        public void Draw(SpriteBatch _spriteBatch, Game1 game)
        {
            Viewport viewport = game._viewport;
            BasicEffect basicEffect = game.basicEffect;

            basicEffect.World = Matrix.CreateScale(1,1,0);
            basicEffect.View = game.view;
            basicEffect.Projection = game.projection;

            Rectangle sourceRectangle = new Rectangle(0,0, texture.Width, texture.Height);

            Vector2 origin = new Vector2(texture.Width/2, texture.Height/2);
            position = Vector3.Transform(position, game.view);
            _spriteBatch.Begin(0, null, null, DepthStencilState.DepthRead, RasterizerState.CullNone, basicEffect);
            _spriteBatch.Draw(texture, new Vector2(position.X, position.Y), sourceRectangle, color*alpha, angle, origin, size, 0, position.Y);
            _spriteBatch.End();
        }
    }
}
