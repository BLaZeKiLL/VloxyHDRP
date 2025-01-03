using System;
using CodeBlaze.Vloxy.Game.Utils;
using UnityEngine;

namespace CodeBlaze.Vloxy.Game.Managers {
    
    public class GameManager : SingletonBehaviour<GameManager> {

        public VloxyInput InputMaps { get; private set; }

        protected override void Initialize() {
            InitializeApplication();
            
            InputMaps = new VloxyInput();
        }

        private void InitializeApplication() {
            if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Handheld) {
                Application.targetFrameRate = (int) Screen.currentResolution.refreshRateRatio.numerator;

                GameLogger.Info<GameManager>("Device Type : Handheld");
            } else {
                GameLogger.Info<GameManager>("Device Type : Desktop");
            }
        }
        
    }
    
}