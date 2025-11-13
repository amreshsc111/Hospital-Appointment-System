namespace HAS.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email is required", nameof(value));

        if (!value.Contains('@'))
            throw new ArgumentException("Invalid email format", nameof(value));


        Value = value.Trim().ToLowerInvariant();
    }

    public override string ToString() => Value;
}
