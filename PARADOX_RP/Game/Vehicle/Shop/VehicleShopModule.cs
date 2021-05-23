﻿using AltV.Net.Async;
using AltV.Net.Data;
using EntityStreamer;
using Microsoft.EntityFrameworkCore;
using PARADOX_RP.Controllers.Event.Interface;
using PARADOX_RP.Controllers.Vehicle.Interface;
using PARADOX_RP.Core.Database;
using PARADOX_RP.Core.Database.Models;
using PARADOX_RP.Core.Extensions;
using PARADOX_RP.Core.Factories;
using PARADOX_RP.Core.Module;
using PARADOX_RP.UI;
using PARADOX_RP.UI.Windows.CarShop;
using PARADOX_RP.Utils;
using PARADOX_RP.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PARADOX_RP.Game.Vehicle.Shop
{
    class VehicleShopModule : ModuleBase<VehicleShopModule>
    {
        private readonly IEventController _eventController;
        private readonly IVehicleController _vehicleController;
        private Dictionary<int, VehicleShops> _vehicleShops = new Dictionary<int, VehicleShops>();

        public VehicleShopModule(PXContext pxContext, IEventController eventController, IVehicleController vehicleController) : base("VehicleShop")
        {
            _eventController = eventController;
            _vehicleController = vehicleController;

            LoadDatabaseTable(pxContext.VehicleShops.Include(v => v.Content).ThenInclude(v => v.VehicleClass), (VehicleShops vehShop) =>
            {
                _vehicleShops.Add(vehShop.Id, vehShop);
            });
        }

        public override void OnPlayerConnect(PXPlayer player)
        {
            _vehicleShops.ForEach((v) =>
            {
                player.AddBlips(v.Value.Name, v.Value.Position, 225, 0, 1, true);
                MarkerStreamer.Create(MarkerTypes.MarkerTypeUpsideDownCone, v.Value.Position, new Vector3(1, 1, 1), new Vector3(0, 0, 0), null, new Rgba(37, 165, 202, 125));
                MarkerStreamer.Create(MarkerTypes.MarkerTypeHorizontalCircleFat, Vector3.Subtract(v.Value.BoughtPosition, new Vector3(0, 0, 0.5f)), new Vector3(1, 1, 1), new Vector3(0, 0, 0), null, new Rgba(37, 165, 202, 125));

                if (Configuration.Instance.DevMode)
                    v.Value.Content.ForEach((preview) => AltAsync.CreateVehicleBuilder(preview.VehicleClass.VehicleModel, preview.PreviewPosition, preview.PreviewRotation).PrimaryColor(0).SecondaryColor(0).Build());
            });
        }

        public async override Task<bool> OnKeyPress(PXPlayer player, KeyEnumeration key)
        {
            if (key != KeyEnumeration.E) return await Task.FromResult(false);
            if (!player.IsValid()) return await Task.FromResult(false);
            if (!player.CanInteract()) return await Task.FromResult(false);

            VehicleShops vehicleShop = _vehicleShops.Values.FirstOrDefault(g => g.Position.Distance(player.Position) < 3);
            if (vehicleShop == null) return await Task.FromResult(false);

            WindowManager.Instance.Get<VehicleShopWindow>().Show(player, new VehicleShopWindowWriter(vehicleShop.Id, vehicleShop.Content.ToList()));

            return await Task.FromResult(true);
        }

        public async void BuyVehicleShop(PXPlayer player, int shopId, string vehicleName)
        {
            if (!player.IsValid()) return;
            if (!player.CanInteract()) return;

            if (!WindowManager.Instance.Get<VehicleShopWindow>().IsVisible(player))
            {
                /*
                 * ADD LOGGER
                 */
                return;
            }

            if (!_vehicleShops.TryGetValue(shopId, out VehicleShops dbVehicleShop))
            {
                /*
                 * ADD LOGGER
                 */
                return;
            }

            VehicleShopsContent vehicleContent = dbVehicleShop.Content.FirstOrDefault(v => v.VehicleClass.VehicleModel == vehicleName);
            if (vehicleContent == null)
            {
                /*
                 * ADD LOGGER
                 */
                return;
            }

            if (Pools.Instance.Get<PXVehicle>(PoolType.VEHICLE).FirstOrDefault(v => v.Position.Distance(dbVehicleShop.BoughtPosition) < 3) != null)
            {
                player.SendNotification("Fahrzeughandel", "Derzeit ist der Ausparkpunkt belegt.", NotificationTypes.SUCCESS);
                return;
            }

            if (await player.TakeMoney(vehicleContent.Price))
            {
                player.SendNotification("Fahrzeughandel", $"Fahrzeug {vehicleContent.VehicleClass.VehicleModel} für {vehicleContent.Price}$ gekauft.", NotificationTypes.SUCCESS);


            }
        }
    }
}