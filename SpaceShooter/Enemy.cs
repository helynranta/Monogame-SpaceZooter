using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter
{
    public class Enemy
    {
        public Vector3 position;
        private Game1 game;
        public Texture2D texture;
        public Model model;
        private float angle;
        private float enemySpeed = 0.3f;
        public Enemy(Game1 g)
        {
            game = g;
            model = game.player.model;
            texture = game.player.texture;
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
                    spawnPoint = new Vector3(game.upLeft.X, game.random.Next((int)game.downRight.Y, -(int)game.downRight.Y), 0);
                    break;
                case 4:
                    spawnPoint = new Vector3(-game.upLeft.X, game.random.Next((int)game.downRight.Y, -(int)game.downRight.Y), 0);
                    break;
            }
            return spawnPoint; 
        }
        public void Update()
        {
            angle = game.LookAt(position,game.player.position,angle,0.1f);
            if((position-game.player.position).Length() > 0.5f)
                position += Vector3.Normalize(game.player.position - position)*enemySpeed;
        }
        //------------Drawing functions for player-----------------//
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
