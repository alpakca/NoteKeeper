using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Project;
using NoteKeeper.Shell;
using NoteKeeper.Shell.Styling;
using PopUpUI.Toolkit;
using System;
using System.Collections.Concurrent;
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

            WindowOptions.AllowUserResizing = true;

            PreferenceLoader.Load();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ContentAssistant.LoadWithContentManager<SpriteFont>(("ComicSans_16", "Font_ComicSans_12"));
            ContentAssistant.LoadWithContentManager<SpriteFont>(("ComicSans_16", "Font_ComicSans_16"));
            ContentAssistant.LoadWithContentManager<SpriteFont>(("ComicSans_24", "Font_ComicSans_24"));
            ContentAssistant.LoadWithContentManager<SpriteFont>(("ComicSans_Bold_16", "Font_ComicSans_Bold_12"));
            ContentAssistant.LoadWithContentManager<SpriteFont>(("ComicSans_Bold_16", "Font_ComicSans_Bold_16"));
            ContentAssistant.LoadWithContentManager<SpriteFont>(("ComicSans_Bold_24", "Font_ComicSans_Bold_24"));

            ContentAssistant.LoadTexture2Ds(
                Directory.EnumerateFiles(ThemeService.AssetsFolderRelativePath, "*.png", SearchOption.AllDirectories)
                .Where(path => Path.GetExtension(path) != ".pui")
                .Select(path => (path, path))
                .ToArray()
            );

            panel = new ControlPanel();
            panel.ExitApp += Exit;

            panel.RearrangeUISizes(this.Window.ClientBounds);
            this.Window.ClientSizeChanged += (sender, _) => panel.RearrangeUISizes(((GameWindow)sender).ClientBounds);

            NoteKeeper.Shell.ChildWindow.CreateTabFileWindow.Show();
        }

        protected override async void OnExiting(object sender, ExitingEventArgs args)
        {
            if (!panel.TryExitApp())
            {
                args.Cancel = true;
                return;
            }

            PreferenceLoader.Save(GraphicsDevice.Viewport);
            base.OnExiting(sender, args);
        }

        protected override void Update(GameTime gameTime)
        {
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
            DrawCallRenderer.DrawUpdate();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ThemeService.CurrentTheme.GetColor(ColorRegistry.BackgroundId));

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
                    Tab? created = TabService.Create(result);
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
            }

            // always show up the window even if file couldn't open, to show the message window
            SDL_RestoreWindow(this.Window.Handle);
            SDL_RaiseWindow(this.Window.Handle);
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
