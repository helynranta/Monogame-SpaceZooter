using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceShooter
{
    public class HeartPickup
    {
        public Vector3 position;
        private Model model;
        private Texture2D texture;
        public bool shouldDie = false;
        private float angle;
        public BoundingBox boundingBox;
        public double spawnTime;
        Game1 game;
        public HeartPickup(Vector3 pos, Model m, Texture2D tex, Game1 g)
        {
            position = pos;
            model = m;
            texture = tex;
            game = g;
            spawnTime = game.time;
        }
        public void Update()
        {
            //if close enough to player, then he probaply touches, so pickup
            if ((game.player.position - position).Length()<=5)
            {
                shouldDie = true;
                game.player.health += 5;
                //particle engine takes list of textures. need to do that even with one texture doh -.-
                List<Texture2D> hearts = new List<Texture2D>();
                hearts.Add(game.heartParticle);
                ParticleEngine heartParticles = new ParticleEngine(hearts, position, game);
                game.emitters.Add(heartParticles);
            }
            if(spawnTime + 10 < game.time)
            {
                shouldDie = true;
                //particle engine takes list of textures. need to do that even with one texture doh -.-
                List<Texture2D> hearts = new List<Texture2D>();
                hearts.Add(game.heartParticle);
                ParticleEngine heartParticles = new ParticleEngine(hearts, position, game);
                game.emitters.Add(heartParticles);
            }
        }
        public void Draw(Game1 game)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    angle += 0.05f;
                    effect.TextureEnabled = true;
                    effect.Texture = texture;
                    effect.World = Matrix.CreateScale(2, 2, 2) * Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(position);
                    effect.View = game.view;
                    effect.Projection = game.projection;
                }
                mesh.Draw();
            }
        }
    }
}
