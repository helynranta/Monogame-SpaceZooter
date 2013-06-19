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
        //variables, most pretty self-explanatory
        //every particle knows where it is and where it should go
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
            position = pos; //starting position is based on emitters location in 3d space
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
            timeToLive--;//decrease livespan
            position += velocity;//add movement
            angle += angularVelocity;//add some rotation to particle
            alpha -= 0.04f;//increase transparency threw life
        }
        //<summary>
        //draw function
        //drawing is based on tutorial on msdn blogs here:
        //http://blogs.msdn.com/b/shawnhar/archive/2011/01/12/spritebatch-billboards-in-a-3d-world.aspx
        //idea is to draw particles in 3d space as a 2d objects in spritebatch
        //</summary>
        public void Draw(SpriteBatch _spriteBatch, Game1 game)
        {
            Viewport viewport = game._viewport;
            BasicEffect particleEffect = game.basicEffect;
            //world scale, no Z axis on particle
            particleEffect.World = Matrix.CreateScale(1, 1, 0);
            //grab view and projection from game and apply it to particle effect
            particleEffect.View = game.view;
            particleEffect.Projection = game.projection;
            Rectangle sourceRectangle = new Rectangle(0,0, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width/2, texture.Height/2);
            //draw this particle
            //TODO why start new spritebatch for every particle, make it draw all existing particles in one
            //--> more efficent
            _spriteBatch.Begin(0, null, null, DepthStencilState.DepthRead, RasterizerState.CullNone, particleEffect);
            _spriteBatch.Draw(texture, new Vector2(position.X, position.Y), sourceRectangle, color*alpha, angle, origin, size, 0, position.Y);
            _spriteBatch.End();
        }
    }
}
