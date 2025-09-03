using System;

namespace Rise.Domain.Exceptions;

/// <summary>
/// <see cref="Exception"/> to throw when the <see cref="Entity"/> already exists.
/// </summary>
/// <param name="entityName">Name / type of the <see cref="Entity"/>.</param>
/// <param name="parameterName">Name of the property that is invalid.</param>
/// <param name="parameterValue">The value that was marked as a duplicate.</param>
public class EntityAlreadyExistsException(string entityName, string parameterName, string? parameterValue) : ApplicationException($"'{entityName}' with '{parameterName}':'{parameterValue}' already exists.")
{
}

