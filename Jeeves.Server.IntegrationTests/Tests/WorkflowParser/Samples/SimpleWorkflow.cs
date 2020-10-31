using System.Collections.Generic;
using Jeeves.Server.Challenges;
using Jeeves.Server.Challenges.Steps;

namespace Jeeves.Server.IntegrationTests.Tests.WorkflowParser.Samples
{
    public class Yaml
    {
        public string Name { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public static class SimpleWorkflow
    {
        public static Yaml Yaml => new Yaml
        {
            Name = nameof(SimpleWorkflow),
            Content =  @"
name: SimpleWorkflow
parameters:
  exeName:
    description: Executable name
    required: true
runs-on: windows-latest
stages:
  Build:
    steps:
      - name: Restore Dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --configuration Release --no-restore --output ./output
  Run:
    steps:
      - name: Execute
        run: ./output/${{ jeeves.inputs.exeName }}
        with:
          metrics: [cpu, ram, time]
          timeout: 2400
      - name: Verify
        assert:  ${{ hashFile(file1) == hashFile(file2) }}
"
        };

        public static Workflow Workflow
        {
            get
            {
                return new Workflow
                {
                    Name = "SimpleWorkflow",
                    HostType = "windows-latest",
                    Parameters = new Dictionary<string, WorkflowParameter>
                    {
                        {"exeName", new WorkflowParameter { Name = "exeName", Description = "Executable name", Required = true}}
                    },
                    Stages = new Dictionary<string, WorkflowStage>
                    {
                        {
                            "Build", new WorkflowStage
                            {
                                Steps = new IWorkflowStep[]
                                {
                                    new WorkflowRunStep { Name = "Restore Dependencies", Command = "dotnet restore" },
                                    new WorkflowRunStep
                                        { Name = "Build", Command = "dotnet build --configuration Release --no-restore --output ./output"}
                                }
                            }
                        },
                        {
                            "Run", new WorkflowStage
                            {
                                Steps = new IWorkflowStep[]
                                {
                                    new WorkflowRunStep
                                    {
                                        Name = "Execute", Command = "./output/${{ jeeves.inputs.exeName }}", Options = new WorkflowRunStepOptions
                                        {
                                            Metrics = WorkflowRunStepMetrics.Cpu | WorkflowRunStepMetrics.Ram | WorkflowRunStepMetrics.Time,
                                            Timeout = 2400
                                        }
                                    },
                                    new WorkflowAssertStep { Name = "Verify", Assert = "${{ hashFile(file1) == hashFile(file2) }}"}
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}