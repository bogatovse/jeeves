using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Jeeves.Server.Challenges;
using Jeeves.Server.Challenges.Steps;
using NUnit.Framework;

namespace Jeeves.Server.UnitTests.Workflows
{
    [TestFixture]
    public class WorkflowRunStepTests
    {
        [Test]
        public void ShouldParseWorkflowRunStep()
        {
            //Arrange
            var stepsResolver = A.Fake<IWorkflowStepResolver>();
            var expectedSteps = new IWorkflowStep[] { new WorkflowRunStep { Name = "step-1", Command = "command-1" } };
            var expectedWorkflow = new Workflow();
            expectedWorkflow.Stages = new Dictionary<string, WorkflowStage> { { "Build", new WorkflowStage { Steps = expectedSteps } } };
            A.CallTo(() => stepsResolver.Resolve(A<IReadOnlyCollection<string>>.Ignored)).Returns(typeof(WorkflowRunStep));
            var parser = new WorkflowParser(stepsResolver);

            //Act
            var workflow = parser.Parse(@"
stages:
  Build:
    steps:
      - name: step-1
        run: command-1
");
            
            //Assert
            workflow.Should().BeEquivalentTo(expectedWorkflow);
        }
        
        [Test]
        public void ShouldParseWorkflowRunStepWithExpression()
        {
            //Arrange
            var stepsResolver = A.Fake<IWorkflowStepResolver>();
            var expectedSteps = new IWorkflowStep[] { new WorkflowRunStep { Name = "step-1", Command = "./output/${{ jeeves.inputs.exeName }}" } };
            var expectedWorkflow = new Workflow();
            expectedWorkflow.Stages = new Dictionary<string, WorkflowStage> { { "Build", new WorkflowStage { Steps = expectedSteps } } };
            A.CallTo(() => stepsResolver.Resolve(A<IReadOnlyCollection<string>>.Ignored)).Returns(typeof(WorkflowRunStep));
            var parser = new WorkflowParser(stepsResolver);

            //Act
            var workflow = parser.Parse(@"
stages:
  Build:
    steps:
      - name: step-1
        run: ./output/${{ jeeves.inputs.exeName }}
");
            
            //Assert
            workflow.Should().BeEquivalentTo(expectedWorkflow);
        }
        
        [Test]
        public void ShouldParseWorkflowRunStepWithTimeout()
        {
            //Arrange
            var stepsResolver = A.Fake<IWorkflowStepResolver>();
            var options = new WorkflowRunStepOptions { Timeout = 2400 };
            var steps = new IWorkflowStep[] { new WorkflowRunStep { Name = "step-1", Command = "command-1", Options = options} };
            var expectedWorkflow = new Workflow();
            expectedWorkflow.Stages = new Dictionary<string, WorkflowStage> { { "Build", new WorkflowStage { Steps = steps } } };
            A.CallTo(() => stepsResolver.Resolve(A<IReadOnlyCollection<string>>.Ignored)).Returns(typeof(WorkflowRunStep));
            var parser = new WorkflowParser(stepsResolver);

            //Act
            var workflow = parser.Parse(@"
stages:
  Build:
    steps:
      - name: step-1
        run: command-1
        with:
          timeout: 2400
");
            
            //Assert
            workflow.Should().BeEquivalentTo(expectedWorkflow);
        }
        
        [Test]
        public void ShouldParseWorkflowRunStepWithOneMetric()
        {
            //Arrange
            var stepsResolver = A.Fake<IWorkflowStepResolver>();
            var options = new WorkflowRunStepOptions { Metrics = WorkflowRunStepMetrics.Cpu };
            var steps = new IWorkflowStep[] { new WorkflowRunStep { Name = "step-1", Command = "command-1", Options = options} };
            var expectedWorkflow = new Workflow();
            expectedWorkflow.Stages = new Dictionary<string, WorkflowStage> { { "Build", new WorkflowStage { Steps = steps } } };
            A.CallTo(() => stepsResolver.Resolve(A<IReadOnlyCollection<string>>.Ignored)).Returns(typeof(WorkflowRunStep));
            var parser = new WorkflowParser(stepsResolver);

            //Act
            var workflow = parser.Parse(@"
stages:
  Build:
    steps:
      - name: step-1
        run: command-1
        with:
          metrics: cpu
");
            
            //Assert
            workflow.Should().BeEquivalentTo(expectedWorkflow);
        }
        
        [Test]
        public void ShouldParseWorkflowRunStepWithMultipleMetrics()
        {
          //Arrange
          var stepsResolver = A.Fake<IWorkflowStepResolver>();
          var options = new WorkflowRunStepOptions { Metrics = WorkflowRunStepMetrics.Cpu | WorkflowRunStepMetrics.Ram };
          var steps = new IWorkflowStep[] { new WorkflowRunStep { Name = "step-1", Command = "command-1", Options = options} };
          var expectedWorkflow = new Workflow();
          expectedWorkflow.Stages = new Dictionary<string, WorkflowStage> { { "Build", new WorkflowStage { Steps = steps } } };
          A.CallTo(() => stepsResolver.Resolve(A<IReadOnlyCollection<string>>.Ignored)).Returns(typeof(WorkflowRunStep));
          var parser = new WorkflowParser(stepsResolver);

          //Act
          var workflow = parser.Parse(@"
stages:
  Build:
    steps:
      - name: step-1
        run: command-1
        with:
          metrics: [cpu, ram]
");
            
          //Assert
          workflow.Should().BeEquivalentTo(expectedWorkflow);
        }
        
        [Test]
        public void ShouldIgnoreCaseForRunStepMetrics()
        {
          //Arrange
          var stepsResolver = A.Fake<IWorkflowStepResolver>();
          var options = new WorkflowRunStepOptions { Metrics = WorkflowRunStepMetrics.Cpu | WorkflowRunStepMetrics.Ram };
          var steps = new IWorkflowStep[] { new WorkflowRunStep { Name = "step-1", Command = "command-1", Options = options} };
          var expectedWorkflow = new Workflow();
          expectedWorkflow.Stages = new Dictionary<string, WorkflowStage> { { "Build", new WorkflowStage { Steps = steps } } };
          A.CallTo(() => stepsResolver.Resolve(A<IReadOnlyCollection<string>>.Ignored)).Returns(typeof(WorkflowRunStep));
          var parser = new WorkflowParser(stepsResolver);

          //Act
          var workflow = parser.Parse(@"
stages:
  Build:
    steps:
      - name: step-1
        run: command-1
        with:
          metrics: [cPu, RAM]
");
            
          //Assert
          workflow.Should().BeEquivalentTo(expectedWorkflow);
        }
        
        [Test]
        public void ShouldParseWorkflowRunStepWithAllMetrics()
        {
          //Arrange
          var stepsResolver = A.Fake<IWorkflowStepResolver>();
          var options = new WorkflowRunStepOptions { Metrics = WorkflowRunStepMetrics.All };
          var steps = new IWorkflowStep[] { new WorkflowRunStep { Name = "step-1", Command = "command-1", Options = options} };
          var expectedWorkflow = new Workflow();
          expectedWorkflow.Stages = new Dictionary<string, WorkflowStage> { { "Build", new WorkflowStage { Steps = steps } } };
          A.CallTo(() => stepsResolver.Resolve(A<IReadOnlyCollection<string>>.Ignored)).Returns(typeof(WorkflowRunStep));
          var parser = new WorkflowParser(stepsResolver);

          //Act
          var workflow = parser.Parse(@"
stages:
  Build:
    steps:
      - name: step-1
        run: command-1
        with:
          metrics: [all]
");
            
          //Assert
          workflow.Should().BeEquivalentTo(expectedWorkflow);
        }
    }
}