using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Accessors.Tests
{
    internal static class TestConstants
    {
        public static string Server => "192.168.1.1"; //"10.200.7.50"

        public static string GuidString => $"{Guid.NewGuid().ToString().Replace("-", "")}";
    }
}
