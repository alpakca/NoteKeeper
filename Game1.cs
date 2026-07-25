using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Toolkit;
using System.Linq;
using NoteKeeper.Shell;
using NoteKeeper.InspectionWindow.PagedTabs.MultiPage;
using NoteKeeper.InspectionWindow.PagedTabs;
using System.Collections.Generic;

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

            string absolutePathStart = @"C:\Users\user\Desktop\C#\NoteKeeper\NoteKeeper\";
            ContentAssistant.LoadTexture2Ds((absolutePathStart + @"Assets\Shell\TransformMode.png", "Button_TransformMode"),
                                            (absolutePathStart + @"Assets\Shell\TransformModeSelected.png", "Button_TransformMode_Selected"),
                                            (absolutePathStart + @"Assets\Shell\WritingMode.png", "Button_WritingMode"),
                                            (absolutePathStart + @"Assets\Shell\WritingModeSelected.png", "Button_WritingMode_Selected"),
                                            (absolutePathStart + @"Assets\Shell\AddTextBlock.png", "Button_AddTextBlock"),
                                            (absolutePathStart + @"Assets\Shell\AddTextBlockHovered.png", "Button_AddTextBlock_Hovered"),
                                            (absolutePathStart + @"Assets\Shell\AddTextBlockHeld.png", "Button_AddTextBlock_Held"));


            SpriteFont font = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans");

            Texture2D red = Util.GetTexture(200, 200, Color.DarkRed);
            Texture2D green = Util.GetTexture(200, 200, Color.DarkGreen);

            NotePage page = new NotePage();
            page.Contents.AddRange([
               new PageImageContent(green, new Point(50, 50))
            {
                Location = new Point(250, 250)
            },new PageImageContent(green, new Point(200, 30)) {
                Location = new Point(50, 50)
            }]);

            NotePage page2 = new NotePage();
            page.Contents.AddRange([
                new PageImageContent(red, new Point(200, 200))
            {
                Location = new Point(50, 500)
            },new PageImageContent(green, new Point(200, 200)) {
                Location = new Point(50, 50)
            }]);

            int leftMargin = 20;
            int upMargin = 200;
            int verticalSize = 700;
            Point viewerSize = new Point(WindowOptions.Size.X - leftMargin * 2, verticalSize);

            MultiPageTab multi = new MultiPageTab(viewerSize);
            multi.Window.Boundaries = new Rectangle(20, upMargin, viewerSize.X, viewerSize.Y);

            NotePage first = new NotePage();
            first.Contents.AddRange(
                new PageImageContent(green, new Point(50, 50))
                {
                    Location = new Point(50, 50),
                },
                new PageImageContent(red, new Point(50, 50))
                {
                    Location = new Point(100, 100)
                }
            );

            NotePage second = new NotePage();
            second.Contents.Add(
                new PageImageContent(Util.GetTexture(1, 1, Color.Pink), new Point(50, 50))
                {
                    Location = new Point(70, 70)
                }
            );

            multi.NotePages.Add(first);
            multi.NotePages.Add(second);
            multi.NotePages.Add(new NotePage());

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

    public class Util
    {
        public static Texture2D GetTexture(int w, int h, Color color)
        {
            Texture2D tex = new Texture2D(DrawHelper.Device, w, h);
            tex.SetData(Enumerable.Repeat(color, w * h).ToArray());

            return tex;
        }
    }
}
