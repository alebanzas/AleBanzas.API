using System.Collections.Generic;
using AB.Common.Wiring;
using Castle.Windsor;

namespace AB.Wiring.Repositories
{
	public class DaosGuyWire: IGuyWire
	{
		private readonly WindsorContainer container;

		public DaosGuyWire(WindsorContainer container)
		{
			this.container = container;
		}

		public void Wire()
		{
			//container.Register(Component.For<IDaoFactory>().ImplementedBy<NHibernateDaoFactory>());

			foreach (IGuyWire guyWire in GuyWires)
			{
				guyWire.Wire();
			}
		}

		public void Dewire()
		{
			throw new System.NotSupportedException("Child GuyWire does not support dewire.");
		}

		private IEnumerable<IGuyWire> GuyWires
		{
			get
			{
				yield return new ABServiciosDaosGuyWire(container);
				yield return new DocumentDaosGuyWire(container);
			}
		}
	}

	internal class DocumentDaosGuyWire : IGuyWire
	{
		private readonly WindsorContainer container;

		public DocumentDaosGuyWire(WindsorContainer container)
		{
			this.container = container;
		}

		public void Wire()
		{
			//RegisterGenericImplementationFileDoc<FichaTecnicaDef>();
			//container.Register(Component.For<IDocumentDaoPerCountry<Lanzamientos>>().Instance(new BlobDocumentDaoPerCountry<Lanzamientos>(AzureAccount.Acs2Account(), new LanzamientosSerializer())));
            //container.Register(Component.For<IDocumentDaoPerCountry<ComparacionesDestacadas>>().Instance(new BlobDocumentDaoPerCountry<ComparacionesDestacadas>(AzureAccount.Acs2Account())));
            //container.Register(Component.For<IDocumentDaoPerCountry<MapasConcesionaria>>().Instance(new BlobDocumentDaoPerCountry<MapasConcesionaria>(AzureAccount.Acs2Account())));
            //
			//			container.Register(Component.For<IDocumentDaoPerCountry<RoadTrip.BLL.ViewModels.HomeForm>>().Instance(new NewsForHomeBlobDocumentDao()));
			//			container.Register(Component.For<INoticiasPorMarcaDao>().Instance(new OnAzureNoticiasPorMarcaDao(Autocosmos.Azure.Storage.AzureAccount.DefaultAccount())));
            //
			//NewsletterTemplateRegistration.Initialize();
            //container.Register(Component.For<INewsletterDefDao<NewsletterClasificadoModel>>().Instance(new NewsletterDefDao<NewsletterClasificadoModel>(AzureAccount.Acs2Account(), new NewsletterTransformerClasificados())));
            //container.Register(Component.For<INewsletterDefDao<NewsletterNoticiaModel>>().Instance(new NewsletterDefDao<NewsletterNoticiaModel>(AzureAccount.Acs2Account(), new NewsletterTransformerNoticias())));
            //container.Register(Component.For<INewsletterDefDao<NewsletterNoticiaPlusModel>>().Instance(new NewsletterDefDao<NewsletterNoticiaPlusModel>(AzureAccount.Acs2Account(), new NewsletterTransformerNoticiasPlus())));
            //container.Register(Component.For<INewsletterDefDao<NewsletterNoticiaPlusDestacadoModel>>().Instance(new NewsletterDefDao<NewsletterNoticiaPlusDestacadoModel>(AzureAccount.Acs2Account(), new NewsletterTransformerNoticiasPlusDestacado())));
            //container.Register(Component.For<INewsletterDefDao<NewsletterNoticiaDestacadoModel>>().Instance(new NewsletterDefDao<NewsletterNoticiaDestacadoModel>(AzureAccount.Acs2Account(), new NewsletterTransformerNoticiasDestacado())));
            //container.Register(Component.For<INewsletterDefDao<NewsletterNoticiaClasificadoModel>>().Instance(new NewsletterDefDao<NewsletterNoticiaClasificadoModel>(AzureAccount.Acs2Account(), new NewsletterTransformerNoticiasClasificado())));
            //container.Register(Component.For<INewsletterDefDao<NewsletterNovedadesModel>>().Instance(new NewsletterDefDao<NewsletterNovedadesModel>(AzureAccount.Acs2Account(), new NewsletterTransformerNovedades())));
            //
            //container.Register(Component.For<ITempStorageAccessor>().Instance(new BlobTempStorageAccessor()));
		}
        
		public void Dewire()
		{
			throw new System.NotSupportedException("Child GuyWire does not support dewire.");
		}
	}
}