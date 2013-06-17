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
            Texture2D texture = textures[game.random.Next(textures.Count)];
            Vector3 position = new Vector3 (emitterLocation.X,emitterLocation.Y,0);
            Vector3 velocity = new Vector3(0.2f * (float)(game.random.NextDouble() * 2 - 1),
                                          0.2f * (float)(game.random.NextDouble() * 2 - 1),
                                          0);
            float angle = 0;
            float angularVelocity = 0.1f * (float)(game.random.NextDouble() * 0.1);
            Color color = new Color((float)game.random.NextDouble(),
                                    (float)game.random.NextDouble(),
                                    (float)game.random.NextDouble());
            float size = (float)game.random.NextDouble();
            int ttl = 20 + game.random.Next(40);
            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }
        //update method
        public void Update()
        {
            timeToLive -= 0.2f;
            if (timeToLive > 0)
            {
                int total = 5;
                for (int i = 0; i < total; i++)
                {
                    particles.Add(GenerateNewParticle());
                }
            }
            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if(particles[particle].timeToLive <= 0)
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
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(_spriteBatch, game);
            }
        }
    }
}
