using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;

struct Grid
{
	public int northings, eastings;
	public string designation;
	public int roamer_northings;
	public int roamer_eastings;
	public int full_grid;
	public int first_half;
	public int second_half;
	public string grid_s;
	public Pos position;
};

struct Pos
{
	public double LATITUDE;
	public double LONGITUDE;
}

class oGrid
{
	public List<Grid> GenerateGridSquares()
	{

		List<Grid> list = new List<Grid>();
		// east devon maps limit 
		// sx 97 78 - 99 91
		// generating sx squares 
		// 
		// generating easting value 
		for (int i = 0; i <= (99 - 97); i++)
		{
			int easting = i + 97;
			// generating northing value
			for (int ii = 0; ii < (91 - 78); ii++)
			{
				int northing = ii + 78;
				Grid grid = new Grid()
				{
					northings = northing,
					eastings = easting,
					designation = "sx"
				};
				list.Add(grid);
				//printf("Coordinate: SX(%i, %i) iteration: %i\n", easting, northing, i);
			}
		}

		// generating sy squares
		// eastings 0-13 northings 78-91
		// generating easting value 
		for (int i = 0; i < 13; i++)
		{
			int easting = i;
			for (int ii = 0; ii < (91 - 78); ii++)
			{
				int northing = ii + 78;
				Grid grid = new Grid()
				{
					northings = northing,
					eastings = easting,
					designation = "sy"
				};
				list.Add(grid);
				//printf("Coordinate: SY(%i, %i) iteration: %i\n", easting, northing, i);
			}
		}

		return list;
	}

	public double GetBearingFromPos(double lat1, double long1, double lat2,
		double long2)
    {
		double dLon = (long2 - long1);

		double y = Math.Sin(dLon) * Math.Cos(lat2);
		double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1)
				* Math.Cos(lat2) * Math.Cos(dLon);

		//90 - (Math.atan2(deltaN, deltaE) / Math.PI * 180) + 360) % 360;

		// radians
		double brng = Math.Atan2(y, x);

		//degress
		brng = brng * (180 / Math.PI);
		brng = (brng + 360) % 360;
		//brng = 360 - brng; // count degrees counter-clockwise - remove to make clockwise

		// converting degress to mils 
		brng = brng * (6400/360);
		
		return brng;// Math.Round(brng, MidpointRounding.AwayFromZero);
	}

	public double GetBearingFromPos2(double lat1, double long1, double lat2,
		double long2)
	{
		double dLon = toRadians(long2 - long1);

		double y = Math.Sin(dLon) * Math.Cos(toRadians(lat2));
		double x = Math.Cos(toRadians(lat1)) * Math.Sin(toRadians(lat2)) - Math.Sin(toRadians(lat1))
				* Math.Cos(toRadians(lat2)) * Math.Cos(dLon);

		//90 - (Math.atan2(deltaN, deltaE) / Math.PI * 180) + 360) % 360;

		// radians
		double brng = ToDegress(Math.Atan2(y, x));

		brng = (brng + 360) % 360;
		//brng = 360 - brng; // count degrees counter-clockwise - remove to make clockwise

		// converting degress to mils 
		//brng = brng * 6400/360;

		return brng;// Math.Round(brng, MidpointRounding.AwayFromZero);
	}

	public double GetBearingFromGrid(Grid Source_Grid, Grid Target_Grid)
    {
		/*  // get E/N distances between ref1 & ref2
		  var deltaE = p2[0]-p1[0];
		  var deltaN = p2[1]-p1[1];

		  // arctan gives us the bearing, just need to convert -pi..+pi to 0..360 deg
		  var deg = (90-(Math.atan2(deltaN, deltaE)/Math.PI*180)+360) % 360;
  
		  return deg.toFixed(0);  // return result in degrees, no decimals
		*/
		int eastings_calc = 0;
		int northings_calc = 0;
		if (Source_Grid.first_half > Target_Grid.first_half)
		{
			eastings_calc = Source_Grid.first_half - Target_Grid.first_half;
			northings_calc = 0;

			if (Source_Grid.second_half > Target_Grid.second_half)
			{
				northings_calc = Source_Grid.second_half - Target_Grid.second_half;
			}
			else
			{
				northings_calc = Target_Grid.second_half - Source_Grid.second_half;
			}


		}
		else
		{
			eastings_calc = Target_Grid.first_half - Source_Grid.first_half;
			northings_calc = 0;

			if (Target_Grid.second_half > Source_Grid.second_half)
			{
				northings_calc = Target_Grid.second_half - Source_Grid.second_half;
			}
			else
			{
				northings_calc = Source_Grid.second_half - Target_Grid.second_half;
			}

			
		}

		double brng = (90 - Math.Atan2(northings_calc, eastings_calc) / Math.PI * 180)+360 % 360;
		return brng * 6400 / 360;
	}

	private double ToDegress(double radians)
    {
		return radians * 180 / Math.PI;
	}

	private double toRadians(double degrees)
	{
		return degrees * Math.PI / 180;
	}

	public Pos GetGridPosition(Grid src)
    {
		string html = string.Empty;
		string url = @"https://webapps.bgs.ac.uk/data/webservices/CoordConvert_LL_BNG.cfc?method=BNGtoLatLng&easting="+ src.first_half +"&northing="+src.second_half;       

		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.AutomaticDecompression = DecompressionMethods.GZip;

		using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
		using (Stream stream = response.GetResponseStream())
		using (StreamReader reader = new StreamReader(stream))
		{
			html = reader.ReadToEnd();

			var data = JsonConvert.DeserializeObject<Pos>(html);

			return data;
		}
    }

}

