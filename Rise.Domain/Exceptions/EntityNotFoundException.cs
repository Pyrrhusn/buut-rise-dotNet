namespace Rise.Domain.Exceptions;

/// <summary>
/// <see cref="Exception"/> to throw when the <see cref="Entity"/> was not found.
/// </summary>
/// <param name="entityName">Name / type of the <see cref="Entity"/>.</param>
/// <param name="id">The identifier of the duplicate key.</param>
public class EntityNotFoundException(string entityName, object id) : ApplicationException($"'{entityName}' with 'Id':'{id}' was not found.")
{
}
