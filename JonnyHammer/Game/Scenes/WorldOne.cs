﻿using JonnyHamer.Engine.Inputs;
using JonnyHammer.Engine;
using JonnyHammer.Engine.Helpers;
using JonnyHammer.Engine.Scenes;
using JonnyHammer.Game.Characters;
using JonnyHammer.Game.Environment;
using JonnyHammer.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace JonnyHammer.Game.Scenes
{
    public class WorldOne : Scene
    {
        private Jonny player;
        private KeyboardInput keyboard = new KeyboardInput();

        public WorldOne()
        {
            var map = TiledLoader.Load("world_one");
            SpawnTiledLayers(map.Layers);
            SpawnTiledObjects(map.Objects);
        }

        void SpawnTiledLayers(Dictionary<string, TileLayer[]> layers)
        {
            foreach (var (layer, tiles) in layers)
                foreach (var (index, tile) in tiles.AsIndexed())
                    switch (tile.Name)
                    {
                        case "bg":
                            Spawn<MainBackground>($"{layer}_bg_{index}", tile.Position,
                                s => s.TextureName = tile.TextureName);
                            break;

                        case "cloud":
                            SpawnClouds(layer, tile);
                            break;

                        default:
                            Spawn<Scenery>($"{layer}_sc_{index}", tile.Position,
                                s =>
                                {
                                    s.TextureName = tile.TextureName;
                                    s.Source = tile.Source;
                                    s.Width = tile.Width;
                                    s.Height = tile.Height;
                                });
                            break;

                    }
        }

        void SpawnClouds(string layer, TileLayer tile)
        {
            var multiplier = tile.Amount - 1;
            var cloudRespawn = new Vector2((tile.Width * multiplier) - multiplier, tile.Position.Y);
            Cloud lastCloud = null;

            for (var i = 0; i < tile.Amount; i++)
            {

                lastCloud = Spawn<Cloud>($"{layer}_cloud_{i}",
                    (tile.Position + new Vector2((i * tile.Width) - 1, 0)),
                    c =>
                    {
                        c.Speed = tile.Speed;
                        c.Cloudrespawn = cloudRespawn;
                    });
            }
        }

        private void SpawnTiledObjects(Dictionary<string, TiledObject[]> objects)
        {
            foreach (var (layer, tiles) in objects)
                foreach (var (index, tile) in tiles.AsIndexed())
                    switch (layer)
                    {
                        case "one_way_blocks":
                        case "blocks":
                            Spawn<Block>(
                               $"floor_{layer}_{index}",
                               new Vector2(tile.Position.X, tile.Position.Y + tile.Height),
                               f =>
                               {
                                   f.Width = tile.Width;
                                   f.Height = tile.Height;
                               });
                            break;

                        case "player_spawn":
                            player = Spawn<Jonny>(
                                "Jonny",
                                tile.Position,
                                j => j.RespawnPosition = tile.Position);
                            break;

                        case "big_narutos":
                            Spawn<BigNaruto>("NarutoRed", tile.Position,
                                bg => bg.MoveAmount = tile.MoveAmount);
                            break;
                    }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            keyboard.Update();

            if (keyboard.IsPressing(Keys.LeftControl))
            {
                if (keyboard.IsPressing(Keys.Left))
                    Camera2D.Move(new Vector2(-5, 0));
                else if (keyboard.IsPressing(Keys.Right))
                    Camera2D.Move(new Vector2(5, 0));

                if (keyboard.IsPressing(Keys.Up))
                    Camera2D.Move(new Vector2(0, -5));
                else if (keyboard.IsPressing(Keys.Down))
                    Camera2D.Move(new Vector2(0, 5));

                return;
            }
            else if (keyboard.HasPressed(Keys.F11))
            {
                Screen.ToggleFullScreen();
                return;
            }
            else if (keyboard.IsPressing(Keys.OemPlus))
                Camera2D.ZoomUp(.02f);
            else if (keyboard.IsPressing(Keys.OemMinus))
                Camera2D.ZoomDown(.02f);

            if (keyboard.IsPressing(Keys.F12))
            {
                Camera2D.Center();
            }
            else
                Camera2D.Follow(player);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera2D.GetViewTransformationMatrix());
            base.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}