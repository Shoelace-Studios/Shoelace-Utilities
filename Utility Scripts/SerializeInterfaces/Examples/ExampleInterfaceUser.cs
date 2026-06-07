using System.Collections.Generic;
using ShoelaceStudios.SerializeInterfaces;
using UnityEngine;
namespace ShoelaceStudio.Utilities.SerializeInterfaces.Examples
{
    public class ExampleInterfaceUser : MonoBehaviour
    {
        [RequireInterface(typeof(IExampleInterface))]
        public MonoBehaviour ExampleMonoWithRequire;
        public InterfaceReference<IExampleInterface> Example;

        public InterfaceReference<IExampleInterface>[] ExampleArray;
        public List<InterfaceReference<IExampleInterface>> ExampleList;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.Log(Example.Value.GetBarkDebug());
        }
    }

    public interface IExampleInterface
    {
        public string GetBarkDebug();
    }
}
