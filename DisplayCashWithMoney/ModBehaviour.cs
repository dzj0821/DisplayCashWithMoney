using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace DisplayCashWithMoney
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private const string Id = "Spuddy.DisplayCashWithMoney";
        public const int CASH_ITEM_TYPE_ID = 451;

        private Harmony harmony;

        private void OnEnable()
        {
            Debug.Log("ItemLevelAndSearchSoundMod OnEnable");

            harmony = new Harmony(Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void OnDisable()
        {
            harmony.UnpatchAll(Id);
        }
    }
}
