using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Project;
using NoteKeeper.Shell;
using PopUpUI.Toolkit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace NoteKeeper
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly ProjectLoader _projectLoader;
        private ControlPanel? panel;

        public Game1(string[] args)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _projectLoader = new ProjectLoader(args);
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

            if (_projectLoader.State != LoadedState.CreatorHub)
            {
                panel = new ControlPanel();
                Thread loadProject = new Thread(() =>
                {
                    List<Tab> loadedTabs = _projectLoader.Load();
                    foreach (var tab in loadedTabs) panel.ActiveTabs.Add(tab);
                });

                loadProject.IsBackground = true;
                loadProject.Start();
            }
            else
            {
                // load project-creation hub
            }
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            if (_projectLoader.State != LoadedState.CreatorHub)
            {
                _projectLoader.Save(panel.ActiveTabs);
            }

            base.OnExiting(sender, args);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
    }

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
