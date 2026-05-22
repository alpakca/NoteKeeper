using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Toolkit;
using NoteKeeper.InspectionWindow;
using NoteKeeper.InspectionWindow.SinglePage;
using System.Linq;
using System.Diagnostics;
using PopUpUI.Elements;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteFont font = Content.Load<SpriteFont>("ComicSans");

            Texture2D red = GetTexture(200, 200, Color.DarkRed);
            Texture2D green = GetTexture(200, 200, Color.DarkGreen);

            NotePage page = new NotePage();
            page.Contents.AddRange([new PageImageContent(red) 
            {
                Size = new Point(200, 200), 
                Location = new Point(50, 500)
            }, new PageImageContent(green) 
            {
                Size = new Point(50, 50),
                Location = new Point(250, 250)
            }, new PageTextContent(font) {
                Size = new Point(200, 200),
                Location = new Point(50, 50)
            }]);

            int leftMargin = 20;
            int upMargin = 200;
            int verticalSize = 700;
            Point viewerSize = new Point(WindowOptions.Size.X - leftMargin * 2, verticalSize);

            SinglePageTab tab = new SinglePageTab(page, viewerSize);
            tab.Window.Boundaries = new Rectangle(20, upMargin, viewerSize.X, viewerSize.Y);
            PopWindowManager.ActiveWindows.Add(tab.Window);
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

        private Texture2D GetTexture(int w, int h, Color color)
        {
            Texture2D tex = new Texture2D(GraphicsDevice, w, h);
            tex.SetData(Enumerable.Repeat(color, w * h).ToArray());

            return tex;
        }
    }
}
