using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.BookStore
{
    public static class TestData
    {
        public static readonly Guid EngineeringId = Guid.Parse("5f8a1c2e-0d3b-4c9a-9e61-1a2b3c4d5e01");
        public static readonly Guid FinanceId = Guid.Parse("5f8a1c2e-0d3b-4c9a-9e61-1a2b3c4d5e02");

        public static readonly Guid PriyaId = Guid.Parse("7b21c9d4-6e5f-4a3b-8c2d-0e1f2a3b4c01"); // EMP-0001 
        public static readonly Guid ArjunId = Guid.Parse("7b21c9d4-6e5f-4a3b-8c2d-0e1f2a3b4c02"); // EMP-0002 
        public static readonly Guid MeeraId = Guid.Parse("7b21c9d4-6e5f-4a3b-8c2d-0e1f2a3b4c03"); // EMP-0003 

        public static readonly Guid TechCorpTenantId = Guid.Parse("a1000000-0000-0000-0000-000000000001");
        public static readonly Guid GlobalSoftTenantId = Guid.Parse("a1000000-0000-0000-0000-000000000002");

        public static readonly Guid TechCorpEmployeeId = Guid.Parse("7b21c9d4-6e5f-4a3b-8c2d-0e1f2a3b4c10");

    }
}


