using VectorArcade.Domain.Services;
using VectorArcade.Domain.Core;
// Assets/Presentation/Bootstrap/GameController.cs
using UnityEngine;
using VectorArcade.Presentation.HUD;
using UnityEngine.UI;

namespace VectorArcade.Presentation.Bootstrap
{
    public sealed class GameController : MonoBehaviour
    {
        public GameInstaller installer;

        // Game Over UI runtime-created
        Canvas _goCanvas;
        GameObject _goRoot;
        Text _titleText;
        Text _restartText;
        Text _quitText;
        int _goSelection = 0; // 0=reintentar, 1=salir
        float _blinkPhase = 0f;


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
            // Game Over: mostrar menú UI con fuentes y permitir selección
            if (installer.gameState.GameOver)
            {
                EnsureGameOverUI();
                UpdateGameOverUI();
                return;
            }


            installer.playerCtrlUC.Execute(installer.gameState);
            installer.fieldUC.UpdateField(installer.gameState);
            installer.planetUC.UpdateField(installer.gameState);
            installer.cometUC.Execute(installer.gameState);

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

            // HUD (score + fps + vitals)
            VectorArcade.Presentation.HUD.VectorHudPresenter.DrawScore(installer.lineRenderer, installer.gameState, Camera.main);
            VectorArcade.Presentation.HUD.VectorHudPresenter.DrawFps(installer.lineRenderer, Camera.main);
            VectorArcade.Presentation.HUD.VectorHudPresenter.DrawVitals(installer.lineRenderer, installer.gameState, Camera.main);

            installer.lineRenderer.EndFrame();

            // ocultar menú si estuviera visible (por si se ha reiniciado)
            if (_goRoot != null) _goRoot.SetActive(false);
        }

        void EnsureGameOverUI()
        {
            if (_goRoot != null)
            {
                _goRoot.SetActive(true);
                // Reparar referencias si se perdieron
                if (_restartText == null || _quitText == null || _titleText == null)
                {
                    var rt = _goRoot.transform.Find("REINICIAR");
                    var qt = _goRoot.transform.Find("SALIR");
                    var tt = _goRoot.transform.Find("TITLE");
                    if (rt != null) _restartText = rt.GetComponent<UnityEngine.UI.Text>();
                    if (qt != null) _quitText = qt.GetComponent<UnityEngine.UI.Text>();
                    if (tt != null) _titleText = tt.GetComponent<UnityEngine.UI.Text>();
                }
                return;
            }

            _goRoot = new GameObject("GameOverCanvas");
            _goCanvas = _goRoot.AddComponent<Canvas>();
            _goCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = _goRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            _goRoot.AddComponent<GraphicRaycaster>();

            // Fondo negro
            var bg = new GameObject("BG");
            bg.transform.SetParent(_goRoot.transform, false);
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = Color.black;
            var bgRt = bg.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one; bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;

            // Utilidad local para crear textos
            Text MakeText(string name, Transform parent, string txt, int size, Color col, Vector2 anchor, Vector2 anchoredPos)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                var t = go.AddComponent<Text>();
                t.text = txt;
                // Built-in font: LegacyRuntime.ttf in modern Unity; fallback to Arial/os font
                Font builtin = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (builtin == null)
                    builtin = Resources.GetBuiltinResource<Font>("Arial.ttf");
                if (builtin == null)
                {
                    try { builtin = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI", "Verdana" }, 16); }
                    catch { builtin = Font.CreateDynamicFontFromOSFont("Arial", 16); }
                }
                t.font = builtin;
                t.fontStyle = FontStyle.Bold;
                t.fontSize = size;
                t.alignment = TextAnchor.MiddleCenter;
                t.color = col;
                var ol = go.AddComponent<Outline>();
                ol.effectColor = new Color(0f, 0f, 0f, 0.85f);
                ol.effectDistance = new Vector2(2f, -2f);
                var rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(1000, 140);
                rt.anchorMin = anchor; rt.anchorMax = anchor; rt.anchoredPosition = anchoredPos;
                return t;
            }

            // Estilo arcade sencillo (cian/amarillo)
            _titleText = MakeText("TITLE", _goRoot.transform, "GAME OVER", 96, new Color(0.2f, 1f, 1f), new Vector2(0.5f, 0.72f), Vector2.zero);
            _restartText = MakeText("REINICIAR", _goRoot.transform, "REINICIAR", 56, Color.white, new Vector2(0.5f, 0.52f), Vector2.zero);
            _quitText = MakeText("SALIR", _goRoot.transform, "SALIR", 48, Color.white, new Vector2(0.5f, 0.38f), Vector2.zero);
            // Instrucciones
            MakeText("HELP", _goRoot.transform, "Flechas para cambiar  -  Intro/Espacio confirmar", 28, new Color(0.7f, 0.7f, 0.7f), new Vector2(0.5f, 0.28f), Vector2.zero);

            _goSelection = 0;
            _blinkPhase = 0f;
        }

        void UpdateGameOverUI()
        {
            // Navegación
            if (Input.GetKeyDown(KeyCode.UpArrow)) _goSelection = (_goSelection + 2 - 1) % 2; // -1 con wrap
            if (Input.GetKeyDown(KeyCode.DownArrow)) _goSelection = (_goSelection + 1) % 2;   // +1 con wrap

            // Confirmar
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                if (_goSelection == 0) { ResetGame(); return; }
                else { UnityEngine.Application.Quit(); }
            }

            // Parpadeo del seleccionado
            if (_restartText == null || _quitText == null) return; // referencias aún no listas
            _blinkPhase += Time.unscaledDeltaTime * 5f; // velocidad
            float a = 0.4f + 0.6f * Mathf.Abs(Mathf.Sin(_blinkPhase));

            if (_goSelection == 0)
            {
                // REINICIAR seleccionado: amarillo y parpadeo; SALIR en blanco sólido
                _restartText.color = new Color(1.0f, 0.95f, 0.35f, a);
                _quitText.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                // SALIR seleccionado: amarillo y parpadeo; REINICIAR en blanco sólido
                _restartText.color = new Color(1f, 1f, 1f, 1f);
                _quitText.color = new Color(1.0f, 0.95f, 0.35f, a);
            }
        }

        void ResetGame()
        {
            var state = new GameState();
            state.Player.Position = new Vec3(0, 0, 0);
            state.Player.Forward = new Vec3(0, 0, 1);
            state.Player.Velocity = Vec3.Zero;
            state.Player.ShieldPercent = 100;
            state.Player.Lives = 3;
            state.Score = 0;
            state.TimeSinceStart = 0f;
            state.GameOver = false;
            state.CometRules = installer.cometRules;

            installer.gameState = state;

            installer.fieldUC.InitializeField(installer.gameState);
            installer.planetUC.InitializeField(installer.gameState);

            installer.cometUC.Reset();

            // Ocultar/destruir UI de Game Over
            if (_goRoot != null) _goRoot.SetActive(false);
        }
    }
}
