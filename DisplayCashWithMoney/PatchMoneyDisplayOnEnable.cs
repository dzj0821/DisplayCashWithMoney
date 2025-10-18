using System;
using System.Collections.Generic;
using System.Reflection;
using Duckov.Economy;
using Duckov.UI;
using HarmonyLib;
using ItemStatsSystem;
using TMPro;

namespace DisplayCashWithMoney
{
    [HarmonyPatch(typeof(MoneyDisplay), "OnEnable")]
    public class PatchMoneyDisplayOnEnable
    {
        public static Dictionary<MoneyDisplay, Action<Item>> onSetStackCountCallbacks = new Dictionary<MoneyDisplay, Action<Item>>();
        public static Dictionary<MoneyDisplay, Action<Inventory, int>> onContentChangedCallbacks = new Dictionary<MoneyDisplay, Action<Inventory, int>>();

        public static void Unregister(MoneyDisplay __instance)
        {
            if (onSetStackCountCallbacks.TryGetValue(__instance, out var onSetStackCount))
            {
                var cashItemList = ItemUtilities.FindAllBelongsToPlayer((Item e) => e != null && e.TypeID == ModBehaviour.CASH_ITEM_TYPE_ID);
                foreach (var cashItem in cashItemList)
                {
                    cashItem.onSetStackCount -= onSetStackCount;
                }
                onSetStackCountCallbacks.Remove(__instance);
            }

            if (onContentChangedCallbacks.TryGetValue(__instance, out var onContentChanged))
            {
                Inventory playerStorageInventory = PlayerStorage.Inventory;
                if (playerStorageInventory != null)
                {
                    playerStorageInventory.onContentChanged -= onContentChanged;
                }
                Inventory characterInventory =  LevelManager.Instance?.MainCharacter?.CharacterItem?.Inventory;
                if (characterInventory != null)
                {
                    characterInventory.onContentChanged -= onContentChanged;
                }
                Inventory inventory = LevelManager.Instance?.PetProxy?.Inventory;
                if (inventory != null)
                {
                    inventory.onContentChanged -= onContentChanged;
                }
                onContentChangedCallbacks.Remove(__instance);
            }
        }
        public static void Prefix(MoneyDisplay __instance)
        {
            Unregister(__instance);
            
            var text = (TextMeshProUGUI) typeof(MoneyDisplay).GetField("text", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var cashTextTransform = text.transform.parent.Find("CashText");
            if (cashTextTransform == null)
            {
                return;
            }
            var cashText = cashTextTransform.GetComponent<TextMeshProUGUI>();


            void Refresh()
            {
                if (!cashText.gameObject.activeInHierarchy)
                {
                    return;
                }
                cashText.text = EconomyManager.Cash.ToString("n0");
            }

            Action<Item> onSetStackCount = item => {
                if (item != null && item.TypeID == ModBehaviour.CASH_ITEM_TYPE_ID)
                {
                    Refresh();
                }
            };
            Action<Inventory, int> onContentChanged = (inventory, index) => {
                if (index < inventory.Content.Count && inventory.Content[index] != null)
                {
                    if (inventory.Content[index].TypeID == ModBehaviour.CASH_ITEM_TYPE_ID)
                    {
                        Refresh();
                        inventory.Content[index].onSetStackCount += onSetStackCount;
                    }
                    
                }
                else
                {
                    Refresh();
                }
            };

            // 搜索玩家所有现金物品，加上堆叠数量变化的回调
            var cashItemList = ItemUtilities.FindAllBelongsToPlayer((Item e) => e != null && e.TypeID == ModBehaviour.CASH_ITEM_TYPE_ID);
            foreach (var cashItem in cashItemList)
            {
                cashItem.onSetStackCount += onSetStackCount;
            }
            onSetStackCountCallbacks.Add(__instance, onSetStackCount);

            // 玩家仓库、角色背包、宠物背包增加物品变化的回调
            Inventory playerStorageInventory = PlayerStorage.Inventory;
            if (playerStorageInventory != null)
            {
                playerStorageInventory.onContentChanged += onContentChanged;
            }
            Inventory characterInventory =  LevelManager.Instance?.MainCharacter?.CharacterItem?.Inventory;
            if (characterInventory != null)
            {
                characterInventory.onContentChanged += onContentChanged;
            }
            Inventory inventory = LevelManager.Instance?.PetProxy?.Inventory;
            if (inventory != null)
            {
                inventory.onContentChanged += onContentChanged;
            }
            onContentChangedCallbacks.Add(__instance, onContentChanged);

            Refresh();

        }
    }
}