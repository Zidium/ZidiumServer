using System.Linq;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Components
{
    public class ComponentSystemNameTests
    {
        /// <summary>
        /// Тест проверяет, что системные имена должны быть уникальны только для родителя
        /// </summary>
        [Fact]
        public void Test1()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            // создадим структуру
            // - root
            //      - A
            //          - B
            //          - С
            //              - D
            //      - B
            //          - A
            //          - C
            //          - D

            var root_a = root.GetOrCreateChildComponentControl("type", "a");
            var root_a_b = root_a.GetOrCreateChildComponentControl("type", "b");
            var root_a_c = root_a.GetOrCreateChildComponentControl("type", "c");
            var root_a_c_d = root_a_c.GetOrCreateChildComponentControl("type", "d");
            var root_b = root.GetOrCreateChildComponentControl("type", "b");
            var root_b_a = root_b.GetOrCreateChildComponentControl("type", "a");
            var root_b_c = root_b.GetOrCreateChildComponentControl("type", "c");
            var root_b_d = root_b.GetOrCreateChildComponentControl("type", "d");

            var all = new[]
            {
                root,
                root_a,
                root_a_b,
                root_a_c,
                root_b,
                root_b_a,
                root_b_c,
                root_b_d,
                root_a_c_d
            };

            // проверим что все создались
            foreach (var control in all)
            {
                Assert.False(control.IsFake());
            }
            
            // проверим, что все разные
            var idCount = all.GroupBy(x => x.Info.Id).Count();
            Assert.Equal(all.Length, idCount);

            // проверим что системные имена одинаковые
            Assert.Equal(root_a.SystemName, root_b_a.SystemName);
            Assert.Equal(root_b.SystemName, root_a_b.SystemName);
            Assert.Equal(root_b_c.SystemName, root_a_c.SystemName);
            Assert.Equal(root_a_c_d.SystemName, root_b_d.SystemName);

            /////////////////////////////////////
            ////// создадим новое соединение
            /////////////////////////////////////
            var account2 = TestHelper.GetTestAccount();
            var client2 = account2.GetClient();
            var root2 = client2.GetRootComponentControl();

            var root_a2 = root2.GetOrCreateChildComponentControl("type", "a");
            var root_a_b2 = root_a2.GetOrCreateChildComponentControl("type", "b");
            var root_a_c2 = root_a2.GetOrCreateChildComponentControl("type", "c");
            var root_a_c_d2 = root_a_c2.GetOrCreateChildComponentControl("type", "d");
            var root_b2 = root2.GetOrCreateChildComponentControl("type", "b");
            var root_b_a2 = root_b2.GetOrCreateChildComponentControl("type", "a");
            var root_b_c2 = root_b2.GetOrCreateChildComponentControl("type", "c");
            var root_b_d2 = root_b2.GetOrCreateChildComponentControl("type", "d");

            var all2 = new[]
            {
                root2,
                root_a2,
                root_a_b2,
                root_a_c2,
                root_b2,
                root_b_a2,
                root_b_c2,
                root_b_d2,
                root_a_c_d2
            };

            // проверим что все создались
            foreach (var control in all2)
            {
                Assert.False(control.IsFake());
            }

            // проверим, что все разные
            var idCount2 = all2.GroupBy(x => x.Info.Id).Count();
            Assert.Equal(all2.Length, idCount2);

            // проверим что системные имена одинаковые
            Assert.Equal(root_a2.SystemName, root_b_a2.SystemName);
            Assert.Equal(root_b2.SystemName, root_a_b2.SystemName);
            Assert.Equal(root_b_c2.SystemName, root_a_c2.SystemName);
            Assert.Equal(root_a_c_d.SystemName, root_b_d.SystemName);

            // компонент С из root создастся новый
            var root_c2 = root2.GetOrCreateChildComponentControl("type", "c");
            Assert.False(root_c2.IsFake());
            Assert.False(all.Any(x=>x.Info.Id == root_c2.Info.Id));
        }

        /// <summary>
        /// Проверим, что папки не влияют на привязку к родителю
        /// </summary>
        [Fact]
        public void Test2()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            // создадим структуру
            // - root
            //      - A
            //          - B
            //          - folder
            //              - C
            //      - folder
            //          - B
            var root_a = root.GetOrCreateChildComponentControl("type", "a");
            var root_a_b = root_a.GetOrCreateChildComponentControl("type", "b");
            var root_a_folder = root_a.GetOrCreateChildFolderControl("folder");
            var root_a_folder_c = root_a_folder.GetOrCreateChildComponentControl("type", "c");
            var root_folder = root.GetOrCreateChildFolderControl("folder");
            var root_folder_b = root_folder.GetOrCreateChildComponentControl("type", "b");

            var all = new[]
            {
                root,
                root_a,
                root_a_b,
                root_a_folder,
                root_a_folder_c,
                root_folder,
                root_folder_b
            };

            // проверим что все создались
            foreach (var control in all)
            {
                Assert.False(control.IsFake());
            }

            // проверим, что все разные
            var idCount = all.GroupBy(x => x.Info.Id).Count();
            Assert.Equal(all.Length, idCount);

            // проверим что системные имена одинаковые
            Assert.Equal(root_a_b.SystemName, root_folder_b.SystemName);
            Assert.Equal(root_a_folder.SystemName, root_folder.SystemName);

            /////////////////////////////////////
            ////// Создадим новое соединение
            /////////////////////////////////////
            var account2 = TestHelper.GetTestAccount();
            root = account2.GetClient().GetRootComponentControl();
            root_a = root.GetOrCreateChildComponentControl("type", "a");
            root_a_b = root_a.GetOrCreateChildComponentControl("type", "b");
            root_a_folder = root_a.GetOrCreateChildFolderControl("folder");
            root_a_folder_c = root_a_folder.GetOrCreateChildComponentControl("type", "c");
            root_folder = root.GetOrCreateChildFolderControl("folder");
            root_folder_b = root_folder.GetOrCreateChildComponentControl("type", "b");

            all = new[]
            {
                root,
                root_a,
                root_a_b,
                root_a_folder,
                root_a_folder_c,
                root_folder,
                root_folder_b
            };

            // проверим что все создались
            foreach (var control in all)
            {
                Assert.False(control.IsFake());
            }

            // проверим, что все разные
            idCount = all.GroupBy(x => x.Info.Id).Count();
            Assert.Equal(all.Length, idCount);

            // проверим что системные имена одинаковые
            Assert.Equal(root_a_b.SystemName, root_folder_b.SystemName);
            Assert.Equal(root_a_folder.SystemName, root_folder.SystemName);
        }
    }
}

