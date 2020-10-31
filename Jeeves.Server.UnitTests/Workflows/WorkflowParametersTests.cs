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
    public class WorkflowParametersTests
    {
        [Test]
        public void WorkflowParametersShouldBeNullIfParametersAreEmpty()
        {
            //Arrange
            var yaml = $@"parameters: ";
            var parser = new WorkflowParser(A.Dummy<IWorkflowStepResolver>());
            
            //Act
            var workflow = parser.Parse(yaml);
            
            //Assert
            workflow.Parameters.Should().BeNull();
        }
        
        [Test]
        public void WorkflowParameterNameShouldBeEqualToKey()
        {
            //Arrange
            var yaml = @"
parameters:
  exeName:
    description: 'Executable name'
    required: true";
            var parser = new WorkflowParser(A.Dummy<IWorkflowStepResolver>());
            
            //Act
            var workflow = parser.Parse(yaml);
            
            //Assert
            workflow.Parameters["exeName"].Name.Should().Be("exeName");
        }

        [Test]
        public void WorkflowParametersShouldBeNullIfParametersAreNotSpecified()
        {
            //Arrange
            var yaml = $@"name: amazing";
            var parser = new WorkflowParser(A.Dummy<IWorkflowStepResolver>());
            
            //Act
            var workflow = parser.Parse(yaml);
            
            //Assert
            workflow.Parameters.Should().BeNull();
        }
        
        [Test]
        public void ShouldParseWorkflowParametersSpecifiedAsScalar()
        {
            //Arrange
            var parameter = new WorkflowParameter { Name = "SomeParameter" };
            var expectedParameters = new Dictionary<string, WorkflowParameter> { { parameter.Name, parameter } };
            var yaml = $@"parameters: {parameter.Name}";
            var parser = new WorkflowParser(A.Dummy<IWorkflowStepResolver>());
            
            //Act
            var workflow = parser.Parse(yaml);
            
            //Assert
            workflow.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void ShouldParseWorkflowParametersSpecifiedAsSequence(int parametersNumber)
        {
            //Arrange
            var faker = new Faker();
            var expectedParameters = Enumerable.Range(0, parametersNumber)
                                               .Select(unused => faker.GetUniqueRandomWord(1, 100))
                                               .Select(parameterName => new WorkflowParameter { Name = parameterName })
                                               .ToDictionary(parameter => parameter.Name);
            var yaml = $@"parameters: [{string.Join(",", expectedParameters.Values.Select(p => p.Name))}]";
            var parser = new WorkflowParser(A.Dummy<IWorkflowStepResolver>());
            
            //Act
            var workflow = parser.Parse(yaml);
            
            //Assert
            workflow.Parameters.Should().BeEquivalentTo(expectedParameters);
        }
        
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void ShouldParseWorkflowParametersSpecifiedAsMapping(int parametersNumber)
        {
            //Arrange
            var faker = new Faker();
            var parser = new WorkflowParser(A.Dummy<IWorkflowStepResolver>());
            var expectedParameters = Enumerable.Range(0, parametersNumber)
                                               .Select(unused => faker.GetUniqueRandomWord(1, 100))
                                               .Select(parameterName => new WorkflowParameter
                                               {
                                                   Name = faker.GetUniqueRandomWord(1, 100), 
                                                   Description = faker.Lorem.Sentence(), 
                                                   Default = faker.Lorem.Sentence(),
                                                   Required = faker.Random.Bool()
                                               })
                                               .ToDictionary(parameter => parameter.Name);

            //Act
            var workflow = parser.Parse(expectedParameters.ToYaml());
            
            //Assert
            workflow.Parameters.Should().BeEquivalentTo(expectedParameters);
        }
    }
}