using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter
{
    public class Enemy
    {
        public Vector3 position, flyDir;
        private Game1 game;
        public Texture2D texture;
        public Model model;
        public BoundingBox physicsBody;
        private float angle;
        private float enemySpeed = 0.3f;
        public bool newDir = false;
        public bool shouldDie = false;
        public Enemy(Game1 g)
        {
            game = g;
            model = game.enemyModel;
            texture = game.enemyTexture;
            position = CreateSpawnPoint();
            
        }
        private Vector3 CreateSpawnPoint()
        {
            int luku = game.random.Next(1,4);
            Vector3 spawnPoint = new Vector3();
            switch(luku){
                case 1:
                    spawnPoint = new Vector3(game.random.Next(-(int)game.downRight.X,(int)game.downRight.X), game.upLeft.Y+5f, 0);
                    break;
                case 2:
                    spawnPoint = new Vector3(game.random.Next(-(int)game.downRight.X, (int)game.downRight.X), -game.upLeft.Y-5f, 0);
                    break;
                case 3:
                    spawnPoint = new Vector3(game.upLeft.X-2, game.random.Next((int)game.downRight.Y, -(int)game.downRight.Y), 0);
                    break;
                case 4:
                    spawnPoint = new Vector3(-game.upLeft.X+2, game.random.Next((int)game.downRight.Y, -(int)game.downRight.Y), 0);
                    break;
            }
            return spawnPoint; 
        }
        public void Update()
        {
            physicsBody = new BoundingBox(position - new Vector3(3, 3, 3), position + new Vector3(3, 3, 3));
            angle = game.LookAt(position,game.player.position,angle,0.08f);
            if ((position - game.player.position).Length() > 3f)
            {
                flyDir = Vector3.Normalize(game.player.position - position) * enemySpeed;
                position += flyDir;
            }
            else
            {
                shouldDie = true;
                game.player.Health -= 1;
            }
            foreach (ModelMesh mesh in model.Meshes)
            {

            }
        }
        //------------Drawing functions for player-----------------//
        #region drawing
        public void UpdateCollision(List<Enemy> boundList, int index)
        {
            bool noCollision = true;
            for (int a = index+2; a < boundList.Count; a++)
            {
                if (physicsBody.Intersects(boundList[a].physicsBody))
                {
                    if ((game.player.position - position).Length() < (game.player.position - boundList[a].position).Length())
                    {
                        enemySpeed += .005f;
                        boundList[a].enemySpeed -= 0.02f;
                        if (boundList[a].enemySpeed < 0.01f)
                            boundList[a].enemySpeed = 0.01f;
                    }
                    else
                    {
                        noCollision = false;
                        boundList[a].enemySpeed += .005f;
                        enemySpeed -= 0.02f;
                        if (enemySpeed < 0.01f)
                            enemySpeed = 0.01f;
                    }
                }
            }
            if (noCollision)
                if (enemySpeed <= .2f)
                    enemySpeed += .01f;
        }
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
                    angle += .08f * (float)game.random.NextDouble();
                    effect.World = Matrix.CreateRotationX(angle) * Matrix.CreateRotationZ(angle) * Matrix.CreateRotationY(angle);
                    effect.World *= Matrix.CreateTranslation(position);
                    effect.View = game.view;
                    effect.Projection = game.projection;
                }
                mesh.Draw();
            }
        }
        #endregion
    }
}
