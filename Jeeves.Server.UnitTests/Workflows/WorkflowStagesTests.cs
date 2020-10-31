using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using FakeItEasy;
using FluentAssertions;
using Jeeves.Server.Challenges;
using Jeeves.Server.Challenges.Steps;
using Jeeves.Server.UnitTests.Extensions;
using NUnit.Framework;

namespace Jeeves.Server.UnitTests.Workflows
{
    public class WorkflowStagesTests
    {
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void ShouldParseEmptyWorkflowStages(int stagesNumber)
        {
            //Arrange
            var faker = new Faker();
            var parser = new WorkflowParser(A.Dummy<IWorkflowStepResolver>());
            var expectedStages = Enumerable.Range(0, stagesNumber)
                .Select(unused => faker.GetUniqueRandomWord(1, 100))
                .ToDictionary(id => id, id => new WorkflowStage { Steps = null });

            //Act
            var workflow = parser.Parse(expectedStages.ToYaml());
            
            //Assert
            workflow.Stages.Should().BeEquivalentTo(expectedStages);
        }


        [Test]
        public void WorkflowStagesShouldBeNullIfStagesAreNotSpecified()
        {
            //Arrange
            var parser = new WorkflowParser(A.Dummy<IWorkflowStepResolver>());

            //Act
            var workflow = parser.Parse("stages: ");
            
            //Assert
            workflow.Stages.Should().BeNull();
        }
        
        [Test]
        public void ShouldParseWorkflowStageSteps()
        {
            //Arrange
            var stepsResolver = A.Fake<IWorkflowStepResolver>();
            var expectedSteps = new IWorkflowStep[]
            {
                new WorkflowRunStep { Name = "step-1", Command = "command-1" },
                new WorkflowAssertStep { Name = "step-2", Assert = "assert-2"}
            };
            var expectedWorkflow = new Workflow
            {
                Stages = new Dictionary<string, WorkflowStage>
                {
                    {"Build", new WorkflowStage { Steps = expectedSteps }}
                }
            };
            A.CallTo(() => stepsResolver.Resolve(A<IReadOnlyCollection<string>>.Ignored)).ReturnsLazily(args =>
            {
                var properties = args.GetArgument<IReadOnlyCollection<string>>(0);
                return properties!.Contains("run", StringComparer.OrdinalIgnoreCase) ? typeof(WorkflowRunStep) : typeof(WorkflowAssertStep);
            });
            var parser = new WorkflowParser(stepsResolver);

            //Act
            var workflow = parser.Parse(@"
stages:
  Build:
    steps:
      - name: step-1
        run: command-1
      - name: step-2
        assert: assert-2
");
            
            //Assert
            workflow.Should().BeEquivalentTo(expectedWorkflow);
        }
    }
}