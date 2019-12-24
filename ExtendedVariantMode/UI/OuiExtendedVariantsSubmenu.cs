﻿using Celeste;
using Celeste.Mod.UI;
using ExtendedVariants.Module;
using System.Collections;

namespace ExtendedVariants.UI {
    /// <summary>
    /// A dedicated submenu containing all extended variants options. Parameters =
    /// 0 = (bool) true if invoked from in-game, false otherwise
    /// </summary>
    class OuiExtendedVariantsSubmenu : AbstractSubmenu {
        private int savedMenuIndex = -1;
        private TextMenu currentMenu;

        public OuiExtendedVariantsSubmenu() : base("MODOPTIONS_EXTENDEDVARIANTS_PAUSEMENU_BUTTON", null) { }

        internal override void addOptionsToMenu(TextMenu menu, bool inGame, object[] parameters) {
            currentMenu = menu;

            if (ExtendedVariantsModule.Settings.SubmenusForEachCategory) {
                // variants submenus + randomizer options
                new ModOptionsEntries().CreateAllOptions(ModOptionsEntries.VariantCategory.None, false, true, true, false,
                    () => OuiModOptions.Instance.Overworld.Goto<OuiExtendedVariantsSubmenu>(),
                    menu, inGame, false /* we don't care since there is no master switch */);
            } else {
                // all variants options + randomizer options
                new ModOptionsEntries().CreateAllOptions(ModOptionsEntries.VariantCategory.All, false, false, true, false,
                    () => OuiModOptions.Instance.Overworld.Goto<OuiExtendedVariantsSubmenu>(),
                    menu, inGame, false /* we don't care since there is no master switch */);
            }
        }
        
        public override IEnumerator Enter(Oui from) {
            // start running Enter, so that the menu is initialized
            IEnumerator enterEnum = base.Enter(from);
            if(enterEnum.MoveNext()) yield return enterEnum.Current;

            if(savedMenuIndex != -1 && currentMenu != null && 
                (from.GetType() == typeof(OuiRandomizerOptions) || from.GetType() == typeof(OuiCategorySubmenu))) {

                // restore selection if coming from submenu
                currentMenu.Selection = savedMenuIndex;
                currentMenu.Position.Y = currentMenu.ScrollTargetY;
            }

            // finish running Enter
            while (enterEnum.MoveNext()) yield return enterEnum.Current;
        }

        public override IEnumerator Leave(Oui next) {
            savedMenuIndex = currentMenu.Selection;
            currentMenu = null;
            return base.Leave(next);
        }

        internal override void gotoMenu(Overworld overworld) {
            overworld.Goto<OuiExtendedVariantsSubmenu>();
        }

        internal override string getMenuName(object[] parameters) {
            return base.getMenuName(parameters).ToUpperInvariant();
        }

        internal override string getButtonName(object[] parameters) {
            return Dialog.Clean($"MODOPTIONS_EXTENDEDVARIANTS_{(((bool) parameters[0]) ? "PAUSEMENU" : "MODOPTIONS")}_BUTTON");
        }
    }
}
