using Duckov.UI;
using Duckov.Utilities;
using HarmonyLib;
using ItemStatsSystem;

namespace DisplayCashWithMoney
{
    [HarmonyPatch(typeof(MoneyDisplay), "OnDestroy")]
    public class PatchMoneyDisplayOnDestroy
    {
        public static void Prefix(MoneyDisplay __instance)
        {
            PatchMoneyDisplayOnEnable.Unregister(__instance);
        }
    }
}