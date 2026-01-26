using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(ReGUI.Core), "ReGUI", "0.0.1", "GraciousCub5622", null)]
[assembly: MelonGame("Keepsake Games", "Jump Space")]

namespace ReGUI
{
    public class Core : MelonMod
    {
        public static bool captions = false;
        public static bool Controls = false;
        public static GameObject gui;
        public static GameObject lobbyHud;

        private static MelonPreferences_Category Prefs;
        private static MelonPreferences_Entry<string> IncreaseKey;
        private static MelonPreferences_Entry<string> DecreaseKey;
        private static MelonPreferences_Entry<float> EquipmentScale;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("[ReGUI] Mod Initialized.");

            Prefs = MelonPreferences.CreateCategory("CompactUI");
            IncreaseKey = Prefs.CreateEntry("IncreaseScaleKey", "F4");
            DecreaseKey = Prefs.CreateEntry("DecreaseScaleKey", "F3");
            EquipmentScale = Prefs.CreateEntry("EquipmentBarScale", 1f);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            MelonCoroutines.Start(LongDelayedActivate());

            if (sceneName == "Global")
                gui = GameObject.Find("PF_GUI/FullScreenGUI");
            if (sceneName == "Dest_Lobby_Start")
                lobbyHud = GameObject.Find("Gameplay/PF_Destination DestData_Lobby_Start/PF_LobbyHud");
        }

        public override void OnApplicationQuit()
        {
            MelonPreferences.Save();
        }

        public override void OnLateUpdate()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.f9Key.wasPressedThisFrame)
            {
                captions = !captions;
                MelonCoroutines.Start(ShortDelayedActivate());
            }

            if (kb.f10Key.wasPressedThisFrame)
            {
                Controls = !Controls;
                MelonCoroutines.Start(ShortDelayedActivate());
            }

            if (kb.f11Key.wasPressedThisFrame)
            {
                var crosshair = GameObject.Find("PF_GUI/FullScreenGUI/PF_FirstPersonHUD/Crosshairs");
                if (crosshair != null)
                    crosshair.SetActive(!crosshair.activeSelf);
            }

            if (kb.f8Key.wasPressedThisFrame)
            {
                if (gui == null)
                    gui = GameObject.Find("PF_GUI/FullScreenGUI");

                if (lobbyHud == null)
                    lobbyHud = GameObject.Find("Gameplay/PF_Destination DestData_Lobby_Start/PF_LobbyHud");

                if (gui != null)
                    gui.SetActive(!gui.activeSelf);
                if (lobbyHud != null)
                    lobbyHud.SetActive(!lobbyHud.activeSelf);

                MelonCoroutines.Start(ShortDelayedActivate());
            }

            if (kb.fKey.wasPressedThisFrame || kb.escapeKey.wasPressedThisFrame || kb.bKey.wasPressedThisFrame)
                MelonCoroutines.Start(ShortDelayedActivate());

            HandleScaleHotkeys();
        }

        private void HandleScaleHotkeys()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (TryGetKey(IncreaseKey.Value, kb, out var incKey) && incKey.wasPressedThisFrame)
            {
                EquipmentScale.Value = Mathf.Clamp(EquipmentScale.Value + 0.05f, 0.2f, 2.0f);
                ApplyEquipmentScale();
                LoggerInstance.Msg($"[CompactUI] Equipment bar scale: {EquipmentScale.Value:F2}");
            }

            if (TryGetKey(DecreaseKey.Value, kb, out var decKey) && decKey.wasPressedThisFrame)
            {
                EquipmentScale.Value = Mathf.Clamp(EquipmentScale.Value - 0.05f, 0.2f, 2.0f);
                ApplyEquipmentScale();
                LoggerInstance.Msg($"[CompactUI] Equipment bar scale: {EquipmentScale.Value:F2}");
            }
        }

        private static bool TryGetKey(string keyName, Keyboard kb, out KeyControl key)
        {
            key = kb.FindKeyOnCurrentKeyboardLayout(keyName);
            return key != null;
        }

        private IEnumerator LongDelayedActivate()
        {
            yield return new WaitForSeconds(5f);
            changeUI();
        }

        private IEnumerator ShortDelayedActivate()
        {
            yield return new WaitForSeconds(0.3f);
            changeUI();
        }

        public static void changeUI()
        {
            MelonPreferences.Load();

            try
            {
                ApplyEquipmentScale();
            }
            catch { }

            try
            {
                var PlayerBar = GameObject.Find("PF_GUI/FullScreenGUI/PF_FirstPersonHUD/PlayerStatusBar/PlayerBar");
                PlayerBar.GetComponent<Image>().enabled = false;
            }
            catch { }

            try
            {
                GameObject.Find("PF_GUI/FullScreenGUI/PF_FirstPersonHUD/PlayerStatusBar/PlayerBar/LocationLabel")
                    .SetActive(false);
            }
            catch { }

            try
            {
                GameObject.Find("PF_GUI/FullScreenGUI/PF_FirstPersonHUD/PlayerStatusBar/PlayerBar/PlayernameLabel")
                    .SetActive(false);
            }
            catch { }

            try
            {
                var PF_Hints = GameObject.Find("PF_GUI/FullScreenGUI/PF_Hints");
                PF_Hints.SetActive(captions);
            }
            catch { }

            try
            {
                GameObject.Find("Gameplay/PF_Destination DestData_Lobby_Start/PF_LobbyHud/PF_PlayerStatusBar_Lobby/PlayerBar/PlayernameLabel")
                    .SetActive(false);
            }
            catch { }

            try
            {
                GameObject.Find("Gameplay/PF_Destination DestData_Lobby_Start/PF_LobbyHud/PF_PlayerGroupBar_Lobby/InviteFriendBox")
                    .SetActive(false);
            }
            catch { }

            try
            {
                GameObject.Find("Gameplay/PF_Destination DestData_Lobby_Start/PF_LobbyHud/PF_PlayerStatusBar_Lobby/PF_Wallet/CreditsBox")
                    .transform.localPosition = new Vector3(165f, -55f, 0f);
            }
            catch { }

            try
            {
                GameObject.Find("Gameplay/PF_Destination DestData_Lobby_Start/PF_LobbyHud/PF_PlayerStatusBar_Lobby/PF_Wallet/IngotsBox")
                    .transform.localPosition = new Vector3(320f, -55f, 0f);
            }
            catch { }

            try
            {
                var controls = FindGameObjectsContaining("PF_IngameFullscreen");
                foreach (var control in controls)
                    control.SetActive(!Controls);
            }
            catch { }
        }

        private static void ApplyEquipmentScale()
        {
            var bar = GameObject.Find("PF_GUI/FullScreenGUI/PF_FirstPersonHUD/PF_EquipmentBar");
            if (bar != null)
            {
                float s = EquipmentScale.Value;
                bar.transform.localScale = new Vector3(s, s, s);
            }
        }

        public static List<GameObject> FindGameObjectsContaining(string substring)
        {
            List<GameObject> matches = new();
            foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
                if (obj?.name?.Contains(substring) == true)
                    matches.Add(obj);
            return matches;
        }
    }
}
