﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PARADOX_RP.Utils.Interface
{
    interface ILogger
    {
        void Console(ConsoleLogType type, string Category, string Log);
    }
}