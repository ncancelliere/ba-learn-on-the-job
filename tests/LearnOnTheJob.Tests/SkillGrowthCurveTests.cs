using LearnOnTheJob;
using Xunit;

public class SkillGrowthCurveTests
{
    private static float WorkHours(SkillGrowthCurve curve, float start, int hours)
    {
        var value = start;
        for (var i = 0; i < hours; i++) value = curve.AfterHourWorked(value);
        return value;
    }

    [Fact]
    public void GainsTwoPointsPerStepBelowFifty()
    {
        var curve = new SkillGrowthCurve(hoursPerStep: 40);

        Assert.Equal(21f, WorkHours(curve, 19f, 40), precision: 3);
    }

    [Fact]
    public void GainsOnePointPerStepBetweenFiftyAndSeventy()
    {
        var curve = new SkillGrowthCurve(hoursPerStep: 40);

        Assert.Equal(51f, WorkHours(curve, 50f, 40), precision: 3);
    }

    [Fact]
    public void StopsAtFiftyBeforeSwitchingToTheSlowerRate()
    {
        var curve = new SkillGrowthCurve(hoursPerStep: 1);

        Assert.Equal(50f, curve.AfterHourWorked(49f));
        Assert.Equal(51f, curve.AfterHourWorked(50f));
    }

    [Fact]
    public void CapsAtSeventy()
    {
        var curve = new SkillGrowthCurve(hoursPerStep: 1);

        Assert.Equal(70f, WorkHours(curve, 19f, 1000));
    }

    [Theory]
    [InlineData(70f)]
    [InlineData(85.5f)]
    [InlineData(100f)]
    public void LeavesSkillsAtOrAboveSeventyAlone(float start)
    {
        var curve = new SkillGrowthCurve(hoursPerStep: 40);

        Assert.False(curve.CanGrow(start));
        Assert.Equal(start, curve.AfterHourWorked(start));
    }

    [Fact]
    public void FullJourneyFromNineteenToSeventyTakesExpectedHours()
    {
        // 31 points at 2/step = 15.5 steps, then 20 points at 1/step = 20 steps.
        var curve = new SkillGrowthCurve(hoursPerStep: 40);
        const int expectedHours = (int)(15.5 * 40 + 20 * 40);

        Assert.True(WorkHours(curve, 19f, expectedHours - 1) < 70f);
        Assert.Equal(70f, WorkHours(curve, 19f, expectedHours + 1), precision: 3);
    }

    [Fact]
    public void TreatsNonPositiveHoursPerStepAsOne()
    {
        var curve = new SkillGrowthCurve(hoursPerStep: 0);

        Assert.Equal(21f, curve.AfterHourWorked(19f));
    }
}
