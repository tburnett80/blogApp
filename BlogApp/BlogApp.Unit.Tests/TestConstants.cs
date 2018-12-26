using System;

namespace BlogApp.Unit.Tests
{
    internal static class TestConstants
    {
        internal static string GuidString => $"{Guid.NewGuid().ToString().Replace("-", "")}";
    }
}
