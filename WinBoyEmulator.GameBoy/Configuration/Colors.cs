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

//using static WinBoyEmulator.GameBoy.Configuration.Color;

namespace WinBoyEmulator.GameBoy.Configuration
{
    internal static class Colors
    {
        /// <summary>
        /// Color Palette. Contains different colors GameBoy is using. <para />
        /// Note: 
        /// </summary>
        public static Color[] Palette = new Color[4]
        {
            Color.White,
            Color.Silver,
            Color.Gray,
            Color.Black
        };
    }
}
