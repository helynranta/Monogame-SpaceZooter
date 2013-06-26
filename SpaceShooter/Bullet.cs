using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceShooter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooter
{
    public class Bullet
    {
        //bullet variables
        private Game1 game;
        private Model model;
        private Texture2D texture;
        private float angle;
        public Vector3 position;
        public Vector3 flyDir;
        public float spawnTime;
        public Boolean shouldDie = false;
        public BoundingBox physicsBody;
        public bool collideWithPlayer = true;
        public bool collideWithUfo = true;

        public Bullet(Vector3 spawnPoint, Vector3 flyDir, bool playerCol, bool ufoCol)
        {
            position = spawnPoint;
            this.flyDir = flyDir;
            collideWithPlayer = playerCol;
            collideWithUfo = ufoCol;
        }
        public void Initialize(Game1 g, float a)
        {
            game = g;
            model = game.bullet;
            texture = game.bulletTexture;
            angle = a;
            spawnTime = (float)game.time;
        }
        public void Update()
        {
            if (spawnTime + 3 <= game.time)
            {
                shouldDie = true;
                ParticleEngine particleEngine = new ParticleEngine(game.particleTextures, position, game);
                game.emitters.Add(particleEngine);
            }
            position.Z = 0;
            position -= flyDir*2;
            physicsBody = new BoundingBox(position - new Vector3(1, 1, 1), position + new Vector3(1, 1, 1));
        }
        public void updateCollision()
        {
            foreach(Enemy e in game.enemyList)
            {
                if (physicsBody.Intersects(e.physicsBody))
                {
                    e.shouldDie = true;
                    shouldDie = true;
                    game.combo++;
                    game.lastHitCombo = (float)game.time;
                    game.explosion.Play(0.1f, 0, 0);
                    game.score += 3*game.combo;
                    if (5 == game.random.Next(1, 10))
                    {
                        HeartPickup heart = new HeartPickup(e.position, game.heart, game.heartTexture, game);
                        game.heartList.Add(heart);
                    }
                }
            }
            if (collideWithUfo)
            {
                foreach (Ufo u in game.ufoList)
                {
                    if (physicsBody.Intersects(u.physicsBody))
                    {
                        u.shouldDie = true;
                        shouldDie = true;
                        game.combo++;
                        game.lastHitCombo = (float)game.time;
                        game.score += 5 * game.combo;
                        game.explosion.Play(0.1f, 0, 0);
                        if (5 == game.random.Next(1,10))
                        {
                            HeartPickup heart = new HeartPickup(u.position, game.heart, game.heartTexture, game);
                            game.heartList.Add(heart);
                        }
                    }
                }
            }
            if (collideWithPlayer)
            {//purkkaa
                if (physicsBody.Intersects(game.player.physicsBody) || ((position - game.player.position).Length() < 1f))
                {
                    shouldDie = true;
                    game.player.health -= 10;
                    game.combo = 0;
                    game.hit.Play(0.1f,0,0);
                }
            }
        }
        #region drawing
        public void Draw()
        {
            DrawModel(texture);
        }
        //draw model function
        public void DrawModel(Texture2D texture)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.TextureEnabled = true;
                    effect.Texture = texture;
                    effect.World = Matrix.CreateRotationZ(angle + MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(position);
                    effect.View = game.view;
                    effect.Projection = game.projection;
                }

                mesh.Draw();
            }
        }
        #endregion
    }
}
