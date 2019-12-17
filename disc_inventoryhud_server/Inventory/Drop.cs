﻿using CitizenFX.Core;
using disc_inventoryhud_common.Inv;
using disc_inventoryhud_common.Inventory;
using disc_inventoryhud_server.MySQL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace disc_inventoryhud_server.Inventory
{
    class Drop : BaseScript
    {
        public Dictionary<string, InventoryData> Drops { get; } = new Dictionary<string, InventoryData>();

        public static Drop Instance { get; private set; }

        public Drop()
        {
            Instance = this;
            LoadDrops();
        }

        public void LoadDrops()
        {
            var pars = new Dictionary<string, object>()
            {
                ["@type"] = "drop"
            };

            MySQLHandler.Instance?.FetchAll("SELECT * FROM disc_inventory WHERE type=@type", pars, new Action<List<dynamic>>((objs) =>
            {
                foreach (var obj in objs)
                {
                    var key = fromOwner(obj.owner);
                    Debug.WriteLine("DROP" + obj.data);
                    var data = JsonConvert.DeserializeObject<InventorySlot>(obj.data);
                    UpdateDrops(key, data, false);
                }
                Debug.WriteLine(JsonConvert.SerializeObject(Drops));
                TriggerClientEvent(Events.UpdateDrops, Drops);
            }));
        }

        private Vector3 fromOwner(string owner)
        {
            var x = float.Parse(Regex.Match(owner, @"x[-+]?[0-9]*\.?[0-9]*", RegexOptions.ECMAScript).Value.Substring(1));
            var y = float.Parse(Regex.Match(owner, @"y[-+]?[0-9]*\.?[0-9]*", RegexOptions.ECMAScript).Value.Substring(1));
            var z = float.Parse(Regex.Match(owner, @"z[-+]?[0-9]*\.?[0-9]*", RegexOptions.ECMAScript).Value.Substring(1));
            return new Vector3(x, y, z);
        }

        private string getOwnerFromCoords(Vector3 vector)
        {
            return 'x' + vector.X.ToString() + 'y' + vector.Y.ToString() + 'z' + vector.Z.ToString();
        }

        public void UpdateDrops(Vector3 vector, dynamic droppingData, bool sideEffect = true)
        {
            var key = getOwnerFromCoords(vector);
            InventorySlot newSlot = new InventorySlot
            {
                Name = droppingData.Name,
                Count = droppingData.Count
            };
            if (Drops.ContainsKey(key))
            {
                InventoryData data = Drops[key];
                var slotTo = data.Inventory.FirstOrDefault(value => value.Value.Name == droppingData.Name);
                if (slotTo.Value != null)
                {
                    InventorySlot currentSlot = data.Inventory[slotTo.Key];
                    currentSlot.Count += droppingData.Count;
                    if (sideEffect) Inventory.UpdateSlot(slotTo.Key, data, currentSlot);
                }
                else
                {
                    int newSlotNumber = data.Inventory.Count + 1;
                    data.Inventory.Add(newSlotNumber, newSlot);
                    if (sideEffect) Inventory.CreateSlot(newSlotNumber, data, newSlot);
                }
            }
            else
            {
                InventoryData newData = new InventoryData
                {
                    Owner = key,
                    Type = "drop",
                    Inventory = new Dictionary<int, InventorySlot>
                    {
                        [1] = newSlot
                    }

                };
                Drops[key] = newData;
                if (sideEffect) Inventory.CreateSlot(1, newData, newSlot);
            };
        }

        public InventoryData DeleteDrop(Vector3 vector, int slot)
        {
            var key = getOwnerFromCoords(vector);
            if (!Drops.ContainsKey(key))
            {
                throw new Exception("Drop does not Exists");
            }
            else
            {
                InventoryData data = Drops[key];
                data.Inventory.Remove(slot);
                return data;
            }
        }
    }
}
