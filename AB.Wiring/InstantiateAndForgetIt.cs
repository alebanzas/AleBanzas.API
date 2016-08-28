using System;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.Windsor.Diagnostics;

namespace AB.Wiring
{
	/// <summary>
	/// http://fabiomaulo.blogspot.com/2010/10/castle-windsor-instantiateandforgetit.html
	/// </summary>
	[Serializable]
	public class InstantiateAndForgetIt : ILifestyleManager
	{
		private IComponentActivator activator;

		#region ILifestyleManager Members

		public void Dispose() {}

		public void Init(IComponentActivator componentActivator, IKernel kernel, ComponentModel model)
		{
			activator = componentActivator;
		}

		public bool Release(object instance)
		{
			return true;
		}

	    public object Resolve(CreationContext context, IReleasePolicy releasePolicy)
	    {
            return activator.Create(context, null);
        }
        

		#endregion
	}

	[Serializable]
	public class LifecycledComponentsReleasePolicy : Castle.MicroKernel.Releasers.LifecycledComponentsReleasePolicy
	{
		private readonly Type instantiateAndForgetItType = typeof (InstantiateAndForgetIt);

		public override void Track(object instance, Burden burden)
		{
			if (instantiateAndForgetItType.Equals(burden.Model.CustomLifestyle))
			{
				return;
			}

			base.Track(instance, burden);
		}

	    public LifecycledComponentsReleasePolicy(IKernel kernel) : base(kernel)
	    {
	    }

	    public LifecycledComponentsReleasePolicy(ITrackedComponentsDiagnostic trackedComponentsDiagnostic, ITrackedComponentsPerformanceCounter trackedComponentsPerformanceCounter) : base(trackedComponentsDiagnostic, trackedComponentsPerformanceCounter)
	    {
	    }
	}
}