using Beam;
using Beam.PostEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepWetAndColdMod
{
    public class Hot : PlayerEffect
    {
        private Dictionary<IPlayer,IMenuEffectsController> _menuEffects = new Dictionary<IPlayer, IMenuEffectsController>();
        private Texture2D _blurMaskTexture = null;

        private FieldInfo fi_settings = typeof(MainMenuEffectsController).GetField("_settings", BindingFlags.NonPublic | BindingFlags.Instance);

        public Hot()
            : base("Heatstroke", "Heatstroke", false, 0, 0, 0, 0, -1)
        {

        }

        public void ShowEffect(IPlayer player)
        {
            if (_blurMaskTexture == null)
            {
                Beam.UI.UQuickCrafterRadialMenuViewAdapter qcmp = Game.FindObjectOfType<Beam.UI.UQuickCrafterRadialMenuViewAdapter>();
                _blurMaskTexture = qcmp.BlurMaskTexture;
            }

            IMenuEffectsController component = player.PlayerCamera.Camera.GetComponent<MainMenuEffectsController>();
            if (!_menuEffects.ContainsKey(player))
            {
                this._menuEffects.Add(player, (component ?? NullObject<NullMenuEffectsController>.Instance));
            }
            if (!_menuEffects.ContainsKey(player))
            {
                return;
            }
            this._menuEffects[player].SetMaskTexture(this._blurMaskTexture);
            //this._menuEffects[player].Show();
            BlurSettings settings = fi_settings.GetValue(this._menuEffects[player]) as BlurSettings;
            // avoid flickering effect
            settings.enabled.Override(true);
            settings.Alpha.Override(1.0f);
        }

        public void HideEffect(IPlayer player)
        {
            if (!_menuEffects.ContainsKey(player))
            {
                return;
            }
            this._menuEffects[player].Hide();
        }
    }
}
