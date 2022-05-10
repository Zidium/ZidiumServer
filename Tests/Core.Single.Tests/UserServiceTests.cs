using System.Linq;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.TestTools;

namespace Zidium.Core.Single.Tests
{
    public class UserServiceTests : BaseTest
    {
        [Fact]
        public void DeleteLastAdminTest()
        {
            // Удалим всех пользователей, кроме админа
            var account = TestHelper.GetTestAccount();
            var admin = TestHelper.GetAccountAdminUser();
            using (var context = TestHelper.GetDbContext())
            {
                foreach (var existingUser in context.Users.ToArray())
                {
                    if (existingUser.Id != admin.Id)
                        context.Users.Remove(existingUser);
                }
            }

            // Создадим дополнительного пользователя (не админа)

            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(password);

            // Проверим, что в аккаунте 1 админ
            using (var context = TestHelper.GetDbContext())
            {
                var adminUsers = context.Users.Where(t => t.Roles.Any(x => x.RoleId == SystemRole.AccountAdministrators.Id)).ToArray();
                Assert.Single(adminUsers);
            }

            // Проверим, что нельзя удалить единственного админа
            var storage = TestHelper.GetStorage();
            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();
            var service = new UserService(storage, timeService);
            Assert.Throws<CantDeleteLastAdminException>(() =>
            {
                service.DeleteUser(admin.Id);
            });

            // Проверим, что админ остался
            using (var context = TestHelper.GetDbContext())
            {
                var existingUser = context.Users.FirstOrDefault(t => t.Id == admin.Id);
                Assert.NotNull(existingUser);
            }
        }

        [Fact]
        public void RemoveAdminRoleFromLastAdminTest()
        {
            // Удалим всех пользователей, кроме админа
            var account = TestHelper.GetTestAccount();
            var admin = TestHelper.GetAccountAdminUser();
            using (var context = TestHelper.GetDbContext())
            {
                foreach (var existingUser in context.Users.ToArray())
                {
                    if (existingUser.Id != admin.Id)
                        context.Users.Remove(existingUser);
                }
            }

            // Проверим, что в аккаунте 1 админ
            using (var context = TestHelper.GetDbContext())
            {
                var adminUsers = context.Users.Where(t => t.Roles.Any(x => x.RoleId == SystemRole.AccountAdministrators.Id)).ToArray();
                Assert.Single(adminUsers);
            }

            // Проверим, что нельзя удалить права админа у единственного админа
            using (var context = TestHelper.GetDbContext())
            {
                var storage = TestHelper.GetStorage();
                var timeService = DependencyInjection.GetServicePersistent<ITimeService>();
                var service = new UserService(storage, timeService);
                Assert.Throws<CantRemoveAdminRoleFromLastAdmin>(() =>
                {
                    var existingAdmin = context.Users.Find(admin.Id);
                    service.RemoveUserRole(existingAdmin.Id, existingAdmin.Roles.First(t => t.RoleId == SystemRole.AccountAdministrators.Id).Id);
                });
            }

            // Проверим, что пользователь с правами администратора остался
            using (var context = TestHelper.GetDbContext())
            {
                var adminUsers = context.Users.Where(t => t.Roles.Any(x => x.RoleId == SystemRole.AccountAdministrators.Id)).ToArray();
                Assert.Single(adminUsers);
            }
        }

    }
}
