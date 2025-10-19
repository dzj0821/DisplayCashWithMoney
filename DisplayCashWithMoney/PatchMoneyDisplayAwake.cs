using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Duckov.Economy;
using Duckov.UI;
using HarmonyLib;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DisplayCashWithMoney
{
    [HarmonyPatch(typeof(MoneyDisplay), "Awake")]
    public class PatchMoneyDisplayAwake
    {
        public static void Prefix(MoneyDisplay __instance)
        {
            var text = (TextMeshProUGUI) typeof(MoneyDisplay).GetField("text", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            var index = text.transform.GetSiblingIndex();

            var item = ItemAssetsCollection.GetPrefab(ModBehaviour.CASH_ITEM_TYPE_ID);

            void CreateSpace()
            {
                GameObject space = new GameObject("Space");
                space.AddComponent<LayoutElement>().preferredWidth = 10;
                space.transform.SetParent(text.transform.parent);
                space.transform.SetSiblingIndex(index + 1);
            }

            // 倒序插入
            var cashText = UnityEngine.Object.Instantiate(text, text.transform.parent);
            cashText.transform.SetSiblingIndex(index + 1);
            cashText.gameObject.name = "CashText";

            CreateSpace();

            GameObject obj = new GameObject("CashIcon");
            obj.transform.SetParent(text.transform.parent);
            obj.transform.SetSiblingIndex(index + 1);
            Image image = obj.AddComponent<Image>();
            image.sprite = item.Icon;
            LayoutElement layoutElement = obj.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 50;
            layoutElement.preferredHeight = 50;

            CreateSpace();

            
        }
    }
}