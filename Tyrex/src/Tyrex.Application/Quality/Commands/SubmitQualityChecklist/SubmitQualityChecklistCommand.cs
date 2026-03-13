using Tyrex.Application.Messaging;

namespace Tyrex.Application.Quality.Commands.SubmitQualityChecklist;

public sealed record SubmitQualityChecklistCommand(
    Guid RepairOrderId,
    Guid InspectorId,
    List<ChecklistItemResult> Items,
    string? FinalNotes) : ICommand<Guid>;

public sealed record ChecklistItemResult(
    string Name,
    string Description,
    Domain.Quality.QualityCheckItemStatus Status,
    string? Notes);
