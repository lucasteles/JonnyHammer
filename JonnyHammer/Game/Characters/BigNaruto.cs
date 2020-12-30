﻿using Caieta.Components.Utils;
using JonnyHamer.Engine.Entities;
using JonnyHamer.Engine.Entities.Sprites;
using JonnyHamer.Engine.Helpers;
using JonnyHamer.Engine.Inputs;
using JonnyHammer.Engine.Entities.Components;
using JonnyHammer.Engine.Entities.Components.Collider;
using JonnyHammer.Engine.Helpers;
using Microsoft.Xna.Framework;

namespace JonnyHammer.Game.Characters
{
    public class BigNaruto : GameObject
    {
        private float speed = 2f;

        private KeyboardInput keyboard;
        private AnimationRenderer animatedSprite;

        public float HorizontalPosition { get; set; }
        public float MoveAmount { get; set; }

        public int Height => animatedSprite.Height;
        public int Width => animatedSprite.Width;

        public BigNaruto(int moveAmount)
        {
            MoveAmount = moveAmount;
            keyboard = new KeyboardInput();

            var animations = CreateNarutaoAnimations();
            animations.Color = Color.Red;
            animatedSprite = AddComponent(animations);

            var collider = AddComponent(new Collider(animatedSprite, true, true, Color.Purple));
            //AddComponent(new PhysicsComponent(BodyType.Dynamic, collider, mass: 10, friction: 0.2f)); ;
            AddComponent<SlimPhysics>();

            Transform.Scale = 0.6f;

            collider.OnTrigger += Collider_OnCollide;
            
            
            HorizontalPosition = Transform.X;
            AddComponent(new Tween(TweenMode.Loop, this, nameof(HorizontalPosition),
                HorizontalPosition + MoveAmount, EaseFunction.Linear, 1000));
            
        }

        private void Collider_OnCollide(GameObject obj)
        {
            if (obj is Jonny jonnny)
            {
                jonnny.TakeDamage(25, Transform.Position);
            }
        }

        AnimationRenderer CreateNarutaoAnimations()
        {
            var spriteSheet = Loader.LoadTexture("narutao");
            var animationFrames = Loader.LoadAsepriteFrames("narutao");

            return new AnimationRenderer(spriteSheet, animationFrames);
        }

        protected override void Update(GameTime gameTime)
        {
            keyboard.Update();
            Transform.MoveHorizontally((int)HorizontalPosition);
        }

        public void Run(Direction.Horizontal direction)
        {
            animatedSprite.Change("Running");
            Transform.MoveAndSlide(new Vector2(
                direction == Direction.Horizontal.Left ? -speed : speed, 0));
        }
    }
}
