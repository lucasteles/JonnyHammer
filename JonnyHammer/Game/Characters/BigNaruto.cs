﻿using JonnyHamer.Engine.Entities;
using JonnyHamer.Engine.Entities.Sprites;
using JonnyHamer.Engine.Helpers;
using JonnyHamer.Engine.Inputs;
using JonnyHammer.Engine;
using JonnyHammer.Engine.Entities.Components;
using JonnyHammer.Engine.Entities.Components.Collider;
using JonnyHammer.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;

namespace JonnyHammer.Game.Characters
{
    public class BigNaruto : Entity
    {
        float speed = 3f;
        bool couldBlink = false;

        KeyboardInput keyboard;

        MoveComponent move;
        private PlatformComponent platform;
        AnimatedSpriteComponent animations;

        public BigNaruto()
        {
            keyboard = new KeyboardInput();

            animations = AddComponent(CreateNarutaoAnimations());
            var collider = AddComponent(new ColliderComponent(new Rectangle(0, 0, animations.Width, animations.Height), true));
            move = AddComponent<MoveComponent>();
            platform = AddComponent<PlatformComponent>();

            collider.OnCollide += (e) => { Console.WriteLine($"colidiu com {e.Name} {DateTime.UtcNow.Millisecond}"); };

            StartCoroutine(ScaleNaruto());
            StartCoroutine(BlinkNaruto());
        }

        IEnumerator ScaleNaruto()
        {
            yield return TimeSpan.FromSeconds(3); // wait 3 seconds

            while (Scale < 3)
            {
                Scale += 0.01f;
                move.MoveAndSlide(0, -1);
                yield return null; // wait 1 frame
            }

            while (Scale > 1)
            {
                Scale -= 0.01f;
                yield return null;
            }

            couldBlink = true;
        }

        IEnumerator BlinkNaruto()
        {
            yield return new WaitUntil(() => couldBlink);

            for (var i = 0; i < 30; i++)
            {
                animations.IsVisible = !animations.IsVisible;
                yield return 5; // wait 5 frames
            }

            animations.IsVisible = true;
        }
        AnimatedSpriteComponent CreateNarutaoAnimations()
        {
            var spriteSheet = Loader.LoadTexture("narutao");
            var animationFrames = Loader.LoadAnimation("narutao");

            return new AnimatedSpriteComponent(spriteSheet, animationFrames);
        }

        public override void Update(GameTime gameTime)
        {
            keyboard.Update();

            if (keyboard.IsPressing(Keys.Space))
                platform.AddForce(new Vector2(0, 50));
            if (keyboard.IsPressing(Keys.Right))
                Run(Direction.Horizontal.Right);
            else if (keyboard.IsPressing(Keys.Left))
                Run(Direction.Horizontal.Left);
            else
                animations.Change("Idle");


            base.Update(gameTime);
        }

        public void Run(Direction.Horizontal direction)
        {
            animations.Change("Running");
            move.MoveAndSlide(new Vector2(
                direction == Direction.Horizontal.Left ? -speed : speed, 0));
        }


    }
}
