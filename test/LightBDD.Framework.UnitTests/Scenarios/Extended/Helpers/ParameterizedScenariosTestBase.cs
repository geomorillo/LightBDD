using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class ParameterizedScenariosTestBase<T> : Steps
    {
        protected StepDescriptor[] CapturedSteps;
        protected Mock<IScenarioRunner> MockScenarioRunner;
        protected MockBddRunner<T> Runner;

        [SetUp]
        public void SetUp()
        {
            CapturedSteps = null;
            MockScenarioRunner = new Mock<IScenarioRunner>();
            Runner = new MockBddRunner<T>(TestableIntegrationContextBuilder.Default().Build().Configuration, MockScenarioRunner.Object);
        }

        protected void ExpectSynchronousScenarioRun()
        {
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunSynchronously();
        }

        protected void ExpectAsynchronousScenarioRun()
        {
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunAsynchronously();
        }

        protected void ExpectRunSynchronously()
        {
            MockScenarioRunner
                .Setup(r => r.RunSynchronously())
                .Verifiable();
        }

        protected void ExpectWithSteps()
        {
            MockScenarioRunner
                .Setup(s => s.WithSteps(It.IsAny<IEnumerable<StepDescriptor>>()))
                .Returns((IEnumerable<StepDescriptor> steps) =>
                {
                    CapturedSteps = steps.ToArray();
                    return MockScenarioRunner.Object;
                })
                .Verifiable();
        }

        protected void ExpectWithCapturedScenarioDetails()
        {
            MockScenarioRunner
                .Setup(s => s.WithCapturedScenarioDetails())
                .Returns(MockScenarioRunner.Object)
                .Verifiable();
        }

        protected void ExpectRunAsynchronously()
        {
            MockScenarioRunner
                .Setup(r => r.RunAsynchronously())
                .Returns(Task.FromResult(0))
                .Verifiable();
        }

        protected void VerifyExpectations()
        {
            MockScenarioRunner.VerifyAll();
        }

        protected void AssertStep(StepDescriptor step, string expectedName, string expectedPredefinedStepType = null, params (string name, object value)[] parameters)
        {
            Assert.That(step.RawName, Is.EqualTo(expectedName), nameof(step.RawName));
            Assert.That(step.PredefinedStepType, Is.EqualTo(expectedPredefinedStepType), nameof(step.PredefinedStepType));
            Assert.That(step.Parameters.Select(p => (name: p.RawName, value: p.ValueEvaluator(null))).ToArray(), Is.EqualTo(parameters), nameof(step.Parameters));

            var ex = Assert.Throws<Exception>(() => step.StepInvocation.Invoke(null, step.Parameters.Select(x => x.ValueEvaluator(null)).ToArray()).GetAwaiter().GetResult());
            Assert.That(ex.Message, Is.EqualTo(expectedName));
        }
    }
}