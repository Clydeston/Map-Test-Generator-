using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

enum ConventionalSigns
{
    BUILDING, 
    NON_CONIFERUS_WOOD,
    CONIFERUS_WOOD,
    BRIDGE,
    TUMULI,
    TRIG_POINT,
    ELECTRIC_POLE, 
    MARSH_LAND,
    IMPORTANT_BUILDING,
    TRAIN_STATION
}

namespace MapTestGen
{
    class ConventionalSign
    {
        public Grid Grid_Reference;
        public ConventionalSigns Type;
        public string Filename;
        public string File_Path;
        public string Comment;

        public void CreateNewEntry()
        {
			string grid_storage = @$"./Grids/{this.Filename}.txt";

			try
			{
				// Check if file already exists. If yes, delete it.     
				if (File.Exists(grid_storage))
				{
                    var confirmResult = MessageBox.Show("This grid is already added, do you want to overwrite it?", "Overwrite Previous Grid Data", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        File.Delete(grid_storage);                        
                    }
                    else
                    {
                        return;
                    }                    
                }

				// Create a new file     
				using (StreamWriter sw = File.CreateText(grid_storage))
				{
                    string json_obj = JsonConvert.SerializeObject(this, Formatting.Indented);
                    sw.WriteLine("{0}", json_obj);
				}

				MessageBox.Show("Added successfuly!");
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.ToString());
			}
		}

        public void RemoveEntry()
        {
            try
            {
                if (File.Exists($"./Grids/{this.Filename}"))
                {
                    var confirmResult = MessageBox.Show("Are you sure you want to remove this?", "Remove Entry", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        File.Delete($"./Grids/{this.Filename}");
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public List<ConventionalSign> GetConventionalSigns()
        {
            List<ConventionalSign> list = new List<ConventionalSign>();

            DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Grids"));
            try
            {
                if (di.Exists)
                {
                    FileInfo[] Files = di.GetFiles("*.txt"); //Getting Text files

                    foreach (FileInfo file in Files)
                    {
                        try
                        {
                            string contents = "";
                            const Int32 BufferSize = 128;
                            using (var fileStream = File.OpenRead(file.FullName))
                            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                            {
                                String line;
                                while ((line = streamReader.ReadLine()) != null)
                                {
                                    contents = contents + line;
                                }
                            }
                            var data = JsonConvert.DeserializeObject<ConventionalSign>(contents);
                            list.Add(data);
                        }
                        catch(Exception ex)
                        {
                            continue;
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return list;
        } 
    }
}
