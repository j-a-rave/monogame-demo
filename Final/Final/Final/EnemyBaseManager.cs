using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    class EnemyBaseManager : GameComponent
    {
        public List<EnemyBase> enemyBases;
        List<EnemyBase> enemyBasesToBeDestroyed;
        ExplosionManager explosionManager;

        public EnemyBaseManager(Game game)
            : base(game)
        {
            enemyBases = new List<EnemyBase>();
            enemyBasesToBeDestroyed = new List<EnemyBase>();
            explosionManager = new ExplosionManager(game, "bigsplode");

            Game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (EnemyBase eb in enemyBases)
            {
                if (eb.life <= 0)
                {
                    QueueDestroy(eb);
                    explosionManager.AddExplosion(Game, eb.loc);
                }
            }
            foreach (EnemyBase eb in enemyBasesToBeDestroyed)
            {
                Destroy(eb);
            }
            enemyBasesToBeDestroyed.Clear();

            base.Update(gameTime);
        }

        public void AddEnemyBase(Vector2 loc)
        {
            enemyBases.Add(new EnemyBase(Game, loc));
        }

        void QueueDestroy(EnemyBase eb)
        {
            enemyBasesToBeDestroyed.Add(eb);
        }

        void Destroy(EnemyBase eb)
        {
            eb.Visible = false;
            eb.Enabled = false;
            enemyBases.Remove(eb);
        }
    }
}
