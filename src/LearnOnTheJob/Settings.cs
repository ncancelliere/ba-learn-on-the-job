using UnityEngine;

namespace LearnOnTheJob
{
    /// <summary>
    /// The mod's single setting. The options menu persists slider values to PlayerPrefs, but only
    /// invokes the change callback when the menu is drawn, so the city entry reads the stored value
    /// directly at load time instead of waiting for a callback that may never come.
    /// </summary>
    public static class Settings
    {
        public const string HoursPerStepId = "hours_per_step";
        public const int HoursPerStepDefault = 40;
        public const int HoursPerStepMin = 4;
        public const int HoursPerStepMax = 200;

        // Mirrors BigAmbitions.ModsInternal.ModOptionPrefs.BuildKey, which is internal to the game.
        // Fully qualified because the game ships its own global-namespace PlayerPrefs class.
        public static int LoadHoursPerStep(string modId)
        {
            var stored = UnityEngine.PlayerPrefs.GetInt("m:" + modId + ":" + HoursPerStepId, HoursPerStepDefault);
            return Mathf.Clamp(stored, HoursPerStepMin, HoursPerStepMax);
        }
    }
}
