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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4Any;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Windows;

using WinBoyEmulator.GameBoy;

using D3D11 = SharpDX.Direct3D11;

namespace WinBoyEmulator.SharpDX4GB
{
    public class Game : IDisposable
    {
        public const int FPS = 120;
        public const int Width = GameBoy.Configuration.Screen.Width;
        public const int Height = GameBoy.Configuration.Screen.Height;

        private LogWriter _logWriter;
        private Emulator _emulator;

        private RenderForm _renderForm;
        private Viewport _viewport;
        private ModeDescription _backBufferDescription;
        private SwapChain _swapChain;
        private SwapChainDescription _swapChainDescription;
        private D3D11.Device _d3dDevice;
        private D3D11.DeviceContext _d3dDeviceContext;
        private D3D11.RenderTargetView _renderTargetView;

        /// <summary>Path of the game.</summary>
        public string GamePath
        {
            get
            {
                return _emulator.GamePath;
            }
            set
            {
                _emulator.GamePath = value;
            }
        }

        public Game() : this("WinBoyEmulator") { }

        public Game(string text)
        {
            _emulator = new Emulator();

            _renderForm = new RenderForm(text);
            _renderForm.ClientSize = new Size(Width, Height);
            // You may need to modify rendering if you set this true.
            _renderForm.AllowUserResizing = false;

            _initializeDeviceResources();
        }

        private void _initializeDeviceResources()
        {
            _backBufferDescription = new ModeDescription(Width, Height, new Rational(FPS, 1), Format.R8G8B8A8_UNorm);
            _swapChainDescription = new SwapChainDescription
            {
                ModeDescription = _backBufferDescription,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                OutputHandle = _renderForm.Handle,
                IsWindowed = true
            };

            var hw = DriverType.Hardware;
            var flags = D3D11.DeviceCreationFlags.None;

            D3D11.Device.CreateWithSwapChain(hw, flags, _swapChainDescription, out _d3dDevice, out _swapChain);
            _d3dDeviceContext = _d3dDevice.ImmediateContext;

            using (D3D11.Texture2D backbuffer = _swapChain.GetBackBuffer<D3D11.Texture2D>(0))
            {
                _renderTargetView = new D3D11.RenderTargetView(_d3dDevice, backbuffer);
            }
        }

        private void _draw()
        {
            // In tutorial (http://www.johanfalk.eu/blog/sharpdx-beginners-tutorial-part-3-initializing-directx),
            // there is:
            //
            //      d3d11DeviceContext.OutputMerger.SetRenderTargets(renderTargetView);
            //      d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(32, 103, 178));
            //
            // Where you can see, that there is used different objects.
            // I don't know whether is that typo or supposed to be so.
            // I assume it's typo, so I just use d3dDeviceContext (_d3dDeviceContext).
            _d3dDeviceContext.OutputMerger.SetRenderTargets(_renderTargetView);
            _d3dDeviceContext.ClearRenderTargetView(_renderTargetView, new SharpDX.Color(255, 255, 255));
            _swapChain.Present(1, PresentFlags.None);


        }

        private void _renderCallBack() => _draw();

        /// <summary>Runs a game.</summary>
        public void Run()
        {
            if (string.IsNullOrEmpty(GamePath))
                throw new InvalidOperationException("Missing property GamePath");

            _emulator.StartEmulation();

            RenderLoop.Run(_renderForm, _renderCallBack);
        }

        /// <summary>
        /// Check if object is disposed. If it's not, then dispose the object. Else do nothing.
        /// </summary>
        /// <param name="disposableObject">object that inherit's SharpDX.DisposeBase</param>
        private void _disposeIfNotDisposed(DisposeBase disposableObject)
        {
            if( !disposableObject.IsDisposed )
            {
                disposableObject.Dispose();
            }
            else
            {

            }
        }

        /// <summary>Dispose objects.</summary>
        public void Dispose()
        {
            _emulator.StopEmulation();

            _disposeIfNotDisposed(_renderTargetView);
            _disposeIfNotDisposed(_swapChain);
            _disposeIfNotDisposed(_d3dDevice);
            _disposeIfNotDisposed(_d3dDeviceContext);

            if( !_renderForm.IsDisposed )
                _renderForm.Dispose();
        }
    }
}
