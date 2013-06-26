using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter
{
    public class Achievement
    {
        public bool isDone = false;
        public Achievement(Func<int, int> callBackFunction)
        {

        }
        public int KillEnemies(int value)
        {
            if (value >= 10)
                return 1;
            else
                return 0;
        }
    }
}
