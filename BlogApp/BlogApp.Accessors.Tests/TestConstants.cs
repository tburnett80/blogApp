using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Accessors.Tests
{
    internal static class TestConstants
    {
        public static string Server => "10.200.7.33"; //"10.200.7.33"

        public static string GuidString => $"{Guid.NewGuid().ToString().Replace("-", "")}";
    }
}
