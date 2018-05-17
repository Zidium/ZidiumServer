// This file can be modified in any way, with two exceptions. 1) The name of
// this class must be "ModuleInitializer". 2) This class must have a public or
// internal parameterless "Run" method that returns void. In addition to these
// modifications, this file may also be moved to any location, as long as it
// remains a part of its current project.

using Zidium.Core.AccountsDb;
using Zidium.Core.ConfigDb;

namespace ApiTests_1._0
{
    internal static class ModuleInitializer
    {
        internal static void Run()
        {
            // Тесты не должны накатывать миграции или создавать базы
            AccountDbContext.DisableMigrations();

            Initialization.SetServices();
        }
    }
}