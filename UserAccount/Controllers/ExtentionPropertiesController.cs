using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Zidium.Core.Api;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models.ExtentionProperties;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ExtentionPropertiesController : ContextController
    {
        public ActionResult ShowTable(Guid? logId, Guid? eventId, Guid? componentId)
        {
            ExtentionPropertiesModel model = null;
            if (logId.HasValue)
            {
                var log = CurrentAccountDbContext.Logs.Find(logId);
                model = ExtentionPropertiesModel.Create(log);
            }
            else if (eventId.HasValue)
            {
                var eventObj = CurrentAccountDbContext.Events.Find(eventId);
                model = ExtentionPropertiesModel.Create(eventObj);
            }
            else if (componentId.HasValue)
            {
                var component = CurrentAccountDbContext.Components.Find(componentId);
                model = ExtentionPropertiesModel.Create(component);
            }
            else
            {
                throw new Exception("Не удалось найти расширенные свойства");
            }
            return PartialView("ExtentionPropertiesTable", model);
        }

        private ExtentionPropertiesModel.Row GetRow(Guid id, ExtentionPropertyOwner owner)
        {
            if (owner == ExtentionPropertyOwner.Log)
            {
                var property = CurrentAccountDbContext.LogProperties.Find(id);
                if (property == null)
                {
                    return null;
                }
                return new ExtentionPropertiesModel.Row()
                {
                    Id = property.Id,
                    DataType = property.DataType,
                    Value = property.Value,
                    Name = property.Name
                };
            }
            if (owner == ExtentionPropertyOwner.Event)
            {
                var property = CurrentAccountDbContext.EventProperties.Find(id);
                if (property == null)
                {
                    return null;
                }
                return new ExtentionPropertiesModel.Row()
                {
                    Id = property.Id,
                    DataType = property.DataType,
                    Value = property.Value,
                    Name = property.Name
                };
            }
            if (owner == ExtentionPropertyOwner.Component)
            {
                var property = CurrentAccountDbContext.ComponentProperties.Find(id);
                if (property == null)
                {
                    return null;
                }
                return new ExtentionPropertiesModel.Row()
                {
                    Id = property.Id,
                    DataType = property.DataType,
                    Value = property.Value,
                    Name = property.Name
                };
            }
            throw new Exception("Неизвестное значение owner: " + owner);
        }

        public ActionResult DownloadFile(Guid id, ExtentionPropertyOwner owner)
        {
            var row = GetRow(id, owner);
            if (row == null)
            {
                throw new HttpException(404, "Файл не найден");
            }

            // банарный файл
            if (row.DataType == DataType.Binary)
            {
                var contentType = GuiHelper.GetContentType(row.Name);
                var bytes = Convert.FromBase64String(row.Value);
                return File(bytes, contentType, row.Name);
            }

            // текстовые файлы
            var textTypes = new[]
            {
                DataType.String,
                DataType.Unknown
            };
            if (textTypes.Contains(row.DataType))
            {
                var bytes = Encoding.UTF8.GetBytes(row.Value);
                bytes = Encoding.UTF8.GetPreamble().Concat(bytes).ToArray();

                var fileName = row.Name.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase)
                    ? row.Name
                    : row.Name + ".txt";
                
                return File(bytes, "text/plain", fileName);
            }
            throw new Exception("Недопустимое для файла значение DataType: " + row.DataType);
        }
    }
}