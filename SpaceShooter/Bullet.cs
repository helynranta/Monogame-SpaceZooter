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

        public Bullet(Vector3 spawnPoint, Vector3 flyDir)
        {
            this.position = spawnPoint;
            this.flyDir = flyDir;
        }
        public void Initialize(Game1 g, float a)
        {
            game = g;
            model = game.bullet;
            texture = game.bulletTexture;
            angle = a;
            spawnTime = (float)game.time;
            position -= flyDir * 2;

        }
        public void Update()
        {
            if (spawnTime + 5 <= game.time)
            {
                shouldDie = true;
            }
            position -= flyDir*2;
            physicsBody = new BoundingBox(position - new Vector3(1, 1, 1), position + new Vector3(1, 1, 1));
            updateCollision();
        }
        public void updateCollision()
        {
            foreach(Enemy e in game.enemyList)
            {
                if (physicsBody.Intersects(e.physicsBody))
                {
                    e.shouldDie = true;
                    shouldDie = true;
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
