using UnityEngine;

public class ShopModule : BaseGameModule
{
    public override string ModuleName => "Shop";
    
    public int[] prices = { 0, 10, 20, 30, 40 };
    
    private DataModule _dataModule;
    
    public int SelectedCarIndex { get; private set; }
    public bool CanBuyCar(int index) => _dataModule.Data.coins >= prices[index];
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        SelectedCarIndex = _dataModule.Data.appliedCarIndex;
        base.Initialize();
    }
    
    public bool BuyCar(int index)
    {
        if (!CanBuyCar(index)) return false;
        
        _dataModule.Data.coins -= prices[index];
        _dataModule.Data.isBuyShop[index] = true;
        _dataModule.SaveData();
        
        return true;
    }
    
    public void SelectCar(int index)
    {
        SelectedCarIndex = index;
        _dataModule.Data.appliedCarIndex = index;
        _dataModule.SaveData();
    }
    
    public void ChangeCar(Direction direction)
    {
        if (direction == Direction.Left)
        {
            SelectedCarIndex = (SelectedCarIndex - 1 + _dataModule.Data.isBuyShop.Count) % _dataModule.Data.isBuyShop.Count;
        }
        else
        {
            SelectedCarIndex = (SelectedCarIndex + 1) % _dataModule.Data.isBuyShop.Count;
        }
        
        SelectCar(SelectedCarIndex);
    }
    
    public enum Direction { Left, Right }
}