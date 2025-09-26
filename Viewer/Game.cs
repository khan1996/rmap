using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Viewer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Fields
        public List<object> ToDraw = new List<object>();
        private GraphicsDeviceManager _graphics;
        private Camera _camera;
        private int _lastMouseX, _lastMouseY;
        private bool _keyTest = false, _wire = false;
        private DateTime _last = DateTime.Now;
        private int _fps;
        private bool _leftButtonDown = false;
        private DateTime lastLeftMouseClick = DateTime.MinValue;
        public event Action<Ray> OnMouseClick;
        private object switchLock = new object();

        #endregion

        public void SelectReachProfile(bool reach)
        {
            _graphics.GraphicsProfile = reach ? GraphicsProfile.Reach : GraphicsProfile.HiDef;
            _graphics.ApplyChanges();
        }

        public void Show(IEnumerable<DrawableGameComponent> objs)
        {
            lock (switchLock)
            {
                Clear();

                foreach (var obj in objs)
                    if (obj != null)
                    {
                        Components.Add(obj);
                    }
            }
        }

        public void SyncObjects(IEnumerable<DrawableGameComponent> objs)
        {
            lock (switchLock)
            {
                foreach (var obj in objs.ToArray())
                {
                    if (obj != null)
                    {
                        if (!Components.Contains(obj))
                            Components.Add(obj);
                    }
                }

                foreach (DrawableGameComponent comp in Components.OfType<DrawableGameComponent>().ToArray())
                {
                    if (!(comp is IdentMesh) && !objs.Contains(comp))
                        Components.Remove(comp);
                }
            }
        }

        public void Clear()
        {
            lock (switchLock)
                ClearObjects();
        }

        public void SetCamera(BoundingBox box)
        {
            //float height = box.Max.Y - box.Min.Y;
            float depth = box.Max.X - box.Min.X;
            float depth2 = box.Min.Z * 1.2f;

            depth = depth / (float)Math.Tan(15f / 180f * Math.PI);
            if (depth2 < -depth)
                depth = -depth2;

            //float midY = box.Min.Y + height / 2;

            Vector3 look = new Vector3(0f, 0.35f, -0.9f); // ~25 degrees

            _camera = new Camera(new Vector3(0, box.Max.Y, -1 * depth), look);
            _camera.Update();
            Camera.DefaultCamera = _camera;
        }

        #region Constructors

        public Game()
        {
            this._graphics = new GraphicsDeviceManager(this);
            this.Window.AllowUserResizing = true;
            this.IsMouseVisible = true;
        }

        #endregion

        #region Game Life

        protected void ClearObjects()
        {
            Components.Clear();
            Components.Add(new IdentMesh(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.Window.Title = "rMap ModelViewer : 60 fps";

            this.IsFixedTimeStep = false;

            this.InitializeCamera();

            ClearObjects();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        private void InitializeCamera()
        {
            _camera = new Camera(new Vector3(0, 0, 0), Vector3.Forward);
            Camera.DefaultCamera = _camera;


            MouseState currentMouseState = Mouse.GetState();
            _lastMouseX = currentMouseState.X;
            _lastMouseY = currentMouseState.Y;
            _camera.Update();
        }

        #endregion

        #region Update and show


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            // TODO: Add your update logic here
            Camera.ElapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState currentKeyboardState = Keyboard.GetState();

            float walkSpeed = 5f;
            float crawlSpeed = 0.01f;
            float runSpeed = 100f;

            float speed = currentKeyboardState[Keys.LeftShift] == KeyState.Down ? runSpeed : (currentKeyboardState[Keys.LeftAlt] == KeyState.Down ? crawlSpeed : walkSpeed);

            if (currentKeyboardState[Keys.W] == KeyState.Down)
                _camera.Walk(-240 * speed);
            if (currentKeyboardState[Keys.S] == KeyState.Down)
                _camera.Walk(240 * speed);
            if (currentKeyboardState[Keys.A] == KeyState.Down)
                _camera.Strafe(-240 * speed);
            if (currentKeyboardState[Keys.D] == KeyState.Down)
                _camera.Strafe(240 * speed);
            if (currentKeyboardState[Keys.E] == KeyState.Down)
                _camera.Fly(240 * speed);
            if (currentKeyboardState[Keys.Q] == KeyState.Down)
                _camera.Land(240 * speed);

            if (currentKeyboardState[Keys.F] == KeyState.Down)
                _camera.Roll(10 * speed);
            if (currentKeyboardState[Keys.G] == KeyState.Down)
                _camera.Roll(-10 * speed);

            if (currentKeyboardState[Keys.Escape] == KeyState.Down)
                Exit();
            if (currentKeyboardState[Keys.R] == KeyState.Down)
            {
                if (!_keyTest)
                {
                    if (!_wire)
                    {
                        _graphics.GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
                        _wire = true;
                    }
                    else
                    {
                        _graphics.GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid };
                        _wire = false;
                    }
                }
                _keyTest = true;
            }
            else
            {
                _keyTest = false;
            }

            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState.X >= 0 && currentMouseState.X < Window.ClientBounds.Width &&
                currentMouseState.Y >= 0 && currentMouseState.Y < Window.ClientBounds.Height)
            {
                if (currentMouseState.RightButton == ButtonState.Pressed)
                {
                    if (currentMouseState.X >= 0 && currentMouseState.Y >= 0 && currentMouseState.X <= Window.ClientBounds.Width && currentMouseState.Y <= Window.ClientBounds.Height)
                        _camera.RotateByMouse(new Vector3(_lastMouseX - currentMouseState.X, currentMouseState.Y - _lastMouseY, 0));

                }
                if (currentMouseState.LeftButton == ButtonState.Pressed && !_leftButtonDown)
                {
                    _leftButtonDown = true;

                    if (lastLeftMouseClick >= DateTime.Now - new TimeSpan(0, 0, 1))
                    {
                        if (OnMouseClick != null)
                            OnMouseClick(GetMouseRay());
                        lastLeftMouseClick = DateTime.MinValue;
                    }
                    else
                        lastLeftMouseClick = DateTime.Now;
                }
                else if (currentMouseState.LeftButton == ButtonState.Released && _leftButtonDown)
                    _leftButtonDown = false;
            }

            _lastMouseX = currentMouseState.X;
            _lastMouseY = currentMouseState.Y;
            _camera.Update();
            /*
            System.Diagnostics.Debug.WriteLine(
                " Pos " + _camera.Position.ToString() + 
                " Look " + _camera.LookAt.ToString() +
                " Up " + _camera.UpVector.ToString() +
                " Right " + _camera.Right.ToString());*/

            if ((DateTime.Now - _last).TotalMilliseconds >= 1000)
            {
                this.Window.Title = "rMap ModelViewer : " + this._fps + " fps";
                _fps = 0;
                this._last = DateTime.Now;
            }
            else
                _fps++;
            
            base.Update(gameTime);
        }

        private Ray GetMouseRay()
        {
            MouseState currentMouseState = Mouse.GetState();
            Vector3 nearsource = new Vector3((float)currentMouseState.X, (float)currentMouseState.Y, 0f);
            Vector3 farsource = new Vector3((float)currentMouseState.X, (float)currentMouseState.Y, 1f);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, _camera.Projection, _camera.View, Matrix.Identity);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, _camera.Projection, _camera.View, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this._graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
 
            base.Draw(gameTime);
        }

        #endregion
    }
}
