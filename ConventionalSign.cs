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
    TRAIN_STATION,
    CHURCH_SPIRE,
    CHURCH_TOWER,
    SPRING,
    BUS_STATION,
    CLIFF,
    MIXED_WOOD
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
        [JsonIgnore]
        public DataGridView dgv;

        public void CreateNewEntry(List<ConventionalSign> sign_list, bool silent = false)
        {
			string grid_storage = @$"./Grids/{this.Filename}.txt";

			try
			{
				// Check if file already exists. If yes, delete it.     
				if (File.Exists(grid_storage))
				{
                    if(!silent)
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
                    else
                    {
                        File.Delete(grid_storage);
                    }
                }

				// Create a new file     
				using (StreamWriter sw = File.CreateText(grid_storage))
				{
                    string json_obj = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    sw.WriteLine("{0}", json_obj);
				}

                if(!silent)
				    MessageBox.Show("Added successfuly!");

                if(!silent)
                {
                    DataGridViewRow row = (DataGridViewRow)dgv.Rows[0].Clone();
                    row.Cells[0].Value = this.Grid_Reference.grid_s;
                    row.Cells[1].Value = Enum.GetName(typeof(ConventionalSigns), this.Type);
                    row.Cells[3].Value = this;
                    dgv.Rows.Add(row);
                    sign_list.Add(this);
                }

            }
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.ToString());
			}
		}

        public void RemoveEntry(DataGridViewRow row, List<ConventionalSign> sign_list)
        {
            try
            {
                if (File.Exists($"./Grids/{this.Filename}.txt"))
                {
                    var confirmResult = MessageBox.Show("Are you sure you want to remove this?", "Remove Entry", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        File.Delete($"./Grids/{this.Filename}.txt");
                        dgv.Rows.Remove(row);
                        sign_list.Remove((ConventionalSign)row.Cells[3].Value);
                        MessageBox.Show("Deleted successfully!");
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

        private void AddComment(string comment)
        {
            try
            {
                if (File.Exists($"./Grids/{this.Filename}.txt"))
                {
                    string contents = "";
                    const Int32 BufferSize = 128;
                    using (var fileStream = File.OpenRead($"{this.Filename}.txt"))
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        String line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            contents = contents + line;
                        }
                    }
                    var data = JsonConvert.DeserializeObject<ConventionalSign>(contents);
                    data.Comment = comment;

                    File.Delete($"./Grids/{this.Filename}.txt");
                    data.CreateNewEntry(null);  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void UpdateComments()
        {
            foreach(DataGridViewRow row in dgv.Rows)
            {
                ConventionalSign cs = (ConventionalSign)dgv.Rows[row.Index].Cells[3].Value;
                if (row.Cells[2].Value != null)
                {
                    if(cs.Comment == null)
                    {
                        cs.Comment = row.Cells[2].Value.ToString();
                        cs.CreateNewEntry(null, true);
                    }
                    else
                    {
                        if (cs.Comment != row.Cells[2].Value.ToString())
                        {
                            cs.Comment = row.Cells[2].Value.ToString();
                            cs.CreateNewEntry(null, true);
                        }
                    }
                }
                else
                {
                    if(cs.Comment != null)
                    {
                        cs.Comment = "";
                        cs.CreateNewEntry(null, true);
                    } 
                }
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
