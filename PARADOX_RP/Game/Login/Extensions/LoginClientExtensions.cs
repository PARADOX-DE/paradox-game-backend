﻿using AltV.Net.Async;
using AltV.Net.Data;
using PARADOX_RP.Core.Factories;
using PARADOX_RP.UI;
using PARADOX_RP.UI.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PARADOX_RP.Game.Login.Extensions
{
    static class LoginClientExtensions
    {
        public static async Task PreparePlayer(this PXPlayer client, Position pos) {
            await client.SpawnAsync(pos);
            client.SendNotification("PARADOX RP", "PreparePlayer", NotificationTypes.SUCCESS);

            WindowManager.Instance.Get<HUDWindow>().Show(client);
        }
    }
}
