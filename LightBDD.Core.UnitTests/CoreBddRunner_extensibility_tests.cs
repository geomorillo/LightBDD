using System;
using System.Linq;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.TestableIntegration;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Steps = LightBDD.Core.UnitTests.Helpers.Steps;

namespace LightBDD.Core.UnitTests
{
    [CustomFeatureDescription("some description")]
    [CustomCategory("some global category")]
    [ScenarioCategory("standard global category")]
    [TestFixture]
    public class CoreBddRunner_extensibility_tests : Steps
    {
        [Test]
        [CustomCategory("some local category")]
        [ScenarioCategory("standard local category")]
        public void It_should_collect_custom_categories()
        {
            var feature = TestableBddRunnerFactory.GetRunner(GetType());
            var runner = feature.GetRunner(this);
            runner.Test().TestScenario(Some_step);
            var scenario = feature.GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Info.Categories, Is.EqualTo(new[] { "some global category", "some local category", "standard global category", "standard local category" }));
        }

        [Test]
        public void It_should_collect_custom_feature_description()
        {
            var feature = TestableBddRunnerFactory.GetRunner(GetType());
            var runner = feature.GetRunner(this);
            runner.Test().TestScenario(Some_step);
            Assert.That(feature.GetFeatureResult().Info.Description, Is.EqualTo("some description"));
        }

        [CustomFeatureDescription("custom description")]
        [FeatureDescription("standard description")]
        class Feature_with_two_descriptions
        {
        }

        [Test]
        public void It_should_collect_standard_feature_description_if_both_are_specified()
        {
            var feature = TestableBddRunnerFactory.GetRunner(typeof(Feature_with_two_descriptions));
            var runner = feature.GetRunner(new Feature_with_two_descriptions());
            runner.Test().TestScenario(Some_step);
            Assert.That(feature.GetFeatureResult().Info.Description, Is.EqualTo("standard description"));
        }

        [Test]
        public void It_should_capture_step_status_with_custom_exception_mapping()
        {
            var feature = TestableBddRunnerFactory.GetRunner(GetType());
            var runner = feature.GetRunner(this);
            Assert.Throws<CustomIgnoreException>(() => runner.Test().TestScenario(
                Given_step_one,
                When_step_two_ignoring_scenario,
                Then_step_three));

            var steps = feature.GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation(2, 3, "WHEN step two ignoring scenario", ExecutionStatus.Ignored, IgnoreReason),
                new StepResultExpectation(3, 3, "THEN step three", ExecutionStatus.NotRun));
        }

        [Test]
        [TestCase(null)]
        [TestCase(" \t\r\n")]
        public void It_should_not_allow_to_run_scenarios_without_name(string name)
        {
            var exception = Assert.Throws<ArgumentException>(() => TestableBddRunnerFactory.GetRunner(GetType()).GetRunner(this).Integrate().NewScenario().WithName(name));
            Assert.That(exception.Message, Does.StartWith("Unable to create scenario without name"));
        }
    }
}