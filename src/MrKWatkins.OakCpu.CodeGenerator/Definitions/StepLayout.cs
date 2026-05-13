using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

/// <summary>
/// Finalized layout/state for a parsed <see cref="Step" /> within a sequence.
/// </summary>
public sealed record StepLayout
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StepLayout" /> record.
    /// </summary>
    /// <param name="step">The parsed step.</param>
    /// <param name="sequence">The owning sequence for the step.</param>
    /// <param name="index">The global step index used by the step emulator.</param>
    /// <param name="methodIndex">The generated handler method index, if one exists for this step.</param>
    /// <param name="implementation">The canonical implementation step for this duplicate group.</param>
    /// <param name="duplicates">The duplicates for the implementation step.</param>
    /// <param name="implementationAndDuplicates">The implementation step followed by all of its duplicates.</param>
    internal StepLayout(
        Step step,
        StepSequence sequence,
        ushort index,
        ushort? methodIndex,
        Step implementation,
        IReadOnlyList<Step> duplicates,
        IReadOnlyList<Step> implementationAndDuplicates)
    {
        Step = step;
        Sequence = sequence;
        Index = index;
        MethodIndex = methodIndex;
        Implementation = implementation;
        Duplicates = duplicates;
        ImplementationAndDuplicates = implementationAndDuplicates;
    }

    /// <summary>
    /// Gets the parsed step this layout describes.
    /// </summary>
    public Step Step { get; }

    /// <summary>
    /// Gets the sequence that owns the step.
    /// </summary>
    public StepSequence Sequence { get; }

    /// <summary>
    /// Gets the global step index used by the generated step tables.
    /// </summary>
    public ushort Index { get; }

    /// <summary>
    /// Gets the generated handler method index, or <c>null</c> if no handler is generated for this step.
    /// </summary>
    public ushort? MethodIndex { get; }

    /// <summary>
    /// Gets the canonical implementation step for this duplicate group.
    /// </summary>
    public Step Implementation { get; }

    /// <summary>
    /// Gets the duplicate steps for the canonical implementation step.
    /// </summary>
    public IReadOnlyList<Step> Duplicates { get; }

    /// <summary>
    /// Gets the canonical implementation step followed by its duplicates.
    /// </summary>
    public IReadOnlyList<Step> ImplementationAndDuplicates { get; }

    /// <summary>
    /// Gets the next opcode mode triggered after this step, if any.
    /// </summary>
    [Pure]
    public NextOpcodeMode? NextOpcode => GetNextOpcode(Step, Sequence);

    /// <summary>
    /// Gets a value indicating whether the step must reset the opcode prefix table before continuing.
    /// </summary>
    [Pure]
    public bool RequiresPrefixReset => GetRequiresPrefixReset(Step, Sequence);

    /// <summary>
    /// Gets a value indicating whether the step executes the queued overlap at the start of the generated handler.
    /// </summary>
    [Pure]
    public bool ExecutesStoredOverlapOnStart => GetExecutesStoredOverlapOnStart(Step, Sequence);

    /// <summary>
    /// Gets a value indicating whether the step only executes as an overlap handler.
    /// </summary>
    [Pure]
    public bool ExecutesAsOverlapOnly => GetExecutesAsOverlapOnly(Step, Sequence);

    /// <summary>
    /// Gets a value indicating whether the step queues a follow-up overlap handler.
    /// </summary>
    [Pure]
    public bool QueuesOverlapStep => GetQueuesOverlapStep(Step, Sequence);

    /// <summary>
    /// Gets the overlap step queued by this step.
    /// </summary>
    [Pure]
    public Step QueuedOverlapStep =>
        QueuesOverlapStep
            ? Sequence.Steps[^1]
            : throw new InvalidOperationException($"The step {Step.Name} does not queue an overlap step.");

    /// <summary>
    /// Gets a value indicating whether the step has no generated handler body.
    /// </summary>
    [Pure]
    public bool DoesNothing => GetDoesNothing(Step, Sequence);

    /// <summary>
    /// Gets the action requested by the explicit <c>request(...)</c> call in the step, if present.
    /// </summary>
    [Pure]
    public Action OverlapAction => Step.SpecifiedAction ?? Action.None;

    /// <summary>
    /// Gets the action the host must perform when this step completes.
    /// </summary>
    [Pure]
    public Action RequiredAction
    {
        get
        {
            if (NextOpcode is null or NextOpcodeMode.Custom)
            {
                return OverlapAction;
            }

            if (Step.SpecifiedAction != null)
            {
                throw new InvalidOperationException($"No {PreDefinedFunction.Request.Name} function should be specified for the last step in an instruction, unless the next_opcode mode is set to custom.");
            }

            return Action.None;
        }
    }

    /// <summary>
    /// Determines whether the step has no generated handler body.
    /// </summary>
    /// <param name="step">The parsed step.</param>
    /// <param name="sequence">The owning sequence.</param>
    /// <returns><c>true</c> if the step generates no handler body; otherwise, <c>false</c>.</returns>
    [Pure]
    internal static bool GetDoesNothing(Step step, StepSequence sequence) => step.Statements.Count == 0 && !GetRequiresPrefixReset(step, sequence) && GetNextOpcode(step, sequence) != NextOpcodeMode.Overlapped;

    /// <summary>
    /// Determines whether the step only executes as an overlap handler.
    /// </summary>
    /// <param name="step">The parsed step.</param>
    /// <param name="sequence">The owning sequence.</param>
    /// <returns><c>true</c> if the step only executes as an overlap handler; otherwise, <c>false</c>.</returns>
    [Pure]
    internal static bool GetExecutesAsOverlapOnly(Step step, StepSequence sequence) =>
        sequence is Instruction { NextOpcode: NextOpcodeMode.Overlapped } &&
        (sequence.Steps.Count == 1 ? sequence.Steps[0] == step : sequence.Steps[^1] == step);

    /// <summary>
    /// Determines whether the step executes a queued overlap at the start of its handler.
    /// </summary>
    /// <param name="step">The parsed step.</param>
    /// <param name="sequence">The owning sequence.</param>
    /// <returns><c>true</c> if the step executes a queued overlap first; otherwise, <c>false</c>.</returns>
    [Pure]
    internal static bool GetExecutesStoredOverlapOnStart(Step step, StepSequence sequence) => sequence.ExecuteOverlapOnStart && sequence.FirstStep == step;

    /// <summary>
    /// Gets the next opcode mode triggered after this step, if any.
    /// </summary>
    /// <param name="step">The parsed step.</param>
    /// <param name="sequence">The owning sequence.</param>
    /// <returns>The next opcode mode for the last step in a sequence, otherwise <c>null</c>.</returns>
    [Pure]
    internal static NextOpcodeMode? GetNextOpcode(Step step, StepSequence sequence) => sequence.Steps[^1] == step ? sequence.NextOpcode : null;

    /// <summary>
    /// Determines whether the step queues a follow-up overlap handler.
    /// </summary>
    /// <param name="step">The parsed step.</param>
    /// <param name="sequence">The owning sequence.</param>
    /// <returns><c>true</c> if the step queues a follow-up overlap; otherwise, <c>false</c>.</returns>
    [Pure]
    internal static bool GetQueuesOverlapStep(Step step, StepSequence sequence) =>
        sequence is Instruction { NextOpcode: NextOpcodeMode.Overlapped } &&
        sequence.Steps.Count > 1 &&
        sequence.Steps[^2] == step;

    /// <summary>
    /// Determines whether the step must reset the opcode prefix table before continuing.
    /// </summary>
    /// <param name="step">The parsed step.</param>
    /// <param name="sequence">The owning sequence.</param>
    /// <returns><c>true</c> if the step must reset the opcode prefix table; otherwise, <c>false</c>.</returns>
    [Pure]
    internal static bool GetRequiresPrefixReset(Step step, StepSequence sequence) => sequence is Instruction { Prefix: not null } && sequence.Steps[0] == step;
}