﻿// This file is part of WinBoyEmulator.
// 
// WinBoyEmulator is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     WinBoyEmulator is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with WinBoyEmulator.  If not, see<http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using Format = SharpDX.DXGI.Format;
using WinBoyEmulator.Rendering.Utils;

namespace WinBoyEmulator.Rendering
{
    /// <summary>Class Renderer. SharpDX class for showing emulator on a form. Disposable.</summary>
    public class Renderer : IDisposable
    {
        private GameBoy.Emulator _gameBoy;
        private byte[] _buffer;
        private Form _form;
        private Factory _factory;
        private WindowRenderTarget _windowRenderTarget;
        private RawRectangleF _drawRectangle;
        private Bitmap _bitmap;
        private bool _isFormClosed;

        static Renderer()
        {
            Configuration.Instance.Title = "WinBoyEmulator";

            // Actually I think is Frames per seconds is just an initial values
            // and the Game's fps could be anything. 
            // Mayber that 60 is minimum fps?
            Configuration.Instance.FPS = 60;
            Configuration.Instance.Width = GameBoy.Emulator.Width;
            Configuration.Instance.Height = GameBoy.Emulator.Height;
            Configuration.Instance.ColorPalette = GameBoy.Emulator.ColorPalette;
        }

        /// <summary>Constructor of a disposable class Renderer</summary>
        public Renderer()
        {
            // TODO: _gameBoy of this class (and project)
            _gameBoy = new GameBoy.Emulator();

            // Check issues #30 and #31
            // Also WinBoyEmulator/MainForm.cs/MainForm_Load - method.
            // There is comments about this.
            _gameBoy.GamePath = "C:\\temp\\game.gb";
            _gameBoy.LoadGameToMemory();

            _factory = new Factory(FactoryType.SingleThreaded, DebugLevel.Information);

            _buffer = new byte[Configuration.Instance.Width
                * Configuration.Instance.Height
                * Configuration.Instance.ColorPalette.Length];
        }

        private Size2 NewClientSize => new Size2(_form.ClientSize.Width, _form.ClientSize.Height);

        private Size2 NewConfigurationSize => new Size2(Configuration.Instance.Width, Configuration.Instance.Height);

        private PixelFormat NewPixelFormat => new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore);

        private void CreateRenderTargets()
        {
            _windowRenderTarget = new WindowRenderTarget(_factory, new RenderTargetProperties {
                PixelFormat = NewPixelFormat,
                Type = RenderTargetType.Default,
                MinLevel = FeatureLevel.Level_DEFAULT
            }, new HwndRenderTargetProperties { Hwnd = _form.Handle, PixelSize = NewClientSize, PresentOptions = PresentOptions.Immediately})
            {
                DotsPerInch = new Size2F(96.0f, 96.0f),
                AntialiasMode = AntialiasMode.Aliased,
            };

            // CreateRenderTargets
            var _screenRenderTarget = new BitmapRenderTarget(_windowRenderTarget, CompatibleRenderTargetOptions.None, 
                new Size2F(Configuration.Instance.Width, Configuration.Instance.Height), new Size2(Configuration.Instance.Width, Configuration.Instance.Height), null);
            // RecalculateDrawRectangle
            _drawRectangle = new RawRectangleF(0, 0, _form.ClientSize.Width, _form.ClientSize.Height);
        }

        private void CreateBitmap()
        {
            _bitmap = new Bitmap(_windowRenderTarget, NewConfigurationSize, new BitmapProperties { PixelFormat = NewPixelFormat });
            _bitmap.CopyFromMemory(_buffer);
        }

        private void SizeChanged(object sender, EventArgs e)
        {
            // If you uncomment next line, screen won't be sctretched if window size changes.
            // _windowRenderTarget?.Resize(NewClientSize);
        }

        // TODO: Separate all game boy stuff from here.
        // (Put them preferably to WinBoyEmulator.Program.cs)
        public void Run(Form targetForm)
        {
            // Before run
            _form = targetForm;
            _form.SizeChanged += SizeChanged;

            CreateRenderTargets();
            CreateBitmap();

            // Run
            RenderLoop.Run(targetForm, () =>
            {
                if (_isFormClosed)
                    return;

                // emulate one cycle of gameboy
                _gameBoy.EmulateCycle();

                _buffer = _gameBoy.Screen.Data;

                // Copy gameboy screen's data to bitmap
                _bitmap.CopyFromMemory(_gameBoy.Screen.Data);

                // TODO:
                // Check whether we need to close form or not.

                // Draw bitmap
                _windowRenderTarget.BeginDraw();
                _windowRenderTarget.DrawBitmap(_bitmap, _drawRectangle, 1.0f, BitmapInterpolationMode.Linear);
                _windowRenderTarget.EndDraw();
            });

            // After run
            _form.SizeChanged -= SizeChanged;
            #region Dispose();
            // Dispose is not necessary needed here, since
            // this class uses interface IDisposable (user can dispose when (s)he wants).
            // Dispose();
            #endregion
        }

        /// <summary>Dispose objects used by Renderer.</summary>
        public void Dispose()
        {
            _bitmap?.Dispose();
            _windowRenderTarget?.Dispose();
        }

    }
}
