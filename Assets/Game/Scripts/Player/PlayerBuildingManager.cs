using System;
using Cysharp.Threading.Tasks;
using GlobalResourceStorageSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerBuildingManager : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private LayerMask placementLayer;
    [SerializeField] private LayerMask collisionConflictLayer;
    [SerializeField] private Material validPlacementMaterial;
    [SerializeField] private Material invalidPlacementMaterial;
    [SerializeField] private Player player;
    [SerializeField] private PlayerUnitsController playerUnitsController;
    
    [Inject] private SignalBus _signalBus;
    [Inject] private BuildingFactory _buildingFactory;
    [Inject] private ResourcesStoragesManager _storagesManager;
    [Inject] private GlobalGrid _grid;
    
    private GlobalResourceStorage _globalStorage;
    
    private BuildingConfigPrefabLink _building;
    
    private MeshRenderer _buildingPreviewRenderer;
    private GameObject _buildingPreviewInstance;

    private Vector3 _spawnPosition;
    
    private void Start()
    {
        _signalBus.Subscribe<ChanglePlayerModeSignal>(register =>
        {
            if (register.Mode != PlayerModes.Default)
            {
                StartBuilding(register.Link);
            }
        });

        _globalStorage = _storagesManager.Get(player.OwnerId);
    }

    public void StartBuilding(BuildingConfigPrefabLink building)
    {
        _building = building;
        var prefab = building.Config.BuildPreviewPrefab;
        _buildingPreviewInstance = Instantiate(prefab);
        _buildingPreviewRenderer = _buildingPreviewInstance.GetComponent<MeshRenderer>();
        SetValidMaterial();
    }

    public async UniTaskVoid PlaceBuilding()
    {
        _signalBus.Fire(new ChanglePlayerModeSignal()
        {
            Mode = PlayerModes.Default
        });
        
        var buildingInstance =  _buildingFactory.Create(
            player.OwnerId,
            _spawnPosition,
            _building
            );

        foreach (var cost in _building.Config.BuildResourceCost)
        {
            _globalStorage.TrySpend(cost.Resource, cost.Amount);
        }
        
        
        Destroy(_buildingPreviewInstance);
        
        await UniTask.Yield();
        await UniTask.Yield();
        
        playerUnitsController.SendBuildCommand(buildingInstance);
    }
    
    public void StopBuilding()
    {
        _signalBus.Fire(new ChanglePlayerModeSignal()
        {
            Mode = PlayerModes.Default
        });
        
        Destroy(_buildingPreviewInstance);
    }
    
    private void Update()
    {
        if (_buildingPreviewInstance != null)
        {
            var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayer))
            {
                _spawnPosition = _grid.WorldToCell(hit.point);
                
                var colliders = Physics.OverlapBox(
                    _spawnPosition,
                    new Vector3(_building.Config.SizeX, 3, _building.Config.SizeZ),
                    Quaternion.identity, 
                    collisionConflictLayer
                    );
                
                _buildingPreviewInstance.transform.position = _spawnPosition;
                
                if (colliders.Length > 0)
                {
                    SetInvalidMaterial();
                }
                else
                {
                    SetValidMaterial();
                }
            }
        }
    }

    private void SetValidMaterial()
    {
        _buildingPreviewRenderer.material = validPlacementMaterial;
    }
    
    private void SetInvalidMaterial()
    {
        _buildingPreviewRenderer.material = invalidPlacementMaterial;
    }
}