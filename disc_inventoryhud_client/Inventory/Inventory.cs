﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using disc_inventoryhud_client.Util;
using Newtonsoft.Json;
using disc_inventoryhud_common.Inventory;

namespace disc_inventoryhud_client.Inventory
{
    class Inventory : BaseScript
    {
        public Inventory()
        {
            Init();
            InitEventHandlers();
        }

        private void Init()
        {
            API.RegisterCommand("closeUI", new Action<int, List<object>, string>((src, args, raw) =>
            {
                Close();
            }), false);
            NUI.Instance.RegisterNUICallback("CloseUI", (data) =>
            {
                Close();
            });
            NUI.Instance.RegisterNUICallback(Callbacks.MOVE_ITEM, (data) =>
            {
                TriggerServerEvent(Events.MoveItem, data);
            });
            NUI.Instance.RegisterNUICallback(Callbacks.DROP_ITEM, (data) =>
            {
                var player = API.PlayerPedId();
                var nonRounded = API.GetEntityCoords(player, true);
                var playerCoords = new Vector3((float)Math.Round(nonRounded.X, 1), (float)Math.Round(nonRounded.Y, 1), (float)Math.Round(nonRounded.Z, 1));
                data.Add("coords", playerCoords);
                TriggerServerEvent(Events.DropItem, data);
            });
        }

        public void InitEventHandlers()
        {
            EventHandlers["onResourceStop"] += new Action<string>(onResourceStop);
            EventHandlers[Events.UpdateInventory] += new Action<dynamic>((inv) => {
                Debug.WriteLine("Loading Inventory");
                API.SendNuiMessage(Actions.SET_INVENTORY(inv));
            });
        }

        private void onResourceStop(string ResourceName)
        {
            if (ResourceName == API.GetCurrentResourceName())
            {
                Close();
            }
        }


        [Tick]
        private async Task HandleOpenInventory()
        {
            await Delay(0);
            if (API.IsControlJustReleased(0, 289))
            {
                Open();
            }
        }

        private void Open()
        {
            API.SendNuiMessage(Actions.APP_SHOW);
            API.SetNuiFocus(true, true);
        }

        private void Close()
        {
            API.SetNuiFocus(false, false);
        }
    }
}
