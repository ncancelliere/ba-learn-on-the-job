using System.Threading.Tasks;
using BAModAPI;

namespace LearnOnTheJob
{
    /// <summary>
    /// Loaded when a save is loaded from the main menu, and unloaded on return to it. While loaded,
    /// it runs the trainer once per in-game hour.
    /// </summary>
    [ModEntryOnCityLoad]
    public sealed class LearnOnTheJobCityEntry : ModBigAmbitionsBase
    {
        private HourlyHook _hook;

        public override Task OnLoadAsync(ModContext context)
        {
            var hoursPerStep = Settings.LoadHoursPerStep(context.ModId);
            var trainer = new OnTheJobTrainer(context.Logger, hoursPerStep);
            LearnOnTheJobOptionsEntry.HoursPerStepChanged = value => trainer.HoursPerStep = value;

            _hook = HourlyHook.Attach(trainer.OnNewHour, context.Logger);

            context.Logger.Info("Learn on the Job active: " + hoursPerStep + " hours on shift per step.");
            return Task.CompletedTask;
        }

        public override Task OnUnloadAsync()
        {
            _hook?.Detach();
            _hook = null;
            LearnOnTheJobOptionsEntry.HoursPerStepChanged = null;
            return Task.CompletedTask;
        }
    }
}
