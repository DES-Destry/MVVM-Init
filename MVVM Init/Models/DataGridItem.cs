using System.IO;

namespace MVVM_Init.Models
{
    class DataGridItem
    {
        public string FilePath { get; set; }
        public string FileName => Path.GetFileName(FilePath);

        public DataGridItem(string filePath)
        {
            this.FilePath = filePath;
        }
    }
}
