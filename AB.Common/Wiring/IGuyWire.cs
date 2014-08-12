namespace AB.Common.Wiring
{
	/// <summary>
	/// Cablea/vincula todas las partes de la applicacción
	/// </summary>
	/// <remarks>
	/// http://en.wikipedia.org/wiki/Guy-wire
	/// A guy-wire or guy-rope is a tensioned cable designed to add stability to structures.
	/// One end of the cable is attached to the structure, and the other is anchored to the ground at a distance from the structure's base.
	/// </remarks>
	public interface IGuyWire
	{
		/// <summary>
		/// Cablea todos los componentes.
		/// </summary>
		void Wire();

		/// <summary>
		/// Suelta todo el cableado.
		/// </summary>
		void Dewire();
	}
}