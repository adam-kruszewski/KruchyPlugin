// Guids.cs
// MUST match guids.h
using System;

namespace KruchyCompany.KruchyPlugin1
{
    static class GuidList
    {
        public const string guidKruchyPlugin1PkgString = "63471d1f-5b0f-4928-a0e3-1645e2c050a6";
        public const string guidKruchyPlugin1CmdSetString = "eccbde9b-9ae4-4438-aece-278a7b5db517";
        public const string guidToolWindowPersistanceString = "ef5eb0e2-70ee-4d2b-9f4a-476c5efe8b91";

        public static readonly Guid guidKruchyPlugin1CmdSet = new Guid(guidKruchyPlugin1CmdSetString);
    };
}