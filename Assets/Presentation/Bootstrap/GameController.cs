// Assets/Presentation/Bootstrap/GameController.cs
using UnityEngine;
using VectorArcade.Presentation.HUD;

namespace VectorArcade.Presentation.Bootstrap
{
    public sealed class GameController : MonoBehaviour
    {
        public GameInstaller installer;

        void Update()
        {
            installer.playerCtrlUC.Execute(installer.gameState);
            installer.fieldUC.UpdateField(installer.gameState);

            // Nuevo: items (spawn + homing + pickup)
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
            VectorArcade.Presentation.HUD.WireframePresenter.DrawAll(
                installer.lineRenderer,
                installer.gameState,
                Camera.main,
                installer.hudSettings != null ? installer.hudSettings.CrosshairDistance : 2.5f,
                installer.hudSettings != null ? installer.hudSettings.CrosshairSize : 0.05f
            );

            // HUD (score + fps)
            VectorArcade.Presentation.HUD.VectorHudPresenter.DrawScore(installer.lineRenderer, installer.gameState, Camera.main);
            VectorArcade.Presentation.HUD.VectorHudPresenter.DrawFps(installer.lineRenderer, Camera.main);

            installer.lineRenderer.EndFrame();
        }
    }
}
