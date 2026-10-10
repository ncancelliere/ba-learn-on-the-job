namespace LearnOnTheJob
{
    /// <summary>
    /// Pure progression math with no game or Unity dependencies, so it can be unit tested.
    ///
    /// Skill values are 0-100. Below <see cref="FastTierCap"/> an employee gains
    /// <see cref="FastTierPointsPerStep"/> points per step; from there up to <see cref="SlowTierCap"/>
    /// they gain <see cref="SlowTierPointsPerStep"/>. At or above <see cref="SlowTierCap"/> on-the-job
    /// learning stops and only formal training helps.
    ///
    /// Growth is applied continuously (a fraction of a step per hour worked) rather than in discrete
    /// jumps. The game stores skill as a float and persists it in the save, so the fractional progress
    /// survives saving and loading without the mod keeping any state of its own.
    /// </summary>
    public sealed class SkillGrowthCurve
    {
        public const float FastTierCap = 50f;
        public const float SlowTierCap = 70f;
        public const float FastTierPointsPerStep = 2f;
        public const float SlowTierPointsPerStep = 1f;

        private readonly int _hoursPerStep;

        public SkillGrowthCurve(int hoursPerStep)
        {
            _hoursPerStep = hoursPerStep < 1 ? 1 : hoursPerStep;
        }

        public bool CanGrow(float skillValue) => skillValue < SlowTierCap;

        /// <summary>
        /// Returns the skill value after one hour on shift. Never lowers a skill, and clamps at the
        /// current tier's cap so a single hour can't carry an employee across a tier boundary at the
        /// faster rate.
        /// </summary>
        public float AfterHourWorked(float skillValue)
        {
            if (skillValue < FastTierCap)
            {
                return Min(skillValue + FastTierPointsPerStep / _hoursPerStep, FastTierCap);
            }

            if (skillValue < SlowTierCap)
            {
                return Min(skillValue + SlowTierPointsPerStep / _hoursPerStep, SlowTierCap);
            }

            return skillValue;
        }

        private static float Min(float a, float b) => a < b ? a : b;
    }
}
