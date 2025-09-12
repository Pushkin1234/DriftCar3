using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private List<GameObject> _cars;

    private void Start()
    {
        PlacingSkin(SaveData.Instance.Data.AppliedCarIndex);
    }

    private void PlacingSkin(int index)
    {
        foreach (var car in _cars)
        {
            car.SetActive(false);
            _cars[index].SetActive(true);
        }
    }
}
