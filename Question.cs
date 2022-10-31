using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

[Flags]
enum QUESTION_TYPE
{
	DISTANCE,
	CONVENTIONAL_SIGN,
	RESECTION,
	BEARING, 
	DISTANCE_BEARING,
	BEARING_DISTANCE_SIGN, 
	BEARING_SIGN, 
	DISTANCE_SIGN
};

namespace MapTestGen
{
	class Question
	{
		public QUESTION_TYPE type;
		public Grid Source_Grid;
		public Grid Target_Grid;
		public int Bearing;
		public int GetDistance()
		{
			int ans = 0;

			this.Source_Grid = this.BuildGrid(this.Source_Grid);
			this.Target_Grid = this.BuildGrid(this.Target_Grid);


			if (this.Source_Grid.first_half > this.Target_Grid.first_half)
			{
				int eastings_calc = this.Source_Grid.first_half - this.Target_Grid.first_half;
				int northings_calc = 0;

				if (this.Source_Grid.second_half > this.Target_Grid.second_half)
				{
					northings_calc = this.Source_Grid.second_half - this.Target_Grid.second_half;
				}
				else
				{
					northings_calc = this.Target_Grid.second_half - this.Source_Grid.second_half;
				}

				int c_squared_result = (northings_calc * northings_calc) + (eastings_calc * eastings_calc);
				ans = (int)Math.Sqrt(c_squared_result) * 10;
			}
			else
			{
				int eastings_calc = this.Target_Grid.first_half - this.Source_Grid.first_half;
				int northings_calc = 0;

				if (this.Target_Grid.second_half > this.Source_Grid.second_half)
				{
					northings_calc = this.Target_Grid.second_half - this.Source_Grid.second_half;
				}
				else
				{
					northings_calc = this.Source_Grid.second_half - this.Target_Grid.second_half;
				}

				int c_squared_result = (northings_calc * northings_calc) + (eastings_calc * eastings_calc);
				ans = (int)Math.Sqrt(c_squared_result) * 10;
			}

			return ans;
		}

		public List<string> Questions_List = new List<string>();

		public List<string> Answers_List = new List<string>();

		private Grid BuildGrid(Grid g)
		{
			int[] eight_figure_grid = new int[4];
			eight_figure_grid[0] = g.eastings;
			eight_figure_grid[1] = g.roamer_eastings < 10 ? g.roamer_eastings * 10 : g.roamer_eastings;
			eight_figure_grid[2] = g.northings;
			eight_figure_grid[3] = g.roamer_northings < 10 ? g.roamer_northings * 10 : g.roamer_northings;

			string crafted_int = "";
			if (g.eastings < 10)
			{
				crafted_int = "0";
			}

			for (int i = 0; i < 4; i++)
			{
				crafted_int = crafted_int + (eight_figure_grid[i]).ToString();
			}
			g.grid_s = (crafted_int);
			g.full_grid =  Convert.ToInt32(crafted_int);
			string[] s_split_grid = new string[2];
			s_split_grid[0] = crafted_int.Substring(0, (crafted_int.Length / 2));
			s_split_grid[1] = crafted_int.Substring(crafted_int.Length / 2);

			g.first_half = Convert.ToInt32(s_split_grid[0]);
			g.second_half = Convert.ToInt32(s_split_grid[1]);
			g.designation = g.designation;
			g.northings = g.northings;
			g.eastings = g.eastings;
			g.roamer_eastings = g.roamer_eastings;
			g.roamer_northings = g.roamer_northings;
			return g;
		}

		public void GenerateQuestion(int amount, List<Grid>Grid_List)
		{
			switch(this.type)
            {
				case QUESTION_TYPE.DISTANCE:
					this.DistanceQuestion(amount, Grid_List);
					break;
				case QUESTION_TYPE.BEARING:
					this.BearingQuestion(amount, Grid_List);
					break;
				case QUESTION_TYPE.CONVENTIONAL_SIGN:
					// TODO
					break;
				case QUESTION_TYPE.DISTANCE_BEARING:					
					this.BearingQuestion(amount/2, Grid_List);
					this.DistanceQuestion(amount/2, Grid_List);
					break;
				case QUESTION_TYPE.BEARING_DISTANCE_SIGN:
					break;
				case QUESTION_TYPE.BEARING_SIGN:
					break;
				case QUESTION_TYPE.DISTANCE_SIGN:
					break;
			}
			this.GeneratePaper();
		}

