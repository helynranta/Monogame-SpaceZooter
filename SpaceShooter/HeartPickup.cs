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
        private Vector3 position;
        private Model model;
        private Texture2D texture;
        public bool shouldDie = false;
        private float angle;
        public BoundingBox boundingBox;
        public HeartPickup(Vector3 pos, Model m, Texture2D tex)
        {
            position = pos;
            model = m;
            texture = tex;
        }
        public void Update(Game1 game)
        {
            //if close enough to player, then he probaply touches, so pickup
            if ((game.player.position - position).Length()<=3)
            {
                shouldDie = true;
                game.player.health += 5;
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
