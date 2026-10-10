using System;
using System.Threading.Tasks;
using BAModAPI;
using BigAmbitions.Mods;
using LearnOnTheJob;

[assembly: RegisterModClass(typeof(LearnOnTheJobOptionsEntry))]
[assembly: RegisterModClass(typeof(LearnOnTheJobCityEntry))]

namespace LearnOnTheJob
{
    /// <summary>
    /// Loaded once at startup and kept for the whole session, so the slider shows up in the options
    /// menu from the main menu as well as in-game.
    /// </summary>
    [ModEntryOnInitializationLoad]
    public sealed class LearnOnTheJobOptionsEntry : ModBigAmbitionsBase
    {
        // Set by the city entry while a save is loaded so slider changes apply without a reload.
        internal static Action<int> HoursPerStepChanged;

        private string _modId;

        public override Task OnLoadAsync(ModContext context)
        {
            _modId = context.ModId;

            var options = new ModOptions()
                .AddHeader("learnonthejob_header")
                .AddSlider(
                    Settings.HoursPerStepId,
                    "learnonthejob_hours_per_step_label",
                    Settings.HoursPerStepMin,
                    Settings.HoursPerStepMax,
                    Settings.HoursPerStepDefault,
                    value => HoursPerStepChanged?.Invoke(value),
                    "learnonthejob_hours_per_step_value");

            OptionsService.Register(_modId, options);
            return Task.CompletedTask;
        }

        public override Task OnUnloadAsync()
        {
            OptionsService.RemoveModOptions(_modId);
            return Task.CompletedTask;
        }
    }
}
