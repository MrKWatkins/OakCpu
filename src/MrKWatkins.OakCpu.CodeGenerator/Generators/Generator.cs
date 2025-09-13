using Microsoft.CodeAnalysis;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class Generator
{
    protected const string ActionRequiredEnumName = "ActionRequired";
    protected const string StepStructName = "Step";
    protected const string StepHandlerFieldName = "Handler";
    protected const string StepNextStepFieldName = "NextStep";
    protected const string StepActionRequiredFieldName = "ActionRequired";
    protected const string EmulatorParameterName = "emulator";
    protected const string ActionRequiredParameterName = "actionRequired";
    protected const string ErrorFunctionName = "Error";
    protected const string HandleInterruptsMethodName = "HandleInterrupts";
    protected const string InterruptModeStepTableFieldName = "InterruptModeStepTable";
    private const string StepFunctionPrefix = "Step_";

    // Filthy hackery to put some newlines and indents where we want because NormalizeWhitespace will remove any normal whitespace we add.
    protected const string NewlineCommentText = "// Newline";
    protected static readonly SyntaxTrivia NewlineComment = Comment(NewlineCommentText);
    protected static readonly SyntaxTrivia IndentComment = Comment("// Indent");

    private protected Generator()
    {
    }

    [Pure]
    protected static string GetStepFunctionName(Step step) => $"{StepFunctionPrefix}{step.Index}";
}