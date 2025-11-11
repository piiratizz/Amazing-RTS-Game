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
    [Inject] GlobalBuildingsStagesController _buildingStagesController;
    
    private GlobalResourceStorage _globalStorage;
    
    private BuildingTypePrefabLink _building;
    private BuildingConfig _buildingConfig;
    
    private MeshRenderer _buildingPreviewRenderer;
    private GameObject _buildingPreviewInstance;

    private Vector3 _spawnPosition;

    private bool _canPlace;
    private bool _isBuilding;
    
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

    public void StartBuilding(BuildingTypePrefabLink building)
    {
        _building = building;
        
        _buildingConfig = _buildingStagesController.GetActualConfig(player.OwnerId, building.Type);
        
        var prefab = _buildingConfig.BuildPreviewPrefab;

        if (_buildingPreviewInstance != null)
        {
            Destroy(_buildingPreviewInstance);
        }
        
        _buildingPreviewInstance = Instantiate(prefab);
        _buildingPreviewRenderer = _buildingPreviewInstance.GetComponent<MeshRenderer>();
        SetValidMaterial();
        _isBuilding = true;
    }

    public async UniTaskVoid PlaceBuilding()
    {
        if(!_canPlace) return;
        
        var buildingInstance =  _buildingFactory.Create(
            player.OwnerId,
            _spawnPosition,
            _building
            );
        
        foreach (var cost in _buildingConfig.BuildResourceCost)
        {
            _globalStorage.TrySpend(cost.Resource, cost.Amount);
        }
        
        _isBuilding = false;
        
        _signalBus.Fire(new ChanglePlayerModeSignal()
        {
            Mode = PlayerModes.Default
        });
        
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
        
        _isBuilding = false;
        Destroy(_buildingPreviewInstance);
    }
    
    private void Update()
    {
        if (!_isBuilding) return;
        
        var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayer))
        {
            _spawnPosition = _grid.WorldToCell(hit.point);
                
            _spawnPosition.y = hit.point.y;
                
            var colliders = Physics.OverlapBox(
                _spawnPosition,
                new Vector3(_buildingConfig.SizeX / 2, 3, _buildingConfig.SizeZ / 2),
                Quaternion.identity, 
                collisionConflictLayer
            );
                
            _buildingPreviewInstance.transform.position = _spawnPosition;
                
            if (colliders.Length > 0)
            {
                _canPlace = false;
                SetInvalidMaterial();
            }
            else
            {
                _canPlace = true;
                SetValidMaterial();
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_spawnPosition, 0.3f);
    }
}