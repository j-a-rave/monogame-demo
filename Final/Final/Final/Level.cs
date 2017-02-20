using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Final
{
    class Level : GameComponent
    {   
        public EnemyManager enemyManager;
        public Ship ship;
        public EnemyBaseManager enemyBaseManager;
        public ExplosionManager explosionManager;
        public AsteroidManager asteroidManager;
        public Boundary boundary;
        public MGLib.ScoreKeeper scoreKeeper;
        public Minimap minimap;
        public int returnCounter;

        enum SpawnScale { TopLeft, TopCenter, TopRight, Left, Right, BottomLeft, BottomCenter, BottomRight };
        Vector2 viewportVector;
        int spawnCounter;

        enum LevelState { Alive, Dead, Over };
        LevelState currentState;
        int deathCounter;

        public Level(Game game)
            : base(game)
        {
            this.ship = new Ship(game);
            this.enemyManager = new EnemyManager(game);
            this.enemyBaseManager = new EnemyBaseManager(game);
            this.explosionManager = new ExplosionManager(game);
            this.asteroidManager = new AsteroidManager(game);
            this.boundary = new Boundary(game);
            this.scoreKeeper = new MGLib.ScoreKeeper(game,"VICTORY!","You are defeated...","font","shipLife");
            this.minimap = new Minimap(game);
            this.spawnCounter = 300;
            this.currentState = LevelState.Alive;
            this.deathCounter = 0;
            this.returnCounter = 0;

            Game.Components.Add(this);
        }

        public override void Initialize()
        {
            this.viewportVector = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height) / MGLib.Camera2D.zoom;
            //hard-coded level size, since this is as of now a one-level game. sorry!
            boundary.AddWall(new Vector2(-4352, -4384), new Vector2(4352, -4352));
            boundary.AddWall(new Vector2(-4384, -4352), new Vector2(-4352, 4352));
            boundary.AddWall(new Vector2(4352, -4352), new Vector2(4384, 4352));
            boundary.AddWall(new Vector2(-4352, 4352), new Vector2(4352, 4384));

            MGLib.ScoreKeeper.setInitialScoreAndLives(0, 3);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            switch (currentState) { 
                case LevelState.Alive:
                    CheckWin();

                    minimap.SetMMBasePosition(enemyBaseManager.enemyBases);
                    minimap.SetMMShipPosition(ship.loc);

                    HandleCollisions();

                    this.enemyManager.AccessPlayerLoc(this.ship.loc);

                    if (spawnCounter <= 0)
                    {
                        HandleEnemySpawning();
                        spawnCounter = 120;
                    }
                    else
                    {
                        spawnCounter--;
                    }
                    break;
                case LevelState.Dead:
                    if (this.deathCounter >= 180)
                    {
                        ship = new Ship(Game);
                        currentState = LevelState.Alive;
                        deathCounter = 0;
                    }
                    else
                    {
                        deathCounter++;
                    }
                    break;
                case LevelState.Over:
                    returnCounter++;
                    break;
            }
            base.Update(gameTime);
        }

        public void SpawnEnemy(Vector2 loc)
        {
            this.enemyManager.SpawnEnemy(loc);
        }

        public void HandleEnemySpawning()
        {
            Matrix transformMatrix = MGLib.Camera2D.GetTransformation(Game.GraphicsDevice);

            foreach (EnemyBase eb in enemyBaseManager.enemyBases)
            {
                Vector2 transformedLoc = Vector2.Transform(eb.loc + eb.origin, transformMatrix);
                //check to see if the base's center is within frame
               if(transformedLoc.Y < Game.GraphicsDevice.Viewport.Height && transformedLoc.Y > 0 &&
                    transformedLoc.X < Game.GraphicsDevice.Viewport.Width && transformedLoc.X > 0)
                {
                    SpawnEnemy(eb.origin);
                }
            }
                SpawnEnemy(((viewportVector + enemyManager.textureSize) * GetRandomScale()) + ship.loc);
        }

        Vector2 GetRandomScale()
        {
            switch ((SpawnScale)new Random().Next(8))
            {
                case SpawnScale.TopLeft:
                    return new Vector2(-1, -1);
                case SpawnScale.TopCenter:
                    return new Vector2(0, -1);
                case SpawnScale.TopRight:
                    return new Vector2(1, -1);
                case SpawnScale.Left:
                    return new Vector2(-1, 0);
                case SpawnScale.Right:
                    return new Vector2(1, 0);
                case SpawnScale.BottomLeft:
                    return new Vector2(-1, 1);
                case SpawnScale.BottomCenter:
                    return new Vector2(0, 1);
                case SpawnScale.BottomRight:
                    return new Vector2(1, 1);
                default:
                    return Vector2.Zero;
            }
        }

        void HandleCollisions()
        {
            foreach (Bullet b in ship.gun.bulletManager.bullets)
            {
                foreach (Enemy e in enemyManager.enemies)
                {
                    if (b.Intersects(e))
                    {
                        if (b.IntersectPixels(e))
                        {
                            enemyManager.QueueDestroyEnemy(e);
                            ship.gun.bulletManager.QueueDestroyBullet(b);
                            explosionManager.AddExplosion(Game, b.loc);
                            MGLib.ScoreKeeper.addScore(100);
                        }
                    }
                }
                foreach (EnemyBase eb in this.enemyBaseManager.enemyBases)
                {
                    if (b.Intersects(eb))
                    {
                        if (b.IntersectPixels(eb))
                        {
                            ship.gun.bulletManager.QueueDestroyBullet(b);
                            explosionManager.AddExplosion(Game, b.loc);
                            eb.Damage();
                            MGLib.ScoreKeeper.addScore(100);
                        }
                    }
                }
                foreach (Asteroid a in this.asteroidManager.asteroids)
                {
                    if (b.Intersects(a))
                    {
                        if (b.IntersectPixels(a))
                        {
                            ship.gun.bulletManager.QueueDestroyBullet(b);
                            explosionManager.AddExplosion(Game, b.loc);
                            asteroidManager.QueueDestroy(a);
                            MGLib.ScoreKeeper.addScore(10);
                        }
                    }
                }
            }

            foreach (Enemy e in enemyManager.enemies)
            {
                if (e.Intersects(ship))
                {
                    if (e.IntersectPixels(ship))
                    {
                        enemyManager.QueueDestroyEnemy(e);
                        explosionManager.AddExplosion(Game, e.loc);
                        KillPlayer();
                    }
                }
                foreach (Asteroid a in asteroidManager.asteroids)
                {
                    if (a.Intersects(e))
                    {
                        enemyManager.QueueDestroyEnemy(e);
                        explosionManager.AddExplosion(Game, e.loc);
                        asteroidManager.QueueDestroy(a);
                        MGLib.ScoreKeeper.addScore(100);
                    }
                }
            }

            foreach (Asteroid a in asteroidManager.asteroids)
            {
                if (a.Intersects(ship))
                {
                    explosionManager.AddExplosion(Game, ship.loc);
                    asteroidManager.QueueDestroy(a);
                    KillPlayer();
                }
            }

            foreach (EnemyBase eb in this.enemyBaseManager.enemyBases)
            {
                if (eb.Intersects(ship))
                {
                    if (eb.IntersectPixels(ship))
                    {
                        explosionManager.AddExplosion(Game, ship.loc);
                        KillPlayer();
                    }
                }
            }

            foreach (Wall w in boundary.Walls)
            {
                if (w.Intersects(ship))
                {
                    explosionManager.AddExplosion(Game, ship.loc);
                    KillPlayer();
                }
            }
        }

        void ClearEnemies()
        {
            foreach (Enemy e in enemyManager.enemies)
            {
                enemyManager.QueueDestroyEnemy(e);
            }
        }

        void KillPlayer()
        {
            ship.Visible = false;
            DisablePlayer();
            ClearEnemies();
            if (MGLib.ScoreKeeper.die())
            {
                currentState = LevelState.Over;
                MGLib.ScoreKeeper.winState = MGLib.ScoreKeeper.WinStates.Lose;
            }
            else
            {
                currentState = LevelState.Dead;
            }
        }

        void CheckWin()
        {
            if (enemyBaseManager.enemyBases.Count == 0 && currentState == LevelState.Alive)
            {
                ClearEnemies();
                DisablePlayer();
                currentState = LevelState.Over;
                MGLib.ScoreKeeper.winState = MGLib.ScoreKeeper.WinStates.Win;
                if (MGLib.ScoreKeeper.Lives == 3)
                {
                    //no death bonus!
                    MGLib.ScoreKeeper.addScore(50000);
                }
            }
        }

        void DisablePlayer()
        {
            ship.Enabled = false;
            ship.gun.Enabled = false;
            ship.gun.bulletManager.Enabled = false;
            ship.gun.bulletManager.bullets.Clear();
            ship.gun.bulletManager.bulletsToBeDestroyed.Clear();
        }
    }
}
