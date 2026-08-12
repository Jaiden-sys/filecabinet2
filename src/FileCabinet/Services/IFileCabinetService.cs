using System.Collections.ObjectModel;

namespace filecabinet
{
    public interface IFileCabinetService
    {
        int CreateRecord(RecordRequest request);
        void EditRecord(RecordRequest request);
        ReadOnlyCollection<FileCabinetRecord> FindByField(string fieldName, string value);
        ReadOnlyCollection<FileCabinetRecord> GetRecords();
        int GetStat();
        void RemoveRecord(int id);
    }
}