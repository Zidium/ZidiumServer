using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Processor;
using Zidium.Api.Dto;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal
{
    public class VirusTotalProcessor
    {
        private VirusTotalLimitManager limitManager;
        private ITimeService timeService;
        private ILogger logger;

        public VirusTotalProcessor(VirusTotalLimitManager limitManager, ITimeService timeService, ILogger logger)
        {
            this.limitManager = limitManager;
            this.timeService = timeService;
            this.logger = logger;
        }

        private bool isSuccessResponse(ResponseBase response)
        {
            return response.response_code == 1;
        }

        private void ValidateResponse(ResponseBase response)
        {
            if (isSuccessResponse(response))
            {
                return;
            }
            throw new VirusTotalException("Ошибка выполнения запроса: " + response.response_code + "; " + response.verbose_msg);
        }

        public VirusTotalLimitManager GetVisrusTotalLimitManager()
        {
            return limitManager;
        }


        private VirusTotalProcessorReport ConvertReport(ReportResponse report)
        {
            // проверки
            if (report.total == null)
            {
                throw new Exception("Response error: total is null");
            }
            if (report.positives == null)
            {
                throw new Exception("Response error: positives is null");
            }
            if (report.scans == null)
            {
                throw new Exception("Response error: scans is null");
            }

            // результат
            var result = new VirusTotalProcessorReport()
            {
                Positives = report.positives.Value,
                Total = report.positives.Value,
                Permalink = report.permalink
            };
            if (report.scan_date != null)
            {
                result.ScanDate = VirusTotalHelper.ParseDateTime(report.scan_date);
            }
            var items = new List<VirusTotalProcessorReport.ScanItem>();
            foreach (var scansKey in report.scans.Keys)
            {
                var val = report.scans[scansKey];
                if (val.detected == null)
                {
                    throw new Exception("scan item detected is null");
                }
                var item = new VirusTotalProcessorReport.ScanItem()
                {
                    Name = scansKey,
                    Message = val.result,
                    Detected = val.detected.Value
                };
                items.Add(item);
            }
            result.Scans = items.ToArray();
            return result;
        }


        private VirusTotalProcessorOutputData ProcessScanStep(VirusTotalProcessorInputData inputData)
        {
            logger.LogInformation("Выполняем Scan для " + inputData.Url);
            // проверка входных данных
            if (inputData == null)
            {
                throw new ArgumentNullException("inputData");
            }
            if (inputData.ApiKey == null)
            {
                throw new ArgumentException("ApiKey is null");
            }
            if (inputData.Url == null)
            {
                throw new ArgumentException("Url is null");
            }

            var client = new VirusTotalClient();
            ScanResponse scanResponse = client.Scan(new ScanRequest()
            {
                Apikey = inputData.ApiKey,
                Url = inputData.Url
            });
            if (isSuccessResponse(scanResponse))
            {
                DateTime scanTime = VirusTotalHelper.ParseDateTime(scanResponse.scan_date);
                return new VirusTotalProcessorOutputData()
                {
                    NextStepProcessTime = GetNextStepTime(),
                    ScanId = scanResponse.scan_id,
                    ScanTime = scanTime,
                    NextStep = VirusTotalStep.Report
                };
            }
            return new VirusTotalProcessorOutputData()
            {
                NextStep = VirusTotalStep.Scan,
                Result = new SendUnitTestResultRequestDataDto()
                {
                    Result = UnitTestResult.Alarm,
                    Message = scanResponse.verbose_msg ?? "Неизвестная ошибка"
                },
                ErrorCode = VirusTotalErrorCode.VirusTotalApiError
            };
        }

        private SendUnitTestResultRequestDataDto CreateUnitTestResult(VirusTotalProcessorReport report)
        {
            // ссылка на отчет
            var properties = new List<ExtentionPropertyDto>();
            properties.Add(ExtentionPropertyDto.Create("Permalink", report.Permalink));
            properties.Add(ExtentionPropertyDto.Create("Scan date utc", report.ScanDate.ToString("yyyy.MM.dd HH:mm:ss")));

            // чистый сайт
            var detectedItems = report.Scans.Where(x => x.Detected).ToList();
            if (detectedItems.Count == 0)
            {
                return new SendUnitTestResultRequestDataDto()
                {
                    Message = "Сайт чистый",
                    Result = UnitTestResult.Success,
                    Properties = properties
                };
            }

            // есть проблемы
            foreach (var item in detectedItems)
            {
                properties.Add(ExtentionPropertyDto.Create("Source " + item.Name, item.Message));
            }
            return new SendUnitTestResultRequestDataDto()
            {
                Message = "Обнаружены проблемы (" + detectedItems.Count + " источников)",
                Result = UnitTestResult.Alarm,
                Properties = properties
            };
        }

        private DateTime GetNextStepTime()
        {
            return timeService.Now().Add(TimeSpan.FromSeconds(20));
        }

        private DateTime GetNextStepTime(int attempCount)
        {
            var pause = TimeSpan.FromSeconds(30 * attempCount);
            if (pause > TimeSpan.FromHours(1))
            {
                pause = TimeSpan.FromHours(1);
            }
            return timeService.Now().Add(pause);
        }

        private VirusTotalProcessorOutputData ProcessReportStep(VirusTotalProcessorInputData inputData)
        {
            logger.LogInformation("Выполняем Report для " + inputData.Url);

            // проверка входных данных
            if (inputData == null)
            {
                throw new ArgumentNullException("inputData");
            }
            if (inputData.ApiKey == null)
            {
                throw new ArgumentException("ApiKey is null");
            }
            if (inputData.Url == null)
            {
                throw new ArgumentException("Url is null");
            }
            if (inputData.ScanId == null)
            {
                throw new ArgumentException("ScanId is null");
            }
            if (inputData.ScanTime == null)
            {
                throw new ArgumentException("ScanTime is null");
            }

            var client = new VirusTotalClient();
            var reportResponse = client.Report(new ReportRequest()
            {
                Apikey = inputData.ApiKey,
                ScanId = inputData.ScanId,
                Resource = inputData.Url
            });

            // неизвестный ресурс (например, изменился url проверки после шага scan)
            if (reportResponse.response_code == 0)
            {
                logger.LogWarning("Неизвестный ресурс " + inputData.Url);
                return new VirusTotalProcessorOutputData()
                {
                    NextStep = VirusTotalStep.Scan,
                    NextStepProcessTime = GetNextStepTime()
                };
            }

            ValidateResponse(reportResponse);
            var scanTime = VirusTotalHelper.ParseDateTime(reportResponse.scan_date);

            // если отчет старый
            if (scanTime < inputData.ScanTime)
            {
                logger.LogWarning("Отчет старый, scanTime: " + inputData.ScanTime);

                // если прошло 2 дня, а нового отчета нет, выполним новое сканирование
                if (inputData.AttempCount > 2
                    && inputData.ScanTime.HasValue &&
                    inputData.ScanTime.Value.AddDays(2) < timeService.Now())
                {
                    logger.LogWarning("Выполним Scan еще раз");
                    return new VirusTotalProcessorOutputData()
                    {
                        NextStep = VirusTotalStep.Scan,
                        NextStepProcessTime = timeService.Now().AddMinutes(1), // увеличиваем паузу с ростом ошибок
                        ScanId = inputData.ScanId,
                        ScanTime = inputData.ScanTime
                    };
                }

                // ждем ногово отчета
                return new VirusTotalProcessorOutputData()
                {
                    NextStep = VirusTotalStep.Report,
                    NextStepProcessTime = GetNextStepTime(inputData.AttempCount), // увеличиваем паузу с ростом ошибок
                    ScanId = inputData.ScanId,
                    ScanTime = inputData.ScanTime
                };
            }

            // актуальный отчет
            logger.LogInformation("Получили актуальный отчет");
            var report = ConvertReport(reportResponse);
            var unitTestResult = CreateUnitTestResult(report);
            VirusTotalErrorCode errorCode = VirusTotalErrorCode.CleanSite;
            if (unitTestResult.Result == UnitTestResult.Alarm)
            {
                errorCode = VirusTotalErrorCode.ProblemsDetected;
            }
            return new VirusTotalProcessorOutputData()
            {
                Result = unitTestResult,
                NextStep = VirusTotalStep.Scan,
                ErrorCode = errorCode
            };
        }

        public VirusTotalProcessorOutputData Process(VirusTotalProcessorInputData inputData)
        {
            // чтобы не превысить лимиты, спим
            limitManager.SleepByLimits(inputData.ApiKey);

            try
            {
                // обработка шагов
                if (inputData.NextStep == VirusTotalStep.Scan)
                {
                    return ProcessScanStep(inputData);
                }
                if (inputData.NextStep == VirusTotalStep.Report)
                {
                    return ProcessReportStep(inputData);
                }
                throw new Exception("Неизвестный шаг: " + inputData.NextStep);
            }
            catch (WebException webException)
            {
                // ServiceForbidden
                var response = webException.Response as HttpWebResponse;
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        logger.LogError("Нет доступа (проверьте ключ api)");
                        return new VirusTotalProcessorOutputData()
                        {
                            Result = new SendUnitTestResultRequestDataDto()
                            {
                                Result = UnitTestResult.Alarm,
                                Message = "Нет доступа (проверьте ключ api)"
                            },
                            NextStep = VirusTotalStep.Scan,
                            ErrorCode = VirusTotalErrorCode.ServiceForbidden
                        };
                    }
                }
                throw;
            }
        }
    }
}
