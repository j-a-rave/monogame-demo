using System;
using Microsoft.Xna.Framework;

namespace Final
{
    class Gun : GameComponent
    {
        public BulletManager bulletManager;

        const int fireRate = 20;
        int loadingStep;
        Vector2 loc;
        float speed;
        enum TriggerState { Firing, Loading, Hold };
        TriggerState state;

        public Gun(Game game)
            : base(game)
        {
            bulletManager = new BulletManager(game);
            Game.Components.Add(bulletManager);
            this.loc = Vector2.Zero;
            this.speed = 0.0f;
            state = TriggerState.Hold;
            loadingStep = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (MGLib.ControlManager.isAutoFiring())
            {
                state = TriggerState.Loading;
            }
            else if (MGLib.ControlManager.isFiring())
            {
                state = TriggerState.Firing;
            }
            else
            {
                state = TriggerState.Hold;
            }

            StateBehavior();

            base.Update(gameTime);
        }
        public void StateBehavior()
        {
            if (state == TriggerState.Loading)
            {
                if (loadingStep >= fireRate)
                {
                    state = TriggerState.Firing;
                }
                else
                {
                    loadingStep++;
                }
            }
            if (state == TriggerState.Firing)
            {
                bulletManager.Shoot(loc, MGLib.ControlManager.currentRotation, speed);
                bulletManager.Shoot(loc, -MGLib.ControlManager.currentRotation, -speed);
                loadingStep = 0;
            }
        }
        public void SetParameters(Vector2 loc, float speed)
        {
            this.loc = loc;
            this.speed = speed;
        }
    }
}
