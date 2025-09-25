using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private List<GameObject> _cars;

    private DataModule _dataModule;

    private void Awake()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
    }

    private void Start()
    {
        if (_dataModule != null && _dataModule.IsInitialized)
        {
            PlacingSkin(_dataModule.Data.appliedCarIndex);
        }
        else
        {
            Debug.Log("DataModule not initialized");
        }
    }

    private void PlacingSkin(int index)
    {
        foreach (var car in _cars)
        {
            car.SetActive(false);
        }
        if (index >= 0 && index < _cars.Count)
        {
            _cars[index].SetActive(true);
        }
    }
}
