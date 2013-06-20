using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter
{
    //<summary>
    //this particle system is based on tutorial here:
    //http://rbwhitaker.wikidot.com/2d-particle-engine-1
    //took what i needed and converted it to work in 3d space
    //TODO make this class support different kind of systems, now it only does explosions...
    //</summary>
    public class ParticleEngine
    {
        //some variables
        public Vector3 emitterLocation;
        public bool shouldDie = false;
        public List<Particle> particles;
        private float timeToLive;
        private List<Texture2D> textures;
        private Game1 game;
        //private enum type {smoke, flamethower}
        //constructor
        public ParticleEngine(List<Texture2D> tex, Vector3 location, Game1 g)
        {
            emitterLocation = location;
            this.textures = tex;
            this.particles = new List<Particle>();
            game = g;
            timeToLive = 1;
        }
        //Generating a particle
        private Particle GenerateNewParticle()
        {
            Texture2D texture = textures[game.random.Next(textures.Count)];//select random particles from list
            Vector3 position = new Vector3 (emitterLocation.X,emitterLocation.Y,0);//get emitter location and apply it to be start position of the new particle
            Vector3 velocity = new Vector3(0.2f * (float)(game.random.NextDouble() * 2 - 1), //add random velocity to each particle
                                          0.2f * (float)(game.random.NextDouble() * 2 - 1),
                                          0); // no Z velocity... this is top-down game
            float angle = (float)(game.random.NextDouble());//random start angle
            float angularVelocity = 0.1f * (float)(game.random.NextDouble() * 0.1);//random rotation speed for all particles
            //this is temporary variable for smoke color
            //each R,G and B value should be same on range 0-1, meaning its going from black to white
            float tempCol = .5f*(float)game.random.NextDouble()+.5f;
            Color color = new Color(tempCol, tempCol, tempCol);
            float size = 0.25f*(float)game.random.NextDouble();
            int ttl = 20 + game.random.Next(40);
            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }
        //update method
        public void Update()
        {
            timeToLive -= 0.2f;
            if (timeToLive > 0)//if there is time to live, add more particles
            {
                int total = 5;
                for (int i = 0; i < total; i++)
                {
                    particles.Add(GenerateNewParticle());
                }
            }
            for (int particle = 0; particle < particles.Count; particle++)//update all particles
            {
                particles[particle].Update();
                if(particles[particle].timeToLive <= 0)//if particles are dead, remove them
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
            if (particles.Count == 0)
            {
                shouldDie = true;
            }
        }
        //draw method
        public void Draw(SpriteBatch _spriteBatch)
        {
            BasicEffect particleEffect = game.basicEffect;
            //world scale, no Z axis on particle
            particleEffect.World = Matrix.CreateScale(1, 1, 0);
            //grab view and projection from game and apply it to particle effect
            particleEffect.View = game.view;
            particleEffect.Projection = game.projection;
            _spriteBatch.Begin(0, null, null, DepthStencilState.DepthRead, RasterizerState.CullNone, particleEffect);
            //loop threw all particles in this system
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(_spriteBatch, game, particleEffect);//draw each particle
            }
            _spriteBatch.End();
        }
    }
}
