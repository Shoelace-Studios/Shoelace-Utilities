using System;

namespace ShoelaceStudios.Utilities.DataStore.Examples
{
    [Serializable] // This needs to be seralizeable or it wotn work
    public class DialogueLine : IDataStoreItem
    {
        public string Id;
        public string Language;
        public string Speaker;
        public string Text;
        public string[] Responses;

        //We can use the interface here to use an ID and any prop to make special versions of the key
        public string Key => $"{Id}_{Language}";
    }
}
