using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MGLib
{
    //a drawablegamecomponent with no spritebatch.
    //implemented by DrawSprite.

    public class Sprite : DrawableGameComponent
    {
        public Vector2 loc, origin;
        public float rotation;
        public SpriteEffects spriteEffects;
        public Rectangle LocRect { get { return locRect; } set { locRect = value; } }
        public Color[] spriteTextureData;
        Texture2D spriteTexture;
        public Texture2D SpriteTexture
        {
            get { return spriteTexture; }
            set
            {
                spriteTexture = value;
                //collision data
                this.spriteTextureData = new Color[this.spriteTexture.Width * this.spriteTexture.Height];
                this.spriteTexture.GetData(this.spriteTextureData);
            }
        }
        public Matrix spriteTransform;
        protected ContentManager content;
        protected GraphicsDeviceManager graphics;
        protected float lastUpdateTime;
        protected Rectangle locRect;
        protected float scale;
        public float Scale
        {
            get { return this.scale; }
            set
            {
                if (value != this.scale)
                    SetTransformAndRect();
                this.scale = value;
            }
        }
        protected bool showMarkers;
        public bool ShowMarkers { get { return this.showMarkers; } set { this.showMarkers = value; } }
        protected Texture2D SpriteMarkersTexture;

        public Sprite(Game game)
            : base(game)
        {
            content = game.Content;
            this.Scale = 1;
        }

        //drawablegamecomponent functions
        public override void Initialize()
        {
            graphics = (GraphicsDeviceManager)Game.Services.GetService(typeof(IGraphicsDeviceManager));
            base.Initialize();
            spriteEffects = SpriteEffects.None;
        }

        protected override void LoadContent()
        {
            this.SpriteMarkersTexture = content.Load<Texture2D>("1x1");
            //this.origin = Vector2.Zero;
            //this.origin = new Vector2(this.SpriteTexture.Width / 2, this.SpriteTexture.Height / 2);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            lastUpdateTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            SetTransformAndRect();
            base.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch sb)
        {
            Draw(sb, Color.White);
        }

        public virtual void Draw(SpriteBatch sb, Color color)
        {
            sb.Draw(spriteTexture,
                new Rectangle((int)loc.X, (int)loc.Y, (int)(spriteTexture.Width * this.Scale), (int)(spriteTexture.Height * this.Scale)),
                null,
                color,
                MathHelper.ToRadians(rotation),
                this.origin,
                spriteEffects,
                0);
            DrawMarkers(sb);
        }

        protected void DrawMarkers(SpriteBatch sb)
        {
            if (showMarkers)
            {
                //top left
                //sb.Draw(this.SpriteMarkersTexture,
                    //new Vector2(this.locRect.Left, this.locRect.Top),
                    //Color.Cyan);
                sb.Draw(this.SpriteMarkersTexture, new Vector2(this.locRect.Left, this.LocRect.Top), null, null, this.origin, MathHelper.ToRadians(this.rotation), new Vector2(scale), Color.Cyan, SpriteEffects.None, 0);
                //top right
                //sb.Draw(this.SpriteMarkersTexture,
                    //new Vector2(this.locRect.Right, this.locRect.Top),
                    //Color.Cyan);
                sb.Draw(this.SpriteMarkersTexture, new Vector2(this.locRect.Right, this.LocRect.Top), null, null, this.origin, MathHelper.ToRadians(this.rotation), new Vector2(scale), Color.Cyan, SpriteEffects.None, 0);
                //bottom left
                //sb.Draw(this.SpriteMarkersTexture,
                    //new Vector2(this.locRect.Left, this.locRect.Bottom),
                    //Color.Cyan);
                sb.Draw(this.SpriteMarkersTexture, new Vector2(this.locRect.Left, this.LocRect.Bottom), null, null, this.origin, MathHelper.ToRadians(this.rotation), new Vector2(scale), Color.Cyan, SpriteEffects.None, 0);
                //bottom right
                //sb.Draw(this.SpriteMarkersTexture,
                    //new Vector2(this.locRect.Right, this.locRect.Bottom),
                    //Color.Cyan);
                sb.Draw(this.SpriteMarkersTexture, new Vector2(this.locRect.Right, this.LocRect.Bottom), null, null, this.origin, MathHelper.ToRadians(this.rotation), new Vector2(scale), Color.Cyan, SpriteEffects.None, 0);
                //center, ie location
                sb.Draw(this.SpriteMarkersTexture,
                    this.loc,
                    Color.Magenta);
            }
        }

        public virtual void SetTransformAndRect()
        {
            //the texture might not be loaded the first time it's called
            //try catch is apparently too slow, so just...
            if (this.SpriteTexture != null) //yeah, that'll do
            {
                spriteTransform = Matrix.CreateTranslation(new Vector3(this.origin * -1, 0.0f)) * Matrix.CreateScale(this.Scale) * Matrix.CreateRotationZ(0.0f) * Matrix.CreateTranslation(new Vector3(this.loc, 0.0f));
                this.locRect = CalculateBoundingRectangle(new Rectangle(0, 0, this.spriteTexture.Width, this.spriteTexture.Height), spriteTransform);
            }
        }

        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            //all 4 corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            //transform to world space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            //find min and max of rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop), Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop), Vector2.Max(leftBottom, rightBottom));

            //return min and max as a rectangle
            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        #region SPRITE COLLISION

        //rectangle collision
        public virtual bool Intersects(Sprite otherSprite)//virtual for 'hitbox' override
        {
            return Sprite.Intersects(this.locRect, otherSprite.locRect);
        }
        public static bool Intersects(Rectangle a, Rectangle b)
        {
            return (a.Right > b.Left && a.Left < b.Right && a.Bottom > b.Top && a.Top < b.Bottom);
        }

        //per pixel collision using rectangles
        public bool IntersectPixels(Sprite otherSprite)
        {
            return Sprite.IntersectPixels(this.locRect, this.spriteTextureData, otherSprite.locRect, otherSprite.spriteTextureData);
        }
        public static bool IntersectPixels(Rectangle rect1, Color[] data1, Rectangle rect2, Color[] data2)
        {
            Rectangle boundaries = Sprite.Intersection(rect1, rect2);
            Color color1, color2;

            if (boundaries != Rectangle.Empty)
            {
                for (int y = boundaries.Top; y < boundaries.Bottom; y++)
                {
                    for (int x = boundaries.Left; x < boundaries.Right; x++)
                    {
                        color1 = data1[(x - rect1.Left) + (y - rect1.Top) * rect1.Width];
                        color2 = data2[(x - rect2.Left) + (y - rect2.Top) * rect2.Width];

                        if (color1.A != 0 && color2.A != 0)//if the pixels aren't transparent
                            return true;
                    }
                }
            }
            return false;
        }
        public static Rectangle Intersection(Rectangle rect1, Rectangle rect2)
        {
            int x1 = Math.Max(rect1.Left, rect2.Left);
            int y1 = Math.Max(rect1.Top, rect2.Top);
            int x2 = Math.Min(rect1.Right, rect2.Right);
            int y2 = Math.Min(rect1.Bottom, rect2.Bottom);

            if (x2 >= x1 && y2 >= y1)
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            return Rectangle.Empty;
        }

        #endregion
    }
}