		private void GeneratePaper()
		{
			string question_file = @"./question_paper.txt";
			string answer_file = @"./answer_paper.txt";

			try
			{
				// Check if file already exists. If yes, delete it.     
				if (File.Exists(question_file))
				{
					File.Delete(question_file);
				}

				if (File.Exists(answer_file))
				{
					File.Delete(answer_file);
				}

				// Create a new file     
				using (StreamWriter sw = File.CreateText(question_file))
				{
					for (int i = 0; i < this.Questions_List.Count; i++)
					{
						sw.WriteLine("{0}", this.Questions_List[i]);
					}
				}

				using (StreamWriter sw = File.CreateText(answer_file))
				{
					for (int i = 0; i < this.Answers_List.Count; i++)
					{
						sw.WriteLine("{0}", Answers_List[i]);
					}					
				}
				MessageBox.Show("Completed!");
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.ToString());
			}
		}

		private int RandomInt(int min, int max)
		{

			Random rnd = new Random();
			return rnd.Next(min, max);
		}

		private void DistanceQuestion(int amount, List<Grid> Grid_List)
        {
			for (int i = 1; i < amount + 1; i++)
			{
				this.Source_Grid = Grid_List[RandomInt(1, Grid_List.Count)];
				this.Target_Grid = Grid_List[RandomInt(1, Grid_List.Count)];

				// allowign distance calc to be accurate
				while (Source_Grid.designation != Target_Grid.designation || (this.Target_Grid.designation == "SX") || this.Source_Grid.designation == "SX")
				{
					this.Source_Grid = Grid_List[RandomInt(1, Grid_List.Count)];
					this.Target_Grid = Grid_List[RandomInt(1, Grid_List.Count)];
				}

				this.Source_Grid.roamer_eastings = RandomInt(1, 100);
				this.Source_Grid.roamer_northings = RandomInt(1, 100);
				this.Target_Grid.roamer_eastings = RandomInt(1, 100);
				this.Target_Grid.roamer_northings = RandomInt(1, 100);

				int distance_answer = this.GetDistance();

				string question_str;

				string base_question = ". What is the distance from ";

				question_str = i.ToString() + base_question + (this.Source_Grid.designation).ToString() + " " + this.Source_Grid.grid_s + " "
				   + "to " + (this.Target_Grid.designation).ToString() + " " + this.Target_Grid.grid_s + "\n";
				Questions_List.Add(question_str);
				string answer_string;
				answer_string = i.ToString() + ". " + distance_answer.ToString() + "m /" + (distance_answer/1000).ToString() + "km\n";
				Answers_List.Add(answer_string);

				oGrid grid = new oGrid();
				this.Source_Grid.position = grid.GetGridPosition(this.Source_Grid);
				this.Target_Grid.position = grid.GetGridPosition(this.Target_Grid);
			}
		}

		private void BearingQuestion(int amount, List<Grid> Grid_List)
        {
			for (int i = 1; i < amount + 1; i++)
			{
				this.Source_Grid = Grid_List[RandomInt(1, Grid_List.Count)];
				this.Target_Grid = Grid_List[RandomInt(1, Grid_List.Count)];

				// allowign distance calc to be accurate
				while (Source_Grid.designation != Target_Grid.designation || (this.Target_Grid.designation == "SX") || this.Source_Grid.designation == "SX")
				{
					this.Source_Grid = Grid_List[RandomInt(1, Grid_List.Count)];
					this.Target_Grid = Grid_List[RandomInt(1, Grid_List.Count)];
				}

				this.Source_Grid.roamer_eastings = RandomInt(1, 100);
				this.Source_Grid.roamer_northings = RandomInt(1, 100);
				this.Target_Grid.roamer_eastings = RandomInt(1, 100);
				this.Target_Grid.roamer_northings = RandomInt(1, 100);

				this.Source_Grid = this.BuildGrid(this.Source_Grid);
				this.Target_Grid = this.BuildGrid(this.Target_Grid);

				oGrid grid = new oGrid();
				this.Source_Grid.position = grid.GetGridPosition(this.Source_Grid);
				this.Target_Grid.position = grid.GetGridPosition(this.Target_Grid);

				/*double bearing_from_src_to_dest = grid.GetBearingFromPos(this.Source_Grid.position.LATITUDE, this.Source_Grid.position.LONGITUDE,
																		this.Target_Grid.position.LATITUDE, this.Target_Grid.position.LONGITUDE);*/
				double bearing_from_src_to_dest = grid.GetBearingFromGrid(this.Source_Grid, this.Target_Grid);
				this.Bearing = (int)bearing_from_src_to_dest;

				string question_str;
				string base_question = ". What is the bearing from ";

				question_str = i.ToString() + base_question + (this.Source_Grid.designation).ToString() + " " + this.Source_Grid.grid_s + " "
				   + "to " + (this.Target_Grid.designation).ToString() + " " + this.Target_Grid.grid_s + "\n";
				Questions_List.Add(question_str);
				string answer_string;
				answer_string = i.ToString() + ". " + this.Bearing.ToString() + " mils" + "\n";
				Answers_List.Add(answer_string);
				//MessageBox.Show("Bearing:" + bearing_from_src_to_dest.ToString());
			}
		}
	};

}
