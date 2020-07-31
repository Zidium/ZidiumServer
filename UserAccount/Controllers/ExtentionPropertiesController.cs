using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models.ExtentionProperties;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ExtentionPropertiesController : BaseController
    {
        public ActionResult ShowTable(Guid? logId, Guid? eventId, Guid? componentId)
        {
            var storage = GetStorage();
            ExtentionPropertiesModel model = null;
            if (logId.HasValue)
            {
                var properties = storage.LogProperties.GetByLogId(logId.Value);
                model = ExtentionPropertiesModel.Create(properties);
            }
            else if (eventId.HasValue)
            {
                var properties = storage.EventProperties.GetByEventId(eventId.Value);
                model = ExtentionPropertiesModel.Create(properties);
            }
            else if (componentId.HasValue)
            {
                var properties = storage.ComponentProperties.GetByComponentId(componentId.Value);
                model = ExtentionPropertiesModel.Create(properties);
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
                var property = GetStorage().LogProperties.GetOneById(id);
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
                var property = GetStorage().EventProperties.GetOneById(id);
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
                var property = GetStorage().ComponentProperties.GetOneById(id);
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

            // бинарный файл
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