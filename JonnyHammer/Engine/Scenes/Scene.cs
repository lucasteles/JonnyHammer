﻿using JonnyHamer.Engine.Entities;
using JonnyHammer.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JonnyHammer.Engine.Scenes
{
    public abstract class Scene
    {
        IList<Entity> entities = new List<Entity>();
        public IReadOnlyList<Entity> Entities => entities.ToArray();
        public void Destroy(Entity entity)
        {
            if (entity is IDisposable d)
                d.Dispose();

            entities.Remove(entity);
        }

        public T Spawn<T>(string name = "no name", Vector2? position = null) where T : Entity, new()
        {
            var entity = new T { Position = position ?? Vector2.Zero, FacingDirection = Direction.Horizontal.Right, Name = name };
            entities.Add(entity);
            return entity;
        }

        public virtual void Update(GameTime gameTime)
        {
            for (int i = 0; i < entities.Count; i++)
                entities[i].Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < entities.Count; i++)
                entities[i].Draw(spriteBatch);
        }
    }
}