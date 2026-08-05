using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Project;
using NoteKeeper.Shell;
using PopUpUI.Toolkit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace NoteKeeper
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly ConcurrentBag<string> _pendingLoadedTabs;
        private ControlPanel panel;

        public Game1(string[] args)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _pendingLoadedTabs = [.. args];

            SingleInstanceChecker.ArgsReceived += LoadTabs;
        }

        private void LoadTabs(string[] obj)
        {
            foreach (var file in obj)
            {
                _pendingLoadedTabs.Add(file);
            }
        }

        protected override void Initialize()
        {
            PopUpUIManager.Initialize(this, _graphics);

            WindowOptions.Size = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                                           GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            WindowOptions.AllowUserResizing = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ContentAssistant.LoadWithContentManager<SpriteFont>(("ComicSans", "Font_ComicSans"));

            string shellAssetsPath = @"C:\Users\user\Desktop\C#\NoteKeeper\NoteKeeper\Assets";
            ContentAssistant.LoadTexture2Ds(
                Directory.EnumerateFiles(shellAssetsPath, "*.png", SearchOption.AllDirectories)
                .Where(path => Path.GetExtension(path) != ".pui")
                .Select(path => (path, path))
                .ToArray()
            );

            panel = new ControlPanel();
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            panel.ActiveTabs.Clear();
            base.OnExiting(sender, args);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            while (!_pendingLoadedTabs.IsEmpty)
            {
                LoadPendingFiles();
            }

            PopUpUIManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            PopUpUIManager.RenderUpdate(gameTime);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            PopUpUIManager.BufferDraw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

#nullable enable

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RaiseWindow(IntPtr window);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RestoreWindow(IntPtr window);

        private void LoadPendingFiles()
        {
            Tab? toDisplay = null;
            if (_pendingLoadedTabs.TryTake(out string? result))
            {
                Tab? sameSource = panel.ActiveTabs.Find(tab => tab.SourcePath == result);

                // same-sourced tab already exists, display it without adding it to active tabs
                if (sameSource != null)
                {
                    toDisplay = sameSource;
                }
                else
                {
                    Tab? created = TabRegistry.Create(result);
                    if (created != null)
                    {
                        panel.ActiveTabs.Add(created);
                        toDisplay = created;
                    }
                }
            }

            if (toDisplay != null)
            {
                panel.DisplayTab(toDisplay);

                SDL_RestoreWindow(this.Window.Handle);
                SDL_RaiseWindow(this.Window.Handle);
            }
        }
    }

#nullable disable

    public static class Util
    {
        public static Texture2D GetTexture(int w, int h, Color color)
        {
            Texture2D tex = new Texture2D(DrawHelper.Device, w, h);
            tex.SetData(Enumerable.Repeat(color, w * h).ToArray());

            return tex;
        }
    }
}
