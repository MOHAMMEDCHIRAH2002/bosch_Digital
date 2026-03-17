using Tyrex.Application.Messaging;

namespace Tyrex.Application.Workshop.Commands.AddIntakePhotos;

public sealed record AddIntakePhotosCommand(
    Guid RepairOrderId,
    List<string> PhotoUrls) : ICommand<bool>;
