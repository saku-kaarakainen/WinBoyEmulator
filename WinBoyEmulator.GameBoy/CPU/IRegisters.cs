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

namespace WinBoyEmulator.GameBoy.CPU
{
    internal interface IRegisters
    {
        // 8-bit registers
        byte A { get; set; }
        byte B { get; set; }
        byte C { get; set; }
        byte D { get; set; }
        byte E { get; set; }
        byte F { get; set; }
        byte H { get; set; }
        byte L { get; set; }

        // 16-bit registers
        int AF { get; set; }
        int BC { get; set; }
        int DE { get; set; }
        int HL { get; set; }

        int SP { get; set; }
        int PC { get; set; }
    }
}
