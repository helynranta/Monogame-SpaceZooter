using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpaceShooter;
using Microsoft.Xna.Framework;

namespace SpaceShooter
{
    class Bullet
    {
        public Vector3 position;
        public Vector3 flyDir;
        public Bullet(Vector3 spawnPoint, Vector3 flyDir)
        {
            this.position = spawnPoint;
            this.flyDir = flyDir;
        }
        public void Initialize()
        {
            
        }
        public void Update()
        {

        }
        public void Draw()
        {

        }
    }
}
