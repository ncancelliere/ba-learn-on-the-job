using System;
using BAModAPI;
using Entities;
using Helpers;
using UnityEngine;

namespace LearnOnTheJob
{
    /// <summary>
    /// Once per in-game hour, nudges the primary skill of every employee who is on shift.
    /// </summary>
    public sealed class OnTheJobTrainer
    {
        private readonly IModLogger _logger;
        private SkillGrowthCurve _curve;

        public OnTheJobTrainer(IModLogger logger, int hoursPerStep)
        {
            _logger = logger;
            HoursPerStep = hoursPerStep;
        }

        public int HoursPerStep
        {
            set => _curve = new SkillGrowthCurve(value);
        }

        public void OnNewHour()
        {
            // GlobalEvents.onNewHour is a multicast delegate shared with the game; an exception here
            // would stop the game's own handlers after ours from running.
            try
            {
                var employees = EmployeeHelper.GetEmployeeInstances();
                if (employees == null) return;

                for (var i = 0; i < employees.Count; i++)
                {
                    TrainIfOnShift(employees[i]);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
            }
        }

        private void TrainIfOnShift(EmployeeInstance employee)
        {
            // Mirrors the conditions EmployeeInstance.RunHourly uses to count a worked hour, plus
            // isAbsent: a sick employee covered by a temp still "works" by the game's definition.
            if (employee == null || employee.IsCandidate || employee.IsTraining || employee.isAbsent) return;
            if (!employee.IsWorking()) return;

            var skills = employee.characterData?.skills;
            if (skills == null || skills.Count == 0) return;

            var skill = skills[0];
            if (!_curve.CanGrow(skill.value)) return;

            var before = skill.value;
            skill.value = _curve.AfterHourWorked(before);

            if (Mathf.FloorToInt(skill.value) > Mathf.FloorToInt(before))
            {
                _logger.Info(employee.characterData.name + " " + skill.name + ": " +
                             Mathf.FloorToInt(before) + " -> " + Mathf.FloorToInt(skill.value));
            }
        }
    }
}
