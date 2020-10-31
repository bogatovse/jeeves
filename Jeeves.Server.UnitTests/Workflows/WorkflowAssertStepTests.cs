using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Jeeves.Server.Challenges;
using Jeeves.Server.Challenges.Steps;
using NUnit.Framework;

namespace Jeeves.Server.UnitTests.Workflows
{
    public class WorkflowAssertStepTests
    {
        [Test]
        public void ShouldParseWorkflowRunStep()
        {
            //Arrange
            var stepsResolver = A.Fake<IWorkflowStepResolver>();
            var expectedSteps = new IWorkflowStep[] { new WorkflowAssertStep { Name = "step-1", Assert = "assert-1" } };
            var expectedWorkflow = new Workflow();
            expectedWorkflow.Stages = new Dictionary<string, WorkflowStage> { { "Build", new WorkflowStage { Steps = expectedSteps } } };
            A.CallTo(() => stepsResolver.Resolve(A<IReadOnlyCollection<string>>.Ignored)).Returns(typeof(WorkflowAssertStep));
            var parser = new WorkflowParser(stepsResolver);

            //Act
            var workflow = parser.Parse(@"
stages:
  Build:
    steps:
      - name: step-1
        assert: assert-1
");
            
            //Assert
            workflow.Should().BeEquivalentTo(expectedWorkflow);
        }
    }
}