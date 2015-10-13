using System;
using System.Collections.Generic;
using System.Linq;
using ABServicios.Azure.QueuesConsumers;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using ABServicios.Azure.Storage.DataAccess.TableStorage.Queries;
using NUnit.Framework;

namespace ABServicios.Azure.Tests
{
    public class QueueConsumerRunner
    {
        [Test]
        public void FullStorageInitializerTest()
        {
            FullStorageInitializer.Initialize();
        }

        [Test]
        public void StartEnqueuingMailsMessages()
        {
            var i = 1;
            while (i <= 5)
            {
                AzureQueue.Enqueue(new MailMessage
                {
                    From = "noreply@abhosting.com.ar",
                    Subject = "TEST" + i,
                    To = "alebanzas@outlook.com",
                    Body = "<b>probando</b> el mensaje",
                });
                i++;
            }
        }

        [Test]
        public void StartConsumigMailsMessagesSender()
        {
            QueueConsumerFor<MailMessage>.WithinCurrentThread.Using(new MailsMessagesSender())
                                            .With(PollingFrequencer.For(MailsMessagesSender.EstimatedTime))
                                            .StartConsimung();
        }

        [Test]
        public void StartConsumigApiAccessLogSaver()
        {
            QueueConsumerFor<ApiAccessLog>.WithinCurrentThread.Using(new ApiAccessLogSaver())
                                            .With(PollingFrequencer.For(ApiAccessLogSaver.EstimatedTime))
                                            .StartConsimung();
        }

        [Test]
        public void StartConsumigDenunciaPreciosSaver()
        {
            QueueConsumerFor<DenunciaPrecios>.WithinCurrentThread.Using(new DenunciaPreciosSaver())
                                            .With(PollingFrequencer.For(DenunciaPreciosSaver.EstimatedTime))
                                            .StartConsimung();
        }

        [Test]
        public void StartConsumigAzureChristmasVoteLogSaver()
        {
            QueueConsumerFor<AzureChristmasVoteLog>.WithinCurrentThread.Using(new AzureChristmasVoteLogSaver())
                                            .With(PollingFrequencer.For(AzureChristmasVoteLogSaver.EstimatedTime))
                                            .StartConsimung();
        }

        [Test]
        public void StartConsumigAzureChristmasReferalRefresh()
        {
            QueueConsumerFor<AzureChristmasRefreshReferal>.WithinCurrentThread.Using(new AzureChristmasReferalRefresh())
                                            .With(PollingFrequencer.For(AzureChristmasReferalRefresh.EstimatedTime))
                                            .StartConsimung();
        }

        [Test]
        public void StartConsumigAzureChristmasPuntosPorUsuario()
        {
            QueueConsumerFor<PuntosProcesados>.WithinCurrentThread.Using(new AzureChristmasPuntosPorUsuario())
                                            .With(PollingFrequencer.For(AzureChristmasPuntosPorUsuario.EstimatedTime))
                                            .StartConsimung();

        }

        [Test]
        public void StartConsumigTrenEnEstacionReduceDuplicates()
        {
            QueueConsumerFor<TrenEnEstacion>.WithinCurrentThread.Using(new TrenEnEstacionReduceDuplicates())
                                            .With(PollingFrequencer.For(TrenEnEstacionReduceDuplicates.EstimatedTime))
                                            .StartConsimung();
        }

        [Test]
        public void GetAzureChristmasResultsQuery()
        {
            var results = new AzureChristmasResultQuery(AzureAccount.DefaultAccount()).GetResults();

            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            var tablePersister = new TablePersister<AzureChristmasVoteUserResultData>(tableClient);

            foreach (var votacionItem in results.Lista)
            {
                Console.WriteLine(votacionItem.Nombre + ":" + votacionItem.Visitas);
                try
                {
                    var i = tablePersister.Get(AzureChristmasVoteUserResultData.PKey, votacionItem.Nombre);

                    if (i == null)
                    {
                        tablePersister.Add(new AzureChristmasVoteUserResultData(votacionItem.Nombre)
                        {
                            Visitas = votacionItem.Visitas,
                        });
                    }
                    else
                    {
                        if (i.Visitas != votacionItem.Visitas)
                        {
                            i.Visitas = votacionItem.Visitas;
                            tablePersister.Update(i);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error al procesar " + votacionItem.Nombre);
                }
            }

        }

        [Test]
        public void StartEnqueingChristmasPuntos()
        {
            var i = 1;
            while (i <= 6)
            {
                AzureQueue.Enqueue(new PuntosProcesados
                {
                    Puntaje = DateTime.UtcNow.Second,
                    UserID = "abhost" + i + ".cloudapp.net",
                });
                AzureQueue.Enqueue(new PuntosProcesados
                {
                    Puntaje = DateTime.UtcNow.Second + 3,
                    UserID = "abhost" + i + ".cloudapp.net",
                });
                i++;
            }
        }

        [Test]
        public void AzureResultsToViewModel()
        {
            var client = AzureAccount.DefaultAccount().CreateCloudTableClient();
            var _tableResults = client.GetTableReference(typeof(AzureChristmasVoteUserResultData).AsTableStorageName());

            var votacionItems = new List<VotacionItem>(
                _tableResults.CreateQuery<AzureChristmasVoteUserResultData>().ToList().Select(x => new VotacionItem
                {
                    Nombre = x.UserId,
                    Puntos = x.Puntos,
                    Visitas = x.Visitas,
                    VisitasReferidas = x.VisitasReferidos,
                }).OrderByDescending(x => (x.Visitas + x.VisitasReferidas)));

            var votacionItemsFiltrado = new List<VotacionItem>();

            foreach (var votacionItem in votacionItems)
            {
                Uri referal;
                if (votacionItem.Nombre == null || !votacionItem.Nombre.EndsWith(".cloudapp.net") ||
                    !Uri.TryCreate("http://" + votacionItem.Nombre, UriKind.Absolute, out referal))
                {
                    continue;
                }
                votacionItemsFiltrado.Add(votacionItem);
                Console.WriteLine(votacionItem.Nombre);
            }

        }


        [Test]
        public void StartEnqueuingChristmasVotes()
        {
            var i = 1;
            while (i <= 5)
            {
                AzureQueue.Enqueue(new AzureChristmasVoteLog
                {
                    Date = DateTime.UtcNow,
                    Ip = "127.0.1." + i,
                    Referal = "abhost" + i + ".azurewebsites.net",
                    Referer = "http://pepe.azurewebsites.net/Home/Index/",
                    UserId = "pepe.azurewebsites.net"
                });
                AzureQueue.Enqueue(new AzureChristmasVoteLog
                {
                    Date = DateTime.UtcNow,
                    Ip = "127.0.1." + i,
                    Referal = "abhost" + i + ".azurewebsites.net",
                    Referer = "http://pepe.azurewebsites.net/Home/Index/",
                    UserId = "pepe.azurewebsites.net"
                });
                AzureQueue.Enqueue(new AzureChristmasVoteLog
                {
                    Date = DateTime.UtcNow,
                    Ip = "127.0.1." + i,
                    Referal = string.Empty,
                    Referer = "http://abhost" + i + ".azurewebsites.net/Home/Index/",
                    UserId = "abhost" + i + ".azurewebsites.net"
                });
                AzureQueue.Enqueue(new AzureChristmasVoteLog
                {
                    Date = DateTime.UtcNow,
                    Ip = "127.0.1." + i,
                    Referal = string.Empty,
                    Referer = "http://abhost" + i + ".azurewebsites.net/Home/Index/",
                    UserId = "abhost" + i + ".azurewebsites.net"
                });
                i++;
            }

        }
    }
}
