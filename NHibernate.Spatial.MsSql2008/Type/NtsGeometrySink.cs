// Copyright 2008 - Ricardo Stuven (rstuven@gmail.com)
//
// This file is part of NHibernate.Spatial.
// NHibernate.Spatial is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NHibernate.Spatial is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with NHibernate.Spatial; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using Microsoft.SqlServer.Types;

namespace NHibernate.Spatial.Type
{
    internal class NtsGeometrySink : IGeometrySink
    {
        private IGeometry geometry;
        private int srid;
        private readonly Stack<OpenGisGeometryType> types = new Stack<OpenGisGeometryType>();
        private List<Coordinate> coordinates = new List<Coordinate>();
        private readonly List<Coordinate[]> rings = new List<Coordinate[]>();
        private readonly List<Geometry> geometries = new List<Geometry>();
        private bool inFigure;

        public IGeometry ConstructedGeometry
        {
            get { return this.geometry; }
        }

        private void AddCoordinate(double x, double y, double? z, double? m)
        {
            Coordinate coordinate;
            if (z.HasValue)
            {
                coordinate = new Coordinate(x, y, z.Value);
            }
            else
            {
                coordinate = new Coordinate(x, y);
            }
            this.coordinates.Add(coordinate);
        }

        #region IGeometrySink Members

        public void AddLine(double x, double y, double? z, double? m)
        {
            if (!this.inFigure)
            {
                throw new ApplicationException();
            }
            AddCoordinate(x, y, z, m);
        }

        public void BeginFigure(double x, double y, double? z, double? m)
        {
            if (this.inFigure)
            {
                throw new ApplicationException();
            }
            this.coordinates = new List<Coordinate>();
            AddCoordinate(x, y, z, m);
            this.inFigure = true;
        }

        public void BeginGeometry(OpenGisGeometryType type)
        {
            this.types.Push(type);
        }

        public void EndFigure()
        {
            OpenGisGeometryType type = this.types.Peek();
            if (type == OpenGisGeometryType.Polygon)
            {
                this.rings.Add(this.coordinates.ToArray());
            }
            this.inFigure = false;
        }

        public void EndGeometry()
        {
            Geometry geometry = null;

            OpenGisGeometryType type = this.types.Pop();

			switch (type)
			{
				case OpenGisGeometryType.Point:
					geometry = BuildPoint();
					break;
				case OpenGisGeometryType.LineString:
					geometry = BuildLineString();
					break;
				case OpenGisGeometryType.Polygon:
					geometry = BuildPolygon();
					break;
				case OpenGisGeometryType.MultiPoint:
					geometry = BuildMultiPoint();
					break;
				case OpenGisGeometryType.MultiLineString:
					geometry = BuildMultiLineString();
					break;
				case OpenGisGeometryType.MultiPolygon:
					geometry = BuildMultiPolygon();
					break;
				case OpenGisGeometryType.GeometryCollection:
					geometry = BuildGeometryCollection();
					break;
			}

            if (this.types.Count == 0)
            {
				this.geometry = geometry;
				this.geometry.SRID = this.srid;
            }
            else
            {
				this.geometries.Add(geometry);
			}
        }

		private Geometry BuildPoint()
		{
			return new Point(this.coordinates[0]);
		}

		private LineString BuildLineString()
		{
			return new LineString(this.coordinates.ToArray());
		}

		private Geometry BuildPolygon()
		{
			if (this.rings.Count == 0)
			{
				return (Polygon)Polygon.Empty;
			}
			LinearRing shell = new LinearRing(this.rings[0]);
			LinearRing[] holes =
				this.rings.GetRange(1, this.rings.Count - 1)
					.ConvertAll<LinearRing>(delegate(Coordinate[] coordinates)
					{
						return new LinearRing(coordinates);
					}).ToArray();
			this.rings.Clear();
			return new Polygon(shell, holes);
		}

		private Geometry BuildMultiPoint()
		{
			Point[] points =
				this.geometries.ConvertAll<Point>(delegate(Geometry g)
				{
					return g as Point;
				}).ToArray();
			return new MultiPoint(points);
		}

		private Geometry BuildMultiLineString()
		{
			LineString[] lineStrings =
				this.geometries.ConvertAll<LineString>(delegate(Geometry g)
				{
					return g as LineString;
				}).ToArray();
			return new MultiLineString(lineStrings);
		}

		private Geometry BuildMultiPolygon()
		{
			Polygon[] polygons =
				this.geometries.ConvertAll<Polygon>(delegate(Geometry g)
				{
					return g as Polygon;
				}).ToArray();
			return new MultiPolygon(polygons);
		}

		private GeometryCollection BuildGeometryCollection()
		{
			return new GeometryCollection(this.geometries.ToArray());
		}

        public void SetSrid(int srid)
        {
            this.srid = srid;
        }

        #endregion
    }
}
