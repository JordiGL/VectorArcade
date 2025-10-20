// Assets/Presentation/Bootstrap/GameController.cs
using UnityEngine;
using VectorArcade.Presentation.HUD;

namespace VectorArcade.Presentation.Bootstrap
{
    public sealed class GameController : MonoBehaviour
    {
        public GameInstaller installer;


        private static Color GetCrosshairColor(VectorArcade.Presentation.Config.CrosshairColor c)
        {
            return c switch
            {
                VectorArcade.Presentation.Config.CrosshairColor.Red => Color.red,
                VectorArcade.Presentation.Config.CrosshairColor.Green => Color.green,
                VectorArcade.Presentation.Config.CrosshairColor.Blue => Color.blue,
                VectorArcade.Presentation.Config.CrosshairColor.Yellow => Color.yellow,
                VectorArcade.Presentation.Config.CrosshairColor.Cyan => Color.cyan,
                VectorArcade.Presentation.Config.CrosshairColor.Magenta => Color.magenta,
                _ => Color.magenta,
            };
        }

        void Update()
        {
            installer.playerCtrlUC.Execute(installer.gameState);
            installer.fieldUC.UpdateField(installer.gameState);
            installer.planetUC.UpdateField(installer.gameState);

            // items (spawn + homing + pickup)
            installer.itemUC.Execute(installer.gameState, installer.weaponRules);

            installer.tickUC.Execute(installer.gameState);
            installer.shootUC.Execute(installer.gameState);

            // cámara y dibujo (como ya lo tienes)
            var p = installer.gameState.Player;
            var pos = new UnityEngine.Vector3(p.Position.x, p.Position.y, p.Position.z);
            var fwd = new UnityEngine.Vector3(p.Forward.x, p.Forward.y, p.Forward.z);
            Camera.main.transform.SetPositionAndRotation(pos, UnityEngine.Quaternion.LookRotation(fwd, UnityEngine.Vector3.up));

            VectorArcade.Presentation.HUD.VectorHudPresenter.UpdateFps();
            installer.lineRenderer.BeginFrame();

            // crosshair
            var hud = installer.hudSettings;
            Color crossColor = hud ? GetCrosshairColor(hud.Color) : Color.white;
            VectorArcade.Presentation.HUD.WireframePresenter.DrawAll(
                installer.lineRenderer,
                installer.gameState,
                Camera.main,
                hud ? hud.CrosshairDistance : 2.5f,
                hud ? hud.CrosshairSize : 0.05f,
                crossColor
            );

            // HUD (score + fps)
            VectorArcade.Presentation.HUD.VectorHudPresenter.DrawScore(installer.lineRenderer, installer.gameState, Camera.main);
            VectorArcade.Presentation.HUD.VectorHudPresenter.DrawFps(installer.lineRenderer, Camera.main);

            installer.lineRenderer.EndFrame();
        }
    }
}
