using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Toolkit;
using System.Linq;
using NoteKeeper.Shell;
using NoteKeeper.InspectionWindow.PagedTabs.MultiPage;
using NoteKeeper.InspectionWindow.PagedTabs;
using System.Collections.Generic;
using System.IO;

namespace NoteKeeper
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {   
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            PopUpUIManager.Initialize(this, _graphics);

            WindowOptions.Size = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 
                                           GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            WindowOptions.AllowUserResizing = true;

            WindowOptions.Position = new Point(10);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ContentAssistant.LoadWithContentManager<SpriteFont>(("ComicSans", "Font_ComicSans"));

            string shellAssetsPath = @"C:\Users\user\Desktop\C#\NoteKeeper\NoteKeeper\Assets\Shell";
            ContentAssistant.LoadTexture2Ds(
                Directory.EnumerateFiles(shellAssetsPath)
                .Where(path => Path.GetExtension(path) != ".pui")
                .Select(path => (path, path))
                .ToArray()
            );

            SpriteFont font = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans");

            Texture2D red = Util.GetTexture(200, 200, Color.DarkRed);
            Texture2D green = Util.GetTexture(200, 200, Color.DarkGreen);

            int leftMargin = 20;
            int upMargin = 200;
            int verticalSize = 700;
            Point viewerSize = new Point(WindowOptions.Size.X - leftMargin * 2, verticalSize);

            MultiPageTab multi = new MultiPageTab(viewerSize, @"C:\Users\user\Desktop\Test.txt");
            multi.Window.Boundaries = new Rectangle(20, upMargin, viewerSize.X, viewerSize.Y);

            ControlPanel panel = new ControlPanel();
            panel.ActiveTabs.Add(multi);
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

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal static PageImageContent Location(this PageImageContent content, Point location)
        {
            content.SetLocation(location);
            return content;
        }
    }
}
