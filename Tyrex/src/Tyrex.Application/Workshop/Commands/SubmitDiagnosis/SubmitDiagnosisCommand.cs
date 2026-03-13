using Tyrex.Application.Messaging;

namespace Tyrex.Application.Workshop.Commands.SubmitDiagnosis;

public sealed record SubmitDiagnosisCommand(
    Guid RepairOrderId,
    Guid TechnicianId,
    string Notes,
    List<string> MediaUrls) : ICommand<Guid>;
