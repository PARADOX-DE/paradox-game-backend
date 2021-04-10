﻿using AltV.Net.Async;
using AltV.Net.Data;
using PARADOX_RP.Core.Factories;
using PARADOX_RP.Core.Module;
using PARADOX_RP.Game.Arrival.Extensions;
using PARADOX_RP.Game.Login;
using PARADOX_RP.Game.Login.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PARADOX_RP.Game.Arrival
{
    class ArrivalModule : ModuleBase<ArrivalModule>
    {

        public ArrivalModule() : base("Arrival")
        {

        }

        private Position ArrivalPosition = new Position(0, 0, 72);

        public async Task NewPlayerArrival(PXPlayer player)
        {
            if (player.IsPlayerArrived()) return;
            await player.PlayArrivalCutscene();
            player.SendNotification("PARADOX RP", $"Du bist hiermit offiziell ein Bürger im Staate Los Santos.", NotificationTypes.SUCCESS);

            //todo: cutscene length
            await Task.Delay(25 * 1000);
            await player?.PreparePlayer(ArrivalPosition);
        }
    }
}
