﻿using PARADOX_RP.Core.Module;
using PARADOX_RP.Core.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PARADOX_RP.Utils.Enums;

namespace PARADOX_RP.Game.Administration
{
    class AdministrationModule : ModuleBase<AdministrationModule>
    {
        public AdministrationModule() : base("Administration") { }

        public override Task<bool> OnKeyPress(PXPlayer player, KeyEnumeration key)
        {
            if (key == KeyEnumeration.F9)
            {
                if (player.DutyType != DutyTypes.ADMINDUTY) EnterAduty(player);
                else LeaveAduty(player);

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public void EnterAduty(PXPlayer player)
        {
            if (PermissionsModule.Instance.HasPermissions(player))
            {
                player.DutyType = DutyTypes.ADMINDUTY;
            }
        }

        public void LeaveAduty(PXPlayer player)
        {
            if (PermissionsModule.Instance.HasPermissions(player))
            {
                if (player.DutyType != DutyTypes.ADMINDUTY) return;

                player.DutyType = DutyTypes.OFFDUTY;
            }
        }
    }
}
